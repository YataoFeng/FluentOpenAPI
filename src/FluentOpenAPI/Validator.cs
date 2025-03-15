using System.Text.RegularExpressions;

namespace FluentOpenAPI;

public abstract class Validator
{
    public abstract Func<object, bool> GetCondition();
    public abstract string GetErrorMessage(string propertyName);
}

public abstract class Validator<T> : Validator
{
    public abstract Func<T, bool> GetTypedCondition();

    public override Func<object, bool> GetCondition()
    {
        return value => GetTypedCondition()((T)value);
    }
}

public class RequiredValidator : Validator<object>
{
    public override Func<object, bool> GetTypedCondition() => value => value == null;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} cannot be null";
}

public class RangeValidator : Validator<int> // 改为 object 以支持多种类型
{
    private readonly RangeRule _rule;

    public RangeValidator(RangeRule rule) => _rule = rule;

    public override Func<int, bool> GetTypedCondition()
    {
        return value => value < _rule.Min || value > _rule.Max;
    }

    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be between {_rule.Min} and {_rule.Max}";
}

public class MinLengthValidator : Validator<string>
{
    private readonly MinLengthRule _rule;

    public MinLengthValidator(MinLengthRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && value.Length < _rule.MinLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must be at least {_rule.MinLength} characters";
}

public class MaxLengthValidator : Validator<string>
{
    private readonly MaxLengthRule _rule;

    public MaxLengthValidator(MaxLengthRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && value.Length > _rule.MaxLength;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} must not exceed {_rule.MaxLength} characters";
}

public class PatternValidator : Validator<string>
{
    private readonly PatternRule _rule;

    public PatternValidator(PatternRule rule) => _rule = rule;

    public override Func<string, bool> GetTypedCondition() => value => value != null && !Regex.IsMatch(value, _rule.Pattern);
    public override string GetErrorMessage(string propertyName) => $"{propertyName} has invalid format";
}