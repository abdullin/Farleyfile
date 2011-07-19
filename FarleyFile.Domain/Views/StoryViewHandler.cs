using System;
using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class StoryViewHandler : 
        IConsume<SimpleStoryStarted>,
        IConsume<SimpleStoryRenamed>

    {
        readonly IAtomicEntityWriter<Identity,StoryView> _writer;
        public StoryViewHandler(IAtomicEntityWriter<Identity,StoryView> writer)
        {
            _writer = writer;
        }

        public void Consume(SimpleStoryStarted e)
        {
            _writer.Add(e.StoryId, new StoryView
                {
                    Name = e.Name,
                    StoryId = e.StoryId
                });
        }

        public void Consume(SimpleStoryRenamed e)
        {
            _writer.UpdateOrThrow(e.StoryId, v => v.Name = e.NewName);
        }
    }
}