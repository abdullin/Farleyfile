using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarleyFile.Views;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionProcessor
    {
        readonly IMessageSender _sender;

        readonly IList<AbstractInteraction> _interactions = new List<AbstractInteraction>();

        public void LoadFromAssembly()
        {
            var bases = typeof (AbstractInteraction)
                .Assembly
                .GetExportedTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => typeof (AbstractInteraction).IsAssignableFrom(t));

            foreach (var @base in bases)
            {
                var info = @base.GetConstructor(Type.EmptyTypes);
                if (null == info)
                {
                    var message = string.Format("empty ctor not found for {0}", @base);
                    throw new InvalidOperationException(message);
                }
                _interactions.Add((AbstractInteraction) info.Invoke(null));
            }
        }

        

        void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        readonly LifelineViewport _viewport;
        readonly NuclearStorage _storage;
        long CurrentStoryId { get; set; }

        public InteractionProcessor(IMessageSender sender, LifelineViewport viewport, NuclearStorage storage)
        {
            _sender = sender;
            _viewport = viewport;
            _storage = storage;
        }

        public InteractionResult Handle(string data)
        {
            _viewport.Log("> " + data);
            

            foreach (var interaction in _interactions)
            {
                string @alias;
                string text;
                if (interaction.WillProcess(data, out @alias, out text))
                {
                    var request = new InteractionRequest(text, CurrentStoryId, @alias, data);
                    var response = new InteractionResponse(_viewport, (l, s) =>
                        {
                            _viewport.SelectStory(l, s);
                            CurrentStoryId = l;
                        }, _sender);
                    var context = new InteractionContext(request, _storage, response);

                    return interaction.Handle(context);
                }
            }

            if (data.StartsWith("an"))
            {
                var txt = data.Substring(2).TrimStart();
                var title = DateTime.Now.ToString("yyyy-MM-hh HH:mm");

                if (!string.IsNullOrEmpty(txt))
                {
                    SendToProject(new AddNote(title, txt, CurrentStoryId));
                }
                else
                {
                    var storyId = CurrentStoryId;
                    GrabFile("", (s, s1) => SendToProject(new AddNote(title, s, storyId)));
                }
                return InteractionResult.Handled;
            }
            
            if (data.StartsWith("cp "))
            {
                var txt = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var item = (txt[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                var story = int.Parse(txt[2]);
                SendToProject(item.Select(i => new AddToStory(int.Parse(i), story)).ToArray());
                return InteractionResult.Handled;
            }
            if (data.StartsWith("rm "))
            {
                var txt = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var item = (txt[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                var story = CurrentStoryId;
                if (txt.Length > 2)
                {
                    story = int.Parse(txt[2]);
                }
                SendToProject(item.Select(i => new RemoveFromStory(int.Parse(i), story)).ToArray());
                return InteractionResult.Handled;
            }
            
            if (data.StartsWith("cd "))
            {
                var txt = data.Substring(3).TrimStart();
                int storyId = 1;
                if (!string.IsNullOrEmpty(txt))
                {
                    storyId = int.Parse(txt);
                }
                TryLoadStory(storyId);
                return InteractionResult.Handled;
            }
            if (data.StartsWith("vim "))
            {
                var txt = data.Substring(3).TrimStart();
                int id = int.Parse(txt);

                var optional = _storage.GetEntity<NoteView>(id);
                if (!optional.HasValue)
                {
                    _viewport.Log("Note {0} does not exist", id);
                }
                else
                {
                    var note = optional.Value;
                    GrabFile(note.Text, (s, s1) => SendToProject(new EditNote(id, s, s1)));
                }
                return InteractionResult.Handled;
            }

            if (data.StartsWith("merge "))
            {
                var txt = data.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                long first = long.Parse(txt[1]);

                var commands = txt.Skip(2).Select(c => new MergeNotes(first, long.Parse(c))).ToArray();
                SendToProject(commands);
                return InteractionResult.Handled;
            }

            _viewport.Log("Unknown command sequence: {0}", data);
            return InteractionResult.Unknown;
        }

        static void GrabFile(string text, Action<string, string> whenDone)
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".md");
            File.WriteAllText(temp, text, Encoding.UTF8);
            var process = Process.Start("gvim.exe", temp);
            var changed = File.GetLastWriteTimeUtc(temp);
            if (null != process)
            {
                Task.Factory.StartNew(() => GrabInner(text, whenDone, process, temp, changed));
            }
        }

        static void GrabInner(string text, Action<string, string> whenDone, Process process, string temp, DateTime changed)
        {
            process.WaitForExit();
            if (File.Exists(temp))
            {
                if (changed < File.GetLastWriteTimeUtc(temp))
                {
                    var newText = File.ReadAllText(temp, Encoding.UTF8);
                    whenDone(newText, text);
                }
            }
        }

        public void TryLoadStory(long storyId)
        {
            var result = _storage.GetEntity<StoryView>(storyId);
            if (result.HasValue)
            {
                //_rich.Clear();
                var story = result.Value;
                _viewport.RenderStory(story, storyId);
                _viewport.SelectStory(story.StoryId, story.Name);
                CurrentStoryId = storyId;
            }
            else
            {
                _viewport.Log("Story {0} not found", storyId);
            }
        }
    }
}