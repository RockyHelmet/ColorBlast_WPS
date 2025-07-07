using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerState : NetworkBehaviour {
    public NetworkVariable<float> counter = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> currentColor = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> score = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasWon = new(writePerm: NetworkVariableWritePermission.Owner);

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text colorText;

    [Header("Game Settings")]
    [SerializeField] private List<string> colorPool;
    [SerializeField] private float timerDuration = 30f;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            PickNewColor();
            counter.Value = timerDuration;
            score.Value = 0;
            hasWon.Value = false;
        }
    }

    void Update() {
        if (!IsOwner) return;

        HandleTimer();
        HandleWinCondition();
        UpdateUI();
    }

    void HandleTimer() {
        if (counter.Value > 0) {
            counter.Value -= Time.deltaTime;
        }
        else {
            PickNewColor();
            counter.Value = timerDuration;
        }
    }

    void HandleWinCondition() {
        if (score.Value >= 10 && !hasWon.Value) {
            hasWon.Value = true;
            Debug.Log("You Won!");
        }
    }

    public void AddScore(int amount) {
        if (!IsOwner || hasWon.Value) return;
        score.Value += amount;
    }

    void PickNewColor() {
        string picked = colorPool[Random.Range(0, colorPool.Count)];
        currentColor.Value = picked;
        score.Value = 0;
    }

    void UpdateUI() {
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(counter.Value)}";

        if (colorText != null)
            colorText.text = $"Current: {currentColor.Value.ToString()}";
    }

    public string GetCurrentColor() {
        return currentColor.Value.ToString();
    }

    public bool CheckWinChance() {
        return Random.Range(1, 101) == 1; // 1% chance
    }
}
