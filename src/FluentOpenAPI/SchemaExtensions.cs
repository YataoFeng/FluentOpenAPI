using Microsoft.OpenApi.Any;
using System.Linq.Expressions;

namespace FluentOpenAPI;

public static class SchemaExtensions
{
    public static PropertyRuleBuilder<T, TProperty> PropertyFor<T, TProperty>(
        this ModelSchemaBase<T> schema,
        Expression<Func<T, TProperty>> propertyExpression) where T : class
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        return new PropertyRuleBuilder<T, TProperty>(schema, propertyName);
    }

    public static PropertyRuleBuilder<T, TProperty> Required<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder) where T : class
    {
        return builder.AddRule(new RequiredRule()).WithValidation(new RequiredValidator());
    }

    public static PropertyRuleBuilder<T, TProperty> WithDescription<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, string description) where T : class
    {
        return builder.AddRule(new DescriptionRule(description));
    }

    public static PropertyRuleBuilder<T, TProperty> WithDefault<T, TProperty, TValue>(
        this PropertyRuleBuilder<T, TProperty> builder, TValue defaultValue) where T : class
    {
        return builder.AddRule(new DefaultRule(ToOpenApiAny(defaultValue)));
    }

    public static PropertyRuleBuilder<T, TProperty> Range<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, double min, double max) where T : class
    {
        var rule = new RangeRule(min, max);
        return builder.AddRule(rule).WithValidation(new RangeValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> MinLength<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, int minLength) where T : class
    {
        var rule = new MinLengthRule(minLength);
        return builder.AddRule(rule).WithValidation(new MinLengthValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> MaxLength<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, int maxLength) where T : class
    {
        var rule = new MaxLengthRule(maxLength);
        return builder.AddRule(rule).WithValidation(new MaxLengthValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> Matches<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, string pattern) where T : class
    {
        var rule = new PatternRule(pattern);
        return builder.AddRule(rule).WithValidation(new PatternValidator(rule));
    }

    private static IOpenApiAny ToOpenApiAny<TValue>(TValue? value)
    {
        if (value == null) return new OpenApiNull();

        Type type = typeof(TValue);
        if (type == typeof(string)) return new OpenApiString((string)(object)value);
        if (type == typeof(int)) return new OpenApiInteger((int)(object)value);
        if (type == typeof(bool)) return new OpenApiBoolean((bool)(object)value);
        if (type == typeof(double)) return new OpenApiDouble((double)(object)value);
        if (type == typeof(float)) return new OpenApiFloat((float)(object)value);
        if (type == typeof(long)) return new OpenApiLong((long)(object)value);
        if (type == typeof(decimal)) return new OpenApiDouble((double)(decimal)(object)value);
        if (type == typeof(DateTime)) return new OpenApiDate((DateTime)(object)value);
        if (type == typeof(byte[])) return new OpenApiByte((byte[])(object)value);
        if (type.IsEnum) return new OpenApiString(Enum.GetName(type, value) ?? value.ToString()!);

        if (type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var array = new OpenApiArray();
            var items = value as IEnumerable<object>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    array.Add(ToOpenApiAny(item));
                }
            }
            return array;
        }

        if (type.IsClass || type.IsValueType)
        {
            var obj = new OpenApiObject();
            foreach (var prop in type.GetProperties())
            {
                var propValue = prop.GetValue(value);
                obj[prop.Name] = ToOpenApiAny(propValue);
            }
            return obj;
        }

        return new OpenApiString(value.ToString() ?? string.Empty);
    }
}

public class PropertyRuleBuilder<T, TProperty> where T : class
{
    private readonly ModelSchemaBase<T> _schema;
    private readonly string _propertyName;
    private SchemaRule? _lastRule;

    public PropertyRuleBuilder(ModelSchemaBase<T> schema, string propertyName)
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
            _schema.AddRule(_propertyName, _lastRule, validator);
        }
        return this;
    }
}

public abstract class ModelSchemaBase
{
    public abstract void ApplyTo(FluentOpenApiProvider provider);
}

public abstract class ModelSchemaBase<T> : ModelSchemaBase where T : class
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