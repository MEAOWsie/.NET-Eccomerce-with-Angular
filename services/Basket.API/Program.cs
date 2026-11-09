using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var options = ConfigurationOptions.Parse(redisConn);
options.AbortOnConnectFail = false;

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("healthy"));

app.MapGet("/api/basket", async (string id, IConnectionMultiplexer redis) =>
{
    var data = await redis.GetDatabase().StringGetAsync(id);
    return data.IsNullOrEmpty
        ? Results.Ok(new Basket(id, []))
        : Results.Ok(JsonSerializer.Deserialize<Basket>(data!));
});

app.MapPost("/api/basket", async (Basket basket, IConnectionMultiplexer redis) =>
{
    var saved = await redis.GetDatabase().StringSetAsync(
        basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
    return saved ? Results.Ok(basket) : Results.Problem("Could not save basket");
});

app.MapDelete("/api/basket", async (string id, IConnectionMultiplexer redis) =>
{
    await redis.GetDatabase().KeyDeleteAsync(id);
    return Results.NoContent();
});

app.Run();

record BasketItem(int ProductId, string ProductName, decimal Price, int Quantity,
                  string PictureUrl, string Brand, string Type);
record Basket(string Id, List<BasketItem> Items);
