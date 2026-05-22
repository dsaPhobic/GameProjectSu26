using UnityEngine;

public class Slime : Enemy
{
    protected override Transform GetTarget()
    {
        Crop[] crops = FindObjectsByType<Crop>(FindObjectsSortMode.None);
        if (crops.Length == 0) return null;

        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var crop in crops)
        {
            float d = Vector2.Distance(transform.position, crop.transform.position);
            if (d < minDist) { minDist = d; nearest = crop.transform; }
        }
        return nearest;
    }

    protected override void AttackTarget()
    {
        if (_target == null) return;
        _animator?.SetTrigger("Attack");
        if (_target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(_data.damage);
    }
}
