using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AuthServer.Configurations;
using AuthServer.Configurations.CustomExtensions;
using AuthServer.Contracts.Version1;
using AuthServer.Contracts.Version1.RequestContracts.Queries;
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Models.Helpers;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AuthServer.Contracts.Version1.RequestContracts.Policies;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;
using static AuthServer.Contracts.Version1.ResponseContracts.Policies;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class PoliciesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IURIService _URIService;

        public PoliciesController(IUnitOfWork unitOfWork, IMapper mapper, IURIService URIService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _URIService = URIService;
        }
        
        /// <summary>
        /// Generates a policy with the given name and claim.
        /// </summary>
        ///<response code="200"> Policy created.</response>
        ///<response code="400"> Policy exists within the Organisation.</response>
        [HttpPost(ApiRoutes.Policies.Post)]
        [ProducesResponseType(typeof(Response<PostResponse>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> PostPolicy([FromBody] PostRequest request)
        {
            string organisationID = HttpContext.GetOrganisationID();
            bool policyExists = await _unitOfWork.PolicyRepository.PolicyExistAsync(request.Name, HttpContext.GetOrganisationID());
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByUuid(organisationID);

            if (policyExists)
            {
                return BadRequest(new ErrorResponse { Message = "Policy with the specified name already exist within your organisation" });
            }

            Policy policy = new Policy { Name = request.Name, Claim = request.Claim, OrganisationID = organisationID };

            organisation.Policies.Add(policy);

            await _unitOfWork.PolicyRepository.AddAsync(policy);
            _unitOfWork.OrganisationRepository.TrackEntity(organisation);
            await _unitOfWork.CompleteAsync();

            var postResponse = new PostResponse { Name = policy.Name, Claim = policy.Claim };
            var locationURI = _URIService.GetPolicyURI(postResponse.Name);

            return Created(locationURI, new Response<PostResponse>(postResponse));
        }

        /// <summary>
        /// Retrieves a policy with the given name.
        /// </summary>
        ///<response code="200"> Policy retrieved.</response>
        ///<response code="400"> Policy with the specified name does not exist.</response>
        [HttpGet(ApiRoutes.Policies.Get)]
        [ProducesResponseType(typeof(Response<GetResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetPolicy(string name)
        {
            Policy policy = await  _unitOfWork.PolicyRepository.GetByOrganisationAsync(name, HttpContext.GetOrganisationID());

            if (policy == null)
            {
                return BadRequest(new ErrorResponse{ Message = "Policy with the specified name does not exist" });
            }
            var getResponse = _mapper.Map<GetResponse>(policy);

            return Ok(new Response<GetResponse>(getResponse));
        }

        /// <summary>
        /// Retrieves all policies.
        /// </summary>
        ///<response code="200"> Policies retrieved.</response>
        [HttpGet(ApiRoutes.Policies.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<GetResponse>), 200)]
        public async Task<IActionResult> GetPolicies([FromQuery] PaginationQuery query)
        {
            var pageFilter = _mapper.Map<PageFilter>(query);

            IEnumerable<Policy> policies = await _unitOfWork.PolicyRepository.GetAllByOrganisationAsync(
                HttpContext.GetOrganisationID(),
                pageFilter
                );

            var getResponses = _mapper.Map<List<GetResponse>>(policies);

            if(pageFilter == null || pageFilter.PageNumber < 1 || pageFilter.PageSize < 1){
                return Ok(new PagedResponse<GetResponse>(getResponses));
            }

            var pagedResponse = Paginator.CreatePagedResponse(_URIService, pageFilter, getResponses);
            
            return Ok(pagedResponse);
               
        }
    }
}