using FluentOpenApi.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FluentOpenApi.Extensions;
public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationEndpointFilter>();
    }
}
