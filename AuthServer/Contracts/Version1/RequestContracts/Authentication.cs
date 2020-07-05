using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Authentication
    {
        public class LoginRequest {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginRequestRules: AbstractValidator<LoginRequest>
        {
            public LoginRequestRules()
            {
                RuleFor(request => request.Email)
                .NotEmpty().NotNull().EmailAddress().MaximumLength(35);

                RuleFor(request => request.Password) //rest of the rules covered by Identity Framework
                .NotEmpty().NotNull();
            }
        }
        
        public class LoginRequestExample : IExamplesProvider<LoginRequest>
        {
            public LoginRequest GetExamples()
            {
                return new LoginRequest
                {
                    Email = DataFixtures.Email1,
                    Password = DataFixtures.Password
                };
            }

            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegistrationRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Sex { get; set; }
            public string OrganisationName { get; set; }
        }

        public class RegistrationRequestRules : AbstractValidator<RegistrationRequest>
        {
            public RegistrationRequestRules()
            {
                RuleFor(request => request.Email)
                .NotEmpty().NotNull().EmailAddress().MaximumLength(35);

                RuleFor(request => request.Password)  //rest of the rules covered by Identity Framework
                .NotEmpty().NotNull();

                RuleFor(request => request.FirstName)
                .NotEmpty().NotNull().MaximumLength(20).Matches("^[A-Za-z]+$");

                RuleFor(request => request.LastName)
                .NotEmpty().NotNull().MaximumLength(20).Matches("^[A-Za-z]+$");

                RuleFor(request => request.Sex)
                .NotEmpty().NotNull().Matches("^[mMfF]$");

                RuleFor(request => request.OrganisationName)
                .NotEmpty().NotNull().MaximumLength(50);
            }
        }

        public class RegistrationRequestExample : IExamplesProvider<RegistrationRequest>
        {
            public RegistrationRequest GetExamples()
            {
                return new RegistrationRequest
                {
                    Email = DataFixtures.Email2,
                    Password = DataFixtures.Password,
                    FirstName = DataFixtures.Firstname2,
                    LastName = DataFixtures.Lastname,
                    Sex = DataFixtures.Male,
                    OrganisationName = DataFixtures.Organisation
                };
            }
        }

        public class RefreshTokenRequest 
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }

        public class RefreshTokenRequestRules : AbstractValidator<RefreshTokenRequest>
        {
            public RefreshTokenRequestRules()
            {
                RuleFor(request => request.Token)
                .NotEmpty().NotNull();

                RuleFor(request => request.RefreshToken)
                .NotEmpty().NotNull();
            }
        }
        
        public class RefreshTokenRequestExample : IExamplesProvider<RefreshTokenRequest>
        {
            public RefreshTokenRequest GetExamples()
            {
                return new RefreshTokenRequest
                {
                    Token = DataFixtures.Token1,
                    RefreshToken = DataFixtures.RefreshToken
                };
            }
        }
    }
}
