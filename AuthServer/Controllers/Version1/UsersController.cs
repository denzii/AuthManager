using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthServer.Models.Entities;
using AuthServer.Persistence.Contexts;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Authorization;
using AuthServer.Contracts.Version1;

namespace AuthServer.Controllers.Version1
{
    // [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/v1/Users
        [HttpGet(ApiRoutes.Users.GetAll)]
        // [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _unitOfWork.UserRepository.ToListAsync();
        }

        // GET: api/v1/Users/{id}
        [HttpGet(ApiRoutes.Users.Get)]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            User user = await _unitOfWork.UserRepository.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}