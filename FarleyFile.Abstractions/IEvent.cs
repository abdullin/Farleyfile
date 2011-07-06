namespace FarleyFile
{
    public interface IEvent : IBaseMessage
    {

    }

    /// <summary>
    /// because we can't use [] for now
    /// </summary>
    
    public sealed class TaskCompleted : IEvent
    {
        public long TaskId { get; internal set; }
        public long[] StoryIds { get; internal set; }

        internal TaskCompleted() { }
        public TaskCompleted(long taskId, long[] storyIds)
        {
            TaskId = taskId;
            StoryIds = storyIds;
        }
    }
}