using AutoFixture;
using FluentAssertions;
using FluentOpenApi.Default;
using FluentOpenApi.Providers;
using FluentOpenApi.Rules;
using FluentOpenApi.Test.Model;
using FluentOpenApi.Test.Schemas;
using FluentOpenApi.Validators;
using Microsoft.OpenApi.Any;
using Moq;

namespace FluentOpenApi.Test;

public class ModelSchemaTests
{
    private readonly Fixture _fixture;

    public ModelSchemaTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior()); 
    }

    [Fact]
    public void ModelSchema_Configure_AddsRulesCorrectly()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([schema]);

        // Act
        var openApiSchema = provider.GetSchema(typeof(Person)) as OpenApiSchema<Person>;
        var descriptor = openApiSchema!.CreateDescriptor();
        var rules = descriptor.GetRules();

        // Assert
        rules.Should().ContainKey("Name");
        var nameRules = rules["Name"];
        nameRules.Should().Contain(r => r.GetType() == typeof(RequiredRule));
        nameRules.Should().Contain(r => r.GetType() == typeof(RegularExpressionRule) && ((RegularExpressionRule)r).Pattern == @"^[a-zA-Z\s]+$");
        nameRules.Should().Contain(r => r.GetType() == typeof(MinLengthRule) && ((MinLengthRule)r).MinLength == 2);
        nameRules.Should().Contain(r => r.GetType() == typeof(MaxLengthRule) && ((MaxLengthRule)r).MaxLength == 50);
        nameRules.Should().Contain(r => r.GetType() == typeof(DescriptionRule) && ((DescriptionRule)r).Description == "Person's full name");
        nameRules.Should().Contain(r =>
                    r.GetType() == typeof(DefaultRule) &&
                    ((DefaultRule)r).DefaultValue.GetType() == typeof(OpenApiString) &&
                    ((OpenApiString)((DefaultRule)r).DefaultValue).Value == "John Doe");

        rules.Should().ContainKey("Age");
        var ageRules = rules["Age"];
        ageRules.Should().Contain(r => r.GetType() == typeof(RangeRule) && ((RangeRule)r).Min == 0 && ((RangeRule)r).Max == 150);

        rules.Should().ContainKey("Email");
        var emailRules = rules["Email"];
        emailRules.Should().Contain(r => r.GetType() == typeof(RequiredRule));
        emailRules.Should().Contain(r => r.GetType() == typeof(DescriptionRule) && ((DescriptionRule)r).Description == "Contact email");

        rules.Should().ContainKey("Items");
        var itemsRules = rules["Items"];
        itemsRules.Should().Contain(r => r.GetType() == typeof(RangeRule) && ((RangeRule)r).IsArrayLength && ((RangeRule)r).Min == 1 && ((RangeRule)r).Max == 5);
    }

    [Fact]
    public void ModelSchema_WithValidation_ReplacesRuleWithValidator()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([schema]);

        // Act
        var openApiSchema = provider.GetSchema(typeof(Person)) as OpenApiSchema<Person>;
        var descriptor = openApiSchema!.CreateDescriptor();

        // Assert
        var rulesAndValidators = descriptor.GetRulesAndValidators().ToList();
        var nameRules = rulesAndValidators.Where(r => r.PropertyName == "Name").ToList();
        nameRules.Should().ContainSingle(r => r.Rule is RequiredRule && r.Validator is RequiredValidator);
        nameRules.Should().ContainSingle(r => r.Rule is RegularExpressionRule && r.Validator is RegularExpressionValidator);
        nameRules.Should().ContainSingle(r => r.Rule is MinLengthRule && r.Validator is MinLengthValidator);
        nameRules.Should().ContainSingle(r => r.Rule is MaxLengthRule && r.Validator is MaxLengthValidator);
        nameRules.Should().ContainSingle(r => r.Rule is DescriptionRule && r.Validator == null);
        nameRules.Should().ContainSingle(r => r.Rule is DefaultRule && r.Validator == null);
    }

    [Fact]
    public void ModelSchema_ApplyTo_CallsProviderAddSchema()
    {
        // Arrange
        var schema = new PersonSchema();
        var provider = new FluentOpenApiProvider([]); 

        // Act
        schema.ApplyTo(provider);
        var openApiSchema = provider.GetSchema(typeof(Person));

        // Assert
        openApiSchema.Should().NotBeNull();
        openApiSchema.Should().BeOfType<OpenApiSchema<Person>>();
    }
}