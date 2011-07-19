using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class NoteListHandler :
        IConsume<NoteAssignedToStory>,
        IConsume<NoteRemovedFromStory>,
        IConsume<NoteRenamed>
    {

        readonly IAtomicEntityWriter<Identity, NoteList> _writer;
        public NoteListHandler(IAtomicEntityWriter<Identity, NoteList> writer)
        {
            _writer = writer;
        }

        public void Consume(NoteAssignedToStory e)
        {
            _writer.UpdateEnforcingNew(e.StoryId, sv => sv.AddNote(e.NoteId, e.Title, e.Text));
        }

        public void Consume(NoteRemovedFromStory e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.RemoveNote(e.NoteId));
        }

        public void Consume(NoteRenamed e)
        {
            if (e.StoryIds == null) return;
            foreach (var storyId in e.StoryIds)
            {
                _writer.UpdateOrThrow(storyId, sv => sv.UpdateNote(e.NoteId, n => n.Title = e.NewName));
            }
        }
     
    }
}