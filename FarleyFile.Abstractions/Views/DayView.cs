using System;
using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class DayView : IEntityBase
    {
        public IList<DayViewNote> Notes { get; set; }

        public DayView()
        {
            Notes = new List<DayViewNote>();
        }

        public void AddNote(DateTime date, string text)
        {
            Notes.Add(new DayViewNote()
                {
                    Date = date,
                    Text = text
                });
        }
    }

    public sealed class DayViewNote
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}