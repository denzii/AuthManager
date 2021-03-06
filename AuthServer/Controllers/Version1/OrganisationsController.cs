﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Models.Entities;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Authorization;
using AuthServer.Contracts.Version1;
using static AuthServer.Contracts.Version1.RequestContracts.Organisations;
using static AuthServer.Contracts.Version1.ResponseContracts.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthServer.Configurations;
using System.Net.Mime;
using AuthServer.Contracts.Version1.ResponseContracts;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;
using AuthServer.Configurations.Middlewares;
using AuthServer.Models.Services.Interfaces;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [ApiKeyAuth]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class OrganisationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IURIService _URIService;

        public OrganisationsController(IUnitOfWork unitOfWork, IMapper mapper, IURIService URIService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _URIService = URIService;
        }
        
        /// <summary>
        /// Retrieves an organisation with the given ID if it exists.
        /// </summary>
        ///<response code="200"> Organisation retrieved.</response>
        ///<response code="400"> Organisation does not exist.</response>
        [HttpGet(ApiRoutes.Organisations.Get)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Response<GetResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetOrganisation(string ID)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByUuid(ID);

            if (organisation == null)
            {
                return BadRequest(new ErrorResponse{Message = "Policy with the specified ID does not exist"});
            }

            GetResponse getResponse = _mapper.Map<GetResponse>(organisation);
            var response = new Response<GetResponse>(getResponse);

            return Ok(response);
        }


        /// <summary>
        /// Creates an organisation with the given name.
        /// </summary>
        ///<response code="200"> Organisation created.</response>
        ///<response code="400"> Organisation name already taken.</response>
       [HttpPost(ApiRoutes.Organisations.Post)]
        [ProducesResponseType(typeof(Response<PostResponse>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> PostOrganisation(PostRequest request)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.Name);

            if (organisation != null)
            {
                return BadRequest(new ErrorResponse{Message = "An organisation with the specified name already exists"});
            }

            //organisationID is not db generated as a reference is required before db record creation
            string organisationID = Guid.NewGuid().ToString();

            var policy = new Policy { Name = InternalPolicies.AdminPolicy, Claim = InternalPolicies.AdminClaim, OrganisationID = organisationID };

            var organisationPolicies = new List<Policy> { policy };

            organisation = new Organisation
            {
                ID = organisationID,
                Name = request.Name,
                EstablishedOn = DateTime.Now,
                Policies = organisationPolicies
            };

            await _unitOfWork.OrganisationRepository.AddAsync(organisation);
            await _unitOfWork.PolicyRepository.AddAsync(policy);
            await _unitOfWork.CompleteAsync();

            PostResponse postResponse = _mapper.Map<PostResponse>(organisation);

            var locationURI = _URIService.GetOrganisationURI(postResponse.ID);

            return Created(locationURI, new Response<PostResponse>(postResponse));
        }
    }
}