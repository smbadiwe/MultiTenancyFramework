using FluentNHibernate.Mapping;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public static class NHMapsExtensions
    {
        public static PropertyPart VarCharMax(this PropertyPart part)
        {
            //Any length more than 4001 sets the string column to varchar(MAX) or Text.
            return part.Length(4444);
        }
    }
}
