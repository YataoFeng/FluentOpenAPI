using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;

public class RangeRule : SchemaRule
{
    public decimal Min { get; }
    public decimal Max { get; }
    public RangeRule(decimal min, decimal max)
    {
        Min = min;
        Max = max;
    }

    public override void Apply(OpenApiSchema schema)
    {
        schema.Minimum = Min;
        schema.Maximum = Max;
    }
}
