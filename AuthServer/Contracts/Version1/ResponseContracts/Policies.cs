using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Policies
    {
        public class PostResponse
        {
            public string Name { get; set; }

            public string Claim { get; set; }
        }

        public class PostResponseExample : IExamplesProvider<PostResponse>
        {
            public PostResponse GetExamples()
            {
                return new PostResponse
                {
                    Name = DataFixtures.PolicyName1,
                    Claim = DataFixtures.PolicyClaim1
                };
            }
        }

        public class GetResponse
        {
            public string Name { get; set; }

            public string Claim { get; set; }

            public List<string> Users { get; set; }
        }

        public class GetResponseExample : IExamplesProvider<GetResponse>
        {
            public GetResponse GetExamples()
            {
                return new GetResponse
                {
                    Name = DataFixtures.PolicyName2,
                    Claim = DataFixtures.PolicyClaim2,
                    Users = new List<string>{
                        DataFixtures.Email2,
                        DataFixtures.Email3
                        }
                };
            }
        }

        public class GetAllResponse
        {
            public string Name { get; set; }

            public string Claim { get; set; }
        }
        public class GetAllResponseExample : IExamplesProvider<GetAllResponse>
        {
            public GetAllResponse GetExamples()
            {
                return new GetAllResponse
                {
                    Name = DataFixtures.PolicyName3,
                    Claim = DataFixtures.PolicyClaim2
                };
            }
        }

    }
}
