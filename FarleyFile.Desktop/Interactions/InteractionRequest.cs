namespace FarleyFile.Interactions
{
    public sealed class InteractionRequest
    {
        public readonly string Raw;
        public readonly string Data;
        public readonly long CurrentStoryId;

        public InteractionRequest(string data, long currentStoryId, string raw)
        {
            Data = data;
            CurrentStoryId = currentStoryId;
            Raw = raw;
        }
    }
}