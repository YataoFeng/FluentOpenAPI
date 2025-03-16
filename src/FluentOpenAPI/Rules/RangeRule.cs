using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;

public class RangeRule : SchemaRule
{
    public double Min { get; }
    public double Max { get; }
    public bool IsArrayLength { get; }
    public RangeRule(double min, double max, bool isArrayLength = false)
    {
        Min = min;
        Max = max;
        IsArrayLength = isArrayLength;
    }

    public override void Apply(OpenApiSchema schema)
    {
        if (IsArrayLength)
        {
            schema.MinItems = (int)Min;
            schema.MaxItems = (int)Max;
        }
        else
        {
            schema.Minimum = (decimal)Min;
            schema.Maximum = (decimal)Max;
        }

    }
}
