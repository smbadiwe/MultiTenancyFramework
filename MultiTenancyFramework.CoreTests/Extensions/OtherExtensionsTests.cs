using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiTenancyFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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