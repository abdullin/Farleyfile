using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FarleyFile.Views;
using Lokad.Cqrs;
using Lokad.Cqrs.Build.Engine;
using Lokad.Cqrs.Core.Dispatch.Events;
using Lokad.Cqrs.Core.Outbox;
using Lokad.Cqrs.Core.Reactive;
using Lokad.Cqrs.Feature.FilePartition;
using Lokad.Cqrs.Feature.TapeStorage;
using ServiceStack.Text;
using Autofac;

namespace FarleyFile
{
    class Sys
    {
        public static CqrsEngineBuilder Configure(FileStorageConfig cache)
        {
            
            var observer = new ImmediateEventsObserver();

            observer.Event += @event =>
            {
                var failed = @event as EnvelopeDispatchFailed;

                if (failed != null)
                {
                    Trace.WriteLine(failed.Exception);
                }
            };

            var builder = new CqrsEngineBuilder();

            builder.Advanced.RegisterObserver(observer);
            builder.Advanced.RegisterQueueWriterFactory(context => new FileQueueWriterFactory(cache, context.Resolve<IEnvelopeStreamer>()));
            builder.Domain(m =>
                {
                    m.HandlerSample<IConsume<IBaseMessage>>(c => c.Consume(null));
                    m.InAssemblyOf<StoryViewHandler>();
                    m.InAssemblyOf<AddNote>();
                    m.InAssemblyOf<StoryList>();
                });
            builder.Advanced.CustomDataSerializer(t => new DataSerializerWithProtoBuf(t));
            builder.Advanced.CustomEnvelopeSerializer(new EnvelopeSerializerWithProtoBuf());
            builder.Storage(c =>
                {
                    RegisterAtomicStorage(cache, c);
                    c.TapeIsInFiles(Path.Combine(cache.Folder.FullName, "tapes"));
                });

            builder.File(m =>
            {
                m.AddFileSender(cache, "router", cm => cm.IdGeneratorForTests());
                m.AddFileProcess(cache, "router", x => x.DispatcherIsLambda(SaveAndRoute));
                m.AddFileProcess(cache, "events", p => p.DispatchAsEvents(md => md.WhereMessagesAre<IEvent>()));
                m.AddFileProcess(cache, "aggregates", p => p.DispatcherIs(context =>
                    {
                        var readers = context.Resolve<ITapeStorageFactory>();
                        var streamer = context.Resolve<IEnvelopeStreamer>();
                        var reg = context.Resolve<QueueWriterRegistry>();
                        return new AggregateDispatcher(readers, streamer, "files:router", reg);
                    }));
            });
            return builder;
        }

        static Action<ImmutableEnvelope> SaveAndRoute(IComponentContext ctx)
        {
            var registry = ctx.Resolve<QueueWriterRegistry>();
            IQueueWriterFactory factory;
            if (!registry.TryGet("files", out factory))
                throw new InvalidOperationException("file queue not configured, yet.");

            var agg = factory.GetWriteQueue("aggregates");
            var events = factory.GetWriteQueue("events");
            var tapeFactory = ctx.Resolve<ITapeStorageFactory>();
            var log = tapeFactory.GetOrCreateStream("full-log");
            var streamer = ctx.Resolve<IEnvelopeStreamer>();

            return e =>
                {
                    if (!log.TryAppend(streamer.SaveEnvelopeData(e)))
                    {
                        throw new InvalidOperationException("Failed to save envelope");
                    }
                    if (e.GetAttribute("to-entity", "") != "")
                    {
                        agg.PutMessage(e);
                        return;
                    }
                    if (e.Items.All(i => typeof (IEvent).IsAssignableFrom(i.MappedType)))
                    {
                        events.PutMessage(e);
                        return;
                    }
                    throw new InvalidOperationException("Unsuported envelope");
                };
        }

        static void RegisterAtomicStorage(FileStorageConfig cache, StorageModule c)
        {
            c.AtomicIsInFiles(cache.Folder.FullName, cb =>
                {
                    cb.CustomSerializer(
                        JsonSerializer.SerializeToStream,
                        JsonSerializer.DeserializeFromStream);
                    cb.WhereEntityIs<IEntityBase>();

                    cb.FolderForEntity(t => "views/" + t.Name.ToLowerInvariant());
                    cb.FolderForSingleton("views");
                    cb.NameForSingleton(type => type.Name + ".txt");
                    cb.NameForEntity((type, o) =>
                        {
                            var id = o as Identity;

                            if (id != null)
                            {
                                return id.Tag.ToString("000") + "-" + id.Id.ToString().ToLowerInvariant() + ".txt";
                            }
                            return o.ToString() + ".txt";
                        });
                });
        }
    }
}
