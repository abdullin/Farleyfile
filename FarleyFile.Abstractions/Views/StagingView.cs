using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StagingView
    {
        public HashSet<long> Items { get; private set; }

        public IList<StoryViewTask> Tasks { get; private set; }
        public IList<StoryViewNote> Notes { get; private set; }

        public StagingView()
        {
            Items = new HashSet<long>();
            Tasks = new List<StoryViewTask>();
            Notes = new List<StoryViewNote>();
        }

        public void TryRemoveTask(long taskId)
        {
            if (Items.Remove(taskId))
            {
                var task = Tasks.First(t => t.TaskId == taskId);
                Tasks.Remove(task);
            }
        }

        public void TryRemoveNote(long noteId)
        {
            if (Items.Remove(noteId))
            {
                var note = Notes.First(t => t.NoteId == noteId);
                Notes.Remove(note);
            }
        }

        public void AddTask(long taskId, string text, bool completed)
        {
            Tasks.Add(new StoryViewTask()
                {
                    TaskId = taskId,
                    Text = text,
                    Completed = completed
                });
            Items.Add(taskId);
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
    }

    
}