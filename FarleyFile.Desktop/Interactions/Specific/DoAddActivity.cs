using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FarleyFile.Views;

namespace FarleyFile.Interactions.Specific
{
    public sealed class DoAddActivity : AbstractInteraction
    {
        protected override string[] Alias
        {
            get { return new[] {"aa"}; }
        }

        public static readonly Regex Reference = new Regex(@"\[(?<name>[ \w]+)\](?<id>\.\d+)", RegexOptions.Compiled);
        public static readonly Regex Point = new Regex(@"\.(?<id>\d+)", RegexOptions.Compiled);

        public override InteractionResult Handle(InteractionContext context)
        {
            var txt = context.Request.Data;
            
            if (string.IsNullOrEmpty(txt))
            {
                return Error("Tweet err.. activity can't be longer than 140 chars. Use notes to record data");
            }

            
            var references = new List<ActivityReference>();
            
            var refMatch = Reference.Match(txt);
            while (refMatch.Success)
            {
                var id = refMatch.Groups["id"].Value;
                var name = refMatch.Groups["name"].Value;
                Identity guid;
                if (!context.Request.TryGetId(id, out guid))
                {
                    return Error("Can't find id for '{0}'", id);
                }
                references.Add(new ActivityReference(guid, name, refMatch.Value));
                refMatch = refMatch.NextMatch();
            }
            var point = Point.Match(txt);
            while(point.Success)
            {
                var id = point.Groups["id"].Value;
                Identity guid;
                if (!context.Request.TryGetId(id, out guid))
                {
                    return Error("Can't find id for '{0}'", id);
                }
                var index = context.Storage.GetSingletonOrNew<ItemIndex>();
                var leaf = index.Index[guid.Id];
                references.Add(new ActivityReference(guid, leaf.Name, point.Value));
                point = point.NextMatch();
            }

            context.Response.SendToProject(new AddActivity(txt, DateTimeOffset.Now, references));
            return Handled();
        }
    }
}