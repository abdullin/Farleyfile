using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class StoryListHandler : 
        IConsume<SimpleStoryStarted>,
        IConsume<SimpleStoryRenamed>
    {
        readonly IAtomicSingletonWriter<StoryList> _writer;

        public StoryListHandler(IAtomicSingletonWriter<StoryList> writer)
        {
            _writer = writer;
        }

        public void Consume(SimpleStoryStarted e)
        {
            var item = new StoryList.Item
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