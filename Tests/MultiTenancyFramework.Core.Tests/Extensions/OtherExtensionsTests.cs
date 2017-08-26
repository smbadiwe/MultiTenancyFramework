using NUnit.Framework;

namespace MultiTenancyFramework.Tests
{
    [TestFixture]
    public class OtherExtensionsTests
    {
        [Test]
        public void GetNameTest()
        {
            Assert.AreEqual(int.MaxValue.ToString(), OtherExtensions.GetName<int, int>(x => int.MaxValue));
        }

        [Test]
        public void GetNameTest2()
        {
            Assert.AreEqual("32", OtherExtensions.GetName<int, int>(x => 32));
        }
    }
}