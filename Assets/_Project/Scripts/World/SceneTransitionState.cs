using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionState
{
    public static string TargetSpawnId { get; set; }
    public static string ReturnSceneName { get; private set; }
    public static Vector3 ReturnPosition { get; private set; }

    private static bool _restoreSavedReturnPoint;

    public static void SaveReturnPoint(string sceneName, Vector3 position)
    {
        ReturnSceneName = sceneName;
        ReturnPosition = position;
    }

    public static void RestoreSavedReturnPointOnNextLoad()
    {
        _restoreSavedReturnPoint = !string.IsNullOrWhiteSpace(ReturnSceneName);
        TargetSpawnId = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= MovePlayerToTargetSpawn;
        SceneManager.sceneLoaded += MovePlayerToTargetSpawn;
    }

    private static void MovePlayerToTargetSpawn(Scene scene, LoadSceneMode mode)
    {
        if (_restoreSavedReturnPoint)
        {
            MovePlayer(ReturnPosition);
            _restoreSavedReturnPoint = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(TargetSpawnId)) return;

        SceneSpawnPoint[] spawnPoints = Object.FindObjectsOfType<SceneSpawnPoint>();
        SceneSpawnPoint targetSpawn = null;

        foreach (SceneSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnId == TargetSpawnId)
            {
                targetSpawn = spawnPoint;
                break;
            }
        }

        if (targetSpawn == null) return;

        MovePlayer(targetSpawn.transform.position);

        TargetSpawnId = null;
    }

    private static void MovePlayer(Vector3 position)
    {
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player == null) return;

        player.transform.position = position;

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = Vector2.zero;

        CameraFollow cameraFollow = Object.FindObjectOfType<CameraFollow>();
        cameraFollow?.SetTarget(player.transform, snap: true);
    }
}
