var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/gateway/health", () => Results.Ok("healthy"));
app.MapReverseProxy();

app.Run();
