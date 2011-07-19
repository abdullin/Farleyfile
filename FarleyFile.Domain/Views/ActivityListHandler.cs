using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class ActivityListHandler : 
        IConsume<ActivityAdded>
    {
        readonly IAtomicEntityWriter<Identity, ActivityList> _writer;

        public ActivityListHandler(IAtomicEntityWriter<Identity, ActivityList> writer)
        {
            _writer = writer;
        }

        public void Consume(ActivityAdded e)
        {
            foreach (var reference in e.References)
            {
                if (reference.Id.Tag == StoryId.Tag)
                {
                    _writer.UpdateEnforcingNew(reference.Id, v => v.AddActivity(e));
                }
            }
        }
    }
}