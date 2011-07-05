using System;
using System.Collections.Generic;
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



        public void RenderStory(StringBuilder builder, IList<StoryViewNote> notes, IList<StoryViewTask> tasks )
        {
            if (tasks.Count > 0)
            {
                builder.AppendLine("Tasks");
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    builder.AppendLine(string.Format("  {1} {2} ({0})", task.TaskId, task.Completed ? "x" : "□", task.Text));
                }
                builder.AppendLine();
            }

            if (notes.Count > 0)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    var note = notes[i];
                    builder.AppendLine(string.Format("{0} ({1})", note.Title, note.NoteId));
                    builder.AppendLine(note.Text);
                    builder.AppendLine();
                }
            }
        }
    }
}