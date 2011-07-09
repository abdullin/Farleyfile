using System.Linq;
using System.Windows.Forms;
using FarleyFile.Desktop;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public class TextRenderers
    {
        RichTextBox _rich;

        public TextRenderers(RichTextBox rich)
        {
            _rich = rich;
        }

        public void RenderStoryList(StoryListView list)
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

        public void RenderStory(StoryView view, long id)
        {
            var txt = string.Format("Story: {0} ({1})", view.Name, id);
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