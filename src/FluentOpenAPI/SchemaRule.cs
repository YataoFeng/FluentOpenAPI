using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FluentOpenAPI;

public abstract class SchemaRule
{
    public string? Description { get; protected set; }
    public abstract void Apply(OpenApiSchema schema);
}

public class RequiredRule : SchemaRule
{
    public override void Apply(OpenApiSchema schema) => schema.Required.Add(Description ?? "Required field");
}

public class DescriptionRule : SchemaRule
{
    public DescriptionRule(string description) => Description = description;
    public override void Apply(OpenApiSchema schema) => schema.Description = Description;
}

public class DefaultRule : SchemaRule
{
    public IOpenApiAny DefaultValue { get; }
    public DefaultRule(IOpenApiAny defaultValue) => DefaultValue = defaultValue;
    public override void Apply(OpenApiSchema schema) => schema.Default = DefaultValue;
}

public class RangeRule : SchemaRule
{
    public double Min { get; }
    public double Max { get; }
    public RangeRule(double min, double max)
    {
        Min = min;
        Max = max;
    }

    public override void Apply(OpenApiSchema schema)
    {
        schema.Minimum = (decimal)Min;
        schema.Maximum = (decimal)Max;
    }
}

public class MinLengthRule : SchemaRule
{
    public int MinLength { get; }
    public MinLengthRule(int minLength) => MinLength = minLength;
    public override void Apply(OpenApiSchema schema) => schema.MinLength = MinLength;
}

public class MaxLengthRule : SchemaRule
{
    public int MaxLength { get; }
    public MaxLengthRule(int maxLength) => MaxLength = maxLength;
    public override void Apply(OpenApiSchema schema) => schema.MaxLength = MaxLength;
}

public class PatternRule : SchemaRule
{
    public string Pattern { get; }
    public PatternRule(string pattern) => Pattern = pattern;
    public override void Apply(OpenApiSchema schema) => schema.Pattern = Pattern;
}