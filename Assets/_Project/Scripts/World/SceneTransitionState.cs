using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionState
{
    public static string TargetSpawnId { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= MovePlayerToTargetSpawn;
        SceneManager.sceneLoaded += MovePlayerToTargetSpawn;
    }

    private static void MovePlayerToTargetSpawn(Scene scene, LoadSceneMode mode)
    {
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

        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player == null) return;

        player.transform.position = targetSpawn.transform.position;

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = Vector2.zero;

        CameraFollow cameraFollow = Object.FindObjectOfType<CameraFollow>();
        cameraFollow?.SetTarget(player.transform, snap: true);

        TargetSpawnId = null;
    }
}
