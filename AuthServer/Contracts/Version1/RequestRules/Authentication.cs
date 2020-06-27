using FluentValidation;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace AuthServer.Contracts.Version1.RequestRules
{
    public static class Authentication
    {
		public class LoginRequestRules : AbstractValidator<LoginRequest>
		{
			public LoginRequestRules()
			{
				RuleFor(request => request.Email)
				.NotEmpty().NotNull().EmailAddress();

				RuleFor(request => request.Password)
				.NotEmpty().NotNull();
			}
		}

		public class RegistrationRequestRules : AbstractValidator<RegistrationRequest>
		{
			public RegistrationRequestRules()
			{
				RuleFor(request => request.Email)
				.NotEmpty().NotNull().EmailAddress();
				
				RuleFor(request => request.Password)
				.NotEmpty().NotNull();

				RuleFor(request => request.FirstName).N
			}
			// public string Email { get; set; }
			// public string Password { get; set; }
			// public string FirstName { get; set; }
			// public string LastName { get; set; }
			// public string Sex { get; set; }
			// public string OrganisationName { get; set; }
		}

		public class RefreshTokenRequestRules : AbstractValidator<RefreshTokenRequest>
		{
			// public string Token { get; set; }
			// public string RefreshToken { get; set; }
		}
    }
}