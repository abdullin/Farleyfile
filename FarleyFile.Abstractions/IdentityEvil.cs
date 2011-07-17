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
                    {NoteId.Tag, g => new NoteId(g)},
                    {StoryId.Tag, g => new StoryId(g)},
                    {TaskId.Tag, g => new TaskId(g)},
                    {ActivityId.Tag, g => new ActivityId(g)}
                };
        }

        public static Identity Upcast(Identity id)
        {
            return Dictionary[id.Tag](id.Id);
        }
    }
}