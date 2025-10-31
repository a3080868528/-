using GameServer.Services; // ���������÷��������ռ�
using GameServer.WebSockets; // ����������WebSocket�����ռ�
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// ����MongoDB����
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient("mongodb://localhost:27017");
    return client.GetDatabase("GameDB");
});

// �޸������WebSocket֧�֣����ݿ����ã�
builder.Services.AddWebSockets(_ => { });

// ���ҵ�����
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BattleService>();

var app = builder.Build();

// ����WebSocket�м��
app.UseWebSockets();

// ע��HTTP�ӿں�WebSocket����
app.MapControllers();
app.Map("/ws", WebSocketHandler.HandleWebSocketAsync); // �������ҵ�WebSocketHandler��

app.Run("http://localhost:5000");