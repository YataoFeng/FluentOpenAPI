namespace FluentOpenApi.Validators;
public class RequiredValidator : Validator<object>
{
    public override Func<object, bool> GetTypedCondition() => value => value == null;
    public override string GetErrorMessage(string propertyName) => $"{propertyName} cannot be null";
}
 