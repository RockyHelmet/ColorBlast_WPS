using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class StoneData : NetworkBehaviour {
    private NetworkVariable<ulong> owner = new(999);
    private NetworkVariable<bool> isActiveSelf = new(true);


    [SerializeField] private LayerMask targetLayer;  
    [SerializeField] private int bonusChance = 1;     

    private GameManager_m gameManager_m;
    private TargetSpawner_m targetSpawner_m;

    private void Awake() {
        gameManager_m = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_m>();
        targetSpawner_m = GameObject.FindGameObjectWithTag("TargetSpawner").GetComponent<TargetSpawner_m>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetOwnershipServerRpc(ulong id) {
        owner.Value = id;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetStoneIsActiveServerRpc(bool isActive) {
        if (!NetworkObject) return;

        isActiveSelf.Value = isActive;

        if (!isActive)
            NetworkObject.Despawn(true);
        else
            NetworkObject.Spawn();
    }

    void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        PlayerData playerData = AllPlayerDataManager.Instance.GetPlayerData(owner.Value);

        int currentColor = playerData.currentColor;

        if (other.CompareTag(gameManager_m.colors[currentColor])) {
            AllPlayerDataManager.Instance.RequestAddScoreServerRpc(1, new ServerRpcParams {
                Receive = new ServerRpcReceiveParams { SenderClientId = owner.Value }
            });
        }


        if (UnityEngine.Random.Range(1, 101) <= bonusChance) {
            AllPlayerDataManager.Instance.RequestAddScoreServerRpc(10, new ServerRpcParams {
                Receive = new ServerRpcReceiveParams { SenderClientId = owner.Value }
            });
        }

        Vector3Int gridPos = TargetSpawner_m.masterGrid_m.WorldToGrid(other.transform.position);
        TargetSpawner_m.masterGrid_m.SwitchCellValue(gridPos.x, gridPos.y, gridPos.z);
        targetSpawner_m.DecreaseTargets();


        NetworkObject targetNetObj = other.GetComponent<NetworkObject>();
        if (targetNetObj != null && targetNetObj.IsSpawned)
            targetNetObj.Despawn(true);
        else
            Destroy(other.gameObject);

        if (NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
    }

    void Update() {
        if (!IsServer) return;

        if (transform.position.sqrMagnitude >= 2500f) {
            GetComponent<NetworkObject>()?.Despawn(true);
        }
    }
}
