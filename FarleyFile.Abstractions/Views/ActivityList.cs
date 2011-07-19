using System;
using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class ActivityList : IEntityBase
    {
        public IList<Item> List { get; set; }

        public ActivityList()
        {
            List = new List<Item>();
        }


        public void AddActivity(ActivityAdded e)
        {
            var activity = new Item()
                {
                    ActivityId = e.ActivityId,
                    Text = e.Text,
                    Created = e.Time,
                    Explicit = true
                };

            foreach (var reference in e.References)
            {
                activity.References.Add(new Reference
                    {
                        Item = reference.Id,
                        Source = reference.OriginalRef,
                        Title = reference.Text
                    });
            }
            List.Add(activity);
        }

        public sealed class Item
        {
            public ActivityId ActivityId { get; set; }
            public string Text { get; set; }
            public DateTimeOffset Created { get; set; }
            public bool Explicit { get; set; }

            public List<Reference> References { get; set; }

            public Item()
            {
                References = new List<Reference>();
            }
        }

        public sealed class Reference
        {
            public Identity Item { get; set; }
            public string Title { get; set; }
            public string Source { get; set; }
        }
    }
}