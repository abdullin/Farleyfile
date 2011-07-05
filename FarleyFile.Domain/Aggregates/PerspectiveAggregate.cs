using System;
using System.Linq;

namespace FarleyFile.Aggregates
{
    public sealed class PerspectiveAggregate : IAggregate
    {
        readonly Action<IEvent> _observer;
        readonly PerspectiveAggregateState _state;

        public PerspectiveAggregate(Action<IEvent> observer, PerspectiveAggregateState state)
        {
            _observer = observer;
            _state = state;

            if (!_state.Created)
            {
                var storyId = _state.GetNextId();
                Apply(new PerspectiveCreated(storyId));
                Apply(new StoryStarted(storyId,"Draft"));
            }
        }

        public void Execute(ICommand c)
        {
            RedirectToWhen.InvokeCommand(this, c);
        }

        public void When(StartStory c)
        {
            var id = _state.GetNextId();
            Apply(new StoryStarted(id, c.Name));
        }

        public void When(AddTask c)
        {
            var nextRecord = _state.GetNextId();
            Apply(new TaskAdded(nextRecord, c.Text));
            Apply(new TaskAssignedToStory(nextRecord, _state.DraftId, c.Text, false));
        }
        public void When(AddNote n)
        {
            var nextRecord = _state.GetNextId();
            Apply(new NoteAdded(nextRecord, n.Title, n.Text));
            Apply(new NoteAssignedToStory(nextRecord, _state.DraftId, n.Title, n.Text));
        }

        public void When(CompleteTask c)
        {
            TaskItem item;
            if (!_state.TryGet(c.TaskId,out item))
            {
                throw Error("Task {0} was not found", c.TaskId);
            }

            if (!item.Completed)
            {
                var stories = item.FeaturedIn.Select(f => f.ItemId).ToArray();
                Apply(new TaskCompleted(c.TaskId,stories));
            }
        }

        Exception Error(string message, params object[] args)
        {
            var txt = string.Format(message, args);
            return new InvalidOperationException(txt);
        }

        void Apply(IEvent e)
        {
            _state.Apply(e);
            _observer(e);
        }
    }
}