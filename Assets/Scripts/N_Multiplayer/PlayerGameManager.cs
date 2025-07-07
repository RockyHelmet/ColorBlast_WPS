using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerGameManager : NetworkBehaviour {
    private ulong clientID;
    private int currentColorCode;

    private GameManager_m gameManager_m;

    public GameObject uiPrefab;
    private GameObject uiInstance;

    private TMP_Text colorText;
    private TMP_Text scoreText;
    private TMP_Text timerText;

    private float colorTimer = 0f;
    private float colorInterval = 30f;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            clientID = NetworkManager.LocalClientId;

            uiInstance = Instantiate(uiPrefab);

            colorText = uiInstance.transform.Find("ColorText").GetComponent<TMP_Text>();
            scoreText = uiInstance.transform.Find("ScoreText").GetComponent<TMP_Text>();
            timerText = uiInstance.transform.Find("TimerText").GetComponent<TMP_Text>();
        }
    }

    private void Start() {
        gameManager_m = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_m>();
    }

    private void Update() {
        if (!IsOwner) return;

        colorTimer += Time.deltaTime;

        if (colorTimer >= colorInterval) {
            colorTimer = 0f;
            ChangeColor();
        }

        currentColorCode = AllPlayerDataManager.Instance.GetColor(clientID);
        colorText.text = gameManager_m.colors[currentColorCode].ToString();
        scoreText.text = AllPlayerDataManager.Instance.GetScore(clientID).ToString();
        timerText.text = ((int)(colorInterval - colorTimer)).ToString();
    }

    private void ChangeColor() {
        int newColor = Random.Range(0, 3);

        if (AllPlayerDataManager.Instance != null) {
            AllPlayerDataManager.Instance.RequestColorChangeServerRpc(newColor);
        }
    }
}
