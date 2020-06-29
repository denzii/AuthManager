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
        public class AssignmentResponseExample: IExamplesProvider<AssignmentResponse>
        {
            public AssignmentResponse GetExamples()
            {
                return new AssignmentResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    UserID = DataFixtures.GUID,
                    PolicyName = DataFixtures.PolicyName1,
                    Info = DataFixtures.PolicyGenericInfo
                };
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
        public class UnassignmentResponseExample : IExamplesProvider<UnassignmentResponse>
        {
            public UnassignmentResponse GetExamples()
            {
                return new UnassignmentResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    UserID = DataFixtures.GUID,
                    PolicyName = DataFixtures.PolicyName2,
                    Info = DataFixtures.PolicyGenericInfo
                };
            }
        }
    }
}