using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.Entities
{
	public class Organisation
	{
		[Key, Column(Order = 0)]
		public int ID { get; set; }

		[Required]
		public string OrganisationName { get; set; }

		public DateTime EstablishedOn { get; set; }

		[Required]
		virtual public ICollection<User> Users { get; set; }
	}
}
