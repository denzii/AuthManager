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
using AuthServer.Configurations.CustomExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthServer.Models.DataTransferObjects;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/v1/Users
        [HttpGet(ApiRoutes.Users.GetAll)]
        public async Task<ActionResult<UserDTO>> GetUsers()
        {
            //TODO Add Pagination
            var users = await _unitOfWork.UserRepository.GetAllByOrganisation(HttpContext.GetOrganisationID());

            return Ok(users);
        }

        // GET: api/v1/Users/{id}
        [HttpGet(ApiRoutes.Users.Get)]
        public async Task<ActionResult<UserDTO>> GetUser(string ID)
        {
            UserDTO user = await _unitOfWork.UserRepository.GetByOrganisation(ID, HttpContext.GetOrganisationID());

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}