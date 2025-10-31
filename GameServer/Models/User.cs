namespace GameServer.Models;

public class User
{

    public string? Id { get; set; } // 允许为null
    public required string Username { get; set; } // 必须初始化
    public required string Password { get; set; }
}