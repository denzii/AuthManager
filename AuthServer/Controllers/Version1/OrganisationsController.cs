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
using static AuthServer.Contracts.Version1.RequestContracts.Organisations;
using static AuthServer.Contracts.Version1.ResponseContracts.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthServer.Configurations;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    public class OrganisationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrganisationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // //// GET: api/v1/Organisations
        // [HttpGet(ApiRoutes.Organisations.GetAll)]
        // // [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]

        // ////TODO implement internal JWT and roles so only SuperUser could access this endpoint
        // ////Do not allow person of organisation access all organisation data (This app will be used on many individual projects independent of each other.)
        // public async Task<ActionResult<IEnumerable<Organisation>>> GetOrganisations()
        // {        
        //     return Ok(await _unitOfWork.OrganisationRepository.GetAllWithUsers());
        // }

        // GET: api/v1/Organisations/{id}
        [HttpGet(ApiRoutes.Organisations.Get)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
        public async Task<ActionResult<GetResponse>> GetOrganisation(string ID)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByUuid(ID);

            if (organisation == null)
            {
                return NotFound();
            }

            return _mapper.Map<GetResponse>(organisation);
        }


        // POST: api/v1/Organisations
        [HttpPost(ApiRoutes.Organisations.Post)]
        public async Task<ActionResult<PostResponse>> PostOrganisation(PostRequest request)
        {
            //TODO Implement internal JWT to authorize clients and selectively allow hitting this endpoint on
            // Currently publicly accessible.

            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.Name);

            if (organisation != null)
            {
                return BadRequest("An organisation with the specified name already exists");
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

            PostResponse response = _mapper.Map<PostResponse>(organisation);

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Organisations.Get.Replace("{ID}", response.ID.ToString());

            return Created(locationUri, response);

        }
    }
}