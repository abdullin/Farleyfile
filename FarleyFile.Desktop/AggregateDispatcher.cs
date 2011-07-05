using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lokad.Cqrs;
using Lokad.Cqrs.Core.Dispatch;
using Lokad.Cqrs.Core.Envelope;
using Lokad.Cqrs.Core.Outbox;
using Lokad.Cqrs.Feature.TapeStorage;

namespace FarleyFile
{
    public sealed class AggregateDispatcher : ISingleThreadMessageDispatcher
    {
        readonly ITapeStorageFactory _factory;
        readonly IEnvelopeStreamer _streamer;
        
        readonly string _path;
        readonly QueueWriterRegistry _queue;

        public AggregateDispatcher(ITapeStorageFactory factory, IEnvelopeStreamer streamer, string path, QueueWriterRegistry queue)
        {
            _factory = factory;
            _streamer = streamer;
            _path = path;
            _queue = queue;
        }

        public void DispatchMessage(ImmutableEnvelope message)
        {
            var entity = message.GetAttribute("to-entity", "");
            if (string.IsNullOrEmpty(entity))
                throw new InvalidOperationException("Message without entity address arrived to this dispatcher");

            var commands = message.Items.Select(i => (ICommand)i.Content);

            Dispatch(entity, commands);
        }

        void Dispatch(string id, IEnumerable<ICommand> commands)
        {
            var stream = _factory.GetOrCreateStream(id);

            var records = stream.ReadRecords(0, int.MaxValue).ToList();
            var events = records
                .Select(tr => _streamer.ReadAsEnvelopeData(tr.Data))
                .SelectMany(i => i.Items)
                .Select(i => (IEvent)i.Content)
                .ToList();

            var then = AggregateFactory
                .LoadProject(events, commands)
                .Select(e => new MessageBuilder(e.GetType(), e)).ToList();
            
            if (then.Count == 0)
                return;

            
            // events are stored here as envelopes )
            var b = new EnvelopeBuilder("unknown");
            foreach (var e in then)
            {
                b.Items.Add(e);
            }

            var last = records.Count == 0 ? 0 : records.Last().Index + 1;

            var data = _streamer.SaveEnvelopeData(b.Build());
            var result = stream.TryAppend( data, TapeAppendCondition.VersionIs(last));
            if (!result)
                throw new InvalidOperationException(
                    "Data was modified concurrently, and we don't have merging implemented, yet");

            var args = _path.Split(':');
            IQueueWriterFactory factory;
            if (!_queue.TryGet(args[0], out factory))
                throw new InvalidOperationException("Not found " + _path);


            var arVersion = events.Count + 1;
            var arName = id;
            for (int i = 0; i < then.Count; i++)
            {
                var name = string.Format("{0}/{1}/{2}", arName, arVersion, i);
                var builder = new EnvelopeBuilder(name);


                builder.Items.Add(then[i]);
                builder.AddString("from-entity", arName);

                factory.GetWriteQueue(args[1]).PutMessage(builder.Build());
            }

        }


        public void Init()
        {

        }
    }
}