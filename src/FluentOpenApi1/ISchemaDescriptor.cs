using FluentOpenApi.Rules;
using FluentOpenApi.Validators;

namespace FluentOpenApi;
public interface ISchemaDescriptor
{
    Type ModelType { get; }
    IReadOnlyDictionary<string, List<object>> GetRules();
    IEnumerable<object> GetValidatorsForMember(string memberName);
    IEnumerable<(string PropertyName, SchemaRule Rule, Validator? Validator)> GetRulesAndValidators();
}
 