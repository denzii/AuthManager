using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        
    }
}