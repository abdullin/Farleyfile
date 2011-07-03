namespace FarleyFile
{
    public interface IEntityCommand : IBaseMessage
    {
        string EntityId { get; }
    }
}