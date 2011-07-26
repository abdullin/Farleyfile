using System;
using System.Collections.Generic;
using System.Linq;

namespace FarleyFile.Views
{
    public sealed class TaskList : IEntityBase
    {
        public IList<Item> List { get; set; }

        public TaskList()
        {
            List = new List<Item>();
        }

        public void ArchiveTask(TaskId taskId)
        {
            var tasks = List.Where(n => n.TaskId == taskId).ToList();
            foreach (var task in tasks)
            {
                List.Remove(task);
            }
        }

        public void AddTask(TaskId taskId, string text, bool completed)
        {
            List.Add(new Item()
                {
                    TaskId = taskId,
                    Text = text,
                    Completed = completed
                });
        }

        public void UpdateTask(TaskId taskId, Action<Item> update)
        {
            foreach (var task in List.Where(t => t.TaskId == taskId))
            {
                update(task);
            }
        }

        public sealed class Item
        {
            public TaskId TaskId { get; set; }
            public string Text { get; set; }
            public bool Completed { get; set; }
        }
    }

    public sealed class TagList : IEntityBase
    {
        public Dictionary<string, TagId> Items { get; private set; }

        public TagList()
        {
            Items = new Dictionary<string, TagId>();
        }
    }

    public sealed class TagView : IEntityBase
    {
        public string Name { get; set; }
        public TagId Id { get; set; }

        public List<StoryItem> Stories { get; private set; }

        public void AddStory(StoryId id, string name)
        {
            Stories.Add(new StoryItem()
                {
                    Name = name,
                    Story = id
                });
        }

        public TagView()
        {
            Stories = new List<StoryItem>();
        }


        public sealed class StoryItem
        {
            public string Name { get; set; }
            public StoryId Story { get; set; }
        }
    }
}