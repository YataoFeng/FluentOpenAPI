using FluentOpenAPI;
using Scalar.AspNetCore;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddOpenApi();
builder.Services.AddFluentOpenAPI(o =>
{
    o.AddSchema<PersonSchema>();
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("/person", (Person person) => Results.Ok(person))
   .WithValidation();

app.Run();
