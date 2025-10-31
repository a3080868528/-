// WebSockets/WebSocketHandler.cs
using GameServer.Models;
using GameServer.Models.Messages;
using GameServer.Services.Battle; // 引用战斗服务接口
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameServer.WebSockets;

// 去掉static，改成普通类
public class WebSocketHandler
{
    private readonly IBattleService _battleService; // 通过注入获取
    private readonly Dictionary<string, WebSocket> _connections = new();

    // 构造函数：注入IBattleService
    public WebSocketHandler(IBattleService battleService)
    {
        _battleService = battleService; // 赋值
    }

    // 原来的HandleWebSocketAsync方法去掉static，参数不变
    public async Task HandleWebSocketAsync(HttpContext context, RequestDelegate next)
    {
        // 内容和原来一样，只是去掉static
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string userId = Guid.NewGuid().ToString();
        _connections.Add(userId, webSocket);
        Console.WriteLine($"用户 {userId} 连接WebSocket");

        try
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(userId, messageText);
                }
            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        finally
        {
            _connections.Remove(userId);
            Console.WriteLine($"用户 {userId} 断开WebSocket");
        }
    }

    // HandleMessageAsync去掉static，使用注入的_battleService
    private async Task HandleMessageAsync(string userId, string messageText)
    {
        var message = JsonSerializer.Deserialize<Message>(messageText);
        if (message == null) return;

        switch (message.Type)
        {
            case "attack":
                var attackRequest = JsonSerializer.Deserialize<AttackRequest>(message.Data);
                // 这里不再new，而是用注入的_battleService
                var attackResult = _battleService.CalculateDamage(attackRequest);
                await SendMessageAsync(userId, "attack_result", attackResult);
                break;
        }
    }

    // 修改方法定义，增加 messageType 参数
    public async Task SendMessageAsync(string userId, string messageType, object data)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            // 构建包含类型和数据的 Message 对象
            var message = new Message
            {
                Type = messageType,
                Data = data
            };
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}