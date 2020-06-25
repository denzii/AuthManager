using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Models.Entities
{
    public class Policy
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Claim { get; set; }
        public Organisation Organisation { get; set; }
        public ICollection<User> Users { get; set; }
    }
}