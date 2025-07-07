using Unity.Netcode;
using UnityEngine;

public class MultiplayerSpawner : NetworkBehaviour {
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject targetSpawnerPrefab;

    private GameObject spawnedGameManager;
    private GameObject spawnedTargetSpawner;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        SpawnCoreGameObjects();
    }

    private void SpawnCoreGameObjects() {
        // Spawn GameManager
        spawnedGameManager = Instantiate(gameManagerPrefab, Vector3.zero, Quaternion.identity);
        var gmNetObj = spawnedGameManager.GetComponent<NetworkObject>();
        if (gmNetObj != null) {
            gmNetObj.Spawn();
        }

        // Spawn TargetSpawner
        spawnedTargetSpawner = Instantiate(targetSpawnerPrefab, Vector3.zero, Quaternion.identity);
        var tsNetObj = spawnedTargetSpawner.GetComponent<NetworkObject>();
        if (tsNetObj != null) {
            tsNetObj.Spawn();
        }
    }
}
