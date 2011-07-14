using System;

namespace FarleyFile.Views
{
    public sealed class StoryViewTask
    {
        public Guid TaskId { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
    }
}