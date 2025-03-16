using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class ItemsMinLengthRule : SchemaRule
{
    public int MinLength { get; }
    public ItemsMinLengthRule(int minLength) => MinLength = minLength;
    public override void Apply(OpenApiSchema schema) => schema.MaxItems = MinLength;
}