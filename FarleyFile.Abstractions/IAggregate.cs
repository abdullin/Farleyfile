namespace FarleyFile
{
    public interface IAggregate
    {
        void Execute(ICommand c);
    }
}