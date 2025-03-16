using Microsoft.OpenApi.Models;

namespace FluentOpenAPI.Rules;
public abstract class SchemaRule
{
    public abstract void Apply(OpenApiSchema schema);
}
