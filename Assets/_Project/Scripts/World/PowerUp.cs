using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpData _data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null) return;

        AudioManager.Instance?.PlaySFX("sfx_powerup_pickup");
        StartCoroutine(ApplyEffect(stats));
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, _data.duration + 0.1f);
    }

    private IEnumerator ApplyEffect(PlayerStats stats)
    {
        switch (_data.type)
        {
            case PowerUpType.Speed:
                stats.ModifyMoveSpeed(_data.magnitude);
                yield return new WaitForSeconds(_data.duration);
                stats.ModifyMoveSpeed(-_data.magnitude);
                break;
            case PowerUpType.DoubleDamage:
                int bonusDamage = Mathf.RoundToInt(stats.Damage * (_data.magnitude - 1));
                stats.ModifyDamage(bonusDamage);
                yield return new WaitForSeconds(_data.duration);
                stats.ModifyDamage(-bonusDamage);
                break;
            case PowerUpType.HealthRestore:
                stats.Heal(Mathf.RoundToInt(_data.magnitude));
                break;
            default:
                yield return new WaitForSeconds(_data.duration);
                break;
        }
    }
}
