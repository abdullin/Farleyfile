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
                Apply(new PerspectiveCreated());
            }
        }

        public void Execute(ICommand c)
        {
            RedirectToWhen.InvokeCommand(this, c);
        }

        public void When(StartSimpleStory c)
        {
            var id = _state.GetNext(s => new StoryId(s));
            Apply(new SimpleStoryStarted(id, c.Name));
        }

        public void When(RenameItem c)
        {
            NoteItem note;
            if (_state.TryGet(c.Id, out note))
            {
                if (note.Title != c.Name)
                {
                    
                    Apply(new NoteRenamed(note.Id, note.Title, c.Name, note.Story));
                }
                return;
            }
            TaskItem task;
            if (_state.TryGet(c.Id, out task))
            {
                if (task.Name != c.Name)
                {
                    Apply(new TaskRenamed(task.Id, task.Name, c.Name, task.Story));
                }
                return;
            }
            SimpleStory story;
            if (_state.TryGetStory(new StoryId(c.Id), out story))
            {
                if (story.Name != c.Name)
                {
                    Apply(new SimpleStoryRenamed(new StoryId(c.Id), story.Name, c.Name));
                }
                return;
            }
            throw Error("Renaming item {0} is not supported", c.Id);

         }


        public void When(AddActivity c)
        {
            var id = _state.GetNext(g => new ActivityId(g));
            var date = DateTime.UtcNow;
            Apply(new ActivityAdded(c.Text, date, id, c.References)) ;
        }


        public void When(EditNote c)
        {
            NoteItem item;
            if (!_state.TryGet(c.NoteId, out item))
            {
                throw Error("Note {0} does not exist", c.NoteId);
            }
            Apply(new NoteEdited(c.NoteId, c.Text, c.OldText, item.Story));
        }

        public void When(AddTask c)
        {
            var nextRecord = _state.GetNext(g => new TaskId(g));
            if (!_state.StoryExists(c.StoryId))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            Apply(new TaskAdded(nextRecord, c.Text, c.StoryId));
        }

        public void When(AddNote c)
        {
            var nextRecord = _state.GetNext(g => new NoteId(g));
            if (!_state.StoryExists(c.StoryId))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            Apply(new NoteAdded(nextRecord, c.Title, c.Text, c.StoryId));
        }

        public void When(ArchiveItem c)
        {
            AbstractItem item;
            if (!_state.TryGet(c.Id, out item))
            {
                throw Error("Item {0} was not found", c.Id);
            }
            AbstractStory story;
            if (!_state.TryGetStory(c.StoryId, out story))
            {
                throw Error("Story {0} was not found", c.StoryId);
            }
            var note = item as NoteItem;
            if (note != null)
            {
                Apply(new NoteArchived(note.Id, c.StoryId));
                return;
            }

            var task = item as TaskItem;

            if (task != null)
            {
                Apply(new TaskArchived(task.Id, c.StoryId));
                return;
            }
            throw Error("We can't move item {0} of type {1} around.", c.Id, item.GetType());
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
                
                Apply(new TaskCompleted(c.TaskId, item.Story));
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