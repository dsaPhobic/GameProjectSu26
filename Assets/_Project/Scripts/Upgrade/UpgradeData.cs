using UnityEngine;

public enum UpgradeEffect
{
    IncreaseDamage,
    IncreaseAttackSpeed,
    IncreaseMaxHP,
    IncreaseMoveSpeed,
    UnlockDragonArmor,
    UnlockDrone,
    ReduceCooldown,
    IncreaseDroneDamage
}

[CreateAssetMenu(fileName = "SO_Upgrade", menuName = "MagicFarm/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    public UpgradeCategory category;
    public UpgradeEffect effect;
    public float value;
    public Sprite icon;
}
