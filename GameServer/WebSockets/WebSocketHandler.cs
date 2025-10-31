using GameServer.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System; // ����Guid
using GameServer.Services;

namespace GameServer.WebSockets;

public static class WebSocketHandler
{
    // �洢�����û����ӣ��û�ID �� WebSocket��
    private static readonly Dictionary<string, WebSocket> _connections = new();

    public static async Task HandleWebSocketAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        // ����WebSocket����
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string userId = Guid.NewGuid().ToString(); // ��ʱ�û�ID����¼���滻��
        _connections.Add(userId, webSocket);
        Console.WriteLine($"�û� {userId} ����WebSocket");

        try
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            // ѭ������ǰ����Ϣ
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(userId, messageText); // ������Ϣ
                }
            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        finally
        {
            _connections.Remove(userId);
            Console.WriteLine($"�û� {userId} �Ͽ�WebSocket");
        }
    }

    // ����ǰ����Ϣ����ʱֻʵ�ֹ����߼���
    private static async Task HandleMessageAsync(string userId, string messageText)
    {
        var message = JsonSerializer.Deserialize<Message>(messageText);
        if (message == null) return;

        switch (message.Type)
        {
            case "attack":
                // ������������
                var attackRequest = JsonSerializer.Deserialize<AttackRequest>(message.Data);
                // ����ս����������˺�������ʵ��BattleService��
                var battleService = new BattleService();
                var attackResult = battleService.CalculateDamage(attackRequest);
                // �����߷��ؽ��
                await SendMessageAsync(userId, "attack_result", attackResult);
                break;
        }
    }

    // ��ָ���û�������Ϣ
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

// �����������ݽṹ��ǰ�˷��ͣ�
public class AttackRequest
{
    public string AttackerId { get; set; } // ������ID
    public string TargetId { get; set; }   // Ŀ��ID
    public int AttackValue { get; set; }   // ����ֵ
}

// ������������ظ�ǰ�ˣ�
public class AttackResult
{
    public string TargetId { get; set; }
    public int FinalDamage { get; set; }
    public int RemainingHp { get; set; }
}