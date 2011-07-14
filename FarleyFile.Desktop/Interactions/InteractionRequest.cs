using System;
using System.Collections.Generic;

namespace FarleyFile.Interactions
{
    public sealed class InteractionRequest
    {
        public readonly string Raw;
        readonly IDictionary<string, Guid> _lookup;
        public readonly string Data;
        public readonly Guid CurrentStoryId;


        public bool TryGetId(string source, out Guid id)
        {
            if (_lookup.TryGetValue(source, out id))
            {
                if (id == Guid.Empty)
                    return false;
                return true;
            }
            return false;
        }

        public InteractionRequest(string data, Guid currentStoryId, string raw, IDictionary<string, Guid> lookup)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Raw = raw;
            _lookup = lookup;
        }
    }
}