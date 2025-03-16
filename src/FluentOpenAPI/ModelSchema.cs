using FluentOpenAPI.Providers;

namespace FluentOpenAPI;
public abstract class ModelSchema
{
    public abstract void ApplyTo(FluentOpenApiProvider provider);
}
