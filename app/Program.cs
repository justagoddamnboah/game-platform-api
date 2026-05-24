using game_platform.net.api;
using game_platform.net.database;
using game_platform.net.dto;
using game_platform.net.interfaces;
using game_platform.net.services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() {
        Title = "Game Platform API",
        Version = "v1",
        Description = "Демонстрационный API игровой платформы."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<PlatformDbContext>(options => {
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IMapper, Mapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    using (var scope = app.Services.CreateScope()) {
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Platform API v1");
        options.RoutePrefix = "swagger";
    });
}

var api = app.MapGroup("/api");
api.MapUsersEndpoints();
api.MapGamesEndpoints();
api.MapPurchasesEndpoints();

app.MapGet("/", () => Results.Ok(new {
    message = "Game Platform API работает."
}));

await app.RunAsync();