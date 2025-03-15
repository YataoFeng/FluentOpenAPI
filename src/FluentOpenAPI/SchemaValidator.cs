using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace FluentOpenAPI;

public class RuleEntry<T>(string name, Func<T, object?> getValue, SchemaRule rule, Validator? validator, Func<object, bool>? check)
{
    public string Name { get; } = name;
    public Func<T, object?> GetValue { get; } = getValue;
    public SchemaRule Rule { get; } = rule;
    public Validator? Validator { get; } = validator;
    public Func<object, bool>? Check { get; } = check;
}
public abstract class SchemaValidator
{
    public abstract ValidationResult ValidateForObject(object instance);
}
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

public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<(string PropertyName, string ErrorMessage)> Failures { get; }

    public ValidationResult(bool isValid, IEnumerable<(string PropertyName, string ErrorMessage)> failures)
    {
        IsValid = isValid;
        Failures = failures.ToList().AsReadOnly();
    }

    public Dictionary<string, string[]> ToDictionary()
    {
        return Failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}

internal class ValidationEndpointFilter(FluentOpenApiProvider provider) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var arg in context.Arguments)
        {
            if (arg == null)
            {
                continue;
            }
            var schemaValidator = provider.GetValidator(arg.GetType());
            if (schemaValidator == null)
            {
                continue;
            }
            var result = schemaValidator.ValidateForObject(arg);
            if (!result.IsValid)
            {
                return Results.BadRequest(result.ToDictionary());
            }
        }
        return await next(context);
    }
}
