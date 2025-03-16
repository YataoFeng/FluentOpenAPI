using FluentOpenAPI.Rules;
using FluentOpenAPI.Validators;
using System.Data;

namespace FluentOpenAPI.Default;
public class PropertyRuleBuilder<T, TProperty> where T : class
{
    private readonly ModelSchema<T> _schema;
    private readonly string _propertyName;
    private SchemaRule? _lastRule;

    public PropertyRuleBuilder(ModelSchema<T> schema, string propertyName)
    {
        _schema = schema;
        _propertyName = propertyName;
    }

    public PropertyRuleBuilder<T, TProperty> AddRule(SchemaRule rule)
    {
        _lastRule = rule;
        _schema.AddRule(_propertyName, rule);
        return this;
    }

    public PropertyRuleBuilder<T, TProperty> WithValidation(Validator validator)
    {
        if (_lastRule != null)
        {
            _schema.ReplaceRule(_propertyName, _lastRule, validator);
        }
        return this;
    }

}