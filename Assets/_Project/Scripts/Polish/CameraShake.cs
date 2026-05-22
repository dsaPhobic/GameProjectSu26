using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration = 0.2f, float magnitude = 0.3f)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 origin = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = origin;
    }
}
