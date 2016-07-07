namespace MultiTenancyFramework.Data.Queries
{
    public interface IDbQueryHandler<TQuery, TResult> where TQuery : IDbQuery<TResult>
    {
        string InstitutionCode { get; set; }
        TResult Handle(TQuery theQuery);
    }
}