using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour, IDamageable, ISaveable
{
    private CropData _data;
    private CropStage _stage = CropStage.Seed;
    private int _currentHP;
    private SpriteRenderer _spriteRenderer;
    private Coroutine _growthCoroutine;

    public bool IsDead => _currentHP <= 0;
    public bool IsMature => _stage == CropStage.Mature;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private System.Action _onKilled;

    public void Init(CropData data, System.Action onKilled = null)
    {
        _data = data;
        _currentHP = data.maxHP;
        _stage = CropStage.Seed;
        _onKilled = onKilled;
        UpdateSprite();
        _growthCoroutine = StartCoroutine(GrowthCoroutine());
    }

    private IEnumerator GrowthCoroutine()
    {
        float timePerStage = _data.growthTime / 3f;
        yield return new WaitForSeconds(timePerStage);
        SetStageIfBehind(CropStage.Sprout);
        yield return new WaitForSeconds(timePerStage);
        SetStageIfBehind(CropStage.Growing);
        yield return new WaitForSeconds(timePerStage);
        SetStageIfBehind(CropStage.Mature);
    }

    private void SetStage(CropStage stage)
    {
        _stage = stage;
        UpdateSprite();
    }

    private void SetStageIfBehind(CropStage stage)
    {
        if ((int)_stage >= (int)stage) return;
        SetStage(stage);
    }

    public void AdvanceStage()
    {
        if (IsDead || IsMature) return;

        SetStage((CropStage)Mathf.Min((int)_stage + 1, (int)CropStage.Mature));
    }

    private void UpdateSprite()
    {
        if (_data?.stageSprites == null) return;
        int idx = (int)_stage;
        if (idx < _data.stageSprites.Length)
            _spriteRenderer.sprite = _data.stageSprites[idx];
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        _currentHP -= damage;
        if (IsDead) OnDestroyed();
    }

    private void OnDestroyed()
    {
        if (_growthCoroutine != null) StopCoroutine(_growthCoroutine);
        _onKilled?.Invoke();
        Destroy(gameObject);
    }

    public void Harvest(PlayerStats stats)
    {
        if (!IsMature) return;
        stats.AddGold(_data.sellPrice);
        stats.AddXP(_data.xpReward);
        AudioManager.Instance?.PlaySFX("sfx_harvest");
        Destroy(gameObject);
    }

    public object GetSaveData() => new { stage = (int)_stage, hp = _currentHP };
    public void LoadSaveData(object data) { }
}
