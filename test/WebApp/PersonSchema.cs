using FluentOpenApi;
using FluentOpenApi.Extensions;

namespace WebApp;

public class PersonSchema : ModelSchema<Person>
{
    public PersonSchema()
    {
        For(x => x.Name)
            .Required()
            .RegularExpression(@"^[a-zA-Z\s]+$")
            .MinLength(2)
            .MaxLength(50)
            .WithDescription("Person's full name")
            .WithDefault("John Doe");

        For(x => x.Age)
            .Range(0, 150);

        For(x => x.Email)
            .Required()
            .WithDescription("Contact email");

        For(x => x.Items)
            .ItemsRange(3, 10);
    }
}
