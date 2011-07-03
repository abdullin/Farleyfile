using System;

namespace FarleyFile.Aggregates
{
    public sealed class ProjectAggregate : IAggregate
    {
        readonly Action<IEvent> _observer;
        readonly ProjectAggregateState _state;

        public ProjectAggregate(Action<IEvent> observer, ProjectAggregateState state)
        {
            _observer = observer;
            _state = state;
        }

        public void Execute(ICommand c)
        {
            RedirectToWhen.InvokeCommand(this, c);
        }

        public void When(AddNote n)
        {
            Apply(new NoteAdded(0, n.Text, n.Date));
        }

        void Apply(IEvent e)
        {
            _state.Apply(e);
            _observer(e);
        }
    }
}