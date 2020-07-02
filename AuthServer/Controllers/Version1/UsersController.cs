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
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Contracts.Version1.RequestContracts.Queries;
using AuthServer.Models.DTOs;
using AuthServer.Models.Helpers;
using AuthServer.Models.Services.Interfaces;

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
        private readonly IURIService _URIService;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IURIService URIService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _URIService = URIService;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        ///<response code="200"> Users retrieved.</response>
        [HttpGet(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<GetResponse>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationQuery query)
        {
            var pageFilter = _mapper.Map<PageFilter>(query);

            IEnumerable<User> users = await _unitOfWork.UserRepository.GetAllByOrganisationAsync(
                HttpContext.GetOrganisationID(),
                pageFilter
                );

            var getResponses = _mapper.Map<List<GetResponse>>(users);

            if (pageFilter == null || pageFilter.PageNumber < 1 || pageFilter.PageSize < 1)
            {
                return Ok(new PagedResponse<GetResponse>(getResponses));
            }

            var pagedResponse = Paginator.CreatePagedResponse(_URIService, pageFilter, getResponses, HttpContext.Request.Path);

            return Ok(pagedResponse);
        }

        /// <summary>
        /// Retrieves a user with the given UUID.
        /// </summary>
        ///<response code="200"> User retrieved.</response>
        ///<response code="400"> User does not exists within the Organisation.</response>
        [HttpGet(ApiRoutes.Users.Get)]
        [ProducesResponseType(typeof(Response<GetResponse>), 200)]
        public async Task<IActionResult> GetUser(string ID)
        {
            User user = await _unitOfWork.UserRepository.GetByOrganisationAsync(ID, HttpContext.GetOrganisationID());

            if (user == null)
            {
                return NotFound();
            }

            var getResponse = _mapper.Map<GetResponse>(user);

            return Ok(new Response<GetResponse>(getResponse));
        }
    }
}