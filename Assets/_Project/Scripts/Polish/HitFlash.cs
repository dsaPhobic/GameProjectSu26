using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;

    private Color _originalColor;

    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
        if (_renderer != null) _originalColor = _renderer.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (_renderer == null) yield break;
        _renderer.color = _flashColor;
        yield return new WaitForSeconds(_flashDuration);
        _renderer.color = _originalColor;
    }
}
