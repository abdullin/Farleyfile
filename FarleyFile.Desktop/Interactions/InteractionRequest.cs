using System;
using System.Collections.Generic;

namespace FarleyFile.Interactions
{
    public sealed class InteractionRequest
    {
        public readonly string Raw;
        readonly IDictionary<string, Identity> _lookup;
        public readonly string Data;
        public readonly StoryId CurrentStoryId;
        public readonly string CurrentStoryName;


        public bool TryGetId<TIdentity>(string source, out TIdentity id)
            where TIdentity : Identity
        {
            Identity stored;
            id = null;

            if (_lookup.TryGetValue(source.TrimStart('.'), out stored))
            {
                if (stored.IsEmpty)
                    return false;
                id =  stored as TIdentity;

                if (id != null)
                return true;
                throw new InvalidOperationException("Invalid identity cast");
            }
            return false;
        }

        public InteractionRequest(string data, StoryId currentStoryId, string raw, IDictionary<string, Identity> lookup, string currentStoryName)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Raw = raw;
            _lookup = lookup;
            CurrentStoryName = currentStoryName;
        }
    }
}