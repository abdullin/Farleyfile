using System.Collections.Generic;
using FarleyFile.Views;
using System.Linq;

namespace FarleyFile.Interactions.Specific
{
    public sealed class DoTagItem : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"tag"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var items = context.Request.Data.Split(' ');
            Identity itemId;
            if (!context.Request.TryGetId(items[0], out itemId))
            {
                return Error("Failed to lookup item '{0}'", items[0]);
            }
            context.Response.SendToProject(new TagItem(items[1], itemId));
            return Handled();
        }
    }

    public sealed class ListTags : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new string[] {"lt"}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            var tags = context.Storage.GetSingletonOrNew<TagList>();
            context.Response.RenderView(tags);
            return Handled();
        }
    }

    public sealed class FocusOnStory : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] { "cd",""}; }
        }

        public override InteractionResult Handle(InteractionContext context)
        {
            Identity id;
            var source = context.Request.Data;
            if (string.IsNullOrEmpty(source))
            {
                id = context.Request.CurrentStoryId;
                if (id.IsEmpty)
                {
                    return Error("Id was not specified and there is no focused story");
                }
            }
            else if (!context.Request.TryGetId(source, out id))
            {
                return Error("Could not find story ID '{0}'", source);
            }
            var store = context.Storage;
            if (id is StoryId)
            {
                var result = store.GetEntity<StoryView>(id);
                if (!result.HasValue)
                {
                    return Error("Story id not found '{0}'", id);
                }
                var story = result.Value;
                var activities = store.GetEntity<ActivityList>(id).GetValue(new ActivityList());
                var tasks = store.GetEntity<TaskList>(id).GetValue(new TaskList());
                var notes = store.GetEntity<NoteList>(id).GetValue(new NoteList());
                var composite = new FocusComposite(story.StoryId, story.Name, activities.List, tasks.List, notes.Notes);
                context.Response.RenderView(composite);
                context.Response.FocusStory(story.StoryId, story.Name);
                return Handled();
            }
            if (id is TagId)
            {
                var result = store.GetEntity<TagView>(id);
                if (!result.HasValue)
                {
                    return Error("Tag not found '{0}'", id);
                }
                var view = result.Value;
                var stories = view.Stories
                    .Select(s => store.GetEntity<StoryView>(s.Story))
                    .Where(o => o.HasValue)
                    .Select(o => o.Value)
                    .ToList();
                var activities = stories.SelectMany(s => store.GetEntity<ActivityList>(s.StoryId).Convert(al => al.List, new List<ActivityList.Item>())).ToList();
                var tasks = stories.SelectMany(s => store.GetEntity<TaskList>(s.StoryId).Convert(al => al.List, new List<TaskList.Item>())).ToList();
                var notes = stories.SelectMany(s => store.GetEntity<NoteList>(s.StoryId).Convert(al => al.Notes, new List<NoteList.Item>())).ToList();
                var composite = new FocusComposite(id, view.Name, activities, tasks, notes);
                context.Response.RenderView(composite);
                return Handled();
            }
            return Error("Can't focus");
        }
    }
}