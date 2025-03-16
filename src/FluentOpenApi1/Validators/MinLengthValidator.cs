using FluentOpenApi.Rules;

namespace FluentOpenApi.Validators;
public class MinLengthValidator : Validator<string>
{
    private readonly MinLengthRule _rule;

    public MinLengthValidator(MinLengthRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && value.Length < _rule.MinLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be at least {_rule.MinLength} characters";
}
 