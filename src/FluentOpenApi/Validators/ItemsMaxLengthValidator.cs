using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class ItemsMaxLengthValidator : Validator<int>
{
    private readonly ItemsMaxLengthRule _rule;

    public ItemsMaxLengthValidator(ItemsMaxLengthRule rule) => _rule = rule;

    public override Func<int, bool> GetTypedCondition() => value => value > _rule.MaxLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must not exceed {_rule.MaxLength} characters";
}
