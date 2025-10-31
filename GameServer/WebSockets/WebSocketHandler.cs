using GameServer.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System; // 用于Guid
using GameServer.Services;

namespace GameServer.WebSockets;

public static class WebSocketHandler
{
    // 存储在线用户连接（用户ID → WebSocket）
    private static readonly Dictionary<string, WebSocket> _connections = new();

    public static async Task HandleWebSocketAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        // 接受WebSocket连接
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string userId = Guid.NewGuid().ToString(); // 临时用户ID（登录后替换）
        _connections.Add(userId, webSocket);
        Console.WriteLine($"用户 {userId} 连接WebSocket");

        try
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            // 循环接收前端消息
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(userId, messageText); // 处理消息
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

    // 处理前端消息（暂时只实现攻击逻辑）
    private static async Task HandleMessageAsync(string userId, string messageText)
    {
        var message = JsonSerializer.Deserialize<Message>(messageText);
        if (message == null) return;

        switch (message.Type)
        {
            case "attack":
                // 解析攻击请求
                var attackRequest = JsonSerializer.Deserialize<AttackRequest>(message.Data);
                // 调用战斗服务计算伤害（后续实现BattleService）
                var battleService = new BattleService();
                var attackResult = battleService.CalculateDamage(attackRequest);
                // 向发送者返回结果
                await SendMessageAsync(userId, "attack_result", attackResult);
                break;
        }
    }

    // 向指定用户发送消息
    public static async Task SendMessageAsync(string userId, string type, object data)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            var message = new Message
            {
                Type = type,
                Data = JsonSerializer.Serialize(data)
            };
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

// 攻击请求数据结构（前端发送）
public class AttackRequest
{
    public string AttackerId { get; set; } // 攻击者ID
    public string TargetId { get; set; }   // 目标ID
    public int AttackValue { get; set; }   // 攻击值
}

// 攻击结果（返回给前端）
public class AttackResult
{
    public string TargetId { get; set; }
    public int FinalDamage { get; set; }
    public int RemainingHp { get; set; }
}