using System;
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
using Lokad.Cqrs.Core.Outbox;
using Lokad.Cqrs.Core.Reactive;
using Lokad.Cqrs.Feature.FilePartition;
using Lokad.Cqrs.Feature.TapeStorage;
using ServiceStack.Text;
using Autofac;

namespace FarleyFile
{
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
                RunCommand(Environment.CommandLine);
            }
        }

        private static bool RunCommand(string args)
        {
            var client = GetSender();
            while (true)
            {
                if (args == "q")
                    return true;
                if (args == "clr")
                {
                    Console.Clear();
                    return false;
                }

                Process(args, client);
                return false;
            }
        }

        static void Process(string args, SystemClient client)
        {
            if (args.StartsWith("an "))
            {
                var txt = args.Substring(3).TrimStart();
                client.SendToProject("default", new AddNote(txt));
                return;
            }

            Console.WriteLine("Unknown command sequence: {0}", args);
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
                    Console.WriteLine("Type command or q to quit");
                    while(!RunCommand(Console.ReadLine()))
                    {
                        
                    }

                    source.Cancel();
                    Console.WriteLine("Stopping...");
                    task.Wait();
                    Console.WriteLine("Stopped");
                }
            }
        }

        static SystemClient GetSender()
        {
            var cache = GetDataFolder();
            var builder = new CqrsClientBuilder();

            builder.Domain(m => m.HandlerSample<IConsume<IBaseMessage>>(c => c.Consume(null)));
            builder.Advanced.DataSerializer(t => new DevSerializer(t));
            builder.File(m => m.AddFileSender(cache, "router"));
            var cqrsClient = builder.Build();
            var sender = cqrsClient.Resolve<IMessageSender>();
            return new SystemClient(sender);
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
            if (e.GetAttribute("to-entity","")!="")
                return "files:aggregates";
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
            builder.Domain(m => m.HandlerSample<IConsume<IBaseMessage>>(c => c.Consume(null)));
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
                m.AddFileProcess(cache, "aggregates", p =>
                    {
                        p.DispatcherIs((context, infos, arg3) =>
                            {
                                var readers = context.Resolve<ITapeReaderFactory>();
                                var writers = context.Resolve<ISingleThreadTapeWriterFactory>();
                                var streamer = context.Resolve<IEnvelopeStreamer>();
                                var reg = context.Resolve<QueueWriterRegistry>();
                                return new AggregateDispatcher(readers, streamer, writers, "files:router", reg);
                            });
                        p.WhereFilter(md => md.WhereMessagesAre<IEntityCommand>());
                    });
            });
            return builder;
        }
    }
}
