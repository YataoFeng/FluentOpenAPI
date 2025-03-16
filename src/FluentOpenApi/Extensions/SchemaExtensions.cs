using FluentOpenApi.Default;
using FluentOpenApi.Rules;
using FluentOpenApi.Validators;
using Microsoft.OpenApi.Any;
using System.Linq.Expressions;

namespace FluentOpenApi.Extensions;

public static class SchemaExtensions
{
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

    public static PropertyRuleBuilder<T, TProperty> WithFormat<T, TProperty, TValue>(
        this PropertyRuleBuilder<T, TProperty> builder, string format) where T : class
    {
        return builder.AddRule(new FormatRule(format));
    }

    public static PropertyRuleBuilder<T, TProperty> Range<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, decimal min, decimal max) where T : class
    {
        var rule = new RangeRule(min, max);
        return builder.AddRule(rule).WithValidation(new RangeValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> ItemsRange<T, TProperty>(
            this PropertyRuleBuilder<T, TProperty> builder, int min, int max) where T : class
    {
        var rule = new ItemsRangeRule(min, max);
        return builder.AddRule(rule).WithValidation(new ItemsRangeValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> MinLength<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, int minLength) where T : class
    {
        var rule = new MinLengthRule(minLength);
        return builder.AddRule(rule).WithValidation(new MinLengthValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> ItemsMinLength<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, int minLength) where T : class
    {
        var rule = new ItemsMinLengthRule(minLength);
        return builder.AddRule(rule).WithValidation(new ItemsMinLengthValidator(rule));
    }
    public static PropertyRuleBuilder<T, TProperty> MaxLength<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, int maxLength) where T : class
    {
        var rule = new MaxLengthRule(maxLength);
        return builder.AddRule(rule).WithValidation(new MaxLengthValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> ItemsMaxLength<T, TProperty>(
     this PropertyRuleBuilder<T, TProperty> builder, int maxLength) where T : class
    {
        var rule = new ItemsMaxLengthRule(maxLength);
        return builder.AddRule(rule).WithValidation(new ItemsMaxLengthValidator(rule));
    }

    public static PropertyRuleBuilder<T, TProperty> RegularExpression<T, TProperty>(
        this PropertyRuleBuilder<T, TProperty> builder, string pattern) where T : class
    {
        var rule = new RegularExpressionRule(pattern);
        return builder.AddRule(rule).WithValidation(new RegularExpressionValidator(rule));
    }

    public static IOpenApiAny ToOpenApiAny<TValue>(TValue? value)
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