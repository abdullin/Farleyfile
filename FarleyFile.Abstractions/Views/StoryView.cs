using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StoryView : IEntityBase
    {
        public string Name { get; set; }
        public Guid StoryId { get; set; }

        public IList<StoryViewNote> Notes { get; set; }
        public IList<StoryViewTask> Tasks { get; set; }
        public IList<StoryViewActivity> Activities { get; set; } 

        public StoryView()
        {
            Notes = new List<StoryViewNote>();
            Tasks = new List<StoryViewTask>();
            Activities = new List<StoryViewActivity>();
        }

        public void RemoveNote(Guid noteId)
        {
            var notes = Notes.Where(n => n.NoteId == noteId).ToList();
            foreach (var note in notes)
            {
                Notes.Remove(note);
            }
        }

        public void RemoveTask(Guid taskId)
        {
            var tasks = Tasks.Where(n => n.TaskId == taskId).ToList();
            foreach (var task in tasks)
            {
                Tasks.Remove(task);
            }
        }

        public void AddNote(Guid noteId, string title, string text)
        {
            Notes.Add(new StoryViewNote()
                {
                    NoteId = noteId,
                    Title = title
                });
        }

        public void AddActivity(ActivityAdded e)
        {
            Activities.Add(new StoryViewActivity()
                {
                    ActivityId = e.ActivityId,
                    Text = e.Text,
                    CreatedUtc = e.CreatedUtc
                });
        }

        public void AddTask(Guid taskId, string text, bool completed)
        {
            Tasks.Add(new StoryViewTask()
                {
                    TaskId = taskId,
                    Text = text,
                    Completed = completed
                });
        }

        public void UpdateTask(Guid taskId, Action<StoryViewTask> update)
        {
            foreach (var task in Tasks.Where(t => t.TaskId == taskId))
            {
                update(task);
            }
        }

        public void UpdateNote(Guid noteId, Action<StoryViewNote> update)
        {
            foreach (var note in Notes.Where(t => t.NoteId == noteId))
            {
                update(note);
            }
        }
    }

    public sealed class StoryListView
    {
        public IDictionary<Guid,StoryListItem> Items { get; private set; }

        public StoryListView()
        {
            Items = new Dictionary<Guid, StoryListItem>();
        }
    }

    public sealed class StoryListItem
    {
        public Guid StoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}