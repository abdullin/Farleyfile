using System;
using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class StoryViewTask
    {
        public TaskId TaskId { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
    }

    public sealed class StoryViewActivity
    {
        public ActivityId ActivityId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool Explicit { get; set; }

        public List<StoryViewActivityReference> References { get; set; }

        public StoryViewActivity()
        {
            References = new List<StoryViewActivityReference>();
        }
    }

    public sealed class StoryViewActivityReference
    {
        public Identity Item { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
    }
}