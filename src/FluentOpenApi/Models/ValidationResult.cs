namespace FluentOpenApi.Models;
public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<(string PropertyName, string ErrorMessage)> Failures { get; }

    public ValidationResult(bool isValid, IEnumerable<(string PropertyName, string ErrorMessage)> failures)
    {
        IsValid = isValid;
        Failures = failures.ToList().AsReadOnly();
    }

    public Dictionary<string, string[]> ToDictionary()
    {
        return Failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}
 