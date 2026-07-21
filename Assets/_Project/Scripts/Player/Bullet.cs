using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _lifetime = 5f;

    private int _damage;
    private Vector2 _direction;
    private bool _fromEnemy;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, int damage, bool fromEnemy = false)
    {
        _direction = direction.normalized;
        _damage = damage;
        _fromEnemy = fromEnemy;
        Destroy(gameObject, _lifetime);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_fromEnemy)
        {
            if (other.GetComponent<Enemy>() != null) return;
        }
        else
        {
            if (other.GetComponent<PlayerController>() != null) return;
        }

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
