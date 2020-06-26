using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Configurations;
using AuthServer.Configurations.CustomExtensions;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Entities;
using AuthServer.Persistence;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AuthServer.Contracts.Version1.RequestContracts.Policies;
using static AuthServer.Contracts.Version1.ResponseContracts.Policies;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
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

        // POST: api/v1/Policies
        [HttpPost(ApiRoutes.Policies.Post)]
        public async Task<ActionResult<PostResponse>> PostPolicy([FromBody] PostRequest request)
        {
            string organisationID = HttpContext.GetOrganisationID();
            bool policyExists = await Task.Run(() => _unitOfWork.PolicyRepository.PolicyExist(request.Name, HttpContext.GetOrganisationID()));
            Organisation organisation = _unitOfWork.OrganisationRepository.Get(Convert.ToInt32(organisationID));

            if (policyExists)
            {
                return BadRequest(new PostResponse{Error = "Policy with the specified name already exist within your organisation"});
            }

            Policy policy = new Policy{Name = request.Name, Claim = request.Claim, Organisation = organisation};
            
            organisation.Policies = new List<Policy>{policy};

             _unitOfWork.PolicyRepository.AddAsync(policy);
             _unitOfWork.OrganisationRepository.TrackEntity(organisation);
            await _unitOfWork.CompleteAsync();

            PostResponse response = new PostResponse{ Name = policy.Name, Claim = policy.Claim };

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Policies.Get.Replace("{Name}", response.Name);

            return Created(locationUri, response);
        }

        // GET: api/v1/Policies/{id}
        [HttpGet(ApiRoutes.Policies.Get)]
        public async Task<ActionResult<Policy>> GetPolicy(string ID)
        {
            Policy policy = await Task.Run(() => _unitOfWork.PolicyRepository.GetByOrganisation(ID, HttpContext.GetOrganisationID()));

            if (policy == null)
            {
                return NotFound("Policy with the specified ID does not exist");
            }

            return policy;
        }

        // GET: api/v1/Policies/
        [HttpGet(ApiRoutes.Policies.GetAll)]
        public async Task<ActionResult<Policy>> GetPolicies()
        {
            IEnumerable<Policy> policies = await Task.Run(() => _unitOfWork.PolicyRepository.GetAllByOrganisation(HttpContext.GetOrganisationID()));

            if (!policies.Any())
            {
                return NotFound("No policies exist for your organisation");
            }

            return Ok(policies.Select(policy => _mapper.Map<GetAllResponse>(policy)));
        }
    }
}