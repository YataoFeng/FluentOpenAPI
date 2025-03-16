using Microsoft.OpenApi.Models;

namespace FluentOpenAPI.Rules;
public class MaxLengthRule : SchemaRule
{
    public int MaxLength { get; }
    public MaxLengthRule(int maxLength) => MaxLength = maxLength;
    public override void Apply(OpenApiSchema schema) => schema.MaxLength = MaxLength;
}
