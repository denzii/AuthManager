namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Authorization
    {
        public class AssignmentResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string UserID { get; set; }
            public string PolicyID { get; set; }
            public string Info { get; set; }
            public string Error { get; set; }
        }

        public class UnassignmentResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string UserID { get; set; }
            public string PolicyID { get; set; }
            public string Info { get; set; }
            public string Error { get; set; }
        }
    }
}