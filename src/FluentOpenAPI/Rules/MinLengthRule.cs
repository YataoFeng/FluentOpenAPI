using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class MinLengthRule : SchemaRule
{
    public int MinLength { get; }
    public MinLengthRule(int minLength) => MinLength = minLength;
    public override void Apply(OpenApiSchema schema) => schema.MinLength = MinLength;
}
