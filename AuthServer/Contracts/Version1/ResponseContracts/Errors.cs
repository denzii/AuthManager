using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Errors
    {
        public class ValidationErrorResponse
        {
            public string FieldName { get; set; }
            public string Message { get; set; }
        }

        public class ValidationErrorResponseExample : IExamplesProvider<ValidationErrorResponse>
        {
            public ValidationErrorResponse GetExamples()
            {
                return new ValidationErrorResponse
                {
                    FieldName = DataFixtures.ValidationFieldName,
                    Message = DataFixtures.ValidationErrorMessage
                };
            }
        }

        public class ErrorResponse
        {
            public string Message { get; set; }
        }
        public class ErrorResponseExample : IExamplesProvider<ErrorResponse>
        {
            public ErrorResponse GetExamples()
            {
                return new ErrorResponse
                {
                    Message = DataFixtures.ErrorMessage
                };
            }
        }
    }
}