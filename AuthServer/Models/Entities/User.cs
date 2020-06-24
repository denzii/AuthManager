using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.Entities
{
	public class User : IdentityUser
	{	
		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(100)]
		public string LastName { get; set; }

		[Required]
		[MaxLength(1)]
		public string Sex { get; set; }

		[Required]
		[EmailAddress]
		public override string Email { get; set; }

		[Required]
		[Range(0, Double.PositiveInfinity)]
		[Column(TypeName = "decimal(18, 3)")]
		public decimal TotalInvestment { get; set; }

		[Required]
		[Column(TypeName = "decimal(18, 3)")]
		public decimal TotalEarnings { get; set; }

		[Required]
		[Range(0, Double.PositiveInfinity)]
		[Column(TypeName = "decimal(18, 3)")]
		public decimal TotalWithdrawals { get; set; }

		[Required]
		public DateTime RegisteredOn { get; set; }

		[Required]
		virtual public Organisation Organisation { get; set; }
	}
}
