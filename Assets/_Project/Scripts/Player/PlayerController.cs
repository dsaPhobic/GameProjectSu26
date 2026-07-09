using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerStats))]
public class PlayerController : Entity
{
    [SerializeField] private float _dashForce = 12f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _chargeDuration = 1.2f;
    [SerializeField] private float _chargedShotScale = 1.8f;
    [SerializeField] private float _chargedDamageMultiplier = 2.5f;
    [SerializeField] private float _burstSpreadAngle = 10f;

    private Rigidbody2D _rb;
    private PlayerInput _input;
    private PlayerStats _stats;
    private PlayerAnimator _animator;
    private PlayerToolHandler _toolHandler;
    private PlayerChargeBar _chargeBar;

    private bool _isDashing;
    private bool _isChargingShot;
    private float _chargeTimer;
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
        _chargeBar = GetComponent<PlayerChargeBar>();
        if (_chargeBar == null) _chargeBar = gameObject.AddComponent<PlayerChargeBar>();
        if (GetComponent<PlayerPetInventory>() == null) gameObject.AddComponent<PlayerPetInventory>();
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

        if (IsInputBlocked())
        {
            CancelChargedShot();
            return;
        }

        if (_input.DashPressed && _dashCooldownTimer <= 0 && !_isDashing)
            StartCoroutine(Dash());

        if (_input.AttackPressed && !_isDashing && _attackCooldownTimer <= 0)
            Shoot();

        HandleChargedShotInput();

        if (_input.ToolUsePressed && !_isDashing && !CanChargeShot())
            _toolHandler?.UseTool();

        if (_input.ToolSwitchInput > 0)
            _toolHandler?.SwitchTool(_input.ToolSwitchInput);

        if (_input.SeedCyclePressed)
            _toolHandler?.CycleSeed();

        FlipSprite(_input.AimDirection);
        _toolHandler?.AimTool(_input.AimDirection);
        _animator?.SetMoving(_input.MovementInput != Vector2.zero);
    }

    private bool CanChargeShot()
    {
        return _toolHandler != null && _toolHandler.CurrentTool == ToolType.Gun;
    }

    private void HandleChargedShotInput()
    {
        if (_isDashing || _attackCooldownTimer > 0)
        {
            if (_isChargingShot) CancelChargedShot();
            return;
        }

        if (!CanChargeShot())
        {
            if (_isChargingShot) CancelChargedShot();
            return;
        }

        if (_input.ToolUsePressed)
        {
            _isChargingShot = true;
            _chargeTimer = 0f;
            _chargeBar?.Show();
        }

        if (_isChargingShot && _input.ToolUseHeld)
        {
            _chargeTimer = Mathf.Min(_chargeDuration, _chargeTimer + Time.deltaTime);
            _chargeBar?.SetProgress(_chargeDuration > 0f ? _chargeTimer / _chargeDuration : 1f);
        }

        if (_isChargingShot && _input.ToolUseReleased)
            ReleaseChargedShot();
    }

    private void ReleaseChargedShot()
    {
        bool fullyCharged = _chargeDuration <= 0f || _chargeTimer >= _chargeDuration;
        _isChargingShot = false;
        _chargeBar?.Hide();

        if (fullyCharged)
            ShootChargedBullet();
        else
            ShootBurst();
    }

    private void CancelChargedShot()
    {
        _isChargingShot = false;
        _chargeTimer = 0f;
        _chargeBar?.Hide();
    }

    private void Shoot()
    {
        _toolHandler?.EquipGun();
        if (_bulletPrefab == null) return;
        _attackCooldownTimer = 1f / Mathf.Max(_stats.AttackSpeed, 0.1f);
        _animator?.SetAttacking(true);
        Vector3 spawnPos = transform.position + (Vector3)(_input.AimDirection * 0.6f);
        GameObject bullet = Instantiate(_bulletPrefab, spawnPos, Quaternion.identity);
        if (bullet.TryGetComponent<Bullet>(out var b))
            b.Init(_input.AimDirection, _stats.Damage);
        AudioManager.Instance?.PlaySFX("sfx_attack");
        StartCoroutine(ResetAttack());
    }

    private void ShootBurst()
    {
        if (_bulletPrefab == null) return;
        _attackCooldownTimer = 1f / Mathf.Max(_stats.AttackSpeed, 0.1f);
        _animator?.SetAttacking(true);

        float[] angles = { -_burstSpreadAngle, 0f, _burstSpreadAngle };
        foreach (float angle in angles)
        {
            Vector3 rotated = Quaternion.Euler(0f, 0f, angle) * (Vector3)_input.AimDirection;
            Vector2 direction = new Vector2(rotated.x, rotated.y);
            SpawnBullet(direction, _stats.Damage, 1f);
        }

        AudioManager.Instance?.PlaySFX("sfx_attack");
        StartCoroutine(ResetAttack());
    }

    private void ShootChargedBullet()
    {
        if (_bulletPrefab == null) return;
        _attackCooldownTimer = 1f / Mathf.Max(_stats.AttackSpeed, 0.1f);
        _animator?.SetAttacking(true);

        int damage = Mathf.RoundToInt(_stats.Damage * _chargedDamageMultiplier);
        SpawnBullet(_input.AimDirection, damage, _chargedShotScale);

        AudioManager.Instance?.PlaySFX("sfx_attack");
        StartCoroutine(ResetAttack());
    }

    private void SpawnBullet(Vector2 direction, int damage, float scale)
    {
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * 0.6f);
        GameObject bullet = Instantiate(_bulletPrefab, spawnPos, Quaternion.identity);
        bullet.transform.localScale *= scale;
        if (bullet.TryGetComponent<Bullet>(out var b))
            b.Init(direction, damage);
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
        bool facingLeft = aimDir.x < 0;
        _spriteRenderer.flipX = facingLeft;
        _toolHandler?.FlipTool(facingLeft);
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
