using NUnit.Framework;

namespace FarleyFile.Interactions.Specific
{
    [TestFixture]
    public sealed class DoAddActivityTests
    {
        // ReSharper disable InconsistentNaming

        [Test]
        public void Verify_regex()
        {
            var text = "Recorded this test at [CaffeeInn caffee](2)";
            Assert.IsTrue(DoAddActivity.Reference.IsMatch(text));
        }

        [Test]
        public void Verify_pointer()
        {
            var text = "Recorded this test at .2.";
            Assert.IsTrue(DoAddActivity.Point.IsMatch(text));
        }
         
    }
}