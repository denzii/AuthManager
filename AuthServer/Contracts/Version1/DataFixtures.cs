using System;

namespace AuthServer.Contracts.Version1
{
    public static class DataFixtures
    {
        public const string Firstname1 = "Ali";

        public const string Firstname2 = "Veli";

        public const string Firstname3 = "Zeki";

        public const string Lastname = "Kurabiyeci";

        public const string Email1 = "alikurabiyeci@example.com";

        public const string Email2 = "velikurabiyeci@example.com";

        public const string Email3 = "zekikurabiyeci@example.com";

        public const string Password = "SuperSecureP@ssword1";

        public const string Male = "M";

        public const string Female = "F";

        public const string Organisation = "MonolithPizza";

        public const string PolicyName1 = "Driver";

        public const string PolicyClaim1 = "IsDriver";

        public const string PolicyName2 = "OvenMaster";

        public const string PolicyClaim2 = "IsOvenMaster";

        public const string PolicyName3 = "Cashier";

        public const string PolicyClaim3 = "IsCashier";

        //Signed with actual secret
        public const string Token1 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ2ZWxpa3VyYWJpeWVjaTdAZXhhbXBsZS5jb20iLCJqdGkiOiI0NzdhMDU5Zi1hMTIxLTQ5NTEtYjJkNS0xNDIxMmIwODU3MjciLCJJRCI6ImMzNWE0ZWFkLWU2YTgtNDExZC1hMzljLWQ3ZWI5MmEzNzJhMCIsIk9yZ2FuaXNhdGlvbklEIjoiNTVhZWRhMjEtZjQwNy00MjdhLWE1OGMtN2E0M2VhYzU2OGYyIiwiSXNBZG1pbiI6InRydWUiLCJuYmYiOjE1OTM4Nzg0MTcsImV4cCI6MTU5Mzg3ODcxNywiaWF0IjoxNTkzODc4NDE3fQ.Xx-jolTCQOfFvNJKwpdRizWls0XlBKFz0pexZeuqrRw";
        
        //Signed with invalid secret
        public const string Token2 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ2ZWxpa3VyYWJpeWVjaTdAZXhhbXBsZS5jb20iLCJqdGkiOiI3ZDc0YWUyMi0xNDQwLTRjNjgtOTBjOC04YTZlMjZkZjQxM2EiLCJJRCI6ImMzNWE0ZWFkLWU2YTgtNDExZC1hMzljLWQ3ZWI5MmEzNzJhMCIsIk9yZ2FuaXNhdGlvbklEIjoiNTVhZWRhMjEtZjQwNy00MjdhLWE1OGMtN2E0M2VhYzU2OGYyIiwiSXNBZG1pbiI6InRydWUiLCJuYmYiOjE1OTM4OTMyNjcsImV4cCI6MTU5Mzg5MzU2NywiaWF0IjoxNTkzODkzMjY3fQ.chPjh9c0odtc1rVjaZAFxR2xO2IQmFYchdOph9hy_KQ";
        
        //Alg HS512
        public const string Token3 = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE1OTM4OTQxNzMsImV4cCI6MTYyNTQzMDE3MywiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsIkdpdmVuTmFtZSI6IkpvaG5ueSIsIlN1cm5hbWUiOiJSb2NrZXQiLCJFbWFpbCI6Impyb2NrZXRAZXhhbXBsZS5jb20iLCJSb2xlIjpbIk1hbmFnZXIiLCJQcm9qZWN0IEFkbWluaXN0cmF0b3IiXX0.cbEjCInZ8JXgNCwhnxwhGrnGC11dUupznE2LCN43Y_n6d5fM5rK5bFDBvXn2Mmj4dCKTUlO7nCYzKqDmhHlcTA";
        
        public const string Token4 = "UnknownTokenFormat";
        
        public const string RefreshToken = "fc5bff51-8741-4163-9124-f93f356b2a34";

        public const string ValidationFieldName = "Sex";

        public const string ValidationErrorMessage = "Sex cannot be \"Unicorn\"";

        public const string ErrorMessage = "Something went wrong with the operation.";

        public const string PolicyGenericInfo = "Operation Successful, please login again for the changes to take effect";

        public const string Identifier = "5";

        public const string PreviousPage = "https://localhost:5001/api/v1/Policies?pageNumber=1&pageSize=10";

        public const string NextPage = "https://localhost:5001/api/v1/Policies?pageNumber=3&pageSize=10";

        public const string TokenSecret = "eeeeeeee-eeee-eeee-eeee-eeeeeeee";

        public const string ApiKeyValue  = "ApiKeyValue";

        public static TimeSpan TokenLifetime = new TimeSpan(0,5,0);
        public static string GUID = Guid.NewGuid().ToString();
        public static DateTime Now = DateTime.Now;

    }
}