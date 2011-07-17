using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FarleyFile.Views;

namespace FarleyFile
{
    public class LifelineViewport
    {
        readonly RichTextBox _rich;
        readonly Label _statusLabel;

        long _lookupReference;
        readonly Dictionary<Identity, string> _lookupId = new Dictionary<Identity, string>();
        public readonly Dictionary<string,Identity> LookupRef = new Dictionary<string, Identity>(StringComparer.InvariantCultureIgnoreCase);
        

        string AddReference(Identity identity, params object[] names)
        {
            if (identity.IsEmpty)
                throw new InvalidOperationException("Can't add an empty reference");
            string readable;
            if (!_lookupId.TryGetValue(identity, out readable))
            {
                _lookupReference += 1;
                readable = _lookupReference.ToString();
                _lookupId.Add(identity, readable);

                // overwrite reverse lookup
                LookupRef[readable] = identity;
            }

            var filtered = names
                .Select(n => n.ToString())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Where(n => !n.Contains(' '))
                .ToList();

            foreach (var name in filtered)
            {
                Identity reference;
                if (!LookupRef.TryGetValue(name, out reference))
                {
                    LookupRef.Add(name, identity);
                }
                else
                {
                    // collision
                    if (reference != identity)
                    {
                        LookupRef[name] = Identity.Empty;
                        //Log("Collision: '{0}' -> '{1}' by '{2}'", reference, identity, name);
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

        public void SelectStory(StoryId storyId, string storyName)
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
                    _rich.ScrollToCaret();
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

        public void When(StoryListView list)
        {
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine("Stories");
            }
            _rich.AppendLine("=======");
            foreach (var item in list.Items.Values.OrderBy(s => s.Name))
            {
                var reference = AddReference(item.StoryId, item.Name);
                _rich.AppendLine("[{1}] {2} .{0}", reference, item.Type, item.Name);
            }
        }

        public void When(StoryView view)
        {
            var txt = string.Format("Story: {0} .{1}", view.Name, AddReference(view.StoryId, view.Name));
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine(txt);
            }
            _rich.AppendLine(new string('=', txt.Length));

            var completed = view.Tasks.Where(c => !c.Completed).ToArray();
            if (view.Tasks.Count > 0)
            {
                using (_rich.Styled(Solarized.Green))
                {
                    _rich.AppendLine("## Tasks");
                }

                foreach (var task in completed)
                {
                    _rich.AppendLine(string.Format("  {1} {2} .{0}", AddReference(task.TaskId), task.Completed ? "x" : "□",
                            task.Text));
                }
                _rich.AppendLine();
            }

            if (view.Activities.Count > 0)
            {
                foreach (var activity in view.Activities)
                {
                    var text = activity.Text;
                    foreach (var r in activity.References)
                    {
                        var rid = AddReference(r.Item);
                        var newRef = string.Format("[{0}]({1})", r.Title, rid);
                        text = text.Replace(r.Source, newRef);
                    }
                    _rich.AppendLine(text);
                    using (_rich.Styled(Solarized.Base1))
                    {
                        var refid = AddReference(activity.ActivityId);
                        _rich.AppendLine("{0:yyyy-MM-dd HH:mm} .{1}", activity.CreatedUtc, refid);
                    }
                }
                _rich.AppendLine();
            }

            if (view.Notes.Count > 0)
            {
                _rich.AppendLine();
                using (_rich.Styled(Solarized.Green))
                {
                    _rich.AppendLine("## Notes");
                }

                foreach (var note in view.Notes.OrderBy(s => s.Title))
                {
                    _rich.AppendLine("{0} .{1}", note.Title, AddReference(note.NoteId));
                }
            }
            if (view.Notes.Count == 0 && view.Tasks.Count == 0 && view.Activities.Count == 0)
            {
                using (_rich.Styled(Solarized.Red))
                {
                    _rich.AppendLine("  Story is empty");
                }
            }
        }
    }
}