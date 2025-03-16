using Microsoft.OpenApi.Models;

namespace FluentOpenApi.Rules;
public class RegularExpressionRule : SchemaRule
{
    public string Pattern { get; }
    public RegularExpressionRule(string pattern) => Pattern = pattern;
    public override void Apply(OpenApiSchema schema) => schema.Pattern = Pattern;
}
 