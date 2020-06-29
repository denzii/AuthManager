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
        public const string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlZW1haWxAZ21haWwuY29tIiwianRpIjoiZjk3ODgwZjQtMGMx" +
        "Ny00Y2IxLWI4NjUtY2U1MGU0MDcwODBlIiwiSUQiOiI0MDM2ODcxMS02MDExLTQzMmItYTZjNS04NGViOWY3OTJkMjAiLCJPcmdhbmlzYXRpb25JRCI6IjdiNWZiYTUwLTFmMW" +
        "MtNDgyOS1iNTZiLTJkODdhYzdmNjU2YSIsIm5iZiI6MTU5MzM1MTkwOSwiZXhwIjoxNTkzMzUyMjA5LCJpYXQiOjE1OTMzNTE5MDl9.OrPG_In2V6hiKpc0vtlrAdkhk_3VXpIJ" +
        "_NuoaDmnS0U";
        public const string RefreshToken = "fc5bff51-8741-4163-9124-f93f356b2a34";
        public const string ValidationFieldName = "Sex";
        public const string ValidationErrorMessage = "Sex cannot be \"Unicorn\"";
        public const string ErrorMessage = "Something went wrong with the operation.";
        public const string PolicyGenericInfo = "Operation Successful, please login again for the changes to take effect";
        public const string Identifier = "5";
        public static string GUID = Guid.NewGuid().ToString();
        public static DateTime Now = DateTime.Now; 
    }
}