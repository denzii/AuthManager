using System.Collections.Generic;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public class Policies
    {
        public class PostResponse
        {
            public string Name { get; set; }

            public string Claim { get; set; }

            public string Error{ get; set; }
        }

        public class GetResponse
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public string Claim { get; set; }

            public List<string> UserEmails { get; set; }
        }

        public class GetAllResponse
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public string Claim { get; set; }
        }
    }
}