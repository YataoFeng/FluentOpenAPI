namespace FluentOpenAPI;

public interface IOpenApiSchema
{
    Type ModelType { get; }
    ISchemaDescriptor CreateDescriptor();
}