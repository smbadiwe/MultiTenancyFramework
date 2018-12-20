namespace MultiTenancyFramework.Data.Queries
{
    public abstract class DbPagingQuery
    {
        /// <summary>
        /// Think of it as the page number, but here, 0 represents 1st page, etc.
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// How many records to return. Set to a value less than zero inorder to return all
        /// </summary>
        public int PageSize { get; set; }
    }
}
