using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StoryView : IEntityBase
    {
        public string Name { get; set; }

        public IList<StoryViewNote> Notes { get; set; }
        public IList<StoryViewTask> Tasks { get; set; } 

        public StoryView()
        {
            Notes = new List<StoryViewNote>();
            Tasks = new List<StoryViewTask>();
        }

        public void AddNote(long noteId, string title, string text)
        {
            Notes.Add(new StoryViewNote()
                {
                    NoteId = noteId,
                    Text = text,
                    Title = title
                });
        }
        public void AddTask(long taskId, string text, bool completed)
        {
            Tasks.Add(new StoryViewTask()
                {
                    TaskId = taskId,
                    Text = text,
                    Completed = completed
                });
        }

        public void UpdateTask(long taskId, Action<StoryViewTask> update)
        {
            foreach (var task in Tasks.Where(t => t.TaskId == taskId))
            {
                update(task);
            }
        }
    }
}