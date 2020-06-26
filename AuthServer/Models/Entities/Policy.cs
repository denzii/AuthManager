using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Models.Entities
{
    public class Policy
    {
        [Key]
        public string Name { get; set; }

         public string Claim { get; set; }

        [Required]
        [ForeignKey("Organisation")]
        public string OrganisationID { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}