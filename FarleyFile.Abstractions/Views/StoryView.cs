using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StoryView : IEntityBase
    {
        public string Name { get; set; }
        public StoryId StoryId { get; set; }

        public IList<StoryViewNote> Notes { get; set; }
        public IList<StoryViewTask> Tasks { get; set; }
        public IList<StoryViewActivity> Activities { get; set; } 

        public StoryView()
        {
            Notes = new List<StoryViewNote>();
            Tasks = new List<StoryViewTask>();
            Activities = new List<StoryViewActivity>();
        }

        public void RemoveNote(NoteId noteId)
        {
            var notes = Notes.Where(n => n.NoteId == noteId).ToList();
            foreach (var note in notes)
            {
                Notes.Remove(note);
            }
        }

        public void RemoveTask(TaskId taskId)
        {
            var tasks = Tasks.Where(n => n.TaskId == taskId).ToList();
            foreach (var task in tasks)
            {
                Tasks.Remove(task);
            }
        }

        public void AddNote(NoteId noteId, string title, string text)
        {
            Notes.Add(new StoryViewNote()
                {
                    NoteId = noteId,
                    Title = title
                });
        }

        public void AddActivity(ActivityAdded e)
        {
            var activity = new StoryViewActivity()
                {
                    ActivityId = e.ActivityId,
                    Text = e.Text,
                    Created = e.Time,
                    Explicit = true
                };

            foreach (var reference in e.References)
            {
                activity.References.Add(new StoryViewActivityReference
                    {
                        Item = reference.Id,
                        Source = reference.OriginalRef,
                        Title = reference.Text
                    });
            }
            Activities.Add(activity);
        }

        public void AddTask(TaskId taskId, string text, bool completed)
        {
            Tasks.Add(new StoryViewTask()
                {
                    TaskId = taskId,
                    Text = text,
                    Completed = completed
                });
        }

        public void UpdateTask(TaskId taskId, Action<StoryViewTask> update)
        {
            foreach (var task in Tasks.Where(t => t.TaskId == taskId))
            {
                update(task);
            }
        }

        public void UpdateNote(NoteId noteId, Action<StoryViewNote> update)
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
        public StoryId StoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}