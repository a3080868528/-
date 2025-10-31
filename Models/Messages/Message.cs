namespace GameServer.Models;

public class Message
{
    public string Type { get; set; } // 消息类型："login"、"attack"等
    public string Data { get; set; } // 消息内容（JSON字符串）
}