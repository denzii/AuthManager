using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Users
    {
        public class GetResponse
        {

            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PolicyName { get; set; }
        }

        public class GetResponseExample : IExamplesProvider<Response<GetResponse>>
        {
            public Response<GetResponse> GetExamples()
            {
                var responseExample = new GetResponse {
                    Email = DataFixtures.Email1,
                    FirstName = DataFixtures.Firstname1,
                    LastName = DataFixtures.Lastname,
                    PolicyName = DataFixtures.PolicyName2
                };

                return new Response<GetResponse>(responseExample);
            }
        }

        public class PagedGetResponseExample : IExamplesProvider<PagedResponse<GetResponse>>
        {
            public PagedResponse<GetResponse> GetExamples()
            {
                var responseExamples = new List<GetResponse> {
                    new GetResponse {
                    Email = DataFixtures.Email1,
                    FirstName = DataFixtures.Firstname1,
                    LastName = DataFixtures.Lastname,
                    PolicyName = DataFixtures.PolicyName2
                },
                    new GetResponse {
                    Email = DataFixtures.Email3,
                    FirstName = DataFixtures.Firstname3,
                    LastName = DataFixtures.Lastname,
                    PolicyName = DataFixtures.PolicyName3
                }
            };

                return new PagedResponse<GetResponse>(responseExamples) {
                    PageNumber = 2,
                    PageSize = 10,
                    NextPage = DataFixtures.PreviousPage,
                    PreviousPage = DataFixtures.NextPage
                };
            }
        }
    }
}