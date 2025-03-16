using FluentOpenAPI.Rules;
using FluentOpenAPI.Validators;

namespace FluentOpenAPI;
public interface ISchemaDescriptor
{
    Type ModelType { get; }
    IReadOnlyDictionary<string, List<object>> GetRules();
    IEnumerable<object> GetValidatorsForMember(string memberName);
    IEnumerable<(string PropertyName, SchemaRule Rule, Validator? Validator)> GetRulesAndValidators();
}
