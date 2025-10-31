// Program.cs
using GameServer.Services.Battle; // 引用战斗服务
using GameServer.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 注册MongoDB（不变）
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient("mongodb://localhost:27017");
    return client.GetDatabase("GameDB");
});

// 注册WebSocket服务（不变）
builder.Services.AddWebSockets(_ => { });

// 注册战斗服务（接口+实现）
builder.Services.AddScoped<IBattleService, BattleService>();
// 注册WebSocketHandler（因为它需要依赖IBattleService）
builder.Services.AddScoped<WebSocketHandler>();

var app = builder.Build();

// 启用WebSocket（不变）
app.UseWebSockets();

// 映射WebSocket路径（这里要改，因为WebSocketHandler现在是普通类，需要通过依赖注入获取）
app.Map("/ws", async context =>
{
    // 从容器中获取WebSocketHandler实例（自动注入IBattleService）
    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await handler.HandleWebSocketAsync(context, _ => Task.CompletedTask);
});

app.Run("http://localhost:5000");