using GameServer.Models;
using GameServer.WebSockets;

namespace GameServer.Services;

public class BattleService
{
    // 临时模拟：计算伤害（后续会从数据库读取目标防御）
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // 这里先硬编码目标防御为50（实际应从MongoDB查询目标英雄数据）
        int targetDefense = 50;
        int finalDamage = Math.Max(1, request.AttackValue - targetDefense);
        int remainingHp = 100 - finalDamage; // 临时模拟目标剩余血量

        return new AttackResult
        {
            TargetId = request.TargetId,
            FinalDamage = finalDamage,
            RemainingHp = remainingHp
        };
    }
}