using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Configurations;
using AuthServer.Configurations.CustomExtensions;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Entities;
using AuthServer.Persistence;
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

        public PoliciesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // POST: api/v1/Policies
        [HttpPost(ApiRoutes.Policies.Post)]
        public async Task<ActionResult<PostResponse>> PostPolicy([FromBody] PostRequest request)
        {
            bool policyExists = await Task.Run(() => _unitOfWork.PolicyRepository.PolicyExist(request.Name, HttpContext.GetOrganisationID()));

            if (policyExists)
            {
                return BadRequest(new PostResponse{Error = "Policy with the specified name already exist within your organisation"});
            }

            Policy policy = new Policy{Name = request.Name, Claim = request.Claim};
            
            _unitOfWork.PolicyRepository.AddAsync(policy);
            await _unitOfWork.CompleteAsync();

            PostResponse response = new PostResponse{ Name = policy.Name, Claim = policy.Claim, ID = policy.ID };

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Policies.Get.Replace("{ID}", response.ID.ToString());

            return Created(locationUri, response);
        }

        // GET: api/v1/Policies/{id}
        [HttpGet(ApiRoutes.Policies.Get)]
        public async Task<ActionResult<Policy>> GetPolicy(string ID)
        {
            Policy policy = await Task.Run(() => _unitOfWork.PolicyRepository.GetByOrganisation(ID, HttpContext.GetOrganisationID()));

            if (policy == null)
            {
                return NotFound("Policy with the specified name does not exist");
            }

            return policy;
        }

        // GET: api/v1/Policies/
        [HttpGet(ApiRoutes.Policies.GetAll)]
        public async Task<ActionResult<Policy>> GetPolicies(string ID)
        {
            IEnumerable<Policy> policies = await Task.Run(() => _unitOfWork.PolicyRepository.GetAllByOrganisation(HttpContext.GetOrganisationID()));

            if (!policies.Any())
            {
                return NotFound("No policies exist for your organisation");
            }

            return Ok(policies);
        }
    }
}