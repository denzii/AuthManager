using AuthServer.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        IOrganisationRepository OrganisationRepository { get; }

        IRefreshTokenRepository RefreshTokenRepository { get; }

        IPolicyRepository PolicyRepository { get; }

        Task<int> CompleteAsync();
        
        int Complete();
    }
}
