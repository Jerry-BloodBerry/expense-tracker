using API.Utils;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using static System.Text.Json.JsonNamingPolicy;

namespace API.Swagger;

public class PascalCaseEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsGenericType || context.Type.GetGenericTypeDefinition() != typeof(CaseInsensitiveEnum<>))
            return;

        schema.Type = "string";
        schema.Enum.Clear();
        Enum.GetNames(context.Type.GenericTypeArguments[0])
            .ToList()
            .ForEach(name => schema.Enum.Add(new OpenApiString(CamelCase.ConvertName(name))));
    }
}