using FluentOpenApi.Rules;
using FluentOpenApi.Validators;

namespace FluentOpenApi.Default;
public class SchemaDescriptor<T> : ISchemaDescriptor where T : class
{
    private readonly Dictionary<string, List<(SchemaRule Rule, Validator? Validator)>> _rules;

    public SchemaDescriptor(Type modelType, Dictionary<string, List<(SchemaRule Rule, Validator? Validator)>> rules)
    {
        ModelType = modelType;
        _rules = rules;
    }

    public Type ModelType { get; }
    public IReadOnlyDictionary<string, List<object>> GetRules() => _rules.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(r => r.Rule as object).ToList());
    public IEnumerable<object> GetValidatorsForMember(string memberName) => _rules.TryGetValue(memberName, out var rules) ? rules.Select(r => r.Rule) : Enumerable.Empty<object>();
    public IEnumerable<(string PropertyName, SchemaRule Rule, Validator? Validator)> GetRulesAndValidators()
    {
        return _rules.SelectMany(kvp => kvp.Value.Select(r => (kvp.Key, r.Rule, r.Validator)));
    }
}
