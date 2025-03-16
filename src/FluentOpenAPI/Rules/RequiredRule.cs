using Microsoft.OpenApi.Models;

namespace FluentOpenAPI.Rules;
public class RequiredRule : SchemaRule
{
    public string? PropertyName { get; internal set; }
    public override void Apply(OpenApiSchema schema)
    {
        schema.Required.Add(PropertyName);
    }
}
