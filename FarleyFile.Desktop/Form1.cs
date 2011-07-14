using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Windows.Forms;
using FarleyFile.Interactions;
using Lokad.Cqrs;
using Lokad.Cqrs.Build.Engine;
using Lokad.Cqrs.Core.Dispatch.Events;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public partial class Form1 : Form
    {
        readonly CqrsEngineHost _host;
        readonly IObservable<ISystemEvent> _observable;
        readonly LifelineViewport _viewport;
        readonly InteractionProcessor _processor;
        readonly IList<IDisposable> _disposers = new List<IDisposable>();

        public Form1(CqrsEngineHost host, IObservable<ISystemEvent> observable)
        {
            _host = host;
            _observable = observable;
            InitializeComponent();

            var sender = _host.Resolve<IMessageSender>();
            var storage = _host.Resolve<NuclearStorage>();

            _viewport = new LifelineViewport(_rich, _status);
            _processor = new InteractionProcessor(sender, _viewport, storage);

            _panel.BackColor = Solarized.Base3;
            _rich.BackColor = Solarized.Base3;
            _rich.ForeColor = Solarized.Base00;
            _input.BackColor = Solarized.Base03;
            _input.ForeColor = Solarized.Base0;
            _status.BackColor = Solarized.Base03;
            _status.ForeColor = Solarized.Base01;
        }

        void Form1_Load(object sender, EventArgs e)
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
                            _viewport.Log(@event.ToString());
                        }
                    });
            _disposers.Add(sub);
            _processor.LoadFromAssembly();
            _processor.Handle("ls");
            _rich.ScrollToCaret();
        }


        readonly LinkedList<string> _inputBuffer = new LinkedList<string>();
        LinkedListNode<string> _currentBuffer;

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // common console short-cuts.
            if (e.KeyCode == Keys.Escape)
            {
                _input.Clear();
                return;
            }
            if (e.KeyCode == Keys.Up)
            {
                var prev = _currentBuffer == null ? _inputBuffer.Last : _currentBuffer.Previous;
                _input.Text = prev== null ? "" : prev.Value;
                _input.SelectionStart = _input.Text.Length;
                _currentBuffer = prev;
                return;
            }
            if (e.KeyCode == Keys.Down)
            {
                var next = _currentBuffer == null ? _inputBuffer.First : _currentBuffer.Next;
                _input.Text = next == null ? "" : next.Value;
                _input.SelectionStart = _input.Text.Length;
                _currentBuffer = next;
                return;
            }
            if ((e.KeyCode == Keys.Enter) && (!e.Shift))
            {
                var data = _input.Text;
                e.SuppressKeyPress = true;

                var result = _processor.Handle(data);
                if (result == InteractionResultStatus.Terminate)
                {
                    Close();
                    return;
                }
                _inputBuffer.AddLast(data);
                _currentBuffer = null;
                _input.Clear();
                _rich.ScrollToCaret();
            }
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var disposable in _disposers)
            {
                disposable.Dispose();
            }
        }
    }
}