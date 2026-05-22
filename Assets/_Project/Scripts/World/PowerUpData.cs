using UnityEngine;

public enum PowerUpType { Speed, DoubleDamage, Shield, HealthRestore, GrowthMagic, Magnet }

[CreateAssetMenu(fileName = "SO_PowerUp", menuName = "MagicFarm/Power-Up Data")]
public class PowerUpData : ScriptableObject
{
    public PowerUpType type;
    public float duration = 10f;
    public float magnitude = 1.5f;
    public Sprite icon;
}
