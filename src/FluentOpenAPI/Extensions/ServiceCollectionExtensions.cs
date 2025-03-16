using FluentOpenApi.Providers;
using FluentOpenApi.Rules;
using FluentOpenApi.Validation;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;

namespace FluentOpenApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static OpenApiOptions AddFluentSchemaTransformer(this OpenApiOptions options)
    {
        return options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            var provider = context.ApplicationServices.GetRequiredService<FluentOpenApiProvider>();
            var type = context.JsonTypeInfo.Type;
            var typeSchema = provider.GetSchema(type);
            if (typeSchema != null)
            {
                var descriptor = typeSchema.CreateDescriptor();
                foreach (var (prop, rules) in descriptor.GetRules())
                {
                    var propName = prop.ToLowerInvariant();
                    var propSchema = schema.Properties[propName];
                    foreach (var rule in rules.Cast<SchemaRule>())
                    {
                        if (rule is RequiredRule requiredRule)
                        {
                            requiredRule.PropertyName = propName;
                            requiredRule.Apply(schema);
                        }
                        else
                        {
                            rule.Apply(propSchema);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        });
    }
    public static IServiceCollection AddFluentOpenApi(this IServiceCollection services, Action<FluentOpenApiBuilder> configure)
    {
        var builder = new FluentOpenApiBuilder(services);
        configure(builder);

        services.AddSingleton(sp =>
        {
            var schemaBases = sp.GetServices<ModelSchema>();
            return new FluentOpenApiProvider(schemaBases);
        });

        services.AddScoped(typeof(ValidationEndpointFilter));

        return services;
    }
}

public class FluentOpenApiBuilder
{
    private readonly IServiceCollection _services;

    public FluentOpenApiBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public FluentOpenApiBuilder AddSchema<T>() where T : ModelSchema
    {
        _services.AddSingleton<ModelSchema, T>();
        return this;
    }
}


