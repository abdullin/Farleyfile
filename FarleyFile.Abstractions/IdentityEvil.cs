using System;
using System.Collections.Generic;

namespace FarleyFile
{
    public static class IdentityEvil
    {
        static readonly Dictionary<int, Func<Guid, Identity>> Dictionary;

        static IdentityEvil()
        {
            Dictionary = new Dictionary<int, Func<Guid, Identity>>
                {
                    {NoteId.TagId, g => new NoteId(g)},
                    {StoryId.TagId, g => new StoryId(g)},
                    {TaskId.TagId, g => new TaskId(g)},
                    {ActivityId.TagId, g => new ActivityId(g)}
                };
        }

        public static Identity Upcast(Identity id)
        {
            return Dictionary[id.Tag](id.Id);
        }
    }
}