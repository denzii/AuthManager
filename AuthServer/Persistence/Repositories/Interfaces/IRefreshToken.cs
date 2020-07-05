using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence.Repositories.Interfaces
{
	public interface IRefreshTokenRepository : IRepository<RefreshToken>
	{
		Task<RefreshToken> GetRefreshToken(string refreshToken);

		void Update(RefreshToken storedRefreshToken);
		
		RefreshToken CreateRefreshToken(string jwtID, string userID);
	}
}
