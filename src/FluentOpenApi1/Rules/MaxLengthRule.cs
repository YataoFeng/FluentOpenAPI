using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class MaxLengthRule : SchemaRule
{
    public int MaxLength { get; }
    public MaxLengthRule(int maxLength) => MaxLength = maxLength;
    public override void Apply(OpenApiSchema schema) => schema.MaxLength = MaxLength;
}
 