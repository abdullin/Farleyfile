using System;
using System.Collections.Generic;
using System.IO;
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
        

        string AddReference(Identity i, params object[] names)
        {
            if (i.IsEmpty)
                throw new InvalidOperationException("Can't add an empty reference");

            i = IdentityEvil.Upcast(i);

            string readable;
            if (!_lookupId.TryGetValue(i, out readable))
            {
                _lookupReference += 1;
                readable = _lookupReference.ToString();
                _lookupId.Add(i, readable);

                // overwrite reverse lookup
                LookupRef[readable] = i;
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
                    LookupRef.Add(name, i);
                }
                else
                {
                    // collision
                    if (!reference.Equals(i))
                    {
                        LookupRef[name] = null;
                        Log("Collision: '{0}' -> '{1}' by '{2}'", reference, i, name);
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
            _statusLabel.Text = string.Format("Story: {0}", storyName);
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
            RedirectToWhen.InvokeOptional(this, view, (i,v) =>
                {
                    using (_rich.Styled(Solarized.Yellow))
                    {
                        _rich.AppendLine(view.GetType().Name);
                    }
                    _rich.AppendText(ServiceStack.Text.TypeSerializer.SerializeAndFormat(view));
                });
        }



        public void When(StoryList list)
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

        public void When(TagList list)
        {
            using (_rich.Styled(Solarized.Violet))
            {
                _rich.AppendLine("Tags");
            }
            foreach (var item in list.Items.OrderBy(s => s.Key))
            {
                var reference = AddReference(item.Value, item.Key);
                _rich.AppendLine("{0} .{1}", item.Key, reference);
            }
        }

        public void When(FocusComposite composite)
        {
            //var view = composite.View;

            string txt;
            if (composite.Focus is StoryId)
            {
                txt = string.Format("Story: {0} .{1}", composite.Name, AddReference(composite.Focus, composite.Name));
            }
            else if (composite.Focus is TagId)
            {
                txt = string.Format("Tag: {0} .{1}", composite.Name, AddReference(composite.Focus, composite.Name));
            }
            else
            {
                throw new InvalidOperationException("Unexpected thing to focus on");
            }


            
            using (_rich.Styled(Solarized.Yellow))
            {
                _rich.AppendLine(txt);
            }
            _rich.AppendLine(new string('=', txt.Length));

            var tasks = composite.Tasks.Where(c => !c.Completed).ToList();
            if (tasks.Count > 0)
            {
                using (_rich.Styled(Solarized.Green))
                {
                    _rich.AppendLine("## Tasks");
                }

                foreach (var task in tasks)
                {
                    _rich.AppendLine(string.Format("  {1} {2} .{0}", AddReference(task.TaskId), task.Completed ? "x" : "□",
                            task.Text));
                }
                _rich.AppendLine();
            }

            var activities = composite.Activities;

            if (activities.Count > 0)
            {
                foreach (var activity in activities)
                {
                    
                    var text = activity.Text;
                    var explicitRefs = activity.References.Where(r => !string.IsNullOrEmpty(r.Source));
                    
                    foreach (var r in explicitRefs)
                    {
                        var rid = AddReference(r.Item);
                        var newRef = string.Format("[{0}].{1}", r.Title, rid);
                        text = text.Replace(r.Source, newRef);
                    }
                    _rich.AppendLine(text);
                    var implicitRefs = activity.References.Where(r => string.IsNullOrEmpty(r.Source)).ToList();
                    using (_rich.Styled(Solarized.Base1))
                    {
                        _rich.AppendText("  -- " + FormatEvil.OffsetUtc(activity.Created));
                        if (implicitRefs.Any())
                        {
                            var values = implicitRefs
                                .Select(r => string.Format("[{0}].{1}", r.Title, AddReference(r.Item))).ToArray();
                            _rich.AppendText(" from " + string.Join(", ", values));
                        }
                        _rich.AppendLine();
                    }
                    
                }
                _rich.AppendLine();
            }

            var notes = composite.Notes;

            if (notes.Count > 0)
            {
                _rich.AppendLine();
                using (_rich.Styled(Solarized.Green))
                {
                    _rich.AppendLine("## Notes");
                }

                foreach (var note in notes.OrderBy(s => s.Title))
                {
                    _rich.AppendLine("{0} .{1}", note.Title, AddReference(note.NoteId));
                }
            }
            if (notes.Count == 0 && tasks.Count == 0 && activities.Count == 0)
            {
                using (_rich.Styled(Solarized.Red))
                {
                    _rich.AppendLine("  Story is empty");
                }
            }
        }
    }
}