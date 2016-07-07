using System;

namespace MultiTenancyFramework.NHibernate.Audit
{
    public class PropertyConfigKey
    {
        public PropertyConfigKey(string propertyName, string typeFullName)
        {
            PropertyName = propertyName;
            TypeFullName = typeFullName;
        }

        public string PropertyName { get; }
        public string TypeFullName { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var otherEntity = obj as PropertyConfigKey;
            if (otherEntity == null) return false;
            bool isNameSame = otherEntity.PropertyName.Equals(PropertyName, StringComparison.OrdinalIgnoreCase);
            bool isTypeSame = otherEntity.TypeFullName.Equals(TypeFullName, StringComparison.OrdinalIgnoreCase);

            return isNameSame && isTypeSame;
        }

        public override int GetHashCode()
        {
            return (PropertyName + TypeFullName).GetHashCode();
        }

        public override string ToString()
        {
            return TypeFullName + "." + PropertyName;
        }
    }
}
