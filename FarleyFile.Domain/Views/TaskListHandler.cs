using FarleyFile.Views;
using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile
{
    public sealed class TaskListHandler :
        IConsume<TaskAssignedToStory>,
        IConsume<TaskCompleted>,
        IConsume<TaskRenamed>,
        IConsume<TaskRemovedFromStory>
    {

        readonly IAtomicEntityWriter<Identity, TaskList> _writer;

        public TaskListHandler(IAtomicEntityWriter<Identity, TaskList> writer)
        {
            _writer = writer;
        }

        public void Consume(TaskRenamed e)
        {
            if (e.StoryIds == null) return;
            foreach (var storyId in e.StoryIds)
            {
                _writer.UpdateOrThrow(storyId, sv => sv.UpdateTask(e.TaskId, n => n.Text = e.NewText));
            }
        }

        public void Consume(TaskAssignedToStory e)
        {
            _writer.UpdateEnforcingNew(e.StoryId, sv => sv.AddTask(e.TaskId, e.Text, e.Completed));
        }


        public void Consume(TaskCompleted e)
        {
            foreach (var storyId in e.StoryIds)
            {
                _writer.UpdateOrThrow(storyId, sv => sv.UpdateTask(e.TaskId, t => t.Completed = true));
            }
        }

        public void Consume(TaskRemovedFromStory e)
        {
            _writer.UpdateOrThrow(e.StoryId, sv => sv.RemoveTask(e.TaskId));
        }
    }
}