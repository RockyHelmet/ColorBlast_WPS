using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

    private GameManager gameManager;
    private TargetSpawner targetSpawner;
    int targetLayer;

    void Awake() {
        gameManager = GameManager.Instance;
        targetSpawner = TargetSpawner.Instance;

        if (gameManager == null || targetSpawner == null)
            Debug.LogWarning("References missing in Stone.cs");
        targetLayer = LayerMask.NameToLayer("Target");
    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.layer == targetLayer) {
            Debug.Log("HIT");
            if (gameManager.CheckWinChance()) {
                gameManager.SetScore(10);
            }

            if (other.CompareTag(gameManager.GetCurrentColor())) {
                Debug.Log("COLOR");
                gameManager.SetScore(1);
            }

            targetSpawner.DecreaseTargets();
            Vector3Int gridPos = GameManager.masterGrid.WorldToGrid(other.transform.position);
            GameManager.masterGrid.SwitchCellValue(gridPos.x, gridPos.y, gridPos.z);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void Update() {
        if (transform.position.sqrMagnitude >= 50f * 50f) {
            Destroy(gameObject);
        }
    }
}