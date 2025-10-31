// Services/Battle/IBattleService.cs
using GameServer.Models.Entities;
using GameServer.Models.Messages; // 引用AttackRequest和AttackResult的命名空间

namespace GameServer.Services.Battle;

public interface IBattleService
{
    // 定义和BattleService一样的方法
    AttackResult CalculateDamage(AttackRequest request);
}