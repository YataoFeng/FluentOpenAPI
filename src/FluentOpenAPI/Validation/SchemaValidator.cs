using FluentOpenApi.Default;
using FluentOpenApi.Models;
using System.Linq.Expressions;

namespace FluentOpenApi.Validation;
public class SchemaValidator<T> : SchemaValidator where T : class
{
    private readonly IOpenApiSchema _schema;
    private readonly List<RuleEntry<T>> _rules;

    public SchemaValidator(IOpenApiSchema schema)
    {
        _schema = schema;
        _rules = CompileRules();
    }

    private List<RuleEntry<T>> CompileRules()
    {
        var descriptor = _schema.CreateDescriptor();
        var rules = descriptor.GetRulesAndValidators();
        var compiled = new List<RuleEntry<T>>();

        foreach (var (propertyName, rule, validator) in rules)
        {
            var parameter = Expression.Parameter(typeof(T), "instance");
            var property = Expression.Property(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var getter = Expression.Lambda<Func<T, object?>>(convert, parameter).Compile();
            var condition = validator?.GetCondition();

            compiled.Add(new RuleEntry<T>(propertyName, getter, rule, validator, condition));
        }

        return compiled;
    }

    public ValidationResult Validate(T instance)
    {
        var failures = new List<(string PropertyName, string ErrorMessage)>();

        if (instance == null)
        {
            failures.Add(("Instance", "Instance cannot be null"));
            return new ValidationResult(false, failures);
        }

        foreach (var rule in _rules)
        {
            if (rule.Validator != null && rule.Check != null)
            {
                var value = rule.GetValue(instance);
                if (rule.Check(value!))
                {
                    failures.Add((rule.Name, rule.Validator.GetErrorMessage(rule.Name)));
                }
            }
        }

        return new ValidationResult(failures.Count == 0, failures);
    }

    public override ValidationResult ValidateForObject(object instance)
    {
        return Validate((T)instance);
    }
}
