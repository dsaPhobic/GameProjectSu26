using System.Collections;
using UnityEngine;

public class DemonBoss : Enemy
{
    [SerializeField] private GameObject _slimePrefab;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _summonCount = 2;
    [SerializeField] private float _summonCooldown = 10f;
    [SerializeField] private int _projectileDamage = 15;

    private int _phase = 1;
    private float _summonTimer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        if (_state == EnemyState.Dead) return;
        UpdatePhase();
        if (_phase == 2) HandlePhase2();
    }

    private void UpdatePhase()
    {
        float ratio = (float)_currentHP / maxHP;
        if (ratio <= 0.2f && _phase < 3) EnterPhase(3);
        else if (ratio <= 0.5f && _phase < 2) EnterPhase(2);
    }

    private void EnterPhase(int phase)
    {
        _phase = phase;
        AudioManager.Instance?.PlaySFX("sfx_boss_phase");
        if (phase == 3)
        {
            _data.moveSpeed *= 2f;
            _data.damage = Mathf.RoundToInt(_data.damage * 1.5f);
        }
    }

    private void HandlePhase2()
    {
        _summonTimer -= Time.deltaTime;
        if (_summonTimer <= 0)
        {
            _summonTimer = _summonCooldown;
            StartCoroutine(SummonMinions());
        }
    }

    private IEnumerator SummonMinions()
    {
        for (int i = 0; i < _summonCount; i++)
        {
            if (_slimePrefab != null)
            {
                Vector2 offset = Random.insideUnitCircle * 2f;
                Instantiate(_slimePrefab, transform.position + (Vector3)offset, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    protected override Transform GetTarget()
    {
        var player = ServiceLocator.Get<PlayerController>();
        return player != null ? player.transform : null;
    }

    protected override void AttackTarget()
    {
        if (_target == null) return;
        _animator?.SetTrigger("Attack");

        if (_phase >= 2 && _projectilePrefab != null)
        {
            ShootFan();
        }
        else
        {
            if (_target.TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(_data.damage);
        }
    }

    private void ShootFan()
    {
        Vector2 baseDir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        float[] angles = { -20f, 0f, 20f };
        foreach (float angle in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;
            var proj = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            if (proj.TryGetComponent<Bullet>(out var bullet))
                bullet.Init(dir, _projectileDamage);
        }
    }
}
