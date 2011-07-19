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

        public void RemoveTask(TaskId taskId)
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
}