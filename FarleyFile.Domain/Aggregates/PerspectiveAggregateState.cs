using System;
using System.Collections.Generic;

namespace FarleyFile.Aggregates
{
    public sealed class PerspectiveAggregateState : IAggregateState
    {
        long _recordId;
        public bool Created { get; private set; }

        public long DraftId { get; private set; }

        readonly Dictionary<long, AbstractStory> _stories = new Dictionary<long, AbstractStory>();
        readonly Dictionary<long, AbstractItem> _items = new Dictionary<long, AbstractItem>();

        readonly HashSet<DateTime> _dayStories = new HashSet<DateTime>();

        public void When(SimpleStoryStarted e)
        {
            StepRecordId(e.StoryId);
            _stories.Add(e.StoryId, new SimpleStory(e.Name));
        }

        public void When(ContactStoryStarted e)
        {
            StepRecordId(e.ContactId);
            _stories.Add(e.ContactId, new ContactStory(e.Name));
        }

        public void When(DayStoryStarted e)
        {
            StepRecordId(e.DayId);
            _stories.Add(e.DayId, new DayStory(e.Date));
            _dayStories.Add(e.Date);
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

        public void When(PerspectiveCreated e)
        {
            Created = true;
            DraftId = e.DraftId;
        }

        public void When(TaskAssignedToStory e)
        {
            var task = (TaskItem) _items[e.TaskId];
            var story = _stories[e.StoryId];

            story.AsItGoes.Add(e.TaskId);
            task.FeaturedIn.Add(e.StoryId);
        }

        public bool TryGetStory(long storyId, out AbstractStory story)
        {
            return _stories.TryGetValue(storyId, out story);
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


        public bool HasDayStory(DateTime date)
        {
            return _dayStories.Contains(date);
        }

      

        void StepRecordId(long recordId)
        {
            if ((recordId) != (_recordId+1))
                throw new InvalidOperationException();

            _recordId = recordId;
        }

        public void Apply(IEvent e)
        {
            RedirectToWhen.InvokeEventOptional(this, e);
        }

        public PerspectiveAggregateState()
        {
            
        }
    }

    public sealed class NoteItem : AbstractItem
    {
        public readonly string Text;
        public readonly string Title;

        public NoteItem(string title, string text)
        {
            Title = title;
            Text = text;
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

    public sealed class DayStory : AbstractStory
    {
        public readonly DateTime Day;
        public DayStory(DateTime day)
        {
            Day = day;
        }
    }

    public sealed class ContactStory : AbstractStory
    {
        public readonly string Name;
        public ContactStory(string name)
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