namespace FarleyFile
{
    public interface IAggregateState
    {
        void Apply(IEvent e);
    }
}