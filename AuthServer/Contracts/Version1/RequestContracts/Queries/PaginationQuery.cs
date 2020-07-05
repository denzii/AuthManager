using FluentValidation;

namespace AuthServer.Contracts.Version1.RequestContracts.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery() { }

        public PaginationQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > 50 ? 50 : pageSize;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class PaginationQueryRules : AbstractValidator<PaginationQuery>
    {
        public PaginationQueryRules()
        {
            RuleFor(query => query.PageNumber)
            .GreaterThan(0);

            RuleFor(query => query.PageSize)
            .GreaterThan(0);
        }
    }
}
