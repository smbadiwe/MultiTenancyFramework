using MultiTenancyFramework;
using Newtonsoft.Json;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiTenancyFramework.NHibernate.Audit
{
    internal class AuditLogSerializer
    {
        /// <summary>
        /// The serialized result will be de-serialized as List<TrailItem> 
        /// </summary>
        /// <returns></returns>
        internal static string SerializeData(IEntityPersister persister, object[] currentValues, object[] oldValues)
        {
            var data = JsonConvert.SerializeObject(ConvertToList(persister, currentValues, oldValues));
            if (string.IsNullOrWhiteSpace(data)) return data;

            return data.CompressString();
        }

        internal static List<TrailItem> DeSerializeData(string jsonData)
        {
            return JsonConvert.DeserializeObject<List<TrailItem>>(jsonData);
        }

        private static List<TrailItem> ConvertToList(IEntityPersister persister, object[] currentValues, object[] oldValues)
        {
            List<TrailItem> trailItems = new List<TrailItem>();
            if (currentValues == null) return trailItems; //this should never happen

            var entityType = GetEntityType(persister.EntityMetamodel.Type);

            var entityProperties = persister.PropertyNames;
            for (int i = 0; i < entityProperties.Length; i++)
            {
                var propertyName = entityProperties[i];
                if (PropertyTracking.IsTrackingEnabled(new PropertyConfigKey(propertyName, entityType.FullName), entityType))
                {
                    string strBefore = null, strAfter = null;
                    if (oldValues != null) //that means we have old values
                    {
                        strBefore = GetOriginalValue(oldValues[i], propertyName);
                    }
                    strAfter = GetCurrentValue(currentValues[i], propertyName);
                    trailItems.Add(new TrailItem(propertyName, strBefore, strAfter));
                }
            }
            return trailItems;
        }

        private static string GetOriginalValue(object originalValue, string propertyName)
        {
            //TODO: Consider formatting the returned string based on the porperty name
            return originalValue?.ToString();
        }

        private static string GetCurrentValue(object currentValue, string propertyName)
        {
            if (currentValue == null) return null;
            var type = currentValue.GetType(); //Note that this type is the type of the property in the IEntity whose name is 'propertyName'
            if (type.IsClass && type != typeof(string))
            {
                //Check if we did a composite mapping with this type
                if (null != type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ComplexTypeAttribute>())
                {
                    return JsonConvert.SerializeObject(currentValue);
                }
            }
            //TODO: Consider formatting the returned string based on the porperty name. That's why 'propertyName' was sent in
            return currentValue.ToString();
        }

        private static Type GetEntityType(Type entityType)
        {
            //if (typeof(INHibernateProxy).IsAssignableFrom(entityType))
            if (entityType.Name.Contains("Proxy"))
            {
                return GetEntityType(entityType.BaseType);
            }

            return entityType;
        }

    }
}
