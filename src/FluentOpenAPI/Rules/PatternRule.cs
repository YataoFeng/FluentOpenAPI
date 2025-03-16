using Microsoft.OpenApi.Models;

namespace FluentOpenAPI.Rules;
public class PatternRule : SchemaRule
{
    public string Pattern { get; }
    public PatternRule(string pattern) => Pattern = pattern;
    public override void Apply(OpenApiSchema schema) => schema.Pattern = Pattern;
}
