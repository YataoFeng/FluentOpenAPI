using FluentOpenApi.Rules;
using System.Text.RegularExpressions;

namespace FluentOpenApi.Validators;
public class PatternValidator : Validator<string>
{
    private readonly PatternRule _rule;

    public PatternValidator(PatternRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && !Regex.IsMatch(value, _rule.Pattern);
    public override string GetErrorMessage(string propertyName) => $"{propertyName} has invalid format";
}
