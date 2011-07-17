using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class StoryListHandler : 
        IConsume<SimpleStoryStarted>,
        IConsume<SimpleStoryRenamed>
    {
        readonly IAtomicSingletonWriter<StoryListView> _writer;

        public StoryListHandler(IAtomicSingletonWriter<StoryListView> writer)
        {
            _writer = writer;
        }

        public void Consume(SimpleStoryStarted e)
        {
            var item = new StoryListItem
                {
                    Name = e.Name,
                    StoryId = e.StoryId,
                    Type = "Simple"
                };
            _writer.UpdateEnforcingNew(v => v.Items.Add(e.StoryId.Id, item));
        }

        public void Consume(SimpleStoryRenamed e)
        {
            _writer.UpdateOrThrow(v => v.Items[e.StoryId.Id].Name = e.NewName);
        }
    }
}