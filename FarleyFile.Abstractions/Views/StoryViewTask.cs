using System;

namespace FarleyFile.Views
{
    public sealed class StoryViewTask
    {
        public Guid TaskId { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
    }

    public sealed class StoryViewActivity
    {
        public Guid ActivityId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}