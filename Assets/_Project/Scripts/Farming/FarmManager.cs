using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [SerializeField] private GameObject _farmTilePrefab;
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _tileSize = 1f;
    [SerializeField] private Vector2 _gridOrigin;

    private FarmTile[,] _tiles;

    private void Awake()
    {
        ServiceLocator.Register(this);
        BuildGrid();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<FarmManager>();
    }

    private void BuildGrid()
    {
        _tiles = new FarmTile[_gridWidth, _gridHeight];
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Vector3 pos = new Vector3(
                    _gridOrigin.x + x * _tileSize,
                    _gridOrigin.y + y * _tileSize, 0);
                var go = Instantiate(_farmTilePrefab, pos, Quaternion.identity, transform);
                var tile = go.GetComponent<FarmTile>();
                tile.Init(x, y);
                _tiles[x, y] = tile;
            }
        }
    }

    public FarmTile GetTileAt(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - _gridOrigin.x) / _tileSize);
        int y = Mathf.RoundToInt((worldPos.y - _gridOrigin.y) / _tileSize);
        if (x < 0 || x >= _gridWidth || y < 0 || y >= _gridHeight) return null;
        return _tiles[x, y];
    }

    public int CountLivingCrops()
    {
        int count = 0;
        foreach (var tile in _tiles)
            if (tile.HasLivingCrop) count++;
        return count;
    }

    public int AdvanceAllCropsOneStage()
    {
        int advanced = 0;
        foreach (var tile in _tiles)
        {
            if (!tile.HasLivingCrop) continue;

            tile.CurrentCrop.AdvanceStage();
            advanced++;
        }

        return advanced;
    }

    public Crop GetNearestCrop(Vector2 worldPos)
    {
        Crop nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (var tile in _tiles)
        {
            if (!tile.HasLivingCrop) continue;

            Crop crop = tile.CurrentCrop;
            float sqrDistance = ((Vector2)crop.transform.position - worldPos).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = crop;
            }
        }

        return nearest;
    }
}
