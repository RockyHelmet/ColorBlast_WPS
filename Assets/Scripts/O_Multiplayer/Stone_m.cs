using Unity.Netcode;
using UnityEngine;

public class Stone_m : NetworkBehaviour {

    private ulong ownerClientId;
    private PlayerState playerState;
    private TargetSpawner_m targetSpawner_m;
    private int targetLayer;

    void Awake() {
        targetSpawner_m = TargetSpawner_m.Instance;
        targetLayer = LayerMask.NameToLayer("Target");
    }

    public void SetOwner(ulong clientId) {
        ownerClientId = clientId;
        AssignPlayerState();
    }

    void AssignPlayerState() {
        foreach (var state in FindObjectsOfType<PlayerState>()) {
            if (state.OwnerClientId == ownerClientId) {
                playerState = state;
                break;
            }
        }

        if (playerState == null) {
            Debug.LogWarning("PlayerState not found for ownerClientId: " + ownerClientId);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!IsServer) return; // Only server should handle logic

        if (other.gameObject.layer != targetLayer || playerState == null) return;

        string currentColor = playerState.GetCurrentColor();

        if (playerState.CheckWinChance()) {
            playerState.AddScore(10);
        }

        if (other.CompareTag(currentColor)) {
            playerState.AddScore(1);
        }

        targetSpawner_m.DecreaseTargets();

        Vector3Int gridPos = TargetSpawner_m.masterGrid_m.WorldToGrid(other.transform.position);
        TargetSpawner_m.masterGrid_m.SwitchCellValue(gridPos.x, gridPos.y, gridPos.z);

        NetworkObject targetNetObj = other.GetComponent<NetworkObject>();
        if (targetNetObj != null) {
            if (targetNetObj.IsSpawned) {
                targetNetObj.Despawn(true);
            }
            else {
                Destroy(other.gameObject);
            }
        }
        else {
            Destroy(other.gameObject);
        }

        GetComponent<NetworkObject>()?.Despawn(true); // Despawn this stone
    }


    void Update() {
        if (!IsServer) return;

        if (transform.position.sqrMagnitude >= 2500f) {
            GetComponent<NetworkObject>()?.Despawn(true);
        }
    }
}
