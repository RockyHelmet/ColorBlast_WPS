using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ARSpawnManager : NetworkBehaviour {
    [SerializeField] private GameObject xrOriginPrefab; // XR Origin prefab for clients
    private bool hasSpawned = false;

    private static readonly HashSet<ulong> spawnedClients = new HashSet<ulong>();

    public void SpawnPlayer() {
        if (hasSpawned) {
            Debug.Log("Player already spawned, skipping.");
            return;
        }

        if (IsHost) {
            GameObject hostOrigin = GameObject.FindGameObjectWithTag("HostXROrigin");
            if (hostOrigin != null) {
                hostOrigin.SetActive(true);
                hasSpawned = true;
                Debug.Log("Host using in-scene XR Origin.");
            }
            else {
                Debug.LogError("HostXROrigin not found in scene! Make sure it's placed and tagged.");
            }
        }
        else {
            Debug.Log("Client requesting XR Origin spawn from server...");
            if (!spawnedClients.Contains(NetworkManager.Singleton.LocalClientId)) {
                SpawnPlayerServerRpc(Vector3.zero, Quaternion.identity, NetworkManager.Singleton.LocalClientId);
            }
            else {
                Debug.LogWarning("This client has already requested a spawn.");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(Vector3 position, Quaternion rotation, ulong callerId) {
        if (spawnedClients.Contains(callerId)) {
            Debug.LogWarning($"Client {callerId} already has an XR Origin.");
            return;
        }

        if (xrOriginPrefab == null) {
            Debug.LogError("xrOriginPrefab is not assigned in ARSpawnManager.");
            return;
        }

        GameObject playerObj = Instantiate(xrOriginPrefab, position, rotation);
        NetworkObject netObj = playerObj.GetComponent<NetworkObject>();

        if (netObj == null) {
            Debug.LogError("XR Origin prefab is missing a NetworkObject component!");
            return;
        }

        netObj.SpawnWithOwnership(callerId);
        spawnedClients.Add(callerId);

        AllPlayerDataManager.Instance?.AddPlacedPlayer(callerId);

        Debug.Log($"XR Origin spawned and ownership given to Client {callerId}");

        SpawnConfirmedClientRpc(callerId);
    }

    [ClientRpc]
    private void SpawnConfirmedClientRpc(ulong clientId) {
        if (NetworkManager.Singleton.LocalClientId == clientId) {
            hasSpawned = true;
            Debug.Log("Client XR Origin spawned and ownership confirmed.");
        }
    }

    public bool HasLocalPlayerSpawned() => hasSpawned;
}
