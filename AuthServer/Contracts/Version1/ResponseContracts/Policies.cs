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

        public class PostResponseExample : IExamplesProvider<Response<PostResponse>>
        {
            public Response<PostResponse> GetExamples()
            {
                var responseExample = new PostResponse
                {
                    Name = DataFixtures.PolicyName1,
                    Claim = DataFixtures.PolicyClaim1
                };

                return new Response<PostResponse>(responseExample);
            }
        }

        public class GetResponse
        {
            public string Name { get; set; }

            public string Claim { get; set; }

            public List<string> Users { get; set; }
        }

        public class GetResponseExample : IExamplesProvider<Response<GetResponse>>
        {
            public Response<GetResponse> GetExamples()
            {
                var responseExample = new GetResponse
                {
                    Name = DataFixtures.PolicyName2,
                    Claim = DataFixtures.PolicyClaim2,
                    Users = new List<string>{
                        DataFixtures.Email2,
                        DataFixtures.Email3
                        }
                };

                return new Response<GetResponse>(responseExample);
            }
        }

        public class PagedGetResponseExample : IExamplesProvider<PagedResponse<GetResponse>>
        {
            public PagedResponse<GetResponse> GetExamples()
            {
                var responseExamples = new List<GetResponse>{
                    new GetResponse {
                    Name = DataFixtures.PolicyName2,
                    Claim = DataFixtures.PolicyClaim2,
                    Users = new List<string>{
                        DataFixtures.Email2,
                        DataFixtures.Email3
                        }
                },
                    new GetResponse {
                    Name = DataFixtures.PolicyName3,
                    Claim = DataFixtures.PolicyClaim3,
                    Users = new List<string>{
                        DataFixtures.Email1,
                        }
                }
            };

                return new PagedResponse<GetResponse>(responseExamples){
                    PageNumber = 2,
                    PageSize = 10,
                    NextPage = DataFixtures.PreviousPage,
                    PreviousPage = DataFixtures.NextPage
                };
            }
        }
    }
}
