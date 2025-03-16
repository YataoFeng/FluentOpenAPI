using FluentOpenApi.Models;

namespace FluentOpenApi.Default;
public abstract class SchemaValidator
{
    public abstract ValidationResult ValidateForObject(object instance);
}
 