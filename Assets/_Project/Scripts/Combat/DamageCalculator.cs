using UnityEngine;

public static class DamageCalculator
{
    [SerializeField] private static float _critChance = 0.1f;
    [SerializeField] private static float _critMultiplier = 2f;

    public static int Calculate(int baseDamage, out bool isCrit)
    {
        isCrit = Random.value < _critChance;
        return isCrit ? Mathf.RoundToInt(baseDamage * _critMultiplier) : baseDamage;
    }

    public static int Calculate(int baseDamage)
    {
        return Calculate(baseDamage, out _);
    }
}
