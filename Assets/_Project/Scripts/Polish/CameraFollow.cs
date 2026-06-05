using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;
    [SerializeField] private float _minY = -10f;
    [SerializeField] private float _maxY = 10f;

    private void LateUpdate()
    {
        if (_target == null) return;
        Vector3 desired = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, _smoothSpeed * Time.deltaTime);
        smoothed.x = Mathf.Clamp(smoothed.x, _minX, _maxX);
        smoothed.y = Mathf.Clamp(smoothed.y, _minY, _maxY);
        transform.position = smoothed;
    }
}
