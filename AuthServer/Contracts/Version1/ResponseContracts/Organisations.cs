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
        public class PostResponseExample : IExamplesProvider<PostResponse>
        {
            public PostResponse GetExamples()
            {
                return new PostResponse
                {
                    ID = DataFixtures.GUID,
                    EstablishedOn = DataFixtures.Now
                };
            }
        }

        public class GetResponse
        {
            public string Name { get; set; }
            public DateTime EstablishedOn { get; set; }
            public List<string> Policies { get; set; }
        }
        public class GetResponseExample : IExamplesProvider<GetResponse>
        {
            public GetResponse GetExamples()
            {
                return new GetResponse
                {
                    Name = DataFixtures.Organisation,
                    EstablishedOn = DataFixtures.Now,
                    Policies = new List<string>{
                        DataFixtures.PolicyName1,
                        DataFixtures.PolicyName2,
                        DataFixtures.PolicyName3
                        }
                };
            }
        }
    }
}