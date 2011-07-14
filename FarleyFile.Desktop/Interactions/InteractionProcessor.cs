using System;
using System.Collections.Generic;
using System.Linq;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionProcessor
    {
        readonly IMessageSender _sender;

        readonly IList<AbstractInteraction> _interactions = new List<AbstractInteraction>();

        public void LoadFromAssembly()
        {
            var bases = typeof (AbstractInteraction)
                .Assembly
                .GetExportedTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => typeof (AbstractInteraction).IsAssignableFrom(t));

            foreach (var @base in bases)
            {
                var info = @base.GetConstructor(Type.EmptyTypes);
                if (null == info)
                {
                    var message = string.Format("empty ctor not found for {0}", @base);
                    throw new InvalidOperationException(message);
                }
                _interactions.Add((AbstractInteraction) info.Invoke(null));
            }
        }

        readonly LifelineViewport _viewport;
        readonly NuclearStorage _storage;
        Guid CurrentStoryId { get; set; }

        public InteractionProcessor(IMessageSender sender, LifelineViewport viewport, NuclearStorage storage)
        {
            _sender = sender;
            _viewport = viewport;
            _storage = storage;
        }

        public InteractionResultStatus Handle(string data)
        {
            _viewport.Log("> " + data);

            try
            {
                foreach (var interaction in _interactions)
                {
                    string @alias;
                    string text;
                    if (interaction.WillProcess(data, out @alias, out text))
                    {
                        var request = new InteractionRequest(text, CurrentStoryId, data, _viewport.LookupRef);
                        var response = new InteractionResponse(_viewport, l => { CurrentStoryId = l; }, _sender);
                        var context = new InteractionContext(request, _storage, response);

                        var result = interaction.Handle(context);
                        if (result.Status == InteractionResultStatus.Error)
                        {
                            _viewport.Error(result.OptionalError);
                        }
                        return result.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                _viewport.Error("Error: {0}", ex.Message);
                return InteractionResultStatus.Error;
            }

            _viewport.Error("Unknown command sequence: {0}", data);
            return InteractionResultStatus.Error;
        }
    }
}