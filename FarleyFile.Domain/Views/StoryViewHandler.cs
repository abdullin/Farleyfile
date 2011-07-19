using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class StoryViewHandler : 
        IConsume<SimpleStoryStarted>,
        

        IConsume<NoteAssignedToStory>,

        IConsume<NoteRemovedFromStory>,
        
        IConsume<NoteRenamed>,
        
        IConsume<SimpleStoryRenamed>

    {
        readonly IAtomicEntityWriter<Identity,StoryView> _writer;
        public StoryViewHandler(IAtomicEntityWriter<Identity,StoryView> writer)
        {
            _writer = writer;
        }

        public void Consume(NoteAssignedToStory e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.AddNote(e.NoteId, e.Title, e.Text));
        }

        public void Consume(SimpleStoryStarted e)
        {
            _writer.Add(e.StoryId, new StoryView
                {
                    Name = e.Name,
                    StoryId = e.StoryId
                });
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

     

        public void Consume(SimpleStoryRenamed e)
        {
            _writer.UpdateOrThrow(e.StoryId, v => v.Name = e.NewName);
        }
    }
}