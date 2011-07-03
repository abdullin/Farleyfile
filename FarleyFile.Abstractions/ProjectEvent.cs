namespace FarleyFile
{
    public sealed class ProjectEvent<T> : IEntityCommand
        where T : IBaseMessage
    {
        public string EntityId { get; private set; }
        public T Command { get; private set; }

        public ProjectEvent(string entityId, T command)
        {
            EntityId = entityId;
            Command = command;
        }

        
    }
}