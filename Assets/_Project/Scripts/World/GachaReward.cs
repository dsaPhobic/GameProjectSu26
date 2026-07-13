using System;
using UnityEngine;

[Serializable]
public class GachaReward
{
    public enum RewardType
    {
        Gold,
        Seed,
        XP,
        Heal,
        Gun,
        PetEgg,
        BuffDamage,
        BuffAttackSpeed,
        BuffMoveSpeed,
        BuffMaxHP
    }

    public string displayName = "Gold";
    public Sprite icon;
    public RewardType type = RewardType.Gold;
    public PetEggData petEgg;
    public int amount = 25;
    public int seedIndex;
    [Min(1)] public int weight = 10;

    public void Give(PlayerToolHandler player)
    {
        PlayerStats stats = player != null ? player.GetComponent<PlayerStats>() : null;

        switch (type)
        {
            case RewardType.Gold:
                stats?.AddGold(amount);
                break;
            case RewardType.Seed:
                player?.AddSeed(seedIndex, amount);
                break;
            case RewardType.XP:
                stats?.AddXP(amount);
                break;
            case RewardType.Heal:
                stats?.Heal(amount);
                break;
            case RewardType.Gun:
                Debug.Log($"Received gun reward: {displayName}");
                break;
            case RewardType.PetEgg:
                if (petEgg != null)
                    PlayerPetInventory.AddEgg(petEgg, Mathf.Max(1, amount));
                else
                    Debug.LogWarning($"Pet egg reward '{displayName}' has no PetEggData assigned.");
                break;
            case RewardType.BuffDamage:
                stats?.ModifyDamage(amount);
                break;
            case RewardType.BuffAttackSpeed:
                stats?.ModifyAttackSpeed(amount / 100f);
                break;
            case RewardType.BuffMoveSpeed:
                stats?.ModifyMoveSpeed(amount / 100f);
                break;
            case RewardType.BuffMaxHP:
                stats?.ModifyMaxHP(amount);
                break;
        }
    }

    public string GetResultText()
    {
        return type switch
        {
            RewardType.Gold => $"+{amount} Gold",
            RewardType.Seed => $"+{amount} {displayName}",
            RewardType.XP => $"+{amount} XP",
            RewardType.Heal => $"+{amount} HP",
            RewardType.Gun => displayName,
            RewardType.PetEgg => $"+{Mathf.Max(1, amount)} {(petEgg != null ? petEgg.eggName : displayName)}",
            RewardType.BuffDamage => $"+{amount} Damage",
            RewardType.BuffAttackSpeed => $"+{amount}% Attack Speed",
            RewardType.BuffMoveSpeed => $"+{amount}% Move Speed",
            RewardType.BuffMaxHP => $"+{amount} Max HP",
            _ => displayName
        };
    }
}
