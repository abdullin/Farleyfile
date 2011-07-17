using System;
using System.Linq;
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
            context.Response.ClearView();
            return Handled();
        }
    }

    public sealed class RecordActivity : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"aa"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var txt = context.Request.Data;
            var storyId = context.Request.CurrentStoryId;
            if (string.IsNullOrEmpty(txt))
            {
                return Error("Tweet err.. activity can't be longer than 140 chars. Use notes to record data");
            }
            context.Response.SendToProject(new AddActivity(storyId, txt));
            return Handled();
        }
    }

    public sealed class DoRemoveFromStory : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"rm"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var txt = context.Request.Data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var item = (txt[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            var story = context.Request.CurrentStoryId;


            if (txt.Length > 2)
            {
                if (!context.Request.TryGetId(txt[2], out story))
                {
                    return Error("Failed to locate '{0}'", txt[2]);
                }
            }
            var records = new Guid[item.Length];
            for (int i = 0; i < records.Length; i++)
            {
                if (!context.Request.TryGetId(item[i], out records[i]))
                    return Error("Failed to locate '{0}'", item[1]);
            }
            context.Response.SendToProject(records.Select(i => new RemoveFromStory(i, story)).ToArray());
            return Handled();
        }
    }

    public sealed class FocusOnStory : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "cd"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            Guid id;
            var source = context.Request.Data;
            if (!context.Request.TryGetId(source, out id))
            {
                return Error("Could not find story ID '{0}'", source);
            }
            
            var result = context.Storage.GetEntity<StoryView>(id);
            if (!result.HasValue)
            {
                return Error("Story id not found '{0}'", id);
            }
            var story = result.Value;
            context.Response.RenderView(story);
            context.Response.FocusStory(story.StoryId, story.Name);

            return Handled();
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
            return Handled();
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
            Guid id;
            if (!context.Request.TryGetId(context.Request.Data, out id))
            {
                return Error("Unknown note id");
            }

            var optional = context.Storage.GetEntity<NoteView>(id);
            if (!optional.HasValue)
            {
                return Error("Note {0} does not exist", id);
            }
            var note = optional.Value;
            context.Response.GrabFile(note.Text, (s, s1) => context.Response.SendToProject(new EditNote(id, s, s1)));
            return Handled();
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
            return new InteractionResult(null, InteractionResultStatus.Terminate);
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
            context.Response.RenderView(view);
            return Handled();
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
            return Handled();
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
            Guid id;
            if (!context.Request.TryGetId(context.Request.Data, out id))
            {
                return Error("Couldn't locate task '{0}'", id);
            }
            context.Response.SendToProject(new CompleteTask(id));
            return Handled();
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
            return Handled();
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
                context.Response.ClearView();
            }

            var id = context.Request.CurrentStoryId;
            var result = context.Storage.GetEntity<StoryView>(id);
            if (!result.HasValue)
            {
                return Error("Story {0} not found", id);
            }
            var story = result.Value;
            context.Response.RenderView(story);
            context.Response.FocusStory(id, story.Name);
            return Handled();
        }
    }
}