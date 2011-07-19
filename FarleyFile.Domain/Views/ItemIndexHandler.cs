using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Views
{
    public sealed class ItemIndexHandler :
        IConsume<SimpleStoryStarted>,
        IConsume<SimpleStoryRenamed>,
        IConsume<NoteAdded>,
        IConsume<TaskAdded>,
        IConsume<NoteRenamed>,
        IConsume<TaskRenamed>
    {
        readonly IAtomicSingletonWriter<ItemIndex> _writer;
        public ItemIndexHandler(IAtomicSingletonWriter<ItemIndex> writer)
        {
            _writer = writer;
        }

        void AddRecord(Identity id, string name)
        {
            _writer.UpdateEnforcingNew(i => i.Index.Add(id.Id, new ItemIndex.Leaf()
                {
                    Id = id,
                    Name = name
                }));
        }
        void Rename(Identity id, string name)
        {
            _writer.UpdateOrThrow(i => i.Index[id.Id].Name = name);
        }

        public void Consume(SimpleStoryStarted e)
        {
            AddRecord(e.StoryId, e.Name);
        }

        public void Consume(SimpleStoryRenamed e)
        {
            Rename(e.StoryId,e.NewName);
        }

        public void Consume(NoteAdded e)
        {
            AddRecord(e.NoteId, e.Title);
        }

        public void Consume(TaskAdded e)
        {
            AddRecord(e.TaskId, e.Text);
        }

        public void Consume(NoteRenamed e)
        {
            Rename(e.NoteId, e.NewName);
        }

        public void Consume(TaskRenamed e)
        {
            Rename(e.TaskId, e.NewText);
        }
    }
}