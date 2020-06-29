using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Organisations
    {
        public class PostRequest
		{
            public string Name { get; set; }
        }

		public class PostRequestRules : AbstractValidator<PostRequest>
        {
            public PostRequestRules()
            {
                RuleFor(request => request.Name)
                .NotEmpty().NotNull();
            }
		}

        public class PostRequestExample : IExamplesProvider<PostRequest>
        {
            public PostRequest GetExamples()
            {
                return new PostRequest
                {
                    Name = DataFixtures.Organisation
                };
            }
        }
    }
}
