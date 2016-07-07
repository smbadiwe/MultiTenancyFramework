using System;
using System.Collections.Generic;

namespace MultiTenancyFramework
{
    /// <summary>
    /// When set on a property, two things are assumed: the property type must be a Class; and
    /// <para>the relevant properties of the class should be marked with FieldNameInDB attribute if those names are not same.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CompositeMappingModifyFieldNamesAttribute : Attribute
    {
        string _fieldNames;
        bool _useAllPPropertiesWithTHeirDefaultNames;
        public string FieldNames
        {
            get { return _fieldNames; }
        }

        /// <summary>
        /// When this is used, all properties of the class it's on will be assumed to be mapped.
        /// So, for our particular use case, just return the property name
        /// </summary>
        public bool UseAllPPropertiesWithTHeirDefaultNames
        {
            get { return _useAllPPropertiesWithTHeirDefaultNames; }
        }

        public Dictionary<string, string> FieldAndPropNames
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fieldNames))
                {
                    return new Dictionary<string, string>();
                }
                var split1 = _fieldNames.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var toReturn = new Dictionary<string, string>();
                foreach (var item in split1)
                {
                    var split2 = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries); //should be 2
                    toReturn.Add(split2[0], split2[1]);
                }
                return toReturn;
            }
        }
        
        /// <summary>
        /// When this is used, all properties of the class it's on will be assumed to be mapped
        /// </summary>
        public CompositeMappingModifyFieldNamesAttribute()
        {
            _useAllPPropertiesWithTHeirDefaultNames = true;
        }

        /// <summary>
        /// NB: FieldNames should be in the form {innerFieldPropName1:innerFieldName1|innerFieldPropName2:innerFieldName2|...}
        /// </summary>
        /// <param name="fieldNames">should be in the form {innerFieldPropName1:innerFieldName1|innerFieldPropName2:innerFieldName2|...}</param>
        public CompositeMappingModifyFieldNamesAttribute(string fieldNames)
        {
            _fieldNames = fieldNames;
        }
        
    }
}
