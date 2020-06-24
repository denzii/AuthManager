using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.Entities
{
	public class RefreshToken
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Token { get; set; }

		[Required]
		public string JwtID { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		[Required]
		public DateTime ExpiryDate { get; set; }

		[Required]
		public bool Invalidated { get; set; }

		[Required]
		public bool Used { get; set; }
		
		[ForeignKey(nameof(User))]
		public string UserID { get; set; }

		[Required]
		public virtual User User { get; set; }
	
	}
}
