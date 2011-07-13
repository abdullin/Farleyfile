using System;
using System.Collections.Generic;
using System.Linq;
using FarleyFile.Views;
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
        long CurrentStoryId { get; set; }

        public InteractionProcessor(IMessageSender sender, LifelineViewport viewport, NuclearStorage storage)
        {
            _sender = sender;
            _viewport = viewport;
            _storage = storage;
        }

        public InteractionResult Handle(string data)
        {
            _viewport.Log("> " + data);
            

            foreach (var interaction in _interactions)
            {
                string @alias;
                string text;
                if (interaction.WillProcess(data, out @alias, out text))
                {
                    var request = new InteractionRequest(text, CurrentStoryId, @alias, data);
                    var response = new InteractionResponse(_viewport, (l, s) =>
                        {
                            _viewport.SelectStory(l, s);
                            CurrentStoryId = l;
                        }, _sender);
                    var context = new InteractionContext(request, _storage, response);

                    return interaction.Handle(context);
                }
            }

            _viewport.Log("Unknown command sequence: {0}", data);
            return InteractionResult.Unknown;
        }


        public void TryLoadStory(long storyId)
        {
            var result = _storage.GetEntity<StoryView>(storyId);
            if (result.HasValue)
            {
                var story = result.Value;
                _viewport.RenderStory(story, storyId);
                _viewport.SelectStory(story.StoryId, story.Name);
                CurrentStoryId = storyId;
            }
            else
            {
                _viewport.Log("Story {0} not found", storyId);
            }
        }
    }
}