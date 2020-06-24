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

        // GET: api/v1/Organisations
        [HttpGet(ApiRoutes.Organisations.GetAll)]
        public async Task<ActionResult<IEnumerable<Organisation>>> GetOrganisations()
        {
            var x = await _unitOfWork.OrganisationRepository.GetAllWithUsers();
        
            return Ok(x);
        }

        // GET: api/v1/Organisations/{id}
        [HttpGet(ApiRoutes.Organisations.Get)]
        public async Task<ActionResult<Organisation>> GetOrganisation(int id)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.FindAsync(id);

            if (organisation == null)
            {
                return NotFound();
            }

            return organisation;
        }


        // POST: api/v1/Organisations
        [HttpPost(ApiRoutes.Organisations.Post)]
        // [Authorize(Policy = "Admin")]
        public async Task<ActionResult<PostResponse>> PostOrganisation(PostRequest request)
        {
            try
            {
                Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.OrganisationName);

                if (organisation != null)
                {
                    return BadRequest("An organisation with the specified name already exists");
                }

                organisation = new Organisation()
                {
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

        // DELETE: api/v1/Organisation/{id}
        [HttpDelete(ApiRoutes.Organisations.Delete)]
        public async Task<ActionResult<Organisation>> DeleteOrganisation(int id)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.FindAsync(id);
            if (organisation == null)
            {
                return NotFound();
            }

            _unitOfWork.OrganisationRepository.Remove(organisation);
            await _unitOfWork.CompleteAsync();

            return organisation;
        }
    }
}