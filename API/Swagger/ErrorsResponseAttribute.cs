using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Swagger;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ErrorsResponseAttribute : SwaggerResponseAttribute
{
    public ErrorsResponseAttribute(int statusCode, string? description = null, string? contentType = null, params ErrorOr.Error[] errors)
        : base(statusCode, description, typeof(ProblemDetails), contentType ?? "application/problem+json")
    {
        Errors = errors;
    }

    public ErrorOr.Error[] Errors { get; set; }
}