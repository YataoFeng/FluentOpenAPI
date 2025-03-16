using FluentOpenAPI.Rules;

namespace FluentOpenAPI.Validators;
public class RangeValidator : Validator<object>
{
    private readonly RangeRule _rule;

    public RangeValidator(RangeRule rule) => _rule = rule;

    public override Func<object, bool> GetTypedCondition()
    {
        if (_rule.IsArrayLength)
        {
            return value => value switch
            {
                Array array => array.Length < _rule.Min || array.Length > _rule.Max,
                _ => true
            };
        }
        else
        {
            return value => value switch
            {
                int i => i < _rule.Min || i > _rule.Max,
                double d => d < _rule.Min || d > _rule.Max,
                float f => f < _rule.Min || f > _rule.Max,
                _ => true
            };
        }
    }

    public override string GetErrorMessage(string propertyName) => _rule.IsArrayLength
            ? $"{propertyName} length must be between {_rule.Min} and {_rule.Max}"
            : $"{propertyName} must be between {_rule.Min} and {_rule.Max}";
}
