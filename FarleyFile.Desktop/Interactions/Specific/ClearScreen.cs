using System.Collections;
using System.Collections.Generic;
using FarleyFile.Views;

namespace FarleyFile.Interactions.Specific
{
    public sealed class ClearScreen : AbstractInteraction
    {
        public override string[] Alias
        {
            get { return new[]{"clr"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.Viewport.Clear();
            return InteractionResult.Handled;
        }
    }

    public sealed class Quit : AbstractInteraction
    {
        public override string[] Alias
        {
            get { return new[] { "q" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            return InteractionResult.Terminate;
        }
    }

    public sealed class ListStories : AbstractInteraction
    {
        public override string[] Alias
        {
            get { return new[] { "ls" }; }
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
        public override string[] Alias
        {
            get { return new[] { "at", "add-task" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.SendToProject(new AddTask(context.Request.Data, context.Request.StoryId));

            return InteractionResult.Handled;
        }
    }

    public sealed class DoCompleteTask : AbstractInteraction
    {
        public override string[] Alias
        {
            get { return new[] { "ct" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var i = int.Parse(context.Request.Data);
            context.SendToProject(new CompleteTask(i));
            return InteractionResult.Handled;
            
        }
    }

    public sealed class NewStory : AbstractInteraction
    {
        public override string[] Alias
        {
            get { return new[] { "new" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.SendToProject(new StartSimpleStory(context.Request.Data));
            return InteractionResult.Handled;
        }
    }
}