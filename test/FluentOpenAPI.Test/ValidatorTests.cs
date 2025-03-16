using AutoFixture;
using FluentAssertions;
using FluentOpenAPI.Providers;
using FluentOpenAPI.Rules;
using FluentOpenAPI.Test.Model;
using FluentOpenAPI.Test.Schemas;
using FluentOpenAPI.Validation;
using FluentOpenAPI.Validators;

namespace FluentOpenAPI.Test;
public class ValidatorTests
{
    private readonly Fixture _fixture;

    public ValidatorTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void RequiredValidator_ValidatesNullValue()
    {
        // Arrange
        var validator = new RequiredValidator();
        var person = new Person { Name = null };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Name!);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Name").Should().Be("Name cannot be null");
    }

    [Fact]
    public void RequiredValidator_ValidatesNonNullValue()
    {
        // Arrange
        var validator = new RequiredValidator();
        var person = new Person { Name = "John" };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Name);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RangeValidator_ValidatesOutOfRangeValue()
    {
        // Arrange
        var rule = new RangeRule(0, 150);
        var validator = new RangeValidator(rule);
        var person = new Person { Age = 200 };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Age);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Age").Should().Be("Age must be between 0 and 150");
    }

    [Fact]
    public void RangeValidator_ValidatesInRangeValue()
    {
        // Arrange
        var rule = new RangeRule(0, 150);
        var validator = new RangeValidator(rule);
        var person = new Person { Age = 25 };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Age);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RangeValidator_ValidatesArrayLength()
    {
        // Arrange
        var rule = new RangeRule(1, 5, true);
        var validator = new RangeValidator(rule);
        var person = new Person { Items = ["a", "b", "c", "d", "e", "f"] };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Items);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Items").Should().Be("Items length must be between 1 and 5");
    }

    [Fact]
    public void MinLengthValidator_ValidatesShortString()
    {
        // Arrange
        var rule = new MinLengthRule(2);
        var validator = new MinLengthValidator(rule);
        var person = new Person { Name = "A" };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Name);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Name").Should().Be("Name must be at least 2 characters");
    }

    [Fact]
    public void MaxLengthValidator_ValidatesLongString()
    {
        // Arrange
        var rule = new MaxLengthRule(50);
        var validator = new MaxLengthValidator(rule);
        var person = new Person { Name = new string('A', 51) };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Name);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Name").Should().Be("Name must not exceed 50 characters");
    }

    [Fact]
    public void PatternValidator_ValidatesInvalidPattern()
    {
        // Arrange
        var rule = new PatternRule(@"^[a-zA-Z\s]+$");
        var validator = new PatternValidator(rule);
        var person = new Person { Name = "123" };

        // Act
        var condition = validator.GetCondition();
        var result = condition(person.Name);

        // Assert
        result.Should().BeTrue();
        validator.GetErrorMessage("Name").Should().Be("Name has invalid format");
    }

    [Fact]
    public void SchemaValidator_ValidatesPersonWithFailures()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([schema]);
        var validator = new SchemaValidator<Person>(provider.GetSchema(typeof(Person))!);
        var person = _fixture.Build<Person>()
                             .With(p => p.Name, "1")
                             .With(p => p.Age, 200)
                             .With(p => p.Email, (string?)null)
                             .With(p => p.Items, ["a", "b", "c", "d", "e", "f"])
                             .Create();

        // Act
        var result = validator.Validate(person);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Failures.Should().Contain(f => f.PropertyName == "Name" && f.ErrorMessage == "Name has invalid format");
        result.Failures.Should().Contain(f => f.PropertyName == "Name" && f.ErrorMessage == "Name must be at least 2 characters");
        result.Failures.Should().Contain(f => f.PropertyName == "Age" && f.ErrorMessage == "Age must be between 0 and 150");
        result.Failures.Should().Contain(f => f.PropertyName == "Email" && f.ErrorMessage == "Email cannot be null");
        result.Failures.Should().Contain(f => f.PropertyName == "Items" && f.ErrorMessage == "Items length must be between 1 and 5");
    }

    [Fact]
    public void SchemaValidator_ValidatesValidPerson()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([schema]);
        var validator = new SchemaValidator<Person>(provider.GetSchema(typeof(Person))!);
        var person = _fixture.Build<Person>()
                             .With(p => p.Name, "John Doe")
                             .With(p => p.Age, 25)
                             .With(p => p.Email, "john.doe@example.com")
                             .With(p => p.Items, ["a", "b"])
                             .Create();

        // Act
        var result = validator.Validate(person);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Failures.Should().BeEmpty();
    }

    [Fact]
    public void SchemaValidator_ValidatesNullInstance()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([schema]);
        var validator = new SchemaValidator<Person>(provider.GetSchema(typeof(Person))!);

        // Act
        var result = validator.Validate(null!);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Failures.Should().ContainSingle(f => f.PropertyName == "Instance" && f.ErrorMessage == "Instance cannot be null");
    }
}