using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class MaxLengthValidator : Validator<string>
{
    private readonly MaxLengthRule _rule;

    public MaxLengthValidator(MaxLengthRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && value.Length > _rule.MaxLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must not exceed {_rule.MaxLength} characters";
}
 