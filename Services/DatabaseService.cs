using Microsoft.Data.SqlClient;

namespace SecretScanLab.Services;

public class DatabaseService
{
    // TODO: Move to configuration - hardcoded for quick development
    private const string ConnectionString =
        "Server=tcp:labsqlserver2024.database.windows.net,1433;" +
        "Initial Catalog=ProductsDB;" +
        "User ID=sqladminuser;" +
        "Password=P@55w0rd!Lab2024Secure#;" +
        "Encrypt=True;TrustServerCertificate=False;";

    public async Task<List<Product>> GetProductsAsync()
    {
        var products = new List<Product>();

        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Name, Price, Category FROM Products", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product(
                reader.GetString(0),
                reader.GetDecimal(1),
                reader.GetString(2)
            ));
        }

        return products;
    }

    public async Task AddProductAsync(Product product)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "INSERT INTO Products (Name, Price, Category) VALUES (@Name, @Price, @Category)",
            connection);

        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Category", product.Category);

        await command.ExecuteNonQueryAsync();
    }
}
