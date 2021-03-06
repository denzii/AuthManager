﻿using AuthServer.Persistence.Contexts;
using AuthServer.Persistence.Repositories;
using AuthServer.Persistence.Repositories.Interfaces;
using System.Threading.Tasks;

namespace AuthServer.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthServerContext _context;

        public UnitOfWork(
            AuthServerContext context,
            IUserRepository userRepository,
            IOrganisationRepository organisationRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPolicyRepository policyRepository
            )
        {
            _context = context;
            UserRepository = userRepository;
            OrganisationRepository = organisationRepository;
            RefreshTokenRepository = refreshTokenRepository;
            PolicyRepository = policyRepository;
        }

        public IUserRepository UserRepository { get; private set; }

        public IOrganisationRepository OrganisationRepository { get; private set; }

        public IRefreshTokenRepository RefreshTokenRepository { get; private set; }

        public IPolicyRepository PolicyRepository { get; private set;}

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}