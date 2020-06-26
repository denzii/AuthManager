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
            var organisationID = HttpContext.GetOrganisationID();

            var users = await Task.Run(() =>_unitOfWork.UserRepository.GetAllByOrganisation(organisationID));

            return Ok(
                await Task.Run(() =>_unitOfWork.UserRepository.GetAllByOrganisation(organisationID))
                );
        }

        // GET: api/v1/Users/{id}
        [HttpGet(ApiRoutes.Users.Get)]
        public async Task<ActionResult<UserDTO>> GetUser(string ID)
        {
            UserDTO user = await Task.Run(() =>_unitOfWork.UserRepository.GetByOrganisation(ID, HttpContext.GetOrganisationID()));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}