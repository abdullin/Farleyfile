using System;
using System.IO;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using ServiceStack.Text;

namespace FarleyFile
{
    [TestFixture]
    public sealed class SerializationExpectations
    {
        // ReSharper disable InconsistentNaming

        [Test]
        public void Json_serializer_should_persist_DTO()
        {
            // our persistence goes to seconds
            var expected = new DateTimeOffset(2011, 12, 2, 4, 5,1, TimeSpan.FromHours(3));
            var s = JsonSerializer.SerializeToString(expected);
            var actual = JsonSerializer.DeserializeFromString<DateTimeOffset>(s);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Json_should_work_with_identities()
        {
            var id = new StoryId(Guid.NewGuid());
            var s = JsonSerializer.SerializeToString(id);
            Console.WriteLine(s);
            var deserialized = JsonSerializer.DeserializeFromString<StoryId>(s);
            Assert.AreEqual(id, deserialized);
        }

        [Test]
        public void Protobuf_should_handle_DTO()
        {
            var expected = new DateTimeOffset(2011, 12, 2, 4, 5, 1, TimeSpan.FromHours(3));
            RuntimeTypeModel.Default[typeof (DateTimeOffset)].Add("m_dateTime", "m_offsetMinutes");
            var actual = Serializer.DeepClone(expected);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ProtoBuf_should_work_with_identities()
        {
            var id = new StoryId(Guid.Empty);
            var metaType = RuntimeTypeModel.Default[typeof(StoryId)];
            RuntimeTypeModel.Default.Add(typeof (Identity), true);
            metaType.UseConstructor = false;
            metaType.Add("Tag", "Id");
            var deserialized = Serializer.DeepClone(id);
            Assert.AreEqual(id, deserialized);
        }
    }
}