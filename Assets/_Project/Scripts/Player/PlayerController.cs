using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerStats))]
public class PlayerController : Entity
{
    [SerializeField] private float _dashForce = 12f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private GameObject _bulletPrefab;

    private Rigidbody2D _rb;
    private PlayerInput _input;
    private PlayerStats _stats;
    private PlayerAnimator _animator;
    private PlayerToolHandler _toolHandler;

    private bool _isDashing;
    private float _dashCooldownTimer;
    private float _attackCooldownTimer;
    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponent<PlayerAnimator>();
        _toolHandler = GetComponent<PlayerToolHandler>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<PlayerController>();
    }

    private bool IsInputBlocked()
    {
        var state = GameManager.Instance?.CurrentState;
        return state == GameState.Paused || state == GameState.GameOver || state == GameState.LevelUp;
    }

    private void Update()
    {
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;
        if (_attackCooldownTimer > 0) _attackCooldownTimer -= Time.deltaTime;

        if (IsInputBlocked()) return;

        if (_input.DashPressed && _dashCooldownTimer <= 0 && !_isDashing)
            StartCoroutine(Dash());

        if (_input.AttackPressed && !_isDashing && _attackCooldownTimer <= 0)
            Shoot();

        if (_input.ToolUsePressed && !_isDashing)
            _toolHandler?.UseTool();

        if (_input.ToolSwitchInput > 0)
            _toolHandler?.SwitchTool(_input.ToolSwitchInput);

        FlipSprite(_input.AimDirection);
        _animator?.SetMoving(_input.MovementInput != Vector2.zero);
    }

    private void Shoot()
    {
        if (_bulletPrefab == null) return;
        _attackCooldownTimer = 1f / Mathf.Max(_stats.AttackSpeed, 0.1f);
        _animator?.SetAttacking(true);
        Vector3 spawnPos = transform.position + (Vector3)(_input.AimDirection * 0.6f);
        GameObject bullet = Instantiate(_bulletPrefab, spawnPos, Quaternion.identity);
        if (bullet.TryGetComponent<Bullet>(out var b))
            b.Init(_input.AimDirection, _stats.Damage);
        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.3f);
        _animator?.SetAttacking(false);
    }

    private void FixedUpdate()
    {
        if (_isDashing) return;
        _rb.MovePosition(_rb.position + _input.MovementInput * (_stats.MoveSpeed * Time.fixedDeltaTime));
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _dashCooldownTimer = _dashCooldown;
        _animator?.TriggerDash();
        Vector2 dir = _input.MovementInput == Vector2.zero ? _input.AimDirection : _input.MovementInput;
        _rb.AddForce(dir * _dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_dashDuration);
        _rb.velocity = Vector2.zero;
        _isDashing = false;
    }

    private void FlipSprite(Vector2 aimDir)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.flipX = aimDir.x < 0;
    }

    public void ReduceDashCooldown(float delta) => _dashCooldown = Mathf.Max(0.1f, _dashCooldown - delta);

    public override void TakeDamage(int damage)
    {
        _stats.TakeDamage(damage);
    }

    protected override void Die()
    {
        GameEvents.RaisePlayerDied();
    }
}
