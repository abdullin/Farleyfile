using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class TaskListHandler :
        IConsume<TaskAdded>,
        IConsume<TaskCompleted>,
        IConsume<TaskRenamed>,
        IConsume<TaskArchived>
    {

        readonly IAtomicEntityWriter<Identity, TaskList> _writer;

        public TaskListHandler(IAtomicEntityWriter<Identity, TaskList> writer)
        {
            _writer = writer;
        }

        public void Consume(TaskRenamed e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.UpdateTask(e.TaskId, n => n.Text = e.NewText));
        }

        public void Consume(TaskAdded e)
        {
            _writer.UpdateEnforcingNew(e.StoryId, sv => sv.AddTask(e.TaskId, e.Text, false));
        }


        public void Consume(TaskCompleted e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.UpdateTask(e.TaskId, t => t.Completed = true));
        }

        public void Consume(TaskArchived e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.ArchiveTask(e.TaskId));
        }
    }

    public sealed class TagListHandler : IConsume<TagCreated>
    {
        IAtomicSingletonWriter<TagList> _writer;
        public TagListHandler(IAtomicSingletonWriter<TagList> writer)
        {
            _writer = writer;
        }

        public void Consume(TagCreated e)
        {
            _writer.UpdateEnforcingNew(v => v.Items.Add(e.Tag, e.TagId));
        }
    }

    public sealed class TagViewHandler : 
        IConsume<TagAddedToStory>,
        IConsume<TagCreated>
    {
        IAtomicEntityWriter<TagId, TagView> _writer;
        public TagViewHandler(IAtomicEntityWriter<TagId, TagView> writer)
        {
            _writer = writer;
        }

        public void Consume(TagAddedToStory e)
        {
            _writer.UpdateOrThrow(e.TagId, v => v.AddStory(e.StoryId, e.StoryName));
        }

        public void Consume(TagCreated e)
        {
            _writer.Add(e.TagId, new TagView()
                {
                    Id = e.TagId,
                    Name = e.Tag
                });
        }
    }
}