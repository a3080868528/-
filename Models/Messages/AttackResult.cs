namespace GameServer.Models.Entities;

// 攻击结果（返回给前端）
public class AttackResult
{

    public string? AttackId { get; set; }
    public string? TargetId { get; set; }
    public string? FinalDamage { get; set; }
    public string? RemainingHp { get; set; }
}