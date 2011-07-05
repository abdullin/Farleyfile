﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FarleyFile.Desktop;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;
using FarleyFile;
using System.Linq;

namespace FarleyFile
{

    public static class ExtendRich
    {
        sealed class DisposableAction : IDisposable
        {
            readonly Action _dispose;
            public DisposableAction(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose();
            }
        }
        public static IDisposable Styled(this RichTextBox box, Color color, bool bold = false, int indent = 0)
        {
            var selectionColor = box.SelectionColor;
            var font = box.SelectionFont;
            var oldindent = box.SelectionIndent;

            if (color != Color.Empty)
            {
                box.SelectionColor = color;
            }
            if (bold)
            {
                box.SelectionFont = new Font(box.Font, box.Font.Style | FontStyle.Bold);
            }
            if (indent != 0)
            {
                box.SelectionIndent = indent;
            }

            

            return new DisposableAction(() =>
                {
                    box.SelectionColor = selectionColor;
                    box.SelectionFont = font;
                    box.SelectionIndent = oldindent;
                });
        }

        public static void AppendLine(this RichTextBox box, string format, params object[] args)
        {
            box.AppendText(string.Format(format, args));
            box.AppendText(Environment.NewLine);
        }
        public static void AppendLine(this RichTextBox box)
        {
            box.AppendText(Environment.NewLine);
        }
    }

    public class TextRenderers
    {
        readonly NuclearStorage _storage;
        public TextRenderers(NuclearStorage storage)
        {
            _storage = storage;
        }

        public void RenderStoryList(RichTextBox text, StoryListView list)
        {
            using (text.Styled(Solarized.Yellow))
            {
                text.AppendLine("## Stories");
            }
            foreach (var item in list.Items)
            {
                text.AppendLine("{0}. [{1}] {2}", item.StoryId, item.Type, item.Name);
            }
        }

        public void RenderStory(RichTextBox text, StoryView view)
        {
            using (text.Styled(Solarized.Base01, true))
            {
                text.AppendLine(view.Name);
            }

            if (view.Tasks.Count > 0)
            {
                text.AppendLine();
                foreach (var task in view.Tasks.OrderBy(c => c.Completed))
                {
                    var color = task.Completed ? Color.Empty : Solarized.Green;
                    using (text.Styled(color))
                    {
                        text.AppendLine(string.Format("  {1} {2} ({0})", task.TaskId, task.Completed ? "x" : "□",
                            task.Text));
                    }
                }
            }

            if (view.Notes.Count > 0)
            {
                foreach (var note in view.Notes)
                {
                    text.AppendLine();
                    using (text.Styled(Solarized.Blue))
                    {
                        text.AppendLine("{0} ({1})", note.Title, note.NoteId);
                    }
                    
                    using (text.Styled(Color.Empty, indent:10))
                    {
                        text.AppendLine(note.Text);
                    }
                }
            }
        }
    }

    
}