using System;
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

        public class GetResponseExample : IExamplesProvider<GetResponse>
        {
            public GetResponse GetExamples()
            {
                return new GetResponse
                {
                    Email = DataFixtures.Email1,
                    FirstName = DataFixtures.Firstname1,
                    LastName = DataFixtures.Lastname,
                    PolicyName = DataFixtures.PolicyName2
                };
            }
        }

        public class GetAllResponse
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PolicyName { get; set; }
        }

        public class GetAllResponseExample : IExamplesProvider<GetAllResponse>
        {
            public GetAllResponse GetExamples()
            {
                return new GetAllResponse
                {
                    Email = DataFixtures.Email1,
                    FirstName = DataFixtures.Firstname1,
                    LastName = DataFixtures.Lastname,
                    PolicyName = DataFixtures.PolicyName2
                };
            }
        }
    }
}