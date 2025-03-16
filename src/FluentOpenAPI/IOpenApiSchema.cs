namespace FluentOpenApi;

public interface IOpenApiSchema
{
    Type ModelType { get; }
    ISchemaDescriptor CreateDescriptor();
}