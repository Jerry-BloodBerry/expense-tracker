using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Swagger;

public static class EndpointExtensions
{
    public static TBuilder ProducesErrors<TBuilder>(
        this TBuilder builder,
        int statusCode = StatusCodes.Status400BadRequest,
        params ErrorOr.Error[] errors
    ) where TBuilder : IEndpointConventionBuilder =>
        builder.ProducesErrors(statusCode, null, null, errors);

    public static TBuilder ProducesErrors<TBuilder>(
        this TBuilder builder,
        int statusCode = StatusCodes.Status400BadRequest,
        string? description = null,
        params ErrorOr.Error[] errors
    ) where TBuilder : IEndpointConventionBuilder =>
        builder.ProducesErrors(statusCode, description, null, errors);

    public static TBuilder ProducesErrors<TBuilder>(
        this TBuilder builder,
        int statusCode = StatusCodes.Status400BadRequest,
        string? description = null,
        string? contentType = null,
        params ErrorOr.Error[] errors
    ) where TBuilder : IEndpointConventionBuilder =>
        builder.WithMetadata(new ErrorsResponseAttribute(
            statusCode: statusCode,
            description: description,
            contentType: contentType,
            errors: errors
        ));

    public static TBuilder WithSummary<TBuilder>(
        this TBuilder builder,
        string summary,
        string? description = null
    ) where TBuilder : IEndpointConventionBuilder =>
        builder.WithMetadata(new SwaggerOperationAttribute(
            summary: summary,
            description: description
        ));
}