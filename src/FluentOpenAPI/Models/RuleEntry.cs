using FluentOpenApi.Rules;
using FluentOpenApi.Validators;

namespace FluentOpenApi.Models;
public class RuleEntry<T>(string name, Func<T, object?> getValue, SchemaRule rule, Validator? validator, Func<object, bool>? check)
{
    public string Name { get; } = name;
    public Func<T, object?> GetValue { get; } = getValue;
    public SchemaRule Rule { get; } = rule;
    public Validator? Validator { get; } = validator;
    public Func<object, bool>? Check { get; } = check;
}
