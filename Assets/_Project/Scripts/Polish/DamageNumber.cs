using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _riseSpeed = 1.5f;
    [SerializeField] private float _lifetime = 1f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _critColor = Color.yellow;

    public void Init(int damage, bool isCrit = false)
    {
        if (_text != null)
        {
            _text.text = damage.ToString();
            _text.color = isCrit ? _critColor : _normalColor;
        }
        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float elapsed = 0f;
        Color startColor = _text != null ? _text.color : Color.white;
        while (elapsed < _lifetime)
        {
            transform.position += Vector3.up * (_riseSpeed * Time.deltaTime);
            if (_text != null)
            {
                Color c = startColor;
                c.a = 1f - (elapsed / _lifetime);
                _text.color = c;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
