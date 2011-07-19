namespace FarleyFile.Views
{
    public sealed class StoryComposite
    {
        public readonly StoryView View;
        public readonly ActivityList Activities;
        public readonly TaskList Tasks;
        public readonly NoteList Notes;

        public StoryComposite(StoryView view, ActivityList activities, TaskList tasks, NoteList notes)
        {
            View = view;
            Activities = activities;
            Tasks = tasks;
            Notes = notes;
        }
    }
}