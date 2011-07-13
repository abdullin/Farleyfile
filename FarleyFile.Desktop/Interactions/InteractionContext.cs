using System;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {
        public readonly LifelineViewport Viewport;

        public readonly InteractionRequest Request;
        public readonly InteractionResponse Response;
        public readonly NuclearStorage Storage;

        

        public InteractionContext(LifelineViewport viewport, InteractionRequest request, NuclearStorage storage, InteractionResponse response)
        {
            Viewport = viewport;
            Request = request;
            Storage = storage;
            Response = response;
        }
    }

    public sealed class InteractionResponse
    {
        public readonly LifelineViewport Viewport;
        readonly Action<long, string> _action;
        readonly IMessageSender _sender;

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionResponse(LifelineViewport viewport, Action<long, string> action, IMessageSender sender)
        {
            Viewport = viewport;
            _action = action;
            _sender = sender;
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