using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] private int _cropAttackSortingOrder = 3;
    [SerializeField] private float _entranceReachDistance = 1.5f;
    [SerializeField] private float _farmSideOffset = 1f;

    private Transform _entranceTarget;
    private bool _reachedEntrance;

    private void Start()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.sortingOrder = Mathf.Max(_spriteRenderer.sortingOrder, _cropAttackSortingOrder);
    }

    protected override Transform GetTarget()
    {
        var farmManager = ServiceLocator.Get<FarmManager>();
        Crop nearestCrop = farmManager != null ? farmManager.GetNearestCrop(transform.position) : null;
        return nearestCrop != null ? nearestCrop.transform : null;
    }

    protected override bool CanChaseTarget(float distanceToTarget)
    {
        return _target != null && _target.TryGetComponent<Crop>(out _);
    }

    protected override void MoveTowardTarget()
    {
        Transform moveTarget = GetSlimeMoveTarget();
        if (moveTarget == null) return;

        Vector2 dir = ((Vector2)moveTarget.position - _rb.position).normalized;
        _rb.MovePosition(_rb.position + dir * (_data.moveSpeed * Time.fixedDeltaTime));
        if (_spriteRenderer != null) _spriteRenderer.flipX = dir.x < 0;
    }

    private Transform GetSlimeMoveTarget()
    {
        if (_reachedEntrance)
            return _target;

        if (_entranceTarget == null)
            _entranceTarget = FarmEntrance.GetNearest(transform.position);

        if (_entranceTarget == null)
        {
            _reachedEntrance = true;
            return _target;
        }

        if (HasReachedEntrance())
        {
            _reachedEntrance = true;
            return _target;
        }

        return _entranceTarget;
    }

    private bool HasReachedEntrance()
    {
        float distToEntrance = Vector2.Distance(transform.position, _entranceTarget.position);
        if (distToEntrance <= _entranceReachDistance)
            return true;

        return transform.position.x < _entranceTarget.position.x - _farmSideOffset;
    }

    protected override void AttackTarget()
    {
        if (_target == null) return;
        if (_target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(GetScaledDamage(_data.damage));
    }
}
