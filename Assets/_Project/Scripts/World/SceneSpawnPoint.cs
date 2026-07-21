using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string _spawnId = "Default";
    [SerializeField] private Vector2 _facingDirection = Vector2.down;

    public string SpawnId => _spawnId;
    public Vector2 FacingDirection => _facingDirection;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_facingDirection.normalized);
    }
}
