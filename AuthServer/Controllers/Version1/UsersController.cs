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
using System.Net.Mime;
using static AuthServer.Contracts.Version1.ResponseContracts.Users;
using AutoMapper;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        ///<response code="200"> Users retrieved.</response>
        [HttpGet(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(GetAllResponse), 200)]
        public async Task<IActionResult> GetUsers()
        {
            //TODO Add Pagination
            var users = await _unitOfWork.UserRepository.GetAllByOrganisation(HttpContext.GetOrganisationID());

            return Ok(users.Select(user => _mapper.Map<GetAllResponse>(user)));
        }

        /// <summary>
        /// Retrieves a user with the given UUID.
        /// </summary>
        ///<response code="200"> User retrieved.</response>
        ///<response code="400"> User does not exists within the Organisation.</response>
        [HttpGet(ApiRoutes.Users.Get)]
        [ProducesResponseType(typeof(GetResponse), 200)]
        public async Task<IActionResult> GetUser(string ID)
        {
            User user = await _unitOfWork.UserRepository.GetByOrganisation(ID, HttpContext.GetOrganisationID());

            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<GetResponse>(user));
        }
    }
}