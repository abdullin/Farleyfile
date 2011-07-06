using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class NoteViewHandler : 
        IConsume<NoteAdded>,
        IConsume<NoteEdited>,
        IConsume<NoteRemoved>
    {
        readonly IAtomicEntityWriter<long, NoteView> _writer;
        public NoteViewHandler(IAtomicEntityWriter<long, NoteView> writer)
        {
            _writer = writer;
        }

        public void Consume(NoteAdded e)
        {
            _writer.Add(e.NoteId, new NoteView()
                {
                    Text = e.Text,
                    Title = e.Title
                });
        }

        public void Consume(NoteEdited e)
        {
            _writer.UpdateOrThrow(e.NoteId, nv => nv.Text = e.NewText);
        }

        public void Consume(NoteRemoved e)
        {
            _writer.TryDelete(e.NoteId);
        }
    }
}