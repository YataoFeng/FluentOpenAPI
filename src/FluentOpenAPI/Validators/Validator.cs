namespace FluentOpenApi.Validators;
public abstract class Validator
{
    public abstract Func<object, bool> GetCondition();
    public abstract string GetErrorMessage(string propertyName);
}
