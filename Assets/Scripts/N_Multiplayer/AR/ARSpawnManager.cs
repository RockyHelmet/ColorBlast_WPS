using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ARSpawnManager : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private GameObject xrOriginPrefab;   
    [SerializeField] private GameObject defaultOrigin;
    [SerializeField] private GameObject SpawnPanel;

    private bool hasSpawned = false;


    public void SpawnPlayer() {
        if (!hasSpawned) {
            Vector3 position = new Vector3 (0,0,0);
            Quaternion rotation = Quaternion.identity;
            SpawnPlayerServerRpc(position, rotation, NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership  = false)]
    void SpawnPlayerServerRpc(Vector3 positon, Quaternion rotation, ulong callerID) {
        Vector3 offset = new Vector3(0f, 1f, 0f);
        GameObject character = Instantiate(xrOriginPrefab, positon + offset, rotation);

        NetworkObject characterNetworkObject = character.GetComponent<NetworkObject>();

        characterNetworkObject.SpawnWithOwnership(callerID);

        AllPlayerDataManager.Instance.AddPlacedPlayer(callerID);
        hasSpawned = true;
        SpawnPanel.SetActive(false);
    }
}
