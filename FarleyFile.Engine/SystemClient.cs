using Lokad.Cqrs;

namespace FarleyFile
{
    public sealed class SystemClient
    {
        readonly IMessageSender _sender;
        public SystemClient(IMessageSender sender)
        {
            _sender = sender;
        }

        public void SendToProject(string projectId, params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", projectId));
        }
    }
}