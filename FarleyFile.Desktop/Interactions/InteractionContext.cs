using Lokad.Cqrs.Feature.AtomicStorage;

namespace FarleyFile.Interactions
{
    public sealed class InteractionContext
    {
        public readonly InteractionRequest Request;
        public readonly InteractionResponse Response;
        public readonly NuclearStorage Storage;

        public InteractionContext(InteractionRequest request, NuclearStorage storage, InteractionResponse response)
        {
            Request = request;
            Storage = storage;
            Response = response;
        }
    }
}