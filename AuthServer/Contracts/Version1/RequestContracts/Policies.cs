namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Policies
    {
        public class PostRequest
        {
            public string Name { get; set; }
            public string Claim { get; set; }
        }
    }
}