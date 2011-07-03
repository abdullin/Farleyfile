using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FarleyFile.Engine;
using Lokad.Cqrs;
using Lokad.Cqrs.Build.Client;
using Lokad.Cqrs.Build.Engine;
using Lokad.Cqrs.Core.Dispatch.Events;
using Lokad.Cqrs.Core.Reactive;
using Lokad.Cqrs.Feature.AtomicStorage;
using Lokad.Cqrs.Feature.FilePartition;
using ServiceStack.Text;
using Autofac;

namespace FarleyFile
{

    public interface IEvent : IBaseMessage
    {
        
    }
    public interface ICommand : IBaseMessage
    {
        
    }

    public interface IBaseMessage
    {

    }

    public interface IEntityBase
    {
    }

    public interface IBaseConsume<in TMessage> where TMessage : IBaseMessage
    {
        void Consume(TMessage e);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            if (args.Length == 0)
            {
                RunHost();
            }
            else
            {
                RunCommand(args);
            }
        }

        private static void RunCommand(string[] args)
        {
            var a = args.ToList();
            if (a[0].Trim() == "/c")
            {
                a.RemoveAt(0);
            }
            switch (a[0])
            {
                case "an":
                    GetSender().SendOne(new AddNote(a[1]));
                    break;
                default:
                    Console.WriteLine("Unkown command sequence: {0}", string.Join(" ", a));
                    break;

            }
        }

        private static void RunHost()
        {
            
            Console.WriteLine("Initializing host");
            var cache = GetDataFolder();
            using (var source = new CancellationTokenSource())
            {
                var builder = Configure(cache);
                using (var host = builder.Build())
                {
                    Console.WriteLine("Starting host");

                    var task = host.Start(source.Token);
                    Console.WriteLine("Press enter to quit");
                    Console.ReadLine();

                    source.Cancel();
                    Console.WriteLine("Stopping...");
                    task.Wait();
                    Console.WriteLine("Stopped");
                }
            }
        }

        static IMessageSender GetSender()
        {
            var cache = GetDataFolder();
            var builder = new CqrsClientBuilder();

            builder.Domain(m => m.HandlerSample<IBaseConsume<IBaseMessage>>(c => c.Consume(null)));
            builder.Advanced.DataSerializer(t => new DevSerializer(t));
            builder.File(m => m.AddFileSender(cache, "router"));
            var cqrsClient = builder.Build();
            return cqrsClient.Resolve<IMessageSender>();
        }

        private static FileStorageConfig GetDataFolder()
        {

            var cache = @"C:\temp\farley";
            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }
            
            return FileStorage.CreateConfig(cache, "files");
        }

        static string SimpleDispatchRule(ImmutableEnvelope e)
        {
            if (e.Items.All(i => typeof(IEvent).IsAssignableFrom(i.MappedType)))
                return "files:events";
            if (e.Items.All(i => typeof(ICommand).IsAssignableFrom(i.MappedType)))
                return "files:commands";
            throw new InvalidOperationException("Unsuported envelope");
        }

        private static CqrsEngineBuilder Configure(FileStorageConfig cache)
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
            builder.Domain(m =>
            {
                m.HandlerSample<IBaseConsume<IBaseMessage>>(c => c.Consume(null));
            });
            builder.Advanced.CustomDataSerializer(t => new DevSerializer(t));
            builder.Storage(c =>
            {
                c.AtomicIsInFiles(cache.Folder.FullName, cb =>
                {
                    cb.CustomSerializer(
                        JsonSerializer.SerializeToStream,
                        JsonSerializer.DeserializeFromStream);
                    cb.WhereEntityIs<IEntityBase>();
                    ;
                    cb.FolderForEntity(t => "view-" + t.Name.ToLowerInvariant());
                    cb.FolderForSingleton("view-single");
                    cb.NameForSingleton(type => type.Name + ".txt");
                });
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
            });
            return builder;
        }
    }

    public sealed class DayText : IEntityBase
    {
        public IList<string> Notes { get; set; }

        public DayText()
        {
            Notes = new List<string>();
        }
    }

    public sealed class NoteHandler : IBaseConsume<AddNote>
    {
        readonly IAtomicSingletonWriter<DayText> _writer;
        public NoteHandler(IAtomicSingletonWriter<DayText> writer)
        {
            _writer = writer;
        }

        public void Consume(AddNote e)
        {
            _writer.UpdateEnforcingNew(d => d.Notes.Add(e.Text));
        }
    }
}
