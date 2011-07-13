using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {

        public readonly InteractionRequest Request;
        public readonly InteractionResponse Response;
        public readonly NuclearStorage Storage;

        public InteractionContext(InteractionRequest request, NuclearStorage storage, InteractionResponse response)
        {
            Request = request;
            Storage = storage;
            Response = response;
        }
    }

    public sealed class InteractionResponse
    {
        public readonly LifelineViewport Viewport;
        readonly Action<long, string> _action;
        readonly IMessageSender _sender;
        

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public InteractionResponse(LifelineViewport viewport, Action<long, string> action, IMessageSender sender)
        {
            Viewport = viewport;
            _action = action;
            _sender = sender;
        }

        public void SetCurrentStory(long storyId, string storyName)
        {
            _action(storyId, storyName);
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

        
    }

    public sealed class InteractionRequest
    {
        public readonly string Raw;
        public readonly string Alias;
        public readonly string Data;
        public readonly long CurrentStoryId;

        public InteractionRequest(string data, long currentStoryId, string @alias, string raw)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Alias = alias;
            Raw = raw;
        }
    }
}