using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lokad.Cqrs;
using ServiceStack.Text;

namespace FarleyFile.Engine
{
    sealed class DevSerializer : IDataSerializer
    {
        readonly IDictionary<string, Type> _stringToType;
        readonly IDictionary<Type, string> _typeToString;

        public DevSerializer(Type[] types)
        {
            _stringToType = types.ToDictionary(t => t.Name, t => t);
            _typeToString = types.ToDictionary(t => t, t => t.Name);
        }

        public void Serialize(object instance, Stream destinationStream)
        {
            JsonSerializer.SerializeToStream(instance, instance.GetType(), destinationStream);
        }

        public object Deserialize(Stream sourceStream, Type type)
        {
            return JsonSerializer.DeserializeFromStream(type, sourceStream);
        }

        public bool TryGetContractNameByType(Type messageType, out string contractName)
        {
            return _typeToString.TryGetValue(messageType, out contractName);
        }

        public bool TryGetContractTypeByName(string contractName, out Type contractType)
        {
            return _stringToType.TryGetValue(contractName, out contractType);
        }
    }
}