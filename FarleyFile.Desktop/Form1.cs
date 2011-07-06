using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Concurrency;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FarleyFile.Views;
using Lokad.Cqrs;
using Lokad.Cqrs.Build.Engine;
using Lokad.Cqrs.Core.Dispatch.Events;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Desktop
{
    public partial class Form1 : Form
    {
        readonly CqrsEngineHost _host;
        readonly IObservable<ISystemEvent> _observable;
        IMessageSender _sender;
        NuclearStorage _storage;
        TextRenderers _renderer;

        IList<IDisposable> _disposers = new List<IDisposable>();

        public Form1(CqrsEngineHost host, IObservable<ISystemEvent> observable)
        {
            _host = host;
            _observable = observable;
            InitializeComponent();

            _sender = _host.Resolve<IMessageSender>();
            _storage = _host.Resolve<NuclearStorage>();

            _renderer = new TextRenderers(_storage);

            label1.BackColor = Solarized.Base03;
            label1.ForeColor = Solarized.Base01;

            panel2.BackColor = Color.FromArgb(253, 246, 227); 
            _rich.BackColor = Solarized.Base3;
            _rich.ForeColor = Solarized.Base00;
            textBox1.BackColor = Solarized.Base03;
            textBox1.ForeColor = Solarized.Base0;
        }

        long CurrentStoryId { get; set; }

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var sub = _observable
                .Where(i =>
                    {
                        var acked = i as EnvelopeAcked;
                        if (acked != null)
                        {
                            return acked.QueueName == "router";
                        }
                        if (i is EventHadNoConsumers)
                            return false;
                        return true;
                    })
                .BufferWithTime(TimeSpan.FromMilliseconds(200), Scheduler.Dispatcher)
                .Subscribe(list =>
                    {
                        foreach (var @event in list)
                        {
                            Log(@event.ToString());
                        }
                    });
            _disposers.Add(sub);
            SelectStory(1, "Draft");
            LoadStory(1);
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
            catch(ObjectDisposedException)
            {
                
            }
        }
        public void Error(string text, params object[] args)
        {
            using (_rich.Styled(Solarized.Red))
            {
                _rich.AppendLine(text, args); 
                _rich.ScrollToCaret();
            }
        }

        
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (!e.Shift))
            {
                var data = textBox1.Text;
                e.SuppressKeyPress = true;

                Log("> " + data);
                try
                {
                    Handle(data);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
                _rich.ScrollToCaret();
                textBox1.Clear();
            }
        }

        void Handle(string data)
        {
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
                return;
            }
            if (data.StartsWith("at "))
            {
                var txt = data.Substring(3).TrimStart();
                SendToProject(new AddTask(txt, CurrentStoryId));
                return;
            }
            if (data == "q")
            {
                Close();
            }
            if (data.StartsWith("ct "))
            {
                var txt = data.Substring(3).TrimStart();
                var id = int.Parse(txt);
                SendToProject(new CompleteTask(id));
                return;
            }
            if (data == "clr")
            {
                _rich.Clear();
                return;
            }
            if (data.StartsWith("new "))
            {
                var txt = data.Substring(4).TrimStart();
                SendToProject(new StartSimpleStory(txt));
                return;
            }
            if (data.StartsWith("cp "))
            {
                var txt = data.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var item = (txt[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                var story = int.Parse(txt[2]);
                SendToProject(item.Select(i => new AddToStory(int.Parse(i), story)).ToArray());
                return;
            }
            if (data.StartsWith("rm "))
            {
                var txt = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var item = (txt[1].Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries));
                var story = CurrentStoryId;
                if (txt.Length > 2)
                {
                    story = int.Parse(txt[2]);
                }
                SendToProject(item.Select(i => new RemoveFromStory(int.Parse(i), story)).ToArray());
                return;
            }
            if (data == "ls")
            {
                var view = _storage.GetSingletonOrNew<StoryListView>();
                _renderer.RenderStoryList(_rich, view);
                return;
            }
            if (data.StartsWith("cd "))
            {
                var txt = data.Substring(3).TrimStart();
                int storyId = 1;
                if (!string.IsNullOrEmpty(txt))
                {
                    storyId = int.Parse(txt);
                }
                LoadStory(storyId);
                return;
            }
            if (data.StartsWith("vim "))
            {
                var txt = data.Substring(3).TrimStart();
                int id = int.Parse(txt);

                var optional = _storage.GetEntity<NoteView>(id);
                if (!optional.HasValue)
                {
                    Log("Note {0} does not exist", id);
                }
                else
                {
                    var note = optional.Value;
                    GrabFile(note.Text, (s, s1) => SendToProject(new EditNote(id,s,s1)));
                }
                return;
            }
            if (data.StartsWith("merge "))
            {
                var txt = data.Split(new[] { ' ',',' }, StringSplitOptions.RemoveEmptyEntries);
                long first = long.Parse(txt[1]);

                var commands = txt.Skip(2).Select(c => new MergeNotes(first, long.Parse(c))).ToArray();
                SendToProject(commands);
            }
            if (data == "")
            {
                LoadStory(CurrentStoryId);
                return;
            }
            if (data == " ")
            {
                _rich.Clear();
                LoadStory(CurrentStoryId);
                return;
            }

            Log("Unknown command sequence: {0}", data);
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

        void LoadStory(long storyId)
        {
            var result = _storage.GetEntity<StoryView>(storyId);
            if (result.HasValue)
            {
                //_rich.Clear();
                var story = result.Value;
                _renderer.RenderStory(_rich, story, storyId);
                SelectStory(storyId, story.Name);
            }
            else
            {
                Log("Story {0} not found", storyId);
            }

        }

        public void SelectStory(long storyId, string storyName)
        {
            label1.Text = string.Format("Story: {0} ({1})", storyName, storyId);
            CurrentStoryId = storyId;
        }


        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            //if ((e.KeyCode == Keys.Enter) && (e.Control))
            //{
            //    textBox1.Clear();
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var disposable in _disposers)
            {
                disposable.Dispose();
            }
        }
    }
}
