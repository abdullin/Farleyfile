using System;
using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class StoryList
    {
        public IDictionary<Guid,Item> Items { get; private set; }

        public StoryList()
        {
            Items = new Dictionary<Guid, Item>();
        }

        public sealed class Item
        {
            public StoryId StoryId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }
    }
}