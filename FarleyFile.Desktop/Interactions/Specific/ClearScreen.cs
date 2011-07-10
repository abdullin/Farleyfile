using FarleyFile.Views;

namespace FarleyFile.Interactions.Specific
{
    public sealed class ClearScreen : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^clr$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            
            context.Viewport.Clear();
            context.Viewport.Log("Yahoo!");
            return InteractionResult.Handled;
        }
    }

    public sealed class Quit : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^q$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            return InteractionResult.Terminate;
        }
    }

    public sealed class ListStories : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^ls$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var view = context.Storage.GetSingletonOrNew<StoryListView>();

            context.Viewport.RenderStoryList(view);
            return InteractionResult.Handled;

        }
    }

    public sealed class DoAddTask : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^at (?<title>.+)$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var match = Matcher.Match(context.Request.Data);
            var value = match.Groups["title"].Value;

            context.SendToProject(new AddTask(value, context.Request.StoryId));

            return InteractionResult.Handled;
        }
    }

    public sealed class DoCompleteTask : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^ct (?<id>.+)$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {

            var task = Matcher.Match(context.Request.Data).Groups["id"].Value;
            var id = int.Parse(task);
            context.SendToProject(new CompleteTask(id));
                
            return InteractionResult.Handled;
            
        }
    }

    public sealed class NewStory : AbstractInteraction
    {
        public override string Pattern
        {
            get { return "^new (?<title>.+)$"; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var match = Matcher.Match(context.Request.Data);
            var value = match.Groups["title"].Value;
            context.SendToProject(new StartSimpleStory(value));
            return InteractionResult.Handled;
        }
    }
}