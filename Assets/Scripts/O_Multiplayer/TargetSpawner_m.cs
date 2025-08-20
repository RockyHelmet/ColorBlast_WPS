using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TargetSpawner_m : NetworkBehaviour {
    public static TargetSpawner_m Instance;

    [Header("Grid Variables")]
    [SerializeField] private int length = 4;
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    public static MasterGrid_m masterGrid_m;

    [SerializeField] private List<GameObject> targetPrefabs;
    [SerializeField] private int maxTargets = 10;

    private int currentTargets;
    private bool isRespawning = false;

    private Transform sharedSpace;   // SharedSpace reference

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Look for SharedSpace in the scene
        sharedSpace = GameObject.FindWithTag("SharedSpace")?.transform;
        if (sharedSpace == null) {
            Debug.LogWarning("[TargetSpawner_m] No SharedSpace found! Falling back to world space.");
        }
    }

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        Debug.Log("TargetSpawner OnNetworkSpawn called, creating masterGrid");
        masterGrid_m = new MasterGrid_m(length, width, height);

        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn() {
        yield return new WaitUntil(() =>
            masterGrid_m != null &&
            masterGrid_m.GetLength() > 0
        );

        currentTargets = 0;
        for (int i = 0; i < maxTargets; i++) {
            SpawnGridTarget();
        }
        Debug.Log("SPAWNED INITIAL TARGETS");
    }

    private void Update() {
        if (!IsServer) return;

        if (currentTargets < maxTargets && !isRespawning) {
            StartCoroutine(StartRespawn());
        }
    }

    public void DecreaseTargets() {
        currentTargets = Mathf.Max(0, currentTargets - 1);
    }

    private void SpawnGridTarget() {
        int x, y, z;
        int tries = 0;

        do {
            x = Random.Range(0, masterGrid_m.GetLength());
            y = Random.Range(0, masterGrid_m.GetWidth());
            z = Random.Range(0, masterGrid_m.GetHeight());
            tries++;
            if (tries > 100) return;
        }
        while (masterGrid_m.GetCellValue(x, y, z));

        Vector3 worldSpawnPos = masterGrid_m.GridToWorld(x, y, z);
        masterGrid_m.SwitchCellValue(x, y, z);

        GameObject prefab = targetPrefabs[Random.Range(0, targetPrefabs.Count)];
        if (prefab == null) {
            Debug.LogError("Null prefab in targetPrefabs list!");
            return;
        }

        // If we have SharedSpace, convert to local-space coordinates
        Vector3 localPos = worldSpawnPos;
        Quaternion localRot = Quaternion.identity;

        if (sharedSpace != null) {
            localPos = sharedSpace.InverseTransformPoint(worldSpawnPos);
            localRot = Quaternion.Inverse(sharedSpace.rotation) * Quaternion.identity; // adjust if you want rotation logic
        }

        // Instantiate under SharedSpace
        GameObject target = Instantiate(prefab, sharedSpace != null ? sharedSpace : null);
        target.transform.localPosition = localPos;
        target.transform.localRotation = localRot;

        var netObj = target.GetComponent<NetworkObject>();
        if (netObj != null) {
            netObj.Spawn(true);
        }
        else {
            Debug.LogError($"[TargetSpawner] Target prefab '{prefab.name}' is missing NetworkObject!");
        }

        currentTargets++;
    }


    private IEnumerator StartRespawn() {
        isRespawning = true;
        yield return new WaitForSeconds(1f);
        SpawnGridTarget();
        isRespawning = false;
    }
}
