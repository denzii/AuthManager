using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static AuthServer.Contracts.Version1.RequestContracts.Authorization;
using static AuthServer.Contracts.Version1.ResponseContracts.Authorization;
using Microsoft.AspNetCore.Identity;
using AuthServer.Models.Entities;
using System.Security.Claims;
using AuthServer.Configurations;
using AuthServer.Configurations.CustomExtensions;
using System.Net.Mime;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;
using AuthServer.Contracts.Version1.ResponseContracts;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AuthorizationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AuthorizationController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Assigns a Claim (Permission/Policy) to the given user.
        /// </summary>
        ///<response code="200">Claim has been assigned.</response>
        ///<response code="400"> User already has the permission or User does not exist, no action has been taken.</response>
        [HttpPost(ApiRoutes.Authorization.Assign)]
        [ProducesResponseType(typeof(Response<AssignmentResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignmentRequest request)
        {
                //TODO Implement many-to-many relationship so user could own more than one permission
                
                //      TODO 
                //     Implement  auto re-login  to update claims info in the jwt
                //     Or invalidate jwt so user must login again 
                //     one of the above solutions can avoid asking user obtain a new token and automatically applying permission on token

            var organisationID = HttpContext.GetOrganisationID();
            var user = await _unitOfWork.UserRepository.GetWithDetails(request.UserID, organisationID);
            var policy = await  _unitOfWork.PolicyRepository.GetByOrganisation(request.PolicyName, organisationID);
                
            if (user == null || policy == null || user.Policy?.Name == policy.Name){
                return BadRequest(new ErrorResponse{
                    Message = "Permission/User does not exist or already has the permission."
                    });
            }
                
            var transaction = _unitOfWork.UserRepository.BeginTransaction();

            user.Policy = policy;
            await _unitOfWork.UserRepository.AddAsync(user);

            await _userManager.AddClaimAsync(user,new Claim(policy.Claim, "true"));
                
            transaction.Commit();

            AssignmentResponse assignmentResponse = new AssignmentResponse{
                UserID = user.Id,
                PolicyName = policy.Name,
                Info = "Permission has been granted, please log-in again for the action to take effect."
            };
            
            return Ok(new Response<AssignmentResponse>(assignmentResponse));

        }

        /// <summary>
        /// Unassigns a Claim (Permission/Policy) from the given user.
        /// </summary>
        ///<response code="200"> Policy unassigned successfully.</response>
        ///<response code="400"> User does not exist or does not own a permission, no action has been taken.</response>
        [HttpPost(ApiRoutes.Authorization.Unassign)]
        [ProducesResponseType(typeof(Response<UnassignmentResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> UnassignPermission([FromBody] UnassignmentRequest request)
        {
                //      TODO 
                //     Implement  auto re-login  to update claims info in the jwt
                //     Or invalidate jwt so user must login again 
                //     one of the above solutions can avoid asking user obtain a new token and automatically applying permission on token

            var user = await Task.Run(() =>_unitOfWork.UserRepository.GetWithDetails(request.UserID, HttpContext.GetOrganisationID()));

            if (user == null || user.Policy == null ){
                return BadRequest(new ErrorResponse{Message = "User does not exist or does not own a permission."});
            }

            user.Policy = null;
            await _unitOfWork.CompleteAsync();

            var response = new Response<UnassignmentResponse>(
                new UnassignmentResponse{
                Info = "Permission has been removed, please log-in again for the action to take effect."
            });

            return Ok(response);
             
        }
    }
}