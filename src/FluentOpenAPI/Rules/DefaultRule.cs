using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FluentOpenAPI.Rules;
public class DefaultRule : SchemaRule
{
    public IOpenApiAny DefaultValue { get; }
    public DefaultRule(IOpenApiAny defaultValue) => DefaultValue = defaultValue;
    public override void Apply(OpenApiSchema schema) => schema.Default = DefaultValue;
}
