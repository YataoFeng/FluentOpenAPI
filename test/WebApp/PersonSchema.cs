using FluentOpenAPI;

namespace WebApp;

public class PersonSchema : ModelSchemaBase<Person>
{
    public PersonSchema()
    {
        PropertyFor(x => x.Name)
            .Required()
            .Matches(@"^[a-zA-Z\s]+$")
            .MinLength(2)
            .MaxLength(50)
            .WithDescription("Person's full name")
            .WithDefault("John Doe");

        PropertyFor(x => x.Age)
            .Range(0, 150);

        PropertyFor(x => x.Email)
            .Required()
            .WithDescription("Contact email");
    }
}
