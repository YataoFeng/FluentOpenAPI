using FluentOpenAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FluentOpenAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentOpenAPI(this IServiceCollection services, Action<FluentOpenApiBuilder> configure)
    {
        var builder = new FluentOpenApiBuilder(services);
        configure(builder);

        services.AddSingleton<FluentOpenApiProvider>(sp =>
        {
            var schemaBases = sp.GetServices<ModelSchemaBase>();
            return new FluentOpenApiProvider(schemaBases);
        });

        services.AddScoped(typeof(ValidationEndpointFilter));
        services.AddOpenApi(o => o.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            var provider = context.ApplicationServices.GetRequiredService<FluentOpenApiProvider>();
            var type = context.JsonTypeInfo.Type;
            var typeSchema = provider.GetSchema(type);
            if (typeSchema != null)
            {
                var descriptor = typeSchema.CreateDescriptor();
                var propertyRules = descriptor.GetRules();
                schema.Required = propertyRules
                .Where(r => r.Value.Any(v => v is RequiredRule))
                .Select(r => char.ToLower(r.Key[0]) + r.Key[1..]).ToHashSet();
                foreach (var (prop, rules) in propertyRules)
                {
                    var propSchema = schema.Properties[prop.ToLowerInvariant()];
                    foreach (var rule in rules.Cast<SchemaRule>())
                    {
                        rule.Apply(propSchema);
                    }
                }
            }
            return Task.CompletedTask;
        }));

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

    public FluentOpenApiBuilder AddSchema<T>() where T : ModelSchemaBase
    {
        _services.AddSingleton<ModelSchemaBase, T>();
        return this;
    }
}

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder) 
    {
        return builder.AddEndpointFilter<ValidationEndpointFilter>();
    }
}
