using FluentOpenApi.Default;
using FluentOpenApi.Providers;
using FluentOpenApi.Rules;
using FluentOpenApi.Validators;
using System.Linq.Expressions;

namespace FluentOpenApi;
public abstract class ModelSchema<T> : ModelSchema where T : class
{
    private readonly OpenApiSchema<T> _schema = new();
    protected internal void AddRule(string propertyName, SchemaRule rule, Validator? validator = null)
    {
        _schema.AddRule(propertyName, rule, validator);
    }

    public override void ApplyTo(FluentOpenApiProvider provider)
    {
        provider.AddSchema(_schema);
    }

    public PropertyRuleBuilder<T, TProperty> PropertyFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        return new PropertyRuleBuilder<T, TProperty>(this, propertyName);
    }
    protected internal void ReplaceRule(string propertyName, SchemaRule rule, Validator? validator)
    {
        var rules = _schema.GetRulesForProperty(propertyName);
        var existingRule = rules.FirstOrDefault(r => r.Rule == rule && r.Validator == null);
        if (existingRule.Rule != null)
        {
            rules.Remove(existingRule);
        }
        _schema.AddRule(propertyName, rule, validator);
    }
}
