﻿using System;
using System.Runtime.Serialization;

// this is a DSL to define code contract classes without writing them
// Good for starting a project quickly
// pressing Ctrl+S updates CS version immediately
namespace FarleyFile
{
    #region Generated by Lokad Code DSL
    
    [DataContract] public sealed class PerspectiveCreated : IEvent
    {
        [DataMember(Order = 1)] public long DraftId { get; internal set; }
        
        internal PerspectiveCreated () {}
        public PerspectiveCreated (long draftId)
        {
            DraftId = draftId;
        }
    }
    
    [DataContract] public sealed class StartSimpleStory : ICommand
    {
        [DataMember(Order = 1)] public string Name { get; internal set; }
        
        internal StartSimpleStory () {}
        public StartSimpleStory (string name)
        {
            Name = name;
        }
    }
    
    [DataContract] public sealed class SimpleStoryStarted : IEvent
    {
        [DataMember(Order = 1)] public long StoryId { get; internal set; }
        [DataMember(Order = 2)] public string Name { get; internal set; }
        
        internal SimpleStoryStarted () {}
        public SimpleStoryStarted (long storyId, string name)
        {
            StoryId = storyId;
            Name = name;
        }
    }
    
    [DataContract] public sealed class StartDayStory : ICommand
    {
        [DataMember(Order = 1)] public DateTime Date { get; internal set; }
        
        internal StartDayStory () {}
        public StartDayStory (DateTime date)
        {
            Date = date;
        }
    }
    
    [DataContract] public sealed class DayStoryStarted : IEvent
    {
        [DataMember(Order = 1)] public long DayId { get; internal set; }
        [DataMember(Order = 2)] public DateTime Date { get; internal set; }
        
        internal DayStoryStarted () {}
        public DayStoryStarted (long dayId, DateTime date)
        {
            DayId = dayId;
            Date = date;
        }
    }
    
    [DataContract] public sealed class StartContactStory : ICommand
    {
        [DataMember(Order = 1)] public string Name { get; internal set; }
        
        internal StartContactStory () {}
        public StartContactStory (string name)
        {
            Name = name;
        }
    }
    
    [DataContract] public sealed class ContactStoryStarted : IEvent
    {
        [DataMember(Order = 1)] public long ContactId { get; internal set; }
        [DataMember(Order = 2)] public string Name { get; internal set; }
        
        internal ContactStoryStarted () {}
        public ContactStoryStarted (long contactId, string name)
        {
            ContactId = contactId;
            Name = name;
        }
    }
    
    [DataContract] public sealed class AddNote : ICommand
    {
        [DataMember(Order = 1)] public string Title { get; internal set; }
        [DataMember(Order = 2)] public string Text { get; internal set; }
        [DataMember(Order = 3)] public long StoryId { get; internal set; }
        
        internal AddNote () {}
        public AddNote (string title, string text, long storyId)
        {
            Title = title;
            Text = text;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class NoteAdded : IEvent
    {
        [DataMember(Order = 1)] public long NoteId { get; internal set; }
        [DataMember(Order = 2)] public string Title { get; internal set; }
        [DataMember(Order = 3)] public string Text { get; internal set; }
        
        internal NoteAdded () {}
        public NoteAdded (long noteId, string title, string text)
        {
            NoteId = noteId;
            Title = title;
            Text = text;
        }
    }
    
    [DataContract] public sealed class NoteAssignedToStory : IEvent
    {
        [DataMember(Order = 1)] public long NoteId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        [DataMember(Order = 3)] public string Title { get; internal set; }
        [DataMember(Order = 4)] public string Text { get; internal set; }
        
        internal NoteAssignedToStory () {}
        public NoteAssignedToStory (long noteId, long storyId, string title, string text)
        {
            NoteId = noteId;
            StoryId = storyId;
            Title = title;
            Text = text;
        }
    }
    
    [DataContract] public sealed class NoteRemovedFromStory : IEvent
    {
        [DataMember(Order = 1)] public long NoteId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal NoteRemovedFromStory () {}
        public NoteRemovedFromStory (long noteId, long storyId)
        {
            NoteId = noteId;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class AddTask : ICommand
    {
        [DataMember(Order = 1)] public string Text { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal AddTask () {}
        public AddTask (string text, long storyId)
        {
            Text = text;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class TaskAdded : IEvent
    {
        [DataMember(Order = 1)] public long TaskId { get; internal set; }
        [DataMember(Order = 2)] public string Text { get; internal set; }
        
        internal TaskAdded () {}
        public TaskAdded (long taskId, string text)
        {
            TaskId = taskId;
            Text = text;
        }
    }
    
    [DataContract] public sealed class TaskAssignedToStory : IEvent
    {
        [DataMember(Order = 1)] public long TaskId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        [DataMember(Order = 3)] public string Text { get; internal set; }
        [DataMember(Order = 4)] public bool Completed { get; internal set; }
        
        internal TaskAssignedToStory () {}
        public TaskAssignedToStory (long taskId, long storyId, string text, bool completed)
        {
            TaskId = taskId;
            StoryId = storyId;
            Text = text;
            Completed = completed;
        }
    }
    
    [DataContract] public sealed class TaskRemovedFromStory : IEvent
    {
        [DataMember(Order = 1)] public long TaskId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal TaskRemovedFromStory () {}
        public TaskRemovedFromStory (long taskId, long storyId)
        {
            TaskId = taskId;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class AddToStory : ICommand
    {
        [DataMember(Order = 1)] public long ItemId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal AddToStory () {}
        public AddToStory (long itemId, long storyId)
        {
            ItemId = itemId;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class RemoveFromStory : ICommand
    {
        [DataMember(Order = 1)] public long ItemId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal RemoveFromStory () {}
        public RemoveFromStory (long itemId, long storyId)
        {
            ItemId = itemId;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class AddTaskToStory : ICommand
    {
        [DataMember(Order = 1)] public long TaskId { get; internal set; }
        [DataMember(Order = 2)] public long StoryId { get; internal set; }
        
        internal AddTaskToStory () {}
        public AddTaskToStory (long taskId, long storyId)
        {
            TaskId = taskId;
            StoryId = storyId;
        }
    }
    
    [DataContract] public sealed class CompleteTask : ICommand
    {
        [DataMember(Order = 1)] public long TaskId { get; internal set; }
        
        internal CompleteTask () {}
        public CompleteTask (long taskId)
        {
            TaskId = taskId;
        }
    }
    #endregion
}
