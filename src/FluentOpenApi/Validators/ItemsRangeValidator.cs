using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class ItemsRangeValidator : Validator<Array>
{
    private readonly ItemsRangeRule _rule;

    public ItemsRangeValidator(ItemsRangeRule rule) => _rule = rule;

    public override Func<Array, bool> GetTypedCondition()
    {
        return value => value.Length < _rule.Min || value.Length > _rule.Max;
    }

    public override string GetErrorMessage(string propertyName) => $"{propertyName} length must be between {_rule.Min} and {_rule.Max}";
}
