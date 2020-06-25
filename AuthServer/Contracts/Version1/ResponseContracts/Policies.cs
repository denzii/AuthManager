namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public class Policies
    {
        public class PostResponse
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public string Claim { get; set; }

            public string Error{ get; set; }
        }
    }
}