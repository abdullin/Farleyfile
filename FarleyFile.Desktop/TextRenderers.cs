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

            var tasks = result.Value.Tasks;
            if (tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    builder.AppendLine(string.Format("{0,4} {1} {2}", task.TaskId, task.Completed ?"x" : "□", task.Text));
                }
                builder.AppendLine();

            }


            var notes = result.Value.Notes;

            if (notes.Count > 0)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    var note = notes[i];
                    builder.AppendLine((i+1) + ". " + note.Date.ToString("HH:mm"));
                    builder.AppendLine(note.Text);
                    builder.AppendLine();
                }
            }
        }
    }
}