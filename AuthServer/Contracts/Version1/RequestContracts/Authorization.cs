namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Authorization
    {
        public class AssignmentRequest
        {
            public string UserID { get; set; }
            public string PolicyID { get; set; }
            public string Info { get; set; }
            public string Error { get; set; }
        }

        public class UnassignmentRequest
        {
            public string UserID { get; set; }
            public string PolicyID { get; set; }
            public string Info { get; set; }
            public string Error { get; set; }
        }
    }
}