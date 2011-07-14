using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using FarleyFile.Views;
using Lokad.Cqrs;

namespace FarleyFile
{
    public class LifelineViewport
    {
        readonly RichTextBox _rich;
        readonly Label _statusLabel;

        long _lookupReference;
        readonly Dictionary<long, string> _lookupId = new Dictionary<long, string>();
        public readonly Dictionary<string,long> LookupRef = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
        
        Optional<long> Lookup(string reference)
        {
            long result;
            if (LookupRef.TryGetValue(reference, out result))
            {
                return result;
            }
            return Optional<long>.Empty;
        }

        string AddReference(long identity, params object[] names)
        {
            string readable;
            if (!_lookupId.TryGetValue(identity, out readable))
            {
                _lookupReference += 1;
                readable = _lookupReference.ToString();
                _lookupId.Add(identity, readable);
            }
            var filtered = names
                .Select(n => n.ToString())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Where(n => !n.Contains(' '))
                .ToList();
            filtered.Insert(0, readable);

            foreach (var name in filtered)
            {
                long reference;
                if (!LookupRef.TryGetValue(name, out reference))
                {
                    LookupRef.Add(name, identity);
                }
                else
                {
                    // collision
                    if (reference != identity)
                    {
                        LookupRef[name] = 0;
                        continue;
                    }
                    // no collision
                }
            }

            return readable;
        }

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
            // by clearing the display, we reset lookups
            _lookupId.Clear();
            _lookupReference = 0;
        }

        public void Log(string text, params object[] args)
        {
            try
            {
                using (_rich.Styled(Solarized.Base1))
                {
                    _rich.AppendText(string.Format(text, args));
                    _rich.AppendText(Environment.NewLine);
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public void Error(string text, params object[] args)
        {
            try
            {
                using (_rich.Styled(Solarized.Red))
                {
                    _rich.AppendText(string.Format(text, args));
                    _rich.AppendText(Environment.NewLine);
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

        public void When(StoryListView list)
        {
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine("Stories");
            }
            _rich.AppendLine("=======");
            foreach (var item in list.Items)
            {
                var reference = AddReference(item.StoryId, item.Name);
                _rich.AppendLine("[{1}] {2} ({0})", reference, item.Type, item.Name);
            }
        }

        public void When(StoryView view)
        {
            var txt = string.Format("Story: {0} ({1})", view.Name, AddReference(view.StoryId, view.Name));
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
                        _rich.AppendLine(string.Format("  {1} {2} ({0})", AddReference(task.TaskId), task.Completed ? "x" : "□",
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
                        _rich.AppendLine("{0} ({1})", note.Title, AddReference(note.NoteId));
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