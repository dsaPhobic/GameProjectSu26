using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    [SerializeField] private float _slowRadius = 3f;
    [SerializeField] private float _slowFactor = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
            enemy.Data.moveSpeed *= _slowFactor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
            enemy.Data.moveSpeed /= _slowFactor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, _slowRadius);
    }
}
