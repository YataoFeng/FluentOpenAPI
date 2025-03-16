using FluentOpenApi.Default;
using FluentOpenApi.Validation;

namespace FluentOpenApi.Providers;

public class FluentOpenApiProvider
{
    private readonly Dictionary<Type, IOpenApiSchema> _schemas = new();
    private readonly Dictionary<Type, SchemaValidator> _validations = new();

    public FluentOpenApiProvider(IEnumerable<ModelSchema> schemaBases)
    {
        foreach (var schemaBase in schemaBases)
        {
            schemaBase.ApplyTo(this);
        }
    }

    public FluentOpenApiProvider AddSchema<T>(OpenApiSchema<T> schema) where T : class
    {
        _schemas[typeof(T)] = schema;
        _validations[typeof(T)] = new SchemaValidator<T>(schema);
        return this;
    }

    public IOpenApiSchema? GetSchema(Type type) => _schemas.TryGetValue(type, out var schema) ? schema : null;
    public SchemaValidator? GetValidator(Type type) => _validations.TryGetValue(type, out var schemaValidator) ? schemaValidator : null;
}
 