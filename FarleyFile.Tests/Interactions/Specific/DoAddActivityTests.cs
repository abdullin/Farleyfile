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
         
    }
}