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
}