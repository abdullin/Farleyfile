using System;
using System.Collections;
using System.Collections.Generic;
using FarleyFile.Views;

namespace FarleyFile.Interactions.Specific
{
    public sealed class ClearScreen : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[]{"clr"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.Response.Viewport.Clear();
            return InteractionResult.Handled;
        }
    }

    public sealed class DoAddNote : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"an"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var title = DateTime.Now.ToString("yyyy-MM-hh HH:mm");

            var txt = context.Request.Data;
            var storyId = context.Request.CurrentStoryId;
            if (!string.IsNullOrEmpty(txt))
            {
                context.Response.SendToProject(new AddNote(title, txt, storyId));
            }
            else
            {
                context.Response.GrabFile("", (s, s1) => context.Response.SendToProject(new AddNote(title, s, storyId)));
            }
            return InteractionResult.Handled;
        }
    }

    public sealed class DoEditNote : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "vim"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            int id = int.Parse(context.Request.Data);

            var optional = context.Storage.GetEntity<NoteView>(id);
            if (!optional.HasValue)
            {
                context.Response.Viewport.Log("Note {0} does not exist", id);
            }
            else
            {
                var note = optional.Value;
                context.Response.GrabFile(note.Text, (s, s1) => context.Response.SendToProject(new EditNote(id, s, s1)));
            }
            return InteractionResult.Handled;
        }
    }

    public sealed class Quit : AbstractInteraction
    {
        protected override string[] Alias
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
        protected override string[] Alias
        {
            get { return new[] { "ls" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var view = context.Storage.GetSingletonOrNew<StoryListView>();

            context.Response.Viewport.RenderStoryList(view);
            return InteractionResult.Handled;

        }
    }

    public sealed class DoAddTask : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "at", "add-task" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.Response.SendToProject(new AddTask(context.Request.Data, context.Request.CurrentStoryId));

            return InteractionResult.Handled;
        }
    }

    public sealed class DoCompleteTask : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "ct" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var i = int.Parse(context.Request.Data);
            context.Response.SendToProject(new CompleteTask(i));
            return InteractionResult.Handled;
            
        }
    }

    public sealed class NewStory : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "new" }; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            context.Response.SendToProject(new StartSimpleStory(context.Request.Data));
            return InteractionResult.Handled;
        }
    }

    public sealed class ReloadStory : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {""}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            if (context.Request.Raw == " ")
            {
                context.Response.Viewport.Clear();
            }

            var id = context.Request.CurrentStoryId;
            var result = context.Storage.GetEntity<StoryView>(id);
            if (!result.HasValue)
            {
                context.Response.Viewport.Log("Story {0} not found", id);
            }
            else
            {
                var story = result.Value;
                context.Response.Viewport.RenderStory(story, id);
                context.Response.SetCurrentStory(id, story.Name);
            }
            return InteractionResult.Handled;
        }
    }
}