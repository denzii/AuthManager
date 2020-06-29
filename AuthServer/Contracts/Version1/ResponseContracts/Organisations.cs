using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Organisations
    {
        public class PostResponse
        {
            public string ID { get; set; }
            public DateTime EstablishedOn { get; set; }
        }
        public class PostResponseExample : IExamplesProvider<Response<PostResponse>>
        {
            public Response<PostResponse> GetExamples()
            {
                var responseExample = new PostResponse
                {
                    ID = DataFixtures.GUID,
                    EstablishedOn = DataFixtures.Now
                };

                return new Response<PostResponse>(responseExample);
            }
        }

        public class GetResponse
        {
            public string Name { get; set; }
            public DateTime EstablishedOn { get; set; }
            public List<string> Policies { get; set; }
        }
        public class GetResponseExample : IExamplesProvider<Response<GetResponse>>
        {
            public Response<GetResponse> GetExamples()
            {
                var responseExample = new GetResponse
                {
                    Name = DataFixtures.Organisation,
                    EstablishedOn = DataFixtures.Now,
                    Policies = new List<string>{
                        DataFixtures.PolicyName1,
                        DataFixtures.PolicyName2,
                        DataFixtures.PolicyName3
                        }
                };

                return new Response<GetResponse>(responseExample);
            }
        }
    }
}