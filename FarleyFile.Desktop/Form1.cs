using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Drawing;
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

            _viewport = new LifelineViewport(_rich, label1);
            _processor = new InteractionProcessor(sender, _viewport, storage);
            
            label1.BackColor = Solarized.Base03;
            label1.ForeColor = Solarized.Base01;

            panel2.BackColor = Color.FromArgb(253, 246, 227); 
            _rich.BackColor = Solarized.Base3;
            _rich.ForeColor = Solarized.Base00;
            textBox1.BackColor = Solarized.Base03;
            textBox1.ForeColor = Solarized.Base0;

            
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
                            _viewport.Log(@event.ToString());
                        }
                    });
            _disposers.Add(sub);
            _processor.LoadFromAssembly();
            _processor.Handle("ls");
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
                
                try
                {
                    var interactionResult = _processor.Handle(data);
                    switch(interactionResult)
                    {
                        case InteractionResult.Handled:
                            textBox1.Clear();
                            break;
                        case InteractionResult.Unknown:
                            // do not clear
                            break;
                        case InteractionResult.Terminate:
                            Close();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }
                _rich.ScrollToCaret();
            }
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
