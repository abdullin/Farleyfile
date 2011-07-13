using System;
using System.Linq;
using System.Windows.Forms;
using FarleyFile.Views;

namespace FarleyFile
{
    public class LifelineViewport
    {
        readonly RichTextBox _rich;
        readonly Label _statusLabel;

        public LifelineViewport(RichTextBox rich, Label statusLabel)
        {
            _rich = rich;
            _statusLabel = statusLabel;
        }

        public void SelectStory(long storyId, string storyName)
        {
            _statusLabel.Text = string.Format("Story: {0} ({1})", storyName, storyId);
        }

        public void Clear()
        {
            _rich.Clear();
        }

        public void When(StoryListView list)
        {
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine("Stories");
            }
            _rich.AppendLine("=======");
            foreach (var item in list.Items)
            {
                _rich.AppendLine("[{1}] {2} ({0})", item.StoryId, item.Type, item.Name);
            }
        }

        public void Log(string text, params object[] args)
        {
            try
            {
                using (_rich.Styled(Solarized.Base1))
                {
                    _rich.AppendLine(text, args);
                    _rich.ScrollToCaret();
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public void RenderView<T>(T view)
        {
            RedirectToWhen.InvokeOptional(this, view, (i,v) => _rich.AppendText(ServiceStack.Text.TypeSerializer.SerializeAndFormat(view)));
        }

        public void When(StoryView view)
        {
            var txt = string.Format("Story: {0} ({1})", view.Name, view.StoryId);
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine(txt);
            }
            _rich.AppendLine(new string('=', txt.Length));
            if (view.Tasks.Count > 0)
            {
                foreach (var task in view.Tasks.OrderBy(c => c.Completed))
                {
                    var color = task.Completed ? Solarized.Base1 : Solarized.Base00;
                    using (_rich.Styled(color))
                    {
                        _rich.AppendLine(string.Format("  {1} {2} ({0})", task.TaskId, task.Completed ? "x" : "□",
                            task.Text));
                    }
                }
                _rich.AppendLine();
            }

            if (view.Notes.Count > 0)
            {
                foreach (var note in view.Notes)
                {
                    using (_rich.Styled(Solarized.Base1))
                    {
                        _rich.AppendLine("{0} ({1})", note.Title, note.NoteId);
                    }

                    using (_rich.Styled(Solarized.Base00, indent : 16))
                    {
                        _rich.AppendLine(note.Text);
                    }
                }
                _rich.AppendLine();
            }
            if (view.Notes.Count == 0 && view.Tasks.Count == 0)
            {
                using (_rich.Styled(Solarized.Red))
                {
                    _rich.AppendLine("  Story is empty");
                }
            }
        }
    }
}