namespace MultiTenancyFramework
{
    /// <summary>
    /// For AuditLog; what happened to the entity
    /// </summary>
    public enum EventType
    {
        Added = 0,
        Modified = 1,
        SoftDeleted = 2,
        UnDeleted = 3,
        DeletedForReal = 4,
        Login = 5,
        Logout = 6,
        FailedLogin = 7
    }

    public enum AccessScope
    {
        BothCentralAndTenants = 1,
        [EnumDescription("Root-Only")]
        CentralOnly,
        [EnumDescription("Tenant-Only")]
        TenantsOnly,
    }

    public enum ImageType
    {
        Passport = 0,
        Picture,
        Signature
    }
    
    public enum Gender
    {
        Male = 1,
        Female
    }

}
