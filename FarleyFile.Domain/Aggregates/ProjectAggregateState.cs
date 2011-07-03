using System;
using System.Collections.Generic;

namespace FarleyFile.Aggregates
{
    public sealed class ProjectAggregateState : IAggregateState
    {
        long _recordId;

        readonly Dictionary<long, TaskState> _tasks = new Dictionary<long, TaskState>();

        public void When(NoteAdded e)
        {
            StepRecordId(e.NoteId);
        }

        public long GetNextRecord()
        {
            return _recordId + 1;
        }

        public void When(TaskCompleted e)
        {
            _tasks[e.TaskId].Completed = true;
        }

        public TaskState GetTask(long task)
        {
            TaskState value;
            if (!_tasks.TryGetValue(task, out value))
            {
                throw new InvalidOperationException("Specified task does not exist");
            }
            return value;
        }

        public void When(TaskAdded e)
        {
            StepRecordId(e.TaskId);
            _tasks.Add(e.TaskId, new TaskState()
                {
                    Completed = false,
                    Name = e.Text
                });
        }

        void StepRecordId(long recordId)
        {
            if ((recordId) != (_recordId+1))
                throw new InvalidOperationException();

            _recordId = recordId;
        }

        public void Apply(IEvent e)
        {
            RedirectToWhen.InvokeEventOptional(this, e);
        }
    }

    public sealed class TaskState
    {
        public string Name { get; set; }
        public bool Completed { get; set; }
    } 

}