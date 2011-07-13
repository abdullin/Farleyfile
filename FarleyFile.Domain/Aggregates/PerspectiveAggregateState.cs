using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Aggregates
{
    public sealed class PerspectiveAggregateState : IAggregateState
    {
        long _recordId;
        public bool Created { get; private set; }

        readonly Dictionary<long, AbstractStory> _stories = new Dictionary<long, AbstractStory>();
        readonly Dictionary<long, AbstractItem> _items = new Dictionary<long, AbstractItem>();


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

        public bool TryGetStory(long storyId, out AbstractStory story)
        {
            return _stories.TryGetValue(storyId, out story);
        }

        public bool StoryExists(long storyId)
        {
            return _stories.ContainsKey(storyId);
        }

        public bool TryGet<TItem>(long itemId, out TItem item)
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

        public long GetNextId()
        {
            return _recordId + 1;
        }

        void StepRecordId(long recordId)
        {
            if ((recordId) != (_recordId+1))
                throw new InvalidOperationException();

            _recordId = recordId;
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
    }



    public abstract class AbstractItem
    {
        public HashSet<long> FeaturedIn = new HashSet<long>();
        
    }

    public abstract class AbstractStory
    {
        public HashSet<long> AsItGoes = new HashSet<long>();

        
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
        public string Name { get; set; }
        public bool Completed { get; set; }

        public TaskItem(string name) 
        {
            Name = name;
        }
    } 

}