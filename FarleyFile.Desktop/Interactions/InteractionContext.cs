using Lokad.Cqrs;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {
        public readonly LifelineViewport Viewport;
        readonly IMessageSender _sender;

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionContext(LifelineViewport viewport, IMessageSender sender)
        {
            Viewport = viewport;
            _sender = sender;
        }
    }
}