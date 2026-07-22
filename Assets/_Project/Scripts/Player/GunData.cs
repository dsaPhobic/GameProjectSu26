using UnityEngine;

public enum GunFireMode
{
    SemiAutomatic,
    Automatic
}

[CreateAssetMenu(fileName = "SO_Gun", menuName = "MagicFarm/Gun Data")]
public class GunData : ScriptableObject
{
    public string id = "arcane_pistol";
    public string displayName = "Arcane Pistol";
    [TextArea] public string description = "Reliable sidearm.";
    public Sprite icon;
    public int price;
    public bool unlockedByDefault;

    [Header("Firing")]
    public GunFireMode fireMode = GunFireMode.SemiAutomatic;
    [Min(0.1f)] public float shotsPerSecond = 2f;
    [Range(0.05f, 5f)] public float damageMultiplier = 1f;
    [Min(1)] public int pellets = 1;
    [Range(0f, 45f)] public float spreadAngle;
    [Range(0.25f, 3f)] public float bulletScale = 1f;
    public bool allowChargedShot = true;
}
