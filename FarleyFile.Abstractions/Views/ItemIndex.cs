using System;
using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class ItemIndex 
    {
        public sealed class Leaf
        {
            public string Name { get; set; }
            public Identity Id { get; set; }
        }

        public IDictionary<Guid, Leaf> Index { get; private set; }

        

        public ItemIndex()
        {
            Index = new Dictionary<Guid, Leaf>();
        }
    }
}