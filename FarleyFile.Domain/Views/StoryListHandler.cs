using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class StoryListHandler : 
        IConsume<SimpleStoryStarted>
    {
        readonly IAtomicSingletonWriter<StoryListView> _writer;

        public StoryListHandler(IAtomicSingletonWriter<StoryListView> writer)
        {
            _writer = writer;
        }

        public void Consume(SimpleStoryStarted e)
        {
            _writer.UpdateEnforcingNew(v => v.Items.Add(new StoryListItem
                {
                    Name = e.Name,
                    StoryId = e.StoryId,
                    Type = "Simple"
                }));
        }
    }
}