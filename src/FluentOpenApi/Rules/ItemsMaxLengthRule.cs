using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class ItemsMaxLengthRule : SchemaRule
{
    public int MaxLength { get; }
    public ItemsMaxLengthRule(int maxLength) => MaxLength = maxLength;
    public override void Apply(OpenApiSchema schema) => schema.MaxItems = MaxLength;
}
