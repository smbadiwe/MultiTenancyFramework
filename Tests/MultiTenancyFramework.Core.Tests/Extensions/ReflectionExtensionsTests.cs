using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MultiTenancyFramework.Core.Tests.Extensions
{
    [TestFixture]
    public class ReflectionExtensionsTests
    {
        #region ReflectionExtensions.IsPrimitiveType()
        [Test]
        public void Int_is_primitive()
        {
            var type = typeof(int);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void String_is_primitive()
        {
            var type = typeof(string);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_array_is_not_primitive()
        {
            var type = typeof(int[]);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_list_is_not_primitive()
        {
            var type = typeof(List<int>);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_IList_is_not_primitive()
        {
            var type = typeof(IList<int>);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_nullable_is_primitive()
        {
            var type = typeof(int?);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Decimal_is_primitive()
        {
            var type = typeof(decimal);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Datetime_is_primitive()
        {
            var type = typeof(DateTime);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Decimal_nullable_is_primitive()
        {
            var type = typeof(decimal?);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Datetime_nullable_is_primitive()
        {
            var type = typeof(DateTime?);
            Assert.IsTrue(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_nullable_array_is_not_primitive()
        {
            var type = typeof(int?[]);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_nullable_list_is_not_primitive()
        {
            var type = typeof(List<int?>);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }

        [Test]
        public void Primitive_nullable_IList_is_not_primitive()
        {
            var type = typeof(IList<int?>);
            Assert.IsFalse(ReflectionExtensions.IsPrimitiveType(type));
        }
        
        #endregion

        #region ReflectionExtensions.IsRealClass()
        [Test]
        public void Primitive_is_not_real_class()
        {
            var type = typeof(int);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void String_is_not_real_class()
        {
            var type = typeof(string);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_array_is_not_real_class()
        {
            var type = typeof(int[]);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_list_is_not_real_class()
        {
            var type = typeof(List<int>);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_IList_is_not_real_class()
        {
            var type = typeof(IList<int>);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_nullable_is_not_real_class()
        {
            var type = typeof(int?);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Decimal_is_not_real_class()
        {
            var type = typeof(decimal);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Datetime_is_not_real_class()
        {
            var type = typeof(DateTime);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Decimal_nullable_is_not_real_class()
        {
            var type = typeof(decimal?);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Datetime_nullable_is_not_real_class()
        {
            var type = typeof(DateTime?);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_nullable_array_is_not_real_class()
        {
            var type = typeof(int?[]);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_nullable_list_is_not_real_class()
        {
            var type = typeof(List<int?>);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void Primitive_nullable_IList_is_not_real_class()
        {
            var type = typeof(IList<int?>);
            Assert.IsFalse(ReflectionExtensions.IsRealClass(type));
        }

        [Test]
        public void This_is_class()
        {
            var type = this.GetType();
            Assert.IsTrue(ReflectionExtensions.IsRealClass(type));
        } 
        #endregion
    }
}
