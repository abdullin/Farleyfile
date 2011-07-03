using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class DayView : IEntityBase
    {
        public IList<DayViewNote> Notes { get; set; }
        public IList<DayViewTask> Tasks { get; set; } 

        public DayView()
        {
            Notes = new List<DayViewNote>();
            Tasks = new List<DayViewTask>();
        }

        public void AddNote(DateTime date, string text)
        {
            Notes.Add(new DayViewNote()
                {
                    Date = date,
                    Text = text
                });
        }
        public void AddTask(long taskId, DateTime date, string text)
        {
            Tasks.Add(new DayViewTask()
                {
                    TaskId = taskId,
                    Date = date,
                    Text = text
                });
        }

        public void UpdateTask(long taskId, Action<DayViewTask> update)
        {
            foreach (var task in Tasks.Where(t => t.TaskId == taskId))
            {
                update(task);
            }
        }
    }

    public sealed class DayViewNote
    {
        public long NoteId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }

    public sealed class DayViewTask
    {
        public long TaskId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
    }
}