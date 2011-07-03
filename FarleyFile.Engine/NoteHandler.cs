using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class NoteHandler : IConsume<NoteAdded>
    {
        readonly IAtomicSingletonWriter<DayText> _writer;
        public NoteHandler(IAtomicSingletonWriter<DayText> writer)
        {
            _writer = writer;
        }
        
        public void Consume(NoteAdded e)
        {
            _writer.UpdateEnforcingNew(d => d.Notes.Add(e.Text));
        }
    }
}