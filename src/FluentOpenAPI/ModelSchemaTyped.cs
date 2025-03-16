using FluentOpenAPI.Default;
using FluentOpenAPI.Providers;
using FluentOpenAPI.Rules;
using FluentOpenAPI.Validators;
using System.Linq.Expressions;

namespace FluentOpenAPI;
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
}
