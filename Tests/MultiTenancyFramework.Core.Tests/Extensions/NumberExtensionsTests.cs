using System;
using NUnit.Framework;

namespace MultiTenancyFramework.Core.Tests.Extensions
{
    [TestFixture]
    public class NumberExtensionsTests
    {
        [Test]
        public void TestToMoneyWithoutTrailingZeros()
        {
            decimal dd = (decimal)19.00;
            Assert.AreEqual(19, dd.ToMoneyWithoutTrailingZeros());
            dd = (decimal)19.55;
            Assert.AreEqual(19.55, dd.ToMoneyWithoutTrailingZeros());
            dd = (decimal)1259.3456;
            Assert.AreEqual(1259.35, dd.ToMoneyWithoutTrailingZeros());
            dd = (decimal)19.3456;
            Assert.AreEqual(19.35, dd.ToMoneyWithoutTrailingZeros());
            dd = (decimal)0.0000;
            Assert.AreEqual(0, dd.ToMoneyWithoutTrailingZeros());
            dd = (decimal)-2.0000;
            Assert.Throws<ArgumentOutOfRangeException>(() => dd.ToMoneyWithoutTrailingZeros());
        }

        [Test]
        public void TestToMoney()
        {
            decimal dd = (decimal)19.00;
            Assert.AreEqual("19.00", dd.ToMoney());
            dd = (decimal)19.55;
            Assert.AreEqual("19.55", dd.ToMoney());
            dd = (decimal)19.3456;
            Assert.AreEqual("19.35", dd.ToMoney());
            dd = (decimal)0.0000;
            Assert.AreEqual("0.00", dd.ToMoney());
            dd = (decimal)-2.0000;
            Assert.AreEqual("-2.00", dd.ToMoney());
        }
    }
}
