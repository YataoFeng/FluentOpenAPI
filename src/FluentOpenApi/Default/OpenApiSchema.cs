using FluentOpenApi.Rules;
using FluentOpenApi.Validators;

namespace FluentOpenApi.Default;
public class OpenApiSchema<T> : IOpenApiSchema where T : class
{
    private readonly Dictionary<string, List<(SchemaRule Rule, Validator? Validator)>> _rules = new();

    public Type ModelType => typeof(T);

    public void AddRule(string propertyName, SchemaRule rule, Validator? validator = null)
    {
        if (!_rules.ContainsKey(propertyName)) _rules[propertyName] = new();
        _rules[propertyName].Add((rule, validator));
    }

    public ISchemaDescriptor CreateDescriptor() => new SchemaDescriptor<T>(ModelType, _rules);

    public List<(SchemaRule Rule, Validator? Validator)> GetRulesForProperty(string propertyName)
    {
        return _rules.TryGetValue(propertyName, out var rules) ? rules : [];
    }
}
 