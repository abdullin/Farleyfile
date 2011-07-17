using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lokad.Cqrs;

namespace FarleyFile.Interactions
{
    public sealed class InteractionResponse
    {
        readonly LifelineViewport _viewport;
        readonly Action<StoryId> _action;
        readonly IMessageSender _sender;

        public void FocusStory(StoryId id, string name)
        {
            _action(id);
            _viewport.SelectStory(id, name);
        }

        public void RenderView<T>(T view)
        {
            _viewport.RenderView(view);
        }

        public void ClearView()
        {
            _viewport.Clear();
        }

        public void Log(string message, params object[] args)
        {
            _viewport.Log(message, args);
        }

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionResponse(LifelineViewport viewport, Action<StoryId> action, IMessageSender sender)
        {
            _viewport = viewport;
            _action = action;
            _sender = sender;
        }

        public void GrabFile(string text, Action<string, string> whenDone)
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

        static void GrabInner(string text, Action<string, string> whenDone, Process process, string temp,
            DateTime changed)
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
    }
}