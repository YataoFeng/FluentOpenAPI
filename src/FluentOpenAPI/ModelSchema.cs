using FluentOpenApi.Providers;

namespace FluentOpenApi;
public abstract class ModelSchema
{
    public abstract void ApplyTo(FluentOpenApiProvider provider);
}
