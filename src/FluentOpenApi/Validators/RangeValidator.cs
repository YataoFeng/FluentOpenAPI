using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class RangeValidator : Validator<decimal>
{
    private readonly RangeRule _rule;

    public RangeValidator(RangeRule rule) => _rule = rule;

    public override Func<decimal, bool> GetTypedCondition() => value => value < _rule.Min || value > _rule.Max;

    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be between {_rule.Min} and {_rule.Max}";
}
