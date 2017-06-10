using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiTenancyFramework.Tests
{
    [TestClass()]
    public class OtherExtensionsTests
    {
        [TestMethod()]
        public void GetNameTest()
        {
            Assert.AreEqual(int.MaxValue.ToString(), OtherExtensions.GetName<int, int>(x => int.MaxValue));
        }

        [TestMethod()]
        public void GetNameTest2()
        {
            Assert.AreEqual("32", OtherExtensions.GetName<int, int>(x => 32));
        }
    }
}