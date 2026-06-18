using UnityEngine;

public class GoblinArcher : Enemy
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _orbitDistance = 4f;
    [SerializeField] private int _arrowDamage = 12;

    protected override Transform GetTarget()
    {
        var player = ServiceLocator.Get<PlayerController>();
        return player != null ? player.transform : null;
    }

    protected override void MoveTowardTarget()
    {
        if (_target == null) return;
        float dist = Vector2.Distance(transform.position, _target.position);
        if (dist > _orbitDistance + 0.5f)
        {
            Vector2 dir = ((Vector2)_target.position - _rb.position).normalized;
            _rb.MovePosition(_rb.position + dir * (_data.moveSpeed * Time.fixedDeltaTime));
        }
        else if (dist < _orbitDistance - 0.5f)
        {
            Vector2 dir = (_rb.position - (Vector2)_target.position).normalized;
            _rb.MovePosition(_rb.position + dir * (_data.moveSpeed * Time.fixedDeltaTime));
        }
    }

    protected override void AttackTarget()
    {
        if (_target == null || _arrowPrefab == null) return;
        Vector2 dir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        var myCol = GetComponent<Collider2D>();
        var arrowCol = arrow.GetComponent<Collider2D>();
        if (myCol != null && arrowCol != null)
            Physics2D.IgnoreCollision(arrowCol, myCol);
        if (arrow.TryGetComponent<Bullet>(out var bullet))
            bullet.Init(dir, _arrowDamage, fromEnemy: true);
    }
}
