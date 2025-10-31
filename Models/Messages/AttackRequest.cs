
namespace GameServer.Models.Messages;
// 攻击请求数据结构（前端发送）
public class AttackRequest
{
    public required string AttackId { get; set; }   // 攻击者ID
    public required string TargetId { get; set; }   // 目标ID
    public required string AttackValue { get; set; }   // 攻击值
}