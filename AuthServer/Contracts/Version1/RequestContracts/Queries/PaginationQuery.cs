namespace AuthServer.Contracts.Version1.RequestContracts.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > 50 ? 50 : pageSize;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
