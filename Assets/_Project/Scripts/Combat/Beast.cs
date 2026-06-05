using UnityEngine;

public class Beast : Enemy
{
    [SerializeField] private float _chargeSpeedMultiplier = 1.5f;
    [SerializeField] private float _chargeRange = 5f;

    protected override Transform GetTarget()
    {
        var player = ServiceLocator.Get<PlayerController>();
        return player != null ? player.transform : null;
    }

    protected override void MoveTowardTarget()
    {
        if (_target == null) return;
        float dist = Vector2.Distance(transform.position, _target.position);
        float speed = dist < _chargeRange ? _data.moveSpeed * _chargeSpeedMultiplier : _data.moveSpeed;
        Vector2 dir = ((Vector2)_target.position - _rb.position).normalized;
        _rb.MovePosition(_rb.position + dir * (speed * Time.fixedDeltaTime));
        if (_spriteRenderer != null) _spriteRenderer.flipX = dir.x < 0;
    }

    protected override void AttackTarget()
    {
        if (_target == null) return;
        if (_target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(_data.damage);
    }
}
