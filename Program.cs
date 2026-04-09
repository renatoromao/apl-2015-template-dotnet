using SecretScanLab.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<StorageService>();

var app = builder.Build();

app.MapGet("/", () => "Secret Scan Lab API is running");

app.MapGet("/products", async (DatabaseService db) =>
{
    var products = await db.GetProductsAsync();
    return Results.Ok(products);
});

app.MapPost("/products", async (DatabaseService db, Product product) =>
{
    await db.AddProductAsync(product);
    return Results.Created($"/products/{product.Name}", product);
});

app.MapPost("/upload", async (StorageService storage, HttpRequest request) =>
{
    using var stream = new MemoryStream();
    await request.Body.CopyToAsync(stream);
    stream.Position = 0;

    var fileName = request.Headers["X-File-Name"].FirstOrDefault() ?? "upload.bin";
    var url = await storage.UploadFileAsync(fileName, stream);
    return Results.Ok(new { url });
});

app.MapGet("/files", async (StorageService storage) =>
{
    var files = await storage.ListFilesAsync();
    return Results.Ok(files);
});

app.Run();

public record Product(string Name, decimal Price, string Category);
