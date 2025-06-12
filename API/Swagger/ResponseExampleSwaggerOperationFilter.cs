using System.Text.Json;
using API.Error;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Swagger;

public sealed class ResponseExampleSwaggerOperationFilter : IOperationFilter
{
    private readonly AnnotationsOperationFilter _baseFilter;
    private readonly JsonSerializerOptions _serializerOptions;

    public ResponseExampleSwaggerOperationFilter(JsonSerializerOptions serializerOptions)
    {
        _baseFilter = new AnnotationsOperationFilter();
        _serializerOptions = serializerOptions;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // first apply Swashbuckle builtin filter 
        _baseFilter.Apply(operation, context);

        var attributes =
            context.MethodInfo.GetCustomAttributes(true)
                .OfType<ErrorsResponseAttribute>();

        if (context.ApiDescription?.ActionDescriptor.EndpointMetadata != null)
        {
            attributes = attributes
                .Union(context.ApiDescription.ActionDescriptor.EndpointMetadata)
                .OfType<ErrorsResponseAttribute>()
                .Distinct();
        }

        foreach (var attribute in attributes)
        {
            var problemDetails = attribute.Errors
                .Select(error => ErrorProblemDetails.Of([error], new DefaultHttpContext()))
                .ToArray();

            if (!operation.Responses.TryGetValue(attribute.StatusCode.ToString(), out var response))
            {
                response = new OpenApiResponse();
            }

            var contentType = attribute.ContentTypes.FirstOrDefault() ?? "application/problem+json";
            if (!response.Content.TryGetValue(contentType, out var content))
            {
                content = new OpenApiMediaType();
                response.Content.Add(contentType, content);
            }

            if (problemDetails.Length == 1)
            {
                content.Example = OpenApiAnyFactory.CreateFromJson(JsonSerializer.Serialize(
                    problemDetails.First(),
                    _serializerOptions
                ));

                continue;
            }

            foreach (var details in problemDetails)
            {
                var serialize = JsonSerializer.Serialize(
                    details,
                    _serializerOptions
                );

                var openApiAny = OpenApiAnyFactory.CreateFromJson(serialize);
                content.Examples.Add(details.Type ?? "Unknown type", new OpenApiExample { Value = openApiAny });
            }
        }
    }
}