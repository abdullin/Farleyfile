using System;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {
        public readonly LifelineViewport Viewport;
        readonly IMessageSender _sender;

        public readonly InteractionRequest Request;
        public readonly InteractionResponse Response;
        public readonly NuclearStorage Storage;

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionContext(LifelineViewport viewport, IMessageSender sender, InteractionRequest request, NuclearStorage storage, InteractionResponse response)
        {
            Viewport = viewport;
            _sender = sender;
            Request = request;
            Storage = storage;
            Response = response;
        }
    }

    public sealed class InteractionResponse
    {
        public readonly LifelineViewport Viewport;
        Action<long, string> _action;
        public InteractionResponse(LifelineViewport viewport, Action<long, string> action)
        {
            Viewport = viewport;
            _action = action;
        }

        public void SetCurrentStory(long storyId, string storyName)
        {
            _action(storyId, storyName);
        }
        
    }

    public sealed class InteractionRequest
    {
        public readonly string Raw;
        public readonly string Alias;
        public readonly string Data;
        public readonly long CurrentStoryId;

        public InteractionRequest(string data, long currentStoryId, string @alias, string raw)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Alias = alias;
            Raw = raw;
        }
    }
}