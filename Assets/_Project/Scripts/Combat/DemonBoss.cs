using System.Collections;
using UnityEngine;

public class DemonBoss : Enemy
{
    [SerializeField] private GameObject _slimePrefab;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _summonCount = 2;
    [SerializeField] private float _summonCooldown = 10f;
    [SerializeField] private int _projectileDamage = 15;
    [SerializeField] private float _meleeWarningTime = 0.25f;
    [SerializeField] private float _meleeEffectRadius = 1.4f;
    [SerializeField] private float _projectileScale = 2.6f;

    private int _phase = 1;
    private int _attackCount;
    private float _summonTimer;
    private bool _isStriking;
    private static Sprite _effectSprite;

    private static Sprite EffectSprite
    {
        get
        {
            if (_effectSprite == null)
            {
                var tex = Texture2D.whiteTexture;
                _effectSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f), 100f);
            }
            return _effectSprite;
        }
    }

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

    protected override bool CanChaseTarget(float distanceToTarget)
    {
        return _target != null && _target.TryGetComponent<PlayerController>(out _);
    }

    protected override void AttackTarget()
    {
        if (_target == null) return;

        _attackCount++;
        bool shouldShoot = _projectilePrefab != null && (_phase >= 2 || _attackCount % 3 == 0);
        if (shouldShoot)
        {
            ShootFan();
        }
        else
        {
            if (!_isStriking)
                StartCoroutine(MeleeStrike());
        }
    }

    private IEnumerator MeleeStrike()
    {
        _isStriking = true;
        Vector3 strikePosition = _target != null ? _target.position : transform.position;

        CreatePulse(strikePosition, new Vector2(_meleeEffectRadius * 2.2f, _meleeEffectRadius * 2.2f),
            new Color(1f, 0.15f, 0.05f, 0.25f), _meleeWarningTime, 0f);

        yield return new WaitForSeconds(_meleeWarningTime);

        if (_target != null)
            strikePosition = _target.position;

        CreateSlash(strikePosition, 35f);
        CreateSlash(strikePosition, -35f);
        CreatePulse(strikePosition, new Vector2(_meleeEffectRadius * 1.4f, _meleeEffectRadius * 1.4f),
            new Color(1f, 0.75f, 0.25f, 0.85f), 0.18f, 0.25f);

        if (_target != null && Vector2.Distance(transform.position, _target.position) <= _data.attackRange + 0.5f &&
            _target.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(_data.damage);
        }

        _isStriking = false;
    }

    private void ShootFan()
    {
        Vector2 baseDir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        float[] angles = { -20f, 0f, 20f };
        var myCol = GetComponent<Collider2D>();
        CreatePulse(transform.position + (Vector3)(baseDir * 0.8f), new Vector2(2f, 2f),
            new Color(0.7f, 0.05f, 1f, 0.6f), 0.16f, 0.2f);

        foreach (float angle in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;
            var proj = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            proj.transform.localScale *= _projectileScale;

            if (proj.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.color = new Color(0.85f, 0.15f, 1f, 1f);
                renderer.sortingOrder = 8;
            }

            var projCol = proj.GetComponent<Collider2D>();
            if (myCol != null && projCol != null)
                Physics2D.IgnoreCollision(projCol, myCol);
            if (proj.TryGetComponent<Bullet>(out var bullet))
                bullet.Init(dir, _projectileDamage, fromEnemy: true);
        }
    }

    private void CreateSlash(Vector3 position, float angle)
    {
        var go = CreateEffect("BossSlash", position, new Vector2(2.6f, 0.25f),
            new Color(1f, 0.15f, 0.05f, 0.9f), 0.16f, 0.15f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void CreatePulse(Vector3 position, Vector2 size, Color color, float duration, float growAmount)
    {
        CreateEffect("BossPulse", position, size, color, duration, growAmount);
    }

    private GameObject CreateEffect(string name, Vector3 position, Vector2 size, Color color, float duration, float growAmount)
    {
        var go = new GameObject(name);
        go.transform.position = new Vector3(position.x, position.y, -0.5f);
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = EffectSprite;
        renderer.color = color;
        renderer.sortingLayerID = _spriteRenderer != null ? _spriteRenderer.sortingLayerID : renderer.sortingLayerID;
        renderer.sortingOrder = 20;

        StartCoroutine(FadeEffect(go, renderer, duration, growAmount));
        return go;
    }

    private IEnumerator FadeEffect(GameObject effect, SpriteRenderer renderer, float duration, float growAmount)
    {
        float elapsed = 0f;
        Color startColor = renderer.color;
        Vector3 startScale = effect.transform.localScale;
        Vector3 endScale = startScale + Vector3.one * growAmount;

        while (elapsed < duration && effect != null)
        {
            elapsed += Time.deltaTime;
            float t = duration > 0f ? elapsed / duration : 1f;
            effect.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, t));
            yield return null;
        }

        if (effect != null)
            Destroy(effect);
    }
}
