using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class StoryListHandler : 
        IConsume<SimpleStoryStarted>,
        IConsume<ContactStoryStarted>,
        IConsume<DayStoryStarted>
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

        public void Consume(ContactStoryStarted e)
        {
            _writer.UpdateEnforcingNew(v => v.Items.Add(new StoryListItem
                {
                    Name = e.Name,
                    StoryId = e.ContactId,
                    Type = "Contact"
                }));
        }

        public void Consume(DayStoryStarted e)
        {
            _writer.UpdateEnforcingNew(v => v.Items.Add(new StoryListItem
                {
                    Name = e.Date.ToString("yyyy-MM-dd"),
                    StoryId = e.DayId,
                    Type = "Day"
                }));
        }
    }
}