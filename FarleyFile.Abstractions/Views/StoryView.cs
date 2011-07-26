using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class StoryView : IEntityBase
    {
        public string Name { get; set; }
        public StoryId StoryId { get; set; }
        public IDictionary<string, TagId> Tags { get; private set; }

        public StoryView()
        {
            Tags = new Dictionary<string,TagId>();
        }

        
    }
}