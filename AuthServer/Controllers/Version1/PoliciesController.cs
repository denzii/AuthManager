using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AuthServer.Configurations;
using AuthServer.Configurations.CustomExtensions;
using AuthServer.Contracts.Version1;
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.Entities;
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

        public PoliciesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Generates a policy with the given name and claim.
        /// </summary>
        ///<response code="200"> Policy created.</response>
        ///<response code="400"> Policy exists within the Organisation.</response>
        [HttpPost(ApiRoutes.Policies.Post)]
        [ProducesResponseType(typeof(PostResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> PostPolicy([FromBody] PostRequest request)
        {
            string organisationID = HttpContext.GetOrganisationID();
            bool policyExists = await Task.Run(() => _unitOfWork.PolicyRepository.PolicyExist(request.Name, HttpContext.GetOrganisationID()));
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

            PostResponse response = new PostResponse { Name = policy.Name, Claim = policy.Claim };

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Policies.Get.Replace("{Name}", response.Name);

            return Created(locationUri, response);
        }

        /// <summary>
        /// Retrieves a policy with the given name.
        /// </summary>
        ///<response code="200"> Policy retrieved.</response>
        ///<response code="400"> Policy with the specified name does not exist.</response>
        [HttpGet(ApiRoutes.Policies.Get)]
        [ProducesResponseType(typeof(GetResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetPolicy(string name)
        {
            Policy policy = await Task.Run(() => _unitOfWork.PolicyRepository.GetByOrganisation(name, HttpContext.GetOrganisationID()));

            if (policy == null)
            {
                return BadRequest(new ErrorResponse{ Message = "Policy with the specified name does not exist" });
            }

            return Ok(_mapper.Map<GetResponse>(policy));
        }

        /// <summary>
        /// Retrieves all policies.
        /// </summary>
        ///<response code="200"> Policies retrieved.</response>
        [HttpGet(ApiRoutes.Policies.GetAll)]
        [ProducesResponseType(typeof(GetResponse), 200)]
        public async Task<IActionResult> GetPolicies()
        {
            IEnumerable<Policy> policies = await Task.Run(() => _unitOfWork.PolicyRepository.GetAllByOrganisation(HttpContext.GetOrganisationID()));

            if (!policies.Any())
            {
                return Ok();
            }

            return Ok(policies.Select(policy => _mapper.Map<GetAllResponse>(policy)));
        }
    }
}