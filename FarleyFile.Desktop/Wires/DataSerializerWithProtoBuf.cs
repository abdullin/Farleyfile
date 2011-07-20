using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Lokad.Cqrs;
using Lokad.Cqrs.Core.Serialization;
using ProtoBuf.Meta;
using System.Linq;

namespace FarleyFile
{
    /// <summary>
    /// Copied from Lokad.CQRS
    /// </summary>
    public class DataSerializerWithProtoBuf : IDataSerializer
    {
        readonly IDictionary<string, Type> _contract2Type = new Dictionary<string, Type>();
        readonly IDictionary<Type, string> _type2Contract = new Dictionary<Type, string>();
        readonly IDictionary<Type, IFormatter> _type2Formatter = new Dictionary<Type, IFormatter>();

        public DataSerializerWithProtoBuf(ICollection<Type> knownTypes)
        {
            if (knownTypes.Count == 0)
                throw new InvalidOperationException(
                    "ProtoBuf requires some known types to serialize. Have you forgot to supply them?");

            
            InitIdentityTree();

            foreach (var type in knownTypes)
            {
                var reference = ContractEvil.GetContractReference(type);
                var formatter = RuntimeTypeModel.Default.CreateFormatter(type);

                try
                {
                    _contract2Type.Add(reference, type);
                }
                catch (ArgumentException e)
                {
                    var msg = string.Format("Duplicate contract '{0}' being added to ProtoBuf dictionary", reference);
                    throw new InvalidOperationException(msg, e);
                }
                try
                {
                    _type2Contract.Add(type, reference);
                    _type2Formatter.Add(type, formatter);
                }
                catch (ArgumentException e)
                {
                    var msg = string.Format("Duplicate type '{0}' being added to ProtoBuf dictionary", type);
                    throw new InvalidOperationException(msg, e);
                }
            }
        }

        public void Serialize(object instance, Stream destination)
        {
            IFormatter formatter;
            if (!_type2Formatter.TryGetValue(instance.GetType(), out formatter))
            {
                var s =
                    string.Format(
                        "Can't find serializer for unknown object type '{0}'. Have you passed all known types to the constructor?",
                        instance.GetType());
                throw new InvalidOperationException(s);
            }
            formatter.Serialize(destination, instance);
        }

        public object Deserialize(Stream source, Type type)
        {
            IFormatter value;
            if (!_type2Formatter.TryGetValue(type, out value))
            {
                var s =
                    string.Format(
                        "Can't find serializer for unknown object type '{0}'. Have you passed all known types to the constructor?",
                        type);
                throw new InvalidOperationException(s);
            }
            return value.Deserialize(source);
        }
        public bool TryGetContractNameByType(Type messageType, out string contractName)
        {
            return _type2Contract.TryGetValue(messageType, out contractName);
        }
        
        public bool TryGetContractTypeByName(string contractName, out Type contractType)
        {
            return _contract2Type.TryGetValue(contractName, out contractType);
        }

        static void InitIdentityTree()
        {
            RuntimeTypeModel.Default[typeof(DateTimeOffset)].Add("m_dateTime", "m_offsetMinutes");

            var id = typeof(Identity);
            var derived = id.Assembly.GetExportedTypes()
                .Where(id.IsAssignableFrom)
                .Where(t => t != id);
            int i = 4;
            foreach (var d in derived)
            {
                RuntimeTypeModel.Default[id].AddSubType(i++, d);
            }
        }
    }
}