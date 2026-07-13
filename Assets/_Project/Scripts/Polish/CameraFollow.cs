using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;
    [SerializeField] private float _minY = -10f;
    [SerializeField] private float _maxY = 10f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (_target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
                SetTarget(player.transform, snap: true);
        }
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;

        Vector3 desired = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, _smoothSpeed * Time.deltaTime);
        smoothed.x = Mathf.Clamp(smoothed.x, _minX + halfW, _maxX - halfW);
        smoothed.y = Mathf.Clamp(smoothed.y, _minY + halfH, _maxY - halfH);
        transform.position = smoothed;
    }

    public void SetTarget(Transform target, bool snap = false)
    {
        _target = target;
        if (!snap || _target == null || _cam == null) return;

        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;

        Vector3 position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        position.x = Mathf.Clamp(position.x, _minX + halfW, _maxX - halfW);
        position.y = Mathf.Clamp(position.y, _minY + halfH, _maxY - halfH);
        transform.position = position;
    }
}
