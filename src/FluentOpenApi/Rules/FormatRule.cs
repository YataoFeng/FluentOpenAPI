using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class FormatRule : SchemaRule
{
    public string Format { get; set; }
    public FormatRule(string format) => Format = format;
    public override void Apply(OpenApiSchema schema)
    {
        schema.Format = Format;
    }
}