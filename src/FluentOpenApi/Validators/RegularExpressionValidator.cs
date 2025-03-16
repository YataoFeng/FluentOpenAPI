using FluentOpenApi.Rules;
using System.Text.RegularExpressions;

namespace FluentOpenApi.Validators;
public class RegularExpressionValidator : Validator<string>
{
    private readonly RegularExpressionRule _rule;

    public RegularExpressionValidator(RegularExpressionRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && !Regex.IsMatch(value, _rule.Pattern);
    public override string GetErrorMessage(string propertyName) => $"{propertyName} has invalid format";
}