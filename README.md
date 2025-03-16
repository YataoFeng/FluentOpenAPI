# FluentOpenApi 框架介绍

`FluentOpenApi` 是一个轻量级、流畅的框架，用于定义 OpenAPI 架构（Schema）并集成数据验证功能。它通过链式调用的方式简化了 API 模型的定义，同时支持与 ASP.NET Core 无缝集成。本文档将介绍如何使用 `FluentOpenApi`，如何将其集成到 ASP.NET Core 项目中，以及如何进行进阶扩展。

## 目录
- [基本使用](#基本使用)
- [与 ASP.NET Core 集成](#与-aspnet-core-集成)
- [进阶扩展规则和验证条件](#进阶扩展规则和验证条件)
- [进阶扩展方法](#进阶扩展方法)
- [总结](#总结)

---

## 基本使用

`FluentOpenApi` 通过 `ModelSchema<T>` 基类和链式调用方法定义模型的架构规则。以下是一个简单的例子：

### 定义模型和架构
```csharp
public class Person
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public string[]? Items { get; set; }
}

public class PersonSchema : ModelSchema<Person>
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

        PropertyFor(x => x.Items)
            .RangeForArray(1, 5);
    }
}
```
- `PropertyFor`：指定模型的属性。
- 链式方法：如 `Required()`、`Matches()`、`MinLength()` 等，用于定义规则和验证条件。

### 核心概念
- **规则（Rules）**：描述属性的元数据，例如是否必填、默认值等。
- **验证器（Validators）**：执行数据验证逻辑，例如检查长度或正则匹配。
- **流畅接口**：通过链式调用配置属性规则。

---

## 与 ASP.NET Core 集成

`FluentOpenApi` 提供了与 ASP.NET Core 的集成支持，可以自动生成 OpenAPI 文档并执行请求验证。以下是集成步骤：

### 配置服务
在 `Program.cs` 中配置 `FluentOpenApi` 和 OpenAPI 服务：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 注册 FluentOpenApi 和 Schema
builder.Services.AddFluentOpenAPI(o =>
{
    o.AddSchema<PersonSchema>();
});

// 配置 OpenAPI 并添加 Schema 转换器
builder.Services.AddOpenApi(o =>
{
    o.AddFluentSchemaTransformer();
});

var app = builder.Build();

// 启用 OpenAPI 端点
app.MapOpenApi();
app.MapScalarApiReference();

// 定义 API 端点并启用验证
app.MapPost("/person", (Person person) => Results.Ok(person))
   .WithValidation();

app.Run();
```
### 工作原理
1. **`AddFluentOpenAPI`**：
   - 注册 `FluentOpenApiProvider` 和 `ModelSchema` 实例。
   - 添加验证过滤器（`ValidationEndpointFilter`）以支持 `.WithValidation()`。
2. **`AddFluentSchemaTransformer`**：
   - 将 `FluentOpenApi` 的规则应用到 OpenAPI 文档中，例如设置必填字段、描述和默认值。
3. **端点验证**：
   - 使用 `.WithValidation()` 启用自动验证，基于 `PersonSchema` 中的规则检查请求数据。

### 测试 API
发送以下请求：
```json
{
    "Name": "123",
    "Age": 200,
    "Email": null,
    "Items": ["a", "b", "c", "d", "e", "f"]
}
```
响应将是验证错误，例如：
```json
{
  "Name": [
    "Name has invalid format"
  ],
  "Age": [
    "Age must be between 0 and 150"
  ],
  "Email": [
    "Email cannot be null"
  ]
}
```

---

## 进阶扩展规则和验证条件

`FluentOpenApi` 支持自定义规则和验证器，以满足复杂需求。

### 自定义规则
创建一个新的规则，例如 `EmailRule`：
```csharp
public class EmailRule : SchemaRule
{
    public override void Apply(OpenApiSchema schema)
    {
        schema.Format = "email";
    }
}
```

### 自定义验证器
创建一个对应的验证器，例如 `EmailValidator`：
```csharp
public class EmailValidator : Validator
{
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public override Func<object?, bool> GetCondition()
    {
        return value => value == null || !EmailRegex.IsMatch(value.ToString()!);
    }

    public override string GetErrorMessage(string propertyName)
    {
        return $"{propertyName} must be a valid email address";
    }
}
```

### 集成自定义规则和验证器
在 `PersonSchema` 中使用：
```csharp
public class PersonSchema : ModelSchema<Person>
{
    public PersonSchema()
    {
        PropertyFor(x => x.Email)
            .Required()
            .WithDescription("Contact email")
            .AddRule(new EmailRule())
            .WithValidation(new EmailValidator());
    }
}
```
- `AddRule`：添加自定义规则。
- `WithValidation`：绑定自定义验证器。

---

## 进阶扩展方法

你可以通过扩展 `SchemaExtensions` 类添加自定义链式方法。

### 添加自定义扩展方法
例如，添加 `Email()` 方法：
```csharp
public static class SchemaExtensions
{
    // 已有方法省略...

    public static PropertyRuleBuilder<T, string> Email<T>(
        this PropertyRuleBuilder<T, string> builder) where T : class
    {
        return builder.AddRule(new EmailRule()).WithValidation(new EmailValidator());
    }
}
```

### 使用扩展方法
更新 `PersonSchema`：
```csharp
public class PersonSchema : ModelSchema<Person>
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
            .Email() // 使用自定义扩展方法
            .WithDescription("Contact email");

        PropertyFor(x => x.Items)
            .RangeForArray(1, 5);
    }
}
```
- `Email()` 封装了 `EmailRule` 和 `EmailValidator`，简化配置。

### 扩展复杂类型
支持复杂类型的默认值：
```csharp
public static PropertyRuleBuilder<T, TProperty> WithDefaultComplex<T, TProperty>(
    this PropertyRuleBuilder<T, TProperty> builder, Func<TProperty> defaultFactory) where T : class
{
    return builder.AddRule(new DefaultRule(ToOpenApiAny(defaultFactory())));
}
```
使用：
```csharp
PropertyFor(x => x.Items)
    .RangeForArray(1, 5)
    .WithDefaultComplex(() => new[] { "item1", "item2" });
```

---

## 总结

`FluentOpenApi` 提供了一种优雅的方式来定义 OpenAPI 架构和验证规则。通过与 ASP.NET Core 的集成，你可以轻松生成文档并验证请求数据。框架的扩展性允许开发者添加自定义规则、验证器和链式方法，适应各种业务需求。

- **优点**：流畅的 API、易于集成、可扩展性强。
- **适用场景**：需要规范化 API 定义和验证的 ASP.NET Core 项目。

如需更多示例或帮助，请联系框架维护者！
--- 
