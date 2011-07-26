using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Aggregates
{
    public sealed class PerspectiveAggregateState : IAggregateState
    {
        public bool Created { get; private set; }

        readonly Dictionary<Identity, AbstractStory> _stories = new Dictionary<Identity, AbstractStory>();
        readonly Dictionary<Identity, AbstractItem> _items = new Dictionary<Identity, AbstractItem>();
        readonly SortedDictionary<Identity,Activity> _activities = new SortedDictionary<Identity, Activity>(); 

        readonly Dictionary<string,TagId> _tags = new Dictionary<string, TagId>();

        public bool TryGetTag(string tag, out TagId id)
        {
            return _tags.TryGetValue(tag, out id);
        }

        public void When(ActivityAdded e)
        {
            _activities.Add(e.ActivityId, new Activity());
        }

        public void When(SimpleStoryStarted e)
        {
            StepRecordId(e.StoryId);
            _stories.Add(e.StoryId, new SimpleStory(e.Name, e.StoryId));
        }

        public void When(NoteAdded e)
        {
            StepRecordId(e.NoteId);
            var item = new NoteItem(e.Title, e.Text, e.NoteId, e.StoryId);
            _items.Add(e.NoteId, item);
            var story = _stories[e.StoryId];
            story.AsItGoes.Add(e.NoteId);
        }

        public void When(TaskAdded e)
        {
            StepRecordId(e.TaskId);
            var item = new TaskItem(e.Text, e.TaskId, e.StoryId);
            _items.Add(e.TaskId, item);
            var story = _stories[e.StoryId];
            story.AsItGoes.Add(e.TaskId);
        }

        
        public void When(NoteArchived e)
        {
            var story = _stories[e.StoryId];
            story.AsItGoes.Remove(e.NoteId);
            _items.Remove(e.NoteId);
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


        public void When(TaskArchived e)
        {
            var story = _stories[e.StoryId];
            story.AsItGoes.Remove(e.TaskId);
            _items.Remove(e.TaskId);
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
        public void When(SimpleStoryRenamed e)
        {
            var story = (SimpleStory) _stories[e.StoryId];
            story.Rename(e.NewName);
        }

        public void When(TagCreated e)
        {
            _tags.Add(e.Tag, e.TagId);
        }
        public void When(TagAddedToStory e)
        {
            _stories[e.StoryId].Tags.Add(e.TagId);
        }

        public bool TryGetStory<TStory>(StoryId storyId, out TStory story)
            where TStory : AbstractStory
        {
            AbstractStory value;
            if (_stories.TryGetValue(storyId, out value))
            {
                story = value as TStory;
                if (story != null)
                {
                    return true;
                }
            }
            story = null;
            return false;
        }

        public bool StoryExists(StoryId storyId)
        {
            return _stories.ContainsKey(storyId);
        }

        public bool TryGet<TItem>(Identity itemId, out TItem item)
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

        public TIdentity GetNext<TIdentity>(Func<Guid,TIdentity> generate)
        {
            var newGuid = Guid.NewGuid();
            return generate(newGuid);
        }

        void StepRecordId(Identity recordId)
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
        public NoteId Id { get; private set; }
        public string Text { get; private set; }
        public string Title { get; private set; }

        public NoteItem(string title, string text, NoteId id, StoryId story) : base(story)
        {
            Title = title;
            Text = text;
            Id = id;
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
        public readonly StoryId Story;

        protected AbstractItem(StoryId story)
        {
            Story = story;
        }
    }

    public sealed class Activity
    {
        
    }

    public abstract class AbstractStory
    {
        public readonly HashSet<Identity> AsItGoes = new HashSet<Identity>();
        public readonly HashSet<TagId> Tags = new HashSet<TagId>(); 
    }

    public sealed class SimpleStory : AbstractStory
    {
        public StoryId Id { get; private set; }
        public string Name { get; private set; }
        public void Rename(string newName)
        {
            Name = newName;
        }

        public SimpleStory(string name, StoryId id) 
        {
            Name = name;
            Id = id;
        }
    }


    public sealed class TaskItem : AbstractItem
    {
        public string Name { get; private set; }
        public TaskId Id { get; private set; }
        public bool Completed { get; set; }

        public void Rename(string name)
        {
            Name = name;
        }

        public TaskItem(string name, TaskId id, StoryId storyId) : base (storyId)
        {
            Name = name;
            Id = id;

        }
    } 

}