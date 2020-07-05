using AuthServer.Models.Entities;
using AuthServer.Persistence.Contexts;
using AuthServer.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence.Repositories
{
	public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(AuthServerContext context) : base(context){ }

		public Task<RefreshToken> GetRefreshToken(string refreshToken)
		{
			return AppContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
		}

		public void Update(RefreshToken refreshToken)
		{
			AppContext.Update(refreshToken);
		}

		public RefreshToken CreateRefreshToken(string jwtID, string userID)
		{
			var refreshToken = new RefreshToken
			{
				JwtID = jwtID,
				UserID = userID,
				CreatedAt = DateTime.UtcNow,
				ExpiryDate = DateTime.UtcNow.AddMonths(6)
			};

			return refreshToken;
		}

		public AuthServerContext AppContext
		{
			get { return _context as AuthServerContext; }
		}
	}
}
