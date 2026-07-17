using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector2 _followOffset = new Vector2(-1.2f, 0.8f);
    [SerializeField] private float _followSpeed = 8f;
    [SerializeField] private float _hoverAmplitude = 0.15f;
    [SerializeField] private float _hoverFrequency = 3f;

    [Header("Combat")]
    [SerializeField] private float _detectionRange = 8f;
    [SerializeField] private float _fireCooldown = 2f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;

    private float _fireTimer;
    private SpriteRenderer _spriteRenderer;
    public int Damage => _damage;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ResolveFollowTarget();
    }

    private void OnEnable()
    {
        ResolveFollowTarget();
        _fireTimer = 0f;
    }

    private void Update()
    {
        FollowPlayer();

        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
            TryShoot();
    }

    private void ResolveFollowTarget()
    {
        if (_followTarget != null) return;

        PlayerController player = ServiceLocator.Get<PlayerController>();
        if (player == null) player = FindObjectOfType<PlayerController>();
        _followTarget = player != null ? player.transform : transform.parent;
    }

    private void FollowPlayer()
    {
        if (_followTarget == null)
        {
            ResolveFollowTarget();
            if (_followTarget == null) return;
        }

        Vector3 offset = _followOffset;
        offset.y += Mathf.Sin(Time.time * _hoverFrequency) * _hoverAmplitude;

        Vector3 targetPosition = _followTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
    }

    private void TryShoot()
    {
        Enemy nearest = FindNearest();
        if (nearest == null || _bulletPrefab == null) return;

        _fireTimer = Mathf.Max(0.05f, _fireCooldown);
        Vector2 dir = (nearest.transform.position - transform.position).normalized;
        Vector3 spawnPosition = _muzzle != null ? _muzzle.position : transform.position;

        if (_spriteRenderer != null && Mathf.Abs(dir.x) > 0.01f)
            _spriteRenderer.flipX = dir.x < 0f;

        var go = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);
        if (go.TryGetComponent<Bullet>(out var bullet))
            bullet.Init(dir, _damage);
        AudioManager.Instance?.PlaySFX("sfx_drone_shoot");
    }

    private Enemy FindNearest()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDistSqr = _detectionRange * _detectionRange;
        foreach (var e in enemies)
        {
            if (e == null || e.IsDead || !e.gameObject.activeInHierarchy) continue;

            float d = ((Vector2)e.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (d < minDistSqr)
            {
                minDistSqr = d;
                nearest = e;
            }
        }
        return nearest;
    }

    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    public void ModifyDamage(int delta)
    {
        _damage = Mathf.Max(0, _damage + delta);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
