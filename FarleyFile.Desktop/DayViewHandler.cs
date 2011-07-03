using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class DayViewHandler : IConsume<NoteAdded>
    {
        readonly IAtomicEntityWriter<string,DayView> _writer;
        public DayViewHandler(IAtomicEntityWriter<string,DayView> writer)
        {
            _writer = writer;
        }
        
        public void Consume(NoteAdded e)
        {
            // we consider everything that happened before 3 AM local time
            // to belong to the previous day
            var date = e.Date.Date;
            if (e.Date.TimeOfDay < TimeSpan.FromHours(3))
            {
                date = date.AddDays(-1);
            }

            var key = date.ToString("yyyy-MM-dd");


            _writer.UpdateEnforcingNew(key, d => d.AddNote(e.Date, e.Text));
        }
    }
}