using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

ProductDto[] products =
[
    new(1, "Mechanical Keyboard", 89.99m),
    new(2, "Wireless Mouse", 34.50m),
    new(3, "USB-C Dock", 129.00m)
];

var api = app.MapGroup("/api/v1/products")
    .WithTags("Products");

api.MapGet("/", () => TypedResults.Ok(products))
    .WithName("GetProducts")
    .WithSummary("Get all products");

api.MapGet("/{id:int}", Results<Ok<ProductDto>, NotFound> (int id) =>
{
    var product = products.FirstOrDefault(product => product.Id == id);

    return product is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(product);
})
    .WithName("GetProductById")
    .WithSummary("Get one product by id");

app.Run();

public sealed record ProductDto(int Id, string Name, decimal Price);
