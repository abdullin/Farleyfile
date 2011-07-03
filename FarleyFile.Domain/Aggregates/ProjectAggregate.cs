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

        public void When(AddTask c)
        {
            var nextRecord = _state.GetNextRecord();
            Apply(new TaskAdded(nextRecord, c.Text, c.Date));
        }

        public void When(CompleteTask c)
        {
            var task = _state.GetTask(c.TaskId);
            if (!task.Completed)
            {
                Apply(new TaskCompleted(c.TaskId, c.Date));
            }
        }

        public void When(AddNote n)
        {
            var nextRecord = _state.GetNextRecord();
            Apply(new NoteAdded(nextRecord, n.Text, n.Date));
        }

        void Apply(IEvent e)
        {
            _state.Apply(e);
            _observer(e);
        }
    }
}