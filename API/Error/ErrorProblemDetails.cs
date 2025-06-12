using System.Diagnostics;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Error;

public static class ErrorProblemDetails
{
    public static ProblemDetails Of(IEnumerable<ErrorOr.Error> errors, HttpContext context)
    {
        if (errors.Any(e => e.Type == ErrorType.Unexpected))
        {
            return new ProblemDetails();
        }

        var firstError = errors.First();

        return new ProblemDetails
        {
            Type = firstError.Code,
            Title = ErrorTitle.GetTitleByErrorCode(firstError.Code)
                    ?? firstError.Description,
            Status = firstError.StatusCode(),
            Detail = firstError.Description,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier },
        };
    }

    public static ProblemDetails Of(ErrorOr.Error error, HttpContext context)
    {
        if (error.Type == ErrorType.Unexpected)
        {
            return new ProblemDetails();
        }

        return new ProblemDetails
        {
            Type = error.Code,
            Title = ErrorTitle.GetTitleByErrorCode(error.Code)
                    ?? error.Description,
            Status = error.StatusCode(),
            Detail = error.Description,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier },
        };
    }

    private static int StatusCode(this ErrorOr.Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    public static ProblemDetails OfValidationErrors(IDictionary<string, string[]> errors, HttpContext context)
    {
        var firstError = errors.First();
        var statusCode = StatusCodes.Status400BadRequest;
        return new ProblemDetails
        {
            Type = ErrorType.Validation.ToString(),
            Title = ErrorTitle.GetTitleByErrorCode(ErrorType.Validation.ToString()),
            Status = statusCode,
            Detail = firstError.Value[0],
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier }
        };
    }
}