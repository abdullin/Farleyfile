using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FarleyFile.Interactions.Specific
{
    public sealed class DoAddActivity : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"aa"}; }
        }

        public static readonly Regex Reference = new Regex(@"\[(?<name>[ \w]+)\]\((?<id>\w+)\)", RegexOptions.Compiled);

        public override InteractionResult Handle(InteractionContext context)
        {
            var txt = context.Request.Data;
            var storyId = context.Request.CurrentStoryId;
            if (string.IsNullOrEmpty(txt))
            {
                return Error("Tweet err.. activity can't be longer than 140 chars. Use notes to record data");
            }

            var match = Reference.Match(txt);

            var references = new List<ActivityReference>();
            while (match.Success)
            {
                var id = match.Groups["id"].Value;
                var name = match.Groups["name"].Value;
                Identity guid;
                if (!context.Request.TryGetId(id, out guid))
                {
                    return Error("Can't find id for '{0}'", id);
                }
                references.Add(new ActivityReference(guid, name, match.Value));
                match = match.NextMatch();
            }

            

            context.Response.SendToProject(new AddActivity(storyId, txt, DateTimeOffset.Now, references));
            return Handled();
        }
    }
}