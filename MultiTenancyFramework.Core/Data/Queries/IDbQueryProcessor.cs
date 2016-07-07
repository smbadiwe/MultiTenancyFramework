namespace MultiTenancyFramework.Data.Queries
{
    /// <summary>
    /// NB: It is advisable to register this entity in IoC as a singleton
    /// </summary>
    public interface IDbQueryProcessor
    {
        string InstitutionCode { get; set; }
        TResult Process<TResult>(IDbQuery<TResult> query);
    }
}
