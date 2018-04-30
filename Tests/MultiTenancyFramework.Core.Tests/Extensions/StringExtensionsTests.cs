using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Core.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void AsSplitPascalCasedString_one_word()
        {
            Assert.AreEqual("Soma", StringExtensions.AsSplitPascalCasedString("Soma"));
        }

        [Test]
        public void AsSplitPascalCasedString_two_words()
        {
            Assert.AreEqual("Soma Dina", StringExtensions.AsSplitPascalCasedString("SomaDina"));
        }

        [Test]
        public void AsSplitPascalCasedString_words_with_abbrev()
        {
            Assert.AreEqual("Soma ID Next", StringExtensions.AsSplitPascalCasedString("SomaIDNext"));
        }

        [Test]
        public void AsSplitPascalCasedString_words_with_number()
        {
            Assert.AreEqual("GL 12 Version", StringExtensions.AsSplitPascalCasedString("GL12Version"));
        }
    }
}
