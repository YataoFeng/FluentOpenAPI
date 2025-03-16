using FluentOpenApi.Providers;
using Microsoft.AspNetCore.Http;

namespace FluentOpenApi.Validation;
internal class ValidationEndpointFilter(FluentOpenApiProvider provider) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var arg in context.Arguments)
        {
            if (arg == null)
            {
                continue;
            }
            var schemaValidator = provider.GetValidator(arg.GetType());
            if (schemaValidator == null)
            {
                continue;
            }
            var result = schemaValidator.ValidateForObject(arg);
            if (!result.IsValid)
            {
                return Results.BadRequest(result.ToDictionary());
            }
        }
        return await next(context);
    }
}
 