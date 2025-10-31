namespace GameServer.Models;

/// <summary>
/// 英雄数据模型（存储玩家英雄的基本信息）
/// </summary>

public class HeroData
{
    // 使用 required 修饰符，强制要求初始化时必须赋值
    public required string HeroId { get; set; }  // 英雄唯一ID（如 "hero_001"）
    public required string Name { get; set; }    // 英雄名称（如 "战士"）
    public required int Level { get; set; }      // 英雄等级
    public required int Hp { get; set; }         // 当前生命值
    public required int MaxHp { get; set; }      // 最大生命值
    public required int Attack { get; set; }     // 攻击力
    public required string OwnerId { get; set; } // 所属玩家ID（关联User表的Id）
}

