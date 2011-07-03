using System;

// this is a DSL to define code contract classes without writing them
// Good for starting a project quickly
// pressing Ctrl+S updates CS version immediately
namespace FarleyFile
{
    
    public sealed class MidnightPassed : IEvent
    {
    }
    
    public sealed class AddNote : ICommand
    {
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal AddNote () {}
        public AddNote (string text, DateTime date)
        {
            Text = text;
            Date = date;
        }
    }
    
    public sealed class NoteAdded : IEvent
    {
        public long NoteId { get; internal set; }
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal NoteAdded () {}
        public NoteAdded (long noteId, string text, DateTime date)
        {
            NoteId = noteId;
            Text = text;
            Date = date;
        }
    }
    
    public sealed class AddTask : ICommand
    {
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal AddTask () {}
        public AddTask (string text, DateTime date)
        {
            Text = text;
            Date = date;
        }
    }
    
    public sealed class TaskAdded : IEvent
    {
        public long TaskId { get; internal set; }
        public string Text { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal TaskAdded () {}
        public TaskAdded (long taskId, string text, DateTime date)
        {
            TaskId = taskId;
            Text = text;
            Date = date;
        }
    }
    
    public sealed class CompleteTask : ICommand
    {
        public long TaskId { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal CompleteTask () {}
        public CompleteTask (long taskId, DateTime date)
        {
            TaskId = taskId;
            Date = date;
        }
    }
    
    public sealed class TaskCompleted : IEvent
    {
        public long TaskId { get; internal set; }
        public DateTime Date { get; internal set; }
        
        internal TaskCompleted () {}
        public TaskCompleted (long taskId, DateTime date)
        {
            TaskId = taskId;
            Date = date;
        }
    }
}
