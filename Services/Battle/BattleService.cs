// Services/Battle/BattleService.cs
using GameServer.Models.Entities;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

// 实现接口
public class BattleService : IBattleService
{
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // 原来的逻辑不变
        int targetDefense = 50;
        int finalDamage = Math.Max(1, request.AttackValue - targetDefense);
        int remainingHp = 100 - finalDamage;

        return new AttackResult
        {
            TargetId = request.TargetId,
            FinalDamage = finalDamage,
            RemainingHp = remainingHp
        };
    }
}