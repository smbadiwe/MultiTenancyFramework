namespace MultiTenancyFramework.Data.Queries
{
    public abstract class DbPagingQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
