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
                Apply(new SimpleStoryStarted(storyId,"Draft"));
            }
        }

        public void Execute(ICommand c)
        {
            RedirectToWhen.InvokeCommand(this, c);
        }

        public void When(StartSimpleStory c)
        {
            var id = _state.GetNextId();
            Apply(new SimpleStoryStarted(id, c.Name));
        }

        public void When(StartContactStory c)
        {
            var id = _state.GetNextId();
            Apply(new ContactStoryStarted(id, c.Name));
        }

        public void When(StartDayStory c)
        {
            if (c.Date.TimeOfDay != TimeSpan.Zero)
                throw Error("Day story should be specified as date");
            if (_state.HasDayStory(c.Date))
                throw Error("Story already exists for day {0:yyyy-MM-dd}. Use it.", c.Date);
            var id = _state.GetNextId();
            Apply(new DayStoryStarted(id, c.Date));
        }

        public void When(AddTask c)
        {
            var nextRecord = _state.GetNextId();
            if (!_state.StoryExists(c.StoryId))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            Apply(new TaskAdded(nextRecord, c.Text));
            Apply(new TaskAssignedToStory(nextRecord, c.StoryId, c.Text, false));
        }
        public void When(AddNote c)
        {
            var nextRecord = _state.GetNextId();
            if (!_state.StoryExists(c.StoryId))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            Apply(new NoteAdded(nextRecord, c.Title, c.Text));
            Apply(new NoteAssignedToStory(nextRecord, c.StoryId, c.Title, c.Text));
        }
        public void When(AddToStory c)
        {
            AbstractItem item;
            if (!_state.TryGet(c.ItemId, out item))
            {
                throw Error("Item {0} was not found", c.ItemId);
            }
            AbstractStory story;
            if (!_state.TryGetStory(c.StoryId, out story))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            if (item.FeaturedIn.Contains(c.StoryId))
            {
                return;
            }
            var note = item as NoteItem;
            if (note != null)
            {
                Apply(new NoteAssignedToStory(c.ItemId, c.StoryId, note.Title, note.Text));
                return;
            }

            var task = item as TaskItem;

            if (task != null)
            {
                Apply(new TaskAssignedToStory(c.ItemId, c.StoryId, task.Name, task.Completed));
                return;
            }
            throw Error("We can't move item {0} of type {1} around.", c.ItemId, item.GetType());
        }

        public void When(RemoveFromStory c)
        {
            AbstractItem item;
            if (!_state.TryGet(c.ItemId, out item))
            {
                throw Error("Item {0} was not found", c.ItemId);
            }
            AbstractStory story;
            if (!_state.TryGetStory(c.StoryId, out story))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            if (!item.FeaturedIn.Contains(c.StoryId))
            {
                return;
            }
            var note = item as NoteItem;
            if (note != null)
            {
                Apply(new NoteRemovedFromStory(c.ItemId, c.StoryId));
                return;
            }

            var task = item as TaskItem;

            if (task != null)
            {
                Apply(new TaskRemovedFromStory(c.ItemId, c.StoryId));
                return;
            }
            throw Error("We can't move item {0} of type {1} around.", c.ItemId, item.GetType());
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
                var stories = item.FeaturedIn.ToArray();
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