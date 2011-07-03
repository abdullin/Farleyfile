using System;
using System.Text;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public class TextRenderers
    {
        readonly NuclearStorage _storage;
        public TextRenderers(NuclearStorage storage)
        {
            _storage = storage;
        }

        public void RenderDay(int shift, StringBuilder builder)
        {
            var date = DateTime.Now.Date.AddDays(shift).ToString("yyyy-MM-dd");

            var result = _storage.GetEntity<DayView>(date);

            
            builder.AppendLine(new string('_', 80 - 12) + " " + date);


            if (!result.HasValue)
            {
                builder.AppendLine("Empty");
                return;

            }
            var notes = result.Value.Notes;

            if (notes.Count > 0)
            {
                foreach (var note in notes)
                {
                    builder.AppendLine(" " + note.Date.ToString("HH:mm"));
                    builder.AppendLine(note.Text);
                    builder.AppendLine();
                }
            }
        }
    }
}