using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Aggregates
{
    public sealed class PerspectiveAggregateState : IAggregateState
    {
        public bool Created { get; private set; }

        readonly Dictionary<Guid, AbstractStory> _stories = new Dictionary<Guid, AbstractStory>();
        readonly Dictionary<Guid, AbstractItem> _items = new Dictionary<Guid, AbstractItem>();
        readonly SortedDictionary<Guid,Activity> _activities = new SortedDictionary<Guid, Activity>(); 

        public void When(ActivityAdded e)
        {
            _activities.Add(e.ActivityId, new Activity());
        }

        public void When(SimpleStoryStarted e)
        {
            StepRecordId(e.StoryId);
            _stories.Add(e.StoryId, new SimpleStory(e.Name));
        }

        public void When(NoteAdded e)
        {
            StepRecordId(e.NoteId);
            var item = new NoteItem(e.Title, e.Text);
            _items.Add(e.NoteId, item);
        }

        public void When(TaskAdded e)
        {
            StepRecordId(e.TaskId);
            var item = new TaskItem(e.Text);
            _items.Add(e.TaskId, item);
        }

        public void When(NoteAssignedToStory e)
        {
            var note = (NoteItem)_items[e.NoteId];
            var story = _stories[e.StoryId];

            story.AsItGoes.Add(e.NoteId);
            note.FeaturedIn.Add(e.StoryId);
        }
        public void When(NoteRemovedFromStory e)
        {
            var note = (NoteItem)_items[e.NoteId];
            var story = _stories[e.StoryId];

            story.AsItGoes.Remove(e.NoteId);
            note.FeaturedIn.Remove(e.StoryId);
        }

        public void When(PerspectiveCreated e)
        {
            Created = true;
        }

        public void When(NoteEdited e)
        {
            var note = (NoteItem) _items[e.NoteId];
            note.Edit(e.NewText);
        }

        public void When(NoteRemoved e)
        {
            var note = (NoteItem) _items[e.NoteId];
            if (note.FeaturedIn.Any())
            {
                throw new InvalidOperationException("Sanity check failure");
            }
            _items.Remove(e.NoteId);

        }

        public void When(TaskAssignedToStory e)
        {
            var task = (TaskItem) _items[e.TaskId];
            var story = _stories[e.StoryId];

            story.AsItGoes.Add(e.TaskId);
            task.FeaturedIn.Add(e.StoryId);
        }

        public void When(TaskRemovedFromStory e)
        {
            var task = (TaskItem)_items[e.TaskId];
            var story = _stories[e.StoryId];

            story.AsItGoes.Remove(e.TaskId);
            task.FeaturedIn.Remove(e.StoryId);
        }

        public void When(NoteRenamed e)
        {
            var item = (NoteItem)_items[e.NoteId];
            item.Rename(e.NewName);
        }
        public void When(TaskRenamed e)
        {
            var item = (TaskItem) _items[e.TaskId];
            item.Rename(e.NewText);
        }

        public bool TryGetStory(Guid storyId, out AbstractStory story)
        {
            return _stories.TryGetValue(storyId, out story);
        }

        public bool StoryExists(Guid storyId)
        {
            return _stories.ContainsKey(storyId);
        }

        public bool TryGet<TItem>(Guid itemId, out TItem item)
            where TItem : AbstractItem
        {
            AbstractItem value;
            if (_items.TryGetValue(itemId, out value))
            {
                item = value as TItem;

                if (item != null)
                {
                    return true;
                }
            }
            item = null;
            return false;
        }

        public Guid GetNextId()
        {
            return Guid.NewGuid();
        }

        void StepRecordId(Guid recordId)
        {
            //if ((recordId) != (_recordId+1))
            //    throw new InvalidOperationException();

            //_recordId = recordId;
        }

        public void Apply(IEvent e)
        {
            RedirectToWhen.InvokeOptional(this, e);
        }

        public PerspectiveAggregateState()
        {
            
        }
    }

    public sealed class NoteItem : AbstractItem
    {
        public string Text { get; private set; }
        public string Title { get; private set; }

        public NoteItem(string title, string text)
        {
            Title = title;
            Text = text;
        }
        public void Edit(string newText)
        {
            Text = newText;
        }

        public void Rename(string newTitle)
        {
            Title = newTitle;
        }
    }



    public abstract class AbstractItem
    {
        public HashSet<Guid> FeaturedIn = new HashSet<Guid>();
    }

    public sealed class Activity
    {
        
    }

    public abstract class AbstractStory
    {
        public HashSet<Guid> AsItGoes = new HashSet<Guid>();

        
    }

    public sealed class SimpleStory : AbstractStory
    {
        public readonly string Name;

        public SimpleStory(string name) 
        {
            Name = name;
        }
    }


    public sealed class TaskItem : AbstractItem
    {
        public string Name { get; private set; }
        public bool Completed { get; set; }

        public void Rename(string name)
        {
            Name = name;
        }

        public TaskItem(string name) 
        {
            Name = name;
        }
    } 

}