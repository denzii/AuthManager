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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrganisationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrganisationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // // GET: api/v1/Organisations
        // [HttpGet(ApiRoutes.Organisations.GetAll)]
        // [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]

        //TODO implement internal JWT and roles so only SuperUser could access this endpoint
        // Do not allow person of organisation access all organisation data (This app will be used on many individual projects independent of each other.)
        // public async Task<ActionResult<IEnumerable<Organisation>>> GetOrganisations()
        // {        
        //     return Ok(await _unitOfWork.OrganisationRepository.GetAllWithUsers());
        // }

        // GET: api/v1/Organisations/{id}
        [HttpGet(ApiRoutes.Organisations.Get)]
        [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
        public async Task<ActionResult<Organisation>> GetOrganisation(string ID)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.FindAsync(Convert.ToInt32(ID));

            if (organisation == null)
            {
                return NotFound();
            }

            return organisation;
        }


        // POST: api/v1/Organisations
        [HttpPost(ApiRoutes.Organisations.Post)]
        public async Task<ActionResult<PostResponse>> PostOrganisation(PostRequest request)
        {
            //TODO Implement internal JWT to authorize clients and selectively allow hitting this endpoint on
            // Currently publicly accessible.
            try
            {
                Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.OrganisationName);

                if (organisation != null)
                {
                    return BadRequest("An organisation with the specified name already exists");
                }

                organisation = new Organisation{
                    OrganisationName = request.OrganisationName,
                    EstablishedOn = DateTime.Now
                };

                _unitOfWork.OrganisationRepository.AddAsync(organisation);
                await _unitOfWork.CompleteAsync();

                PostResponse response = _mapper.Map<PostResponse>(organisation);

                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                string locationUri = baseUrl + "/" + ApiRoutes.Organisations.Get.Replace("{ID}", response.ID.ToString());

                return Created(locationUri, response);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }
    }
}