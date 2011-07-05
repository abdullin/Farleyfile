using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class StoryViewHandler : 
        IConsume<SimpleStoryStarted>,
        IConsume<TaskAssignedToStory>,
        IConsume<NoteAssignedToStory>,
        IConsume<TaskCompleted>,
        IConsume<ContactStoryStarted>,
        IConsume<DayStoryStarted>

    {
        readonly IAtomicEntityWriter<long,StoryView> _writer;
        public StoryViewHandler(IAtomicEntityWriter<long,StoryView> writer)
        {
            _writer = writer;
        }

        public void Consume(TaskAssignedToStory e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.AddTask(e.TaskId, e.Text, e.Completed));
        }

        public void Consume(NoteAssignedToStory e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.AddNote(e.NoteId, e.Title, e.Text));
        }


        public void Consume(TaskCompleted e)
        {
            foreach (var storyId in e.StoryIds)
            {
                _writer.UpdateOrThrow(storyId, sv => sv.UpdateTask(e.TaskId, t => t.Completed = true));
            }
        }

        public void Consume(SimpleStoryStarted e)
        {
            _writer.Add(e.StoryId, new StoryView { Name = e.Name });
        }

        public void Consume(ContactStoryStarted e)
        {
            _writer.Add(e.ContactId, new StoryView {Name = e.Name});
        }

        public void Consume(DayStoryStarted e)
        {
            _writer.Add(e.DayId, new StoryView { Name = e.Date.ToString("yyyy-MM-dd") });
        }
    }

    
}