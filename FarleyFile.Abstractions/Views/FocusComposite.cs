using System.Collections.Generic;

namespace FarleyFile.Views
{
    public sealed class FocusComposite
    {
        public readonly Identity Focus;
        public readonly string Name;

        //public readonly StoryView View;
        public readonly ICollection<ActivityList.Item> Activities;
        public readonly ICollection<TaskList.Item> Tasks;
        public readonly ICollection<NoteList.Item> Notes;

        public FocusComposite(Identity id, string name, ICollection<ActivityList.Item> activities, ICollection<TaskList.Item> tasks, ICollection<NoteList.Item> notes)
        {
            Focus = id;
            Name = name;
            Activities = activities;
            Tasks = tasks;
            Notes = notes;
        }
    }
}