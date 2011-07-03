namespace FarleyFile
{
    public interface IConsume<in TMessage> where TMessage : IBaseMessage
    {
        void Consume(TMessage e);
    }


}