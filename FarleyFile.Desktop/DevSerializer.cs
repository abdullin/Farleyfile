using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lokad.Cqrs;
using ServiceStack.Text;

namespace FarleyFile
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
            using (var writer = new StreamWriter(destinationStream, Encoding.UTF8))
            {
                JsonSerializer.SerializeToWriter(instance, instance.GetType(),writer);
            }
        }

        public object Deserialize(Stream sourceStream, Type type)
        {
            using (var reader = new StreamReader(sourceStream, Encoding.UTF8))
            {
                return JsonSerializer.DeserializeFromReader(reader, type);
            }
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