using FluentOpenAPI.Models;

namespace FluentOpenAPI.Default;
public abstract class SchemaValidator
{
    public abstract ValidationResult ValidateForObject(object instance);
}
