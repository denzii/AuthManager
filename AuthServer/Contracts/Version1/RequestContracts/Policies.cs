using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Policies
    {
        public class PostRequest
        {
            public string Name { get; set; }
            public string Claim { get; set; }
        }
        public class PostRequestRules : AbstractValidator<PostRequest>
        {
            public PostRequestRules()
			{
				RuleFor(request => request.Name)
				.NotEmpty().NotNull();

                RuleFor(request => request.Claim)
                .NotEmpty().NotNull();
			}
        }
        
        public class PostRequestExample : IExamplesProvider<PostRequest>
        {
            public PostRequest GetExamples()
            {
                return new PostRequest{
                    Name = DataFixtures.PolicyName2,
                    Claim = DataFixtures.PolicyClaim2,
                };
            }
        }
    }
}