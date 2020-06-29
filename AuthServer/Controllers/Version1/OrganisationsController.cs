using System;
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

        public OrganisationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an organisation with the given ID if it exists.
        /// </summary>
        ///<response code="200"> Organisation retrieved.</response>
        ///<response code="400"> Organisation does not exist.</response>
        [HttpGet(ApiRoutes.Organisations.Get)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]       
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
            //TODO Implement internal JWT to authorize clients and selectively allow hitting this endpoint on
            // Currently publicly accessible.

            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.Name);

            if (organisation != null)
            {
                return BadRequest(new ErrorResponse{Message = "An organisation with the specified name already exists"});
            }

            //organisationID is guid since a reference to the organisationID is required before the organisation is created
            // (both of the related entities have to be generated together)
            string organisationID = Guid.NewGuid().ToString();

            var policy = new Policy { Name = AuthorizationPolicies.AdminPolicy, Claim = AuthorizationPolicies.AdminClaim, OrganisationID = organisationID };

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

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Organisations.Get.Replace("{ID}", postResponse.ID.ToString());

            return Created(locationUri, new Response<PostResponse>(postResponse));

        }
    }
}