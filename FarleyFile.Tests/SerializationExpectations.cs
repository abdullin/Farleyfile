using System;
using NUnit.Framework;
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
    }
}