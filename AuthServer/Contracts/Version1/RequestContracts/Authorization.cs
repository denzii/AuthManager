using System;
using FluentValidation;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.RequestContracts
{
    public static class Authorization
    {
        public class AssignmentRequest 
        {
            public string UserID { get; set; }
            public string PolicyName { get; set; }
        }
        
        public class AssignmentRequestRules: AbstractValidator<AssignmentRequest>
        {
            public AssignmentRequestRules()
            {
                RuleFor(request => request.UserID)
                .NotEmpty().NotNull();

                RuleFor(request => request.PolicyName)
                .NotEmpty().NotNull().MaximumLength(30).Matches("^[A-Za-z]+$");
            }
        }

        public class AssignmentRequestExample : IExamplesProvider<AssignmentRequest>
        {
            public AssignmentRequest GetExamples()
            {
                return new AssignmentRequest
                {
                    UserID = DataFixtures.GUID,
                    PolicyName = DataFixtures.PolicyName2
                };
            }
        }

        public class UnassignmentRequest 
        {
            public string UserID { get; set; }
        }

        public class UnassignmentRequestRules: AbstractValidator<UnassignmentRequest>
        {
            public UnassignmentRequestRules()
            {
                RuleFor(request => request.UserID)
                .NotEmpty().NotNull();
            }
        }

        public class UnassignmentRequestExample : IExamplesProvider<UnassignmentRequest>
        {
            public UnassignmentRequest GetExamples()
            {
                return new UnassignmentRequest
                {
                    UserID = DataFixtures.GUID
                };
            }
        }

    }
}