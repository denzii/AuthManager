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
		public string ID { get; set; }

		[Required]
		public string Name { get; set; }

		public DateTime EstablishedOn { get; set; }

		[Required]
		virtual public ICollection<User> Users { get; set; } = new List<User>();

		[Required]
		virtual public ICollection<Policy> Policies { get; set; } = new List<Policy>();
	}
}
