using System.Collections.Generic;

namespace FarleyFile.Interactions
{
    public sealed class InteractionRequest
    {
        public readonly string Raw;
        readonly IDictionary<string, long> _lookup;
        public readonly string Data;
        public readonly long CurrentStoryId;


        public bool TryGetId(string source, out long id)
        {
            return _lookup.TryGetValue(source, out id);
        }

        public InteractionRequest(string data, long currentStoryId, string raw, IDictionary<string, long> lookup)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Raw = raw;
            _lookup = lookup;
        }
    }
}