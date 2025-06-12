using API.Error;

namespace API.Utils.Response
{
    public static class ProblemResult
    {
        public static IResult Of(List<ErrorOr.Error> errors, HttpContext context) =>
            Results.Problem(ErrorProblemDetails.Of(errors, context));

        public static IResult Of(ErrorOr.Error error, HttpContext context) =>
            Results.Problem(ErrorProblemDetails.Of(error, context));

        public static IResult OfValidationErrors(IDictionary<string, string[]> errors, HttpContext context) =>
            Results.Problem(ErrorProblemDetails.OfValidationErrors(errors, context));
    }
}