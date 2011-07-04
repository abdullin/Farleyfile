using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        static string SimpleDispatchRule(ImmutableEnvelope e)
        {
            if (e.GetAttribute("to-entity","")!="")
                return "files:aggregates";
            if (e.Items.All(i => typeof(IEvent).IsAssignableFrom(i.MappedType)))
                return "files:events";
            if (e.Items.All(i => typeof(ICommand).IsAssignableFrom(i.MappedType)))
                return "files:commands";
            throw new InvalidOperationException("Unsuported envelope");
        }

        public static CqrsEngineBuilder Configure(FileStorageConfig cache)
        {
            
            var observer = new ImmediateEventsObserver();

            observer.Event += @event =>
            {
                var failed = @event as EnvelopeDispatchFailed;

                if (failed != null)
                {
                    Debug.WriteLine(failed.Exception);
                }
            };

            var builder = new CqrsEngineBuilder();

            builder.Advanced.RegisterObserver(observer);
            builder.Advanced.RegisterQueueWriterFactory(context => new FileQueueWriterFactory(cache, context.Resolve<IEnvelopeStreamer>()));
            builder.Domain(m => m.HandlerSample<IConsume<IBaseMessage>>(c => c.Consume(null)));
            builder.Advanced.CustomDataSerializer(t => new DevSerializer(t));
            builder.Storage(c =>
                {
                    RegisterAtomicStorage(cache, c);
                    c.TapeIsInFiles(Path.Combine(cache.Folder.FullName, "tapes"));
                });

            builder.File(m =>
            {
                m.AddFileSender(cache, "router", cm => cm.IdGeneratorForTests());

                m.AddFileRouter(cache, "router", SimpleDispatchRule);
                m.AddFileProcess(cache, "events", p =>
                {
                    p.DispatchAsEvents();
                    p.WhereFilter(md => md.WhereMessagesAre<IEvent>());
                });
                m.AddFileProcess(cache, "commands", p =>
                {
                    p.DispatchAsCommandBatch();
                    p.WhereFilter(md => md.WhereMessagesAre<ICommand>());
                });
                m.AddFileProcess(cache, "aggregates", p =>
                    {
                        p.DispatcherIs((context, infos, arg3) =>
                            {
                                var readers = context.Resolve<ITapeStorageFactory>();
                                var streamer = context.Resolve<IEnvelopeStreamer>();
                                var reg = context.Resolve<QueueWriterRegistry>();
                                return new AggregateDispatcher(readers, streamer, "files:router", reg);
                            });
                        p.WhereFilter(md => md.WhereMessagesAre<IEntityCommand>());
                    });
            });
            return builder;
        }

        static void RegisterAtomicStorage(FileStorageConfig cache, StorageModule c)
        {
            c.AtomicIsInFiles(cache.Folder.FullName, cb =>
                {
                    cb.CustomSerializer(
                        JsonSerializer.SerializeToStream,
                        JsonSerializer.DeserializeFromStream);
                    cb.WhereEntityIs<IEntityBase>();

                    cb.FolderForEntity(t => "view-" + t.Name.ToLowerInvariant());
                    cb.FolderForSingleton("view-single");
                    cb.NameForSingleton(type => type.Name + ".txt");
                });
        }
    }
}
