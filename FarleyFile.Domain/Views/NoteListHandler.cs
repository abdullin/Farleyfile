using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class NoteListHandler :
        IConsume<NoteAdded>,
        IConsume<NoteArchived>,
        IConsume<NoteRenamed>
    {

        readonly IAtomicEntityWriter<Identity, NoteList> _writer;
        public NoteListHandler(IAtomicEntityWriter<Identity, NoteList> writer)
        {
            _writer = writer;
        }

        public void Consume(NoteRenamed e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.UpdateNote(e.NoteId, n => n.Title = e.NewName));
        }

        public void Consume(NoteAdded e)
        {
            _writer.UpdateEnforcingNew(e.StoryId, sv => sv.AddNote(e.NoteId, e.Title, e.Text));
        }

        public void Consume(NoteArchived e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.ArchiveNote(e.NoteId));
        }
    }
}