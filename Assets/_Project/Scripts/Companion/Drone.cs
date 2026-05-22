using System.Collections;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float _orbitRadius = 1.5f;
    [SerializeField] private float _orbitSpeed = 90f;
    [SerializeField] private float _detectionRange = 8f;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _bulletPrefab;

    private float _angle;
    private float _fireTimer;

    private void Update()
    {
        Orbit();
        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
        {
            _fireTimer = _fireRate;
            TryShoot();
        }
    }

    private void Orbit()
    {
        _angle += _orbitSpeed * Time.deltaTime;
        float rad = _angle * Mathf.Deg2Rad;
        transform.localPosition = new Vector3(
            Mathf.Cos(rad) * _orbitRadius,
            Mathf.Sin(rad) * _orbitRadius, 0);
    }

    private void TryShoot()
    {
        Enemy nearest = FindNearest();
        if (nearest == null || _bulletPrefab == null) return;
        Vector2 dir = (nearest.transform.position - transform.position).normalized;
        var go = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        if (go.TryGetComponent<Bullet>(out var bullet))
            bullet.Init(dir, _damage);
        AudioManager.Instance?.PlaySFX("sfx_drone_shoot");
    }

    private Enemy FindNearest()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDist = _detectionRange;
        foreach (var e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; nearest = e; }
        }
        return nearest;
    }
}
