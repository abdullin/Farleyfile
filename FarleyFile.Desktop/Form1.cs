using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Concurrency;
using System.Drawing;
using System.Linq;
using System.Text;
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

            panel2.BackColor = Color.FromArgb(253, 246, 227); 
            _rich.BackColor = Solarized.Base3;
            _rich.ForeColor = Solarized.Base00;
            textBox1.BackColor = Solarized.Base03;
            textBox1.ForeColor = Solarized.Base0;

        }

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
                    using (_rich.Styled(Solarized.Red))
                    {
                        Log(ex.Message);
                    }
                }
                _rich.ScrollToCaret();
                textBox1.Clear();
            }
        }

        DateTime _currentDay = DateTime.Now.Date;

        void Handle(string data)
        {
            if (data.StartsWith("an "))
            {
                var txt = data.Substring(3).TrimStart();
                var title = DateTime.Now.ToString("HH:mm");
                SendToProject(new AddNote(title, txt));
                return;
            }
            if (data.StartsWith("at "))
            {
                var txt = data.Substring(3).TrimStart();
                SendToProject(new AddTask(txt));
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
            if (data.StartsWith("ss "))
            {
                var txt = data.Substring(3).TrimStart();
                SendToProject(new StartSimpleStory(txt));
                return;
            }
            if (data.StartsWith("add "))
            {
                var txt = data.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var item = int.Parse(txt[1]);
                var story = int.Parse(txt[2]);
                SendToProject(new AddToStory(item, story));
            }
            if (data == "stories")
            {
                var view = _storage.GetSingletonOrNew<StoryListView>();
                _renderer.RenderStoryList(_rich, view);
                return;
            }
            if (data.StartsWith("story"))
            {
                var txt = data.Substring(5).TrimStart();
                int count = 1;
                if (!string.IsNullOrEmpty(txt))
                {
                    count = int.Parse(txt);
                }

                var result = _storage.GetEntity<StoryView>(count);
                if (result.HasValue)
                {
                    var story = result.Value;
                    
                    _renderer.RenderStory(_rich, story);
                }
                else
                {
                    Log("Story {0} not found", count);
                }

                return;
            }

            Log("Unknown command sequence: {0}", data);
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
