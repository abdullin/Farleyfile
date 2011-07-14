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
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (!e.Shift))
            {
                var data = _input.Text;
                e.SuppressKeyPress = true;

                var interactionResult = _processor.Handle(data);
                switch (interactionResult)
                {
                    case InteractionResultStatus.Handled:
                        _input.Clear();
                        break;
                    case InteractionResultStatus.Unknown:
                        // do not clear, let user fix and retype
                        break;
                    case InteractionResultStatus.Terminate:
                        Close();
                        break;
                }
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