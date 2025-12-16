#region

using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace Dometrain.Monolith.Api.OpenApi;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse
                ? "default"
                : responseType.StatusCode.ToString();

            if (operation.Responses?.TryGetValue(responseKey, out var response) != true || response?.Content == null)
                continue;

            foreach (var contentType in response.Content.Keys)
                if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                    response.Content.Remove(contentType);
        }

        if (operation.Parameters == null) return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .First(p => p.Name == parameter.Name);

            // Cast to concrete type to mutate properties
            if (parameter is not OpenApiParameter openApiParameter) continue;
            openApiParameter.Description ??= description.ModelMetadata.Description;

            if (parameter.Schema is OpenApiSchema { Default: null } schema
                && description.DefaultValue != null
                && description.DefaultValue is not DBNull
                && description.ModelMetadata is { } modelMetadata)
            {
                var json = JsonSerializer.Serialize(
                    description.DefaultValue,
                    description.ModelMetadata.ModelType);
                schema.Default = JsonNode.Parse(json);
            }

            openApiParameter.Required |= description.IsRequired;
        }
    }
}