using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence.Repositories.Interfaces
{
	public interface IRefreshTokenRepository : IRepository<RefreshToken>
	{
		public Task<RefreshToken> GetRefreshToken(string refreshToken);
		public void Update(RefreshToken storedRefreshToken);
		public RefreshToken CreateRefreshToken(string id, string Id);
	}
}
