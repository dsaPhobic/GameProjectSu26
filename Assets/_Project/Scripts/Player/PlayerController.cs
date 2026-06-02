using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerStats))]
public class PlayerController : Entity
{
    [SerializeField] private float _dashForce = 12f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private float _dashDuration = 0.15f;

    private Rigidbody2D _rb;
    private PlayerInput _input;
    private PlayerStats _stats;
    private PlayerAnimator _animator;
    private PlayerToolHandler _toolHandler;

    private bool _isDashing;
    private float _dashCooldownTimer;
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
    }

    private void Start()
    {
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<PlayerController>();
    }

    private void Update()
    {
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;

        if (_input.DashPressed && _dashCooldownTimer <= 0 && !_isDashing)
            StartCoroutine(Dash());

        if (_input.AttackPressed && !_isDashing)
            _toolHandler?.UseTool();

        if (_input.ToolSwitchInput > 0)
            _toolHandler?.SwitchTool(_input.ToolSwitchInput);

        FlipSprite(_input.AimDirection);
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

    public override void TakeDamage(int damage)
    {
        _stats.TakeDamage(damage);
    }

    protected override void Die()
    {
        GameEvents.RaisePlayerDied();
    }
}
