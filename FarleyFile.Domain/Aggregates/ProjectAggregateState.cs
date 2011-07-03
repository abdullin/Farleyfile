namespace FarleyFile.Aggregates
{
    public sealed class ProjectAggregateState : IAggregateState
    {
        public void Apply(IEvent e)
        {
            RedirectToWhen.InvokeEventOptional(this, e);
        }
    }
}