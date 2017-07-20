using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class PersonMap<T> : BaseEntityMap<T> where T : Person
    {
        public PersonMap()
        {
            Map(x => x.LastName);
            Map(x => x.OtherNames);
            Map(x => x.Gender);
            Map(x => x.Email);
            Map(x => x.PhoneNumber);
        }
    }
}
