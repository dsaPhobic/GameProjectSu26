using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUpBuffs : MonoBehaviour
{
    private class RuntimeBuff
    {
        public PowerUpData Data;
        public float Remaining;
        public int AppliedIntAmount;
        public float AppliedFloatAmount;
    }

    private readonly Dictionary<PowerUpType, RuntimeBuff> _active = new();
    private PlayerStats _stats;
    private PlayerController _controller;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_active.Count == 0) return;

        var expiredTypes = new List<PowerUpType>();
        foreach (var pair in _active)
        {
            pair.Value.Remaining -= Time.deltaTime;
            if (pair.Value.Remaining <= 0f)
                expiredTypes.Add(pair.Key);
        }

        foreach (PowerUpType type in expiredTypes)
            RemoveBuff(type, revertEffect: true);
    }

    public bool Apply(PowerUpData data)
    {
        if (_stats == null || data == null || data.duration <= 0f) return false;

        switch (data.type)
        {
            case PowerUpType.Speed:
                ApplySpeed(data);
                return true;
            case PowerUpType.DoubleDamage:
                ApplyDoubleDamage(data);
                return true;
            case PowerUpType.Shield:
                ApplyShield(data);
                return true;
            default:
                return false;
        }
    }

    public List<ActivePowerUpBuffSaveData> GetSaveData()
    {
        var data = new List<ActivePowerUpBuffSaveData>();
        foreach (var pair in _active)
        {
            RuntimeBuff buff = pair.Value;
            if (buff.Remaining <= 0f || buff.Data == null) continue;

            data.Add(new ActivePowerUpBuffSaveData
            {
                type = pair.Key,
                remaining = buff.Remaining,
                duration = buff.Data.duration,
                magnitude = buff.Data.magnitude,
                appliedIntAmount = buff.AppliedIntAmount,
                appliedFloatAmount = buff.AppliedFloatAmount
            });
        }

        return data;
    }

    public void Restore(List<ActivePowerUpBuffSaveData> savedBuffs, IEnumerable<PowerUpData> dataSet)
    {
        _active.Clear();
        if (savedBuffs == null) return;

        var lookup = new Dictionary<PowerUpType, PowerUpData>();
        if (dataSet != null)
        {
            foreach (PowerUpData data in dataSet)
            {
                if (data != null)
                    lookup[data.type] = data;
            }
        }

        foreach (ActivePowerUpBuffSaveData saved in savedBuffs)
        {
            if (saved.remaining <= 0f) continue;

            lookup.TryGetValue(saved.type, out PowerUpData powerUpData);
            var buff = new RuntimeBuff
            {
                Data = powerUpData,
                Remaining = saved.remaining,
                AppliedIntAmount = saved.appliedIntAmount,
                AppliedFloatAmount = saved.appliedFloatAmount
            };

            _active[saved.type] = buff;

            if (powerUpData != null)
                PowerUpTimerUI.Show(_stats, powerUpData, saved.remaining);

            if (saved.type == PowerUpType.Shield)
                _controller?.ActivateShield(saved.remaining, powerUpData != null ? powerUpData.icon : null);
        }
    }

    private void ApplySpeed(PowerUpData data)
    {
        RemoveBuff(PowerUpType.Speed, revertEffect: true);

        var buff = new RuntimeBuff
        {
            Data = data,
            Remaining = data.duration,
            AppliedFloatAmount = data.magnitude
        };

        _stats.ModifyMoveSpeed(buff.AppliedFloatAmount);
        _active[PowerUpType.Speed] = buff;
        PowerUpTimerUI.Show(_stats, data);
    }

    private void ApplyDoubleDamage(PowerUpData data)
    {
        RemoveBuff(PowerUpType.DoubleDamage, revertEffect: true);

        int bonusDamage = Mathf.RoundToInt(_stats.Damage * (data.magnitude - 1f));
        var buff = new RuntimeBuff
        {
            Data = data,
            Remaining = data.duration,
            AppliedIntAmount = bonusDamage
        };

        _stats.ModifyDamage(buff.AppliedIntAmount);
        _active[PowerUpType.DoubleDamage] = buff;
        PowerUpTimerUI.Show(_stats, data);
    }

    private void ApplyShield(PowerUpData data)
    {
        RemoveBuff(PowerUpType.Shield, revertEffect: false);

        _active[PowerUpType.Shield] = new RuntimeBuff
        {
            Data = data,
            Remaining = data.duration
        };

        PowerUpTimerUI.Show(_stats, data);
        _controller?.ActivateShield(data.duration, data.icon);
    }

    private void RemoveBuff(PowerUpType type, bool revertEffect)
    {
        if (!_active.TryGetValue(type, out RuntimeBuff buff)) return;
        _active.Remove(type);

        if (!revertEffect || _stats == null) return;

        if (type == PowerUpType.Speed)
            _stats.ModifyMoveSpeed(-buff.AppliedFloatAmount);
        else if (type == PowerUpType.DoubleDamage)
            _stats.ModifyDamage(-buff.AppliedIntAmount);
    }
}
