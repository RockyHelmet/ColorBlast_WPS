using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {
    [SerializeField] private NetworkObject playerAvatarPrefab;
    [SerializeField] private NetworkObject headProxyPrefab;
    [SerializeField] private Transform arCamera;

    private Transform sharedSpace;
    private GameObject localHeadProxy;
    private GameObject localAvatar;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        sharedSpace = GameObject.FindWithTag("SharedSpace")?.transform;

        if (!IsOwner) return;

        if (sharedSpace == null) {
            Debug.LogError("[PlayerSpawner] SharedSpace not found in scene!");
            return;
        }

        if (NetworkManager.Singleton.IsServer) {
            SpawnPlayerForOwner();
        }
        else {
            // Delay spawn request until server knows the client is ready
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId) {
        if (clientId == OwnerClientId) {
            SpawnPlayerServerRpc();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void SpawnPlayerForOwner() {
        if (playerAvatarPrefab != null) {
            NetworkObject avatarInstance = Instantiate(playerAvatarPrefab, sharedSpace);
            avatarInstance.transform.localPosition = Vector3.zero;
            avatarInstance.transform.localRotation = Quaternion.identity;
            avatarInstance.SpawnWithOwnership(OwnerClientId);

            CacheLocalAvatarClientRpc(avatarInstance.NetworkObjectId, new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } }
            });
        }

        if (headProxyPrefab != null) {
            NetworkObject headInstance = Instantiate(headProxyPrefab, sharedSpace);
            headInstance.transform.localPosition = Vector3.zero;
            headInstance.transform.localRotation = Quaternion.identity;
            headInstance.SpawnWithOwnership(OwnerClientId);

            CacheLocalHeadProxyClientRpc(headInstance.NetworkObjectId, new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } }
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ServerRpcParams rpcParams = default) {
        SpawnPlayerForOwner();
    }

    [ClientRpc]
    private void CacheLocalHeadProxyClientRpc(ulong headNetworkId, ClientRpcParams clientRpcParams = default) {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(headNetworkId, out NetworkObject netObj)) {
            localHeadProxy = netObj.gameObject;
        }
    }

    [ClientRpc]
    private void CacheLocalAvatarClientRpc(ulong avatarNetworkId, ClientRpcParams clientRpcParams = default) {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avatarNetworkId, out NetworkObject netObj)) {
            localAvatar = netObj.gameObject;
        }
    }

    private void Update() {
        if (IsOwner && arCamera != null && localHeadProxy != null) {
            localHeadProxy.transform.position = arCamera.position;
            localHeadProxy.transform.rotation = arCamera.rotation;
        }
    }
}
