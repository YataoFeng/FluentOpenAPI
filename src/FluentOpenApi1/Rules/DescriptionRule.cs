using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class DescriptionRule : SchemaRule
{
    public string? Description { get; protected set; }
    public DescriptionRule(string description) => Description = description;
    public override void Apply(OpenApiSchema schema) => schema.Description = Description;
}
 