using GameServer.Models;
using GameServer.WebSockets;

namespace GameServer.Services;

public class BattleService
{
    // ��ʱģ�⣺�����˺�������������ݿ��ȡĿ�������
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // ������Ӳ����Ŀ�����Ϊ50��ʵ��Ӧ��MongoDB��ѯĿ��Ӣ�����ݣ�
        int targetDefense = 50;
        int finalDamage = Math.Max(1, request.AttackValue - targetDefense);
        int remainingHp = 100 - finalDamage; // ��ʱģ��Ŀ��ʣ��Ѫ��

        return new AttackResult
        {
            TargetId = request.TargetId,
            FinalDamage = finalDamage,
            RemainingHp = remainingHp
        };
    }
}