using System;
using FarleyFile.Views;
using Lokad.Cqrs;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class SystemClient
    {
        readonly IMessageSender _sender;
        readonly NuclearStorage _storage;
        public SystemClient(IMessageSender sender, NuclearStorage storage)
        {
            _sender = sender;
            _storage = storage;
        }

        public void SendToProject(params ICommand[] commands)
        {
            _sender.SendBatch(commands, eb => eb.AddString("to-entity", "default"));
        }

        public void PrintDay(int shift)
        {
            var date = DateTime.Now.Date.AddDays(shift).ToString("yyyy-MM-dd");

            var result = _storage.GetEntity<DayView>(date);

            Console.Clear();
            Console.WriteLine(new string('_', Console.WindowWidth-12) + " " +date);
            

            if (!result.HasValue)
            {
                Console.WriteLine("Empty");
                return;
            }
            var notes = result.Value.Notes;

            if (notes.Count > 0)
            {
                foreach (var note in notes)
                {
                    Console.WriteLine(" " +note.Date.ToString("HH:mm"));
                    Console.WriteLine(note.Text);
                    Console.WriteLine();
                }
            }
        }
    }
}