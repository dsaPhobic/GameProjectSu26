using UnityEngine;

public enum EnemyState { Idle, Chase, Attack, Dead }

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : Entity
{
    [SerializeField] protected EnemyData _data;
    [SerializeField] private float _bounceSpeed = 10f;
    [SerializeField] private float _bounceAmount = 0.12f;

    protected Rigidbody2D _rb;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected EnemyState _state = EnemyState.Idle;
    protected Transform _target;
    private float _attackTimer;
    private Vector3 _baseScale;

    protected static readonly int AnimIsMoving = Animator.StringToHash("IsMoving");
    protected static readonly int AnimIsAttacking = Animator.StringToHash("IsAttacking");
    protected static readonly int AnimIsDead = Animator.StringToHash("IsDead");

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_data != null) { maxHP = _data.maxHP; _currentHP = maxHP; }
        _baseScale = transform.localScale;
        gameObject.AddComponent<EnemyHealthBar>();
    }

    private void Update()
    {
        if (_state == EnemyState.Dead) return;
        _target = GetTarget();
        UpdateState();
        UpdateBounce();
    }

    private void UpdateBounce()
    {
        if (_state == EnemyState.Chase)
        {
            float bounce = 1f + Mathf.Abs(Mathf.Sin(Time.time * _bounceSpeed)) * _bounceAmount;
            transform.localScale = new Vector3(_baseScale.x, _baseScale.y * bounce, _baseScale.z);
        }
        else
        {
            transform.localScale = _baseScale;
        }
    }

    private void FixedUpdate()
    {
        if (_state == EnemyState.Chase && _target != null)
            MoveTowardTarget();
    }

    private void UpdateState()
    {
        if (_target == null) { _state = EnemyState.Idle; return; }

        float dist = Vector2.Distance(transform.position, _target.position);

        if (dist <= _data.attackRange)
        {
            _state = EnemyState.Attack;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                _attackTimer = _data.attackCooldown;
                AttackTarget();
            }
        }
        else if (CanChaseTarget(dist))
        {
            _state = EnemyState.Chase;
        }
        else
        {
            _state = EnemyState.Idle;
        }

        _animator?.SetBool(AnimIsMoving, _state == EnemyState.Chase);
        _animator?.SetBool(AnimIsAttacking, _state == EnemyState.Attack);
        if (_animator != null)
            _animator.speed = (_state == EnemyState.Idle) ? 0f : 1f;
    }

    protected virtual void MoveTowardTarget()
    {
        Vector2 dir = ((Vector2)_target.position - _rb.position).normalized;
        _rb.MovePosition(_rb.position + dir * (_data.moveSpeed * Time.fixedDeltaTime));
        if (_spriteRenderer != null) _spriteRenderer.flipX = dir.x < 0;
    }

    protected virtual bool CanChaseTarget(float distanceToTarget)
    {
        return distanceToTarget <= _data.detectionRange;
    }

    protected abstract Transform GetTarget();
    protected abstract void AttackTarget();

    protected override void Die()
    {
        _state = EnemyState.Dead;
        if (_animator != null) _animator.speed = 1f;
        _animator?.SetBool(AnimIsDead, true);
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        GameEvents.RaiseEnemyDied(this);
        Destroy(gameObject, 1.5f);
    }

    public EnemyData Data => _data;
}
