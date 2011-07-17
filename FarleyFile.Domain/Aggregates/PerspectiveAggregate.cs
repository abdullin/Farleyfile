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

        public void When(RenameItem c)
        {
            NoteItem item;
            if (_state.TryGet(c.Id, out item))
            {
                if (item.Title != c.Name)
                {
                    Apply(new NoteRenamed(c.Id, item.Title, c.Name, item.FeaturedIn));
                }
                return;
            }
            TaskItem task;
            if (_state.TryGet(c.Id, out task))
            {
                if (task.Name != c.Name)
                {
                    Apply(new TaskRenamed(c.Id, task.Name, c.Name, task.FeaturedIn));
                }
                return;
            }
            SimpleStory story;
            if (_state.TryGetStory(c.Id, out story))
            {
                if (story.Name != c.Name)
                {
                    Apply(new SimpleStoryRenamed(c.Id, story.Name, c.Name));
                }
            }
            throw Error("Renaming item {0} is not supported", c.Id);

        }


        public void When(AddActivity c)
        {
            if (!_state.StoryExists(c.StoryId))
            {
                throw Error("Story not found {0}", c.StoryId);
            }
            var id = _state.GetNextId();
            var date = DateTime.UtcNow;
            Apply(new ActivityAdded(c.StoryId, c.Text, date, id)) ;
        }

        public void When(MergeNotes c)
        {
            NoteItem first;
            if (!_state.TryGet(c.NoteId, out first))
            {
                throw Error("Note not found {0}", c.NoteId);
            }
            NoteItem second;
            if (!_state.TryGet(c.Secondary, out second))
            {
                throw Error("Note not found {0}", c.Secondary);
            }

            var oldText = first.Text;
            var newText = oldText + Environment.NewLine + Environment.NewLine + second.Text;
            Apply(new NoteEdited(c.NoteId, newText, oldText, first.FeaturedIn));

            foreach (var l in second.FeaturedIn.ToList())
            {
                Apply(new NoteRemovedFromStory(c.Secondary, l));
                if (!first.FeaturedIn.Contains(l))
                {
                    Apply(new NoteAssignedToStory(c.NoteId, l, first.Title, newText));
                }
            }
            Apply(new NoteRemoved(c.Secondary));
        }


        public void When(EditNote c)
        {
            NoteItem item;
            if (!_state.TryGet(c.NoteId, out item))
            {
                throw Error("Note {0} does not exist", c.NoteId);
            }
            Apply(new NoteEdited(c.NoteId, c.Text, c.OldText, item.FeaturedIn));
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
            if (!_state.TryGet(c.TaskId, out item))
            {
                throw Error("Task {0} was not found", c.TaskId);
            }

            if (!item.Completed)
            {
                var stories = item.FeaturedIn.ToArray();
                Apply(new TaskCompleted(c.TaskId, stories));
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