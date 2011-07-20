using System.IO;
using Lokad.Cqrs;
using Lokad.Cqrs.Core.Envelope;
using ProtoBuf;

namespace FarleyFile
{
    public sealed class EnvelopeSerializerWithProtoBuf : IEnvelopeSerializer
    {
        public void SerializeEnvelope(Stream stream, EnvelopeContract contract)
        {
            Serializer.Serialize(stream, contract);
        }

        public EnvelopeContract DeserializeEnvelope(Stream stream)
        {
            return Serializer.Deserialize<EnvelopeContract>(stream);
        }
    }
}