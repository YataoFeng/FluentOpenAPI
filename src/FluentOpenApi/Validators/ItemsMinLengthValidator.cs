using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class ItemsMinLengthValidator : Validator<int>
{
    private readonly ItemsMinLengthRule _rule;

    public ItemsMinLengthValidator(ItemsMinLengthRule rule) => _rule = rule;

    public override Func<int, bool> GetTypedCondition() => value => value < _rule.MinLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be at least {_rule.MinLength} characters";
}
