using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour {

    public static TargetSpawner Instance;

    [SerializeField] private List<GameObject> targetPrefabs;

    [SerializeField] private int maxTargets = 10;
    private int currentTargets;

    private bool isRespawing = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start() {
        currentTargets = 0;
        for (int i = 0; i < maxTargets; i++) {
            SpawnGridTargets();
        }
        Debug.Log("SPAWNED");
    }

    private void Update() {
        if (currentTargets < maxTargets && !isRespawing) {
            StartCoroutine(StartRespawn());
        }
    }

    public void DecreaseTargets() {
        int newTargets = currentTargets - 1;
        if (newTargets > 0) currentTargets = newTargets;
        else currentTargets = 0;
    }

    private void SpawnGridTargets() {
        int x, y, z;
        int tries = 0;
        do {
            x = Random.Range(0, GameManager.masterGrid.GetLength());
            y = Random.Range(0, GameManager.masterGrid.GetWidth());
            z = Random.Range(0, GameManager.masterGrid.GetHeight());
            tries++;
            if (tries > 100) return;
        }
        while (GameManager.masterGrid.GetCellValue(x, y, x));

        Vector3 spawnPos = GameManager.masterGrid.GridToWorld(x, y, z);
        GameManager.masterGrid.SwitchCellValue(x, y, z);
        GameObject targetPrefab = targetPrefabs[Random.Range(0, targetPrefabs.Count)];

        //GameObject target = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

        GameObject target = WPS.Instance.SpawnGameObjectAtLocation(targetPrefab, spawnPos);
        currentTargets++;
    }

    private IEnumerator StartRespawn() {
        isRespawing = true;
        yield return new WaitForSeconds(1f);
        SpawnGridTargets();
        isRespawing = false;
    }
}

