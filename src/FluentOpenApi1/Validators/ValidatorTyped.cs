namespace FluentOpenApi.Validators;
public abstract class Validator<T> : Validator
{
    public abstract Func<T, bool> GetTypedCondition();

    public override Func<object, bool> GetCondition()
    {
        return value => GetTypedCondition()((T)value);
    }
}
 