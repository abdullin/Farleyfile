namespace FarleyFile.Views
{
    public sealed class StoryComposite
    {
        public readonly StoryView View;
        public readonly ActivityList Activities;
        public readonly TaskList Tasks;

        public StoryComposite(StoryView view, ActivityList activities, TaskList tasks)
        {
            View = view;
            Activities = activities;
            Tasks = tasks;
        }
    }
}