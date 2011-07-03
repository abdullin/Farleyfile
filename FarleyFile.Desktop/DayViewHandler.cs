using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class DayViewHandler : 
        IConsume<NoteAdded>,
        IConsume<TaskAdded>,
        IConsume<TaskCompleted>

    {
        readonly IAtomicEntityWriter<string,DayView> _writer;
        public DayViewHandler(IAtomicEntityWriter<string,DayView> writer)
        {
            _writer = writer;
        }
        
        public void Consume(NoteAdded e)
        {
            var key = GrabKey(e.Date);
            _writer.UpdateEnforcingNew(key, d => d.AddNote(e.Date, e.Text));
        }

        static string GrabKey(DateTime dateTime)
        {
            // we consider everything that happened before 3 AM local time
            // to belong to the previous day
            var date = dateTime.Date;
            if (dateTime.TimeOfDay < TimeSpan.FromHours(3))
            {
                date = date.AddDays(-1);
            }

            var key = date.ToString("yyyy-MM-dd");
            return key;
        }

        public void Consume(TaskAdded e)
        {
            var key = GrabKey((e).Date);
            _writer.UpdateEnforcingNew(key, d => d.AddTask(e.TaskId, e.Date, e.Text));
        }
        public void Consume(TaskCompleted e)
        {
            var key = GrabKey((e).Date);
            _writer.UpdateOrThrow(key, d => d.UpdateTask(e.TaskId, x => x.Completed = true));
        }

    }
}