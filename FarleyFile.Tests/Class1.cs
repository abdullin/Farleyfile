using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ServiceStack.Text;

namespace FarleyFile.Tests
{
    [TestFixture]
    public sealed class IdentityTests
    {
        // ReSharper disable InconsistentNaming

        [Test]
        public void Test()
        {
            var id = new StoryId(Guid.NewGuid());
            var s = JsonSerializer.SerializeToString(id);
            Console.WriteLine(s);
            var deserialized = JsonSerializer.DeserializeFromString<StoryId>(s);
            Assert.AreEqual(id, deserialized);
        }
         
    }
}
