using System;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Authorization
    {
        public class AssignmentResponse 
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string UserID { get; set; }
            public string PolicyName { get; set; }
            public string Info { get; set; }
        }
        public class AssignmentResponseExample: IExamplesProvider<Response<AssignmentResponse>>
        {
            public Response<AssignmentResponse> GetExamples()
            {
                var responseExample =  new AssignmentResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    UserID = DataFixtures.GUID,
                    PolicyName = DataFixtures.PolicyName1,
                    Info = DataFixtures.PolicyGenericInfo
                };

                return new Response<AssignmentResponse>(responseExample);
            }
        }

        public class UnassignmentResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string UserID { get; set; }
            public string PolicyName { get; set; }
            public string Info { get; set; }
        }
        public class UnassignmentResponseExample : IExamplesProvider<Response<UnassignmentResponse>>
        {
            public Response<UnassignmentResponse> GetExamples()
            {
                var responseExample = new UnassignmentResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    UserID = DataFixtures.GUID,
                    PolicyName = DataFixtures.PolicyName2,
                    Info = DataFixtures.PolicyGenericInfo
                };

                return new Response<UnassignmentResponse>(responseExample);
            }
        }
    }
}