namespace FarleyFile.Interactions
{
    public sealed class InteractionResult
    {
        public readonly string OptionalError;
        public readonly InteractionResultStatus Status;

        public InteractionResult(string optionalError, InteractionResultStatus status)
        {
            OptionalError = optionalError;
            Status = status;
        }
    }
}