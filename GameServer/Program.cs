using GameServer.Services; // 新增：引用服务命名空间
using GameServer.WebSockets; // 新增：引用WebSocket命名空间
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// 配置MongoDB连接
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient("mongodb://localhost:27017");
    return client.GetDatabase("GameDB");
});

// 修复：添加WebSocket支持（传递空配置）
builder.Services.AddWebSockets(_ => { });

// 添加业务服务
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BattleService>();

var app = builder.Build();

// 启用WebSocket中间件
app.UseWebSockets();

// 注册HTTP接口和WebSocket处理
app.MapControllers();
app.Map("/ws", WebSocketHandler.HandleWebSocketAsync); // 现在能找到WebSocketHandler了

app.Run("http://localhost:5000");