using FluentOpenAPI.Extensions;
using Scalar.AspNetCore;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentOpenAPI(o =>
{
    o.AddSchema<PersonSchema>();
});
builder.Services.AddOpenApi(o =>
{
    o.AddFluentSchemaTransformer();
});
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("/person", (Person person) => Results.Ok(person))
   .WithValidation();

app.Run();
