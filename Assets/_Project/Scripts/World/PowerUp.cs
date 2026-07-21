using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private static readonly List<PowerUp> ActivePowerUps = new();

    [SerializeField] private PowerUpData _data;
    [SerializeField] private float _magnetPullSpeed = 12f;

    private bool _isCollected;
    private Coroutine _attractCoroutine;

    public PowerUpData Data => _data;
    public PowerUpType Type => _data != null ? _data.type : PowerUpType.Speed;
    public bool CanBeSaved => !_isCollected && _data != null && gameObject.activeInHierarchy;

    private void OnEnable()
    {
        ActivePowerUps.Add(this);
    }

    private void OnDisable()
    {
        ActivePowerUps.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        Collect(stats);
    }

    private void Collect(PlayerStats stats)
    {
        if (_isCollected || _data == null) return;

        _isCollected = true;
        AudioManager.Instance?.PlaySFX("sfx_powerup_pickup");
        StartCoroutine(ApplyEffect(stats));

        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        if (TryGetComponent<SpriteRenderer>(out var renderer))
            renderer.enabled = false;
    }

    private IEnumerator ApplyEffect(PlayerStats stats)
    {
        var buffs = stats.GetComponent<PlayerPowerUpBuffs>();
        if (buffs == null)
            buffs = stats.gameObject.AddComponent<PlayerPowerUpBuffs>();

        if (buffs.Apply(_data))
        {
            Destroy(gameObject, 0.1f);
            yield break;
        }

        switch (_data.type)
        {
            case PowerUpType.HealthRestore:
                stats.Heal(Mathf.RoundToInt(_data.magnitude));
                Destroy(gameObject, 0.1f);
                break;
            case PowerUpType.GrowthMagic:
                ServiceLocator.Get<FarmManager>()?.AdvanceAllCropsOneStage();
                Destroy(gameObject, 0.1f);
                break;
            case PowerUpType.Magnet:
                PullDroppedPowerUpsTo(stats);
                Destroy(gameObject, 0.1f);
                break;
            default:
                yield return new WaitForSeconds(_data.duration);
                Destroy(gameObject, 0.1f);
                break;
        }
    }

    private void PullDroppedPowerUpsTo(PlayerStats stats)
    {
        var snapshot = ActivePowerUps.ToArray();
        foreach (var powerUp in snapshot)
        {
            if (powerUp == null || powerUp == this || powerUp._isCollected) continue;
            powerUp.AttractTo(stats);
        }
    }

    private void AttractTo(PlayerStats stats)
    {
        if (_isCollected || stats == null) return;

        if (_attractCoroutine != null)
            StopCoroutine(_attractCoroutine);

        _attractCoroutine = StartCoroutine(AttractRoutine(stats));
    }

    private IEnumerator AttractRoutine(PlayerStats stats)
    {
        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        while (!_isCollected && stats != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                stats.transform.position,
                _magnetPullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, stats.transform.position) <= 0.25f)
            {
                Collect(stats);
                yield break;
            }

            yield return null;
        }
    }
}
