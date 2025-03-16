using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class ItemsRangeValidator : Validator<int>
{
    private readonly ItemsRangeRule _rule;

    public ItemsRangeValidator(ItemsRangeRule rule) => _rule = rule;

    public override Func<int, bool> GetTypedCondition()
    {
        return value => value < _rule.Min || value > _rule.Max;
    }

    public override string GetErrorMessage(string propertyName) => $"{propertyName} length must be between {_rule.Min} and {_rule.Max}";
}
