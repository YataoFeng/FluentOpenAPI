using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public abstract class SchemaRule
{
    public abstract void Apply(OpenApiSchema schema);
}
