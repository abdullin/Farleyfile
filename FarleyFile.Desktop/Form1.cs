using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Concurrency;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lokad.Cqrs;
using Lokad.Cqrs.Build.Engine;
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
        }

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var sub = _observable
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
            _rich.AppendText(string.Format(text, args));
            _rich.AppendText(Environment.NewLine);
        }

        
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (!e.Shift))
            {
                var data = textBox1.Text;
                e.SuppressKeyPress = true;

                Handle(data);
                textBox1.Clear();
            }
        }

        void Handle(string data)
        {
            if (data.StartsWith("an "))
            {
                var txt = data.Substring(3).TrimStart();
                SendToProject(new AddNote(txt, DateTime.Now));
                return;
            }
            if (data.StartsWith("at "))
            {
                var txt = data.Substring(3).TrimStart();
                SendToProject(new AddTask(txt, DateTime.Now));
                return;
            }
            if (data.StartsWith("ct "))
            {
                var txt = data.Substring(3).TrimStart();
                var id = int.Parse(txt);
                SendToProject(new CompleteTask(id, DateTime.Now));
            }
            if (data == "clr")
            {
                _rich.Clear();
                return;
            }
            if (data.StartsWith("day"))
            {
                var txt = data.Substring(3).TrimStart();
                int count = 0;
                if (!string.IsNullOrEmpty(txt))
                {
                    count = int.Parse(txt);
                }
                var builder = new StringBuilder();
                _renderer.RenderDay(count, builder);
                _rich.Text = builder.ToString();

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
