using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class ItemsRangeRule : SchemaRule
{
    public int Min { get; }
    public int Max { get; }
    public ItemsRangeRule(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override void Apply(OpenApiSchema schema)
    {
        schema.MinItems = Min;
        schema.MaxItems = Max;
    }
}