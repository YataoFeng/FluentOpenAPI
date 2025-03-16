using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class RangeValidator : Validator<object>
{
    private readonly RangeRule _rule;

    public RangeValidator(RangeRule rule) => _rule = rule;


    public override Func<object?, bool> GetTypedCondition()
    {
        return value =>
        {
            if (value == null)
                return false; // null 不触发范围验证

            return value switch
            {
                int intValue => intValue < _rule.Min || intValue > _rule.Max,
                long longValue => longValue < _rule.Min || longValue > _rule.Max,
                decimal decimalValue => decimalValue < _rule.Min || decimalValue > _rule.Max,
                _ => true,// 非数值类型，返回验证失败
            };
        };
    }
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be between {_rule.Min} and {_rule.Max}";
}
