using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {
        public readonly LifelineViewport Viewport;
        readonly IMessageSender _sender;

        public readonly InteractionRequest Request;
        public readonly NuclearStorage Storage;

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionContext(LifelineViewport viewport, IMessageSender sender, InteractionRequest request, NuclearStorage storage)
        {
            Viewport = viewport;
            _sender = sender;
            Request = request;
            Storage = storage;
        }
    }

    public sealed class InteractionRequest
    {
        public readonly string Data;
        public readonly long StoryId;
        public InteractionRequest(string data, long storyId)
        {
            Data = data;
            StoryId = storyId;
        }
    }
}