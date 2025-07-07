using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AllPlayerDataManager : NetworkBehaviour {
    public static AllPlayerDataManager Instance;

    private NetworkList<PlayerData> allPlayerData;

    public event Action<ulong> OnPlayerWin;

    private const int WIN_SCORE = 10;

    private void Awake() {
        allPlayerData = new NetworkList<PlayerData>();

        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }

        Instance = this;
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            AddNewClientToList(NetworkManager.LocalClientId);
        }
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += AddNewClientToList;
    }

    public override void OnNetworkDespawn() {
        NetworkManager.Singleton.OnClientConnectedCallback -= AddNewClientToList;
    }


    public void AddPlacedPlayer(ulong id) {
        for (int i = 0; i < allPlayerData.Count; i++) {
            if (allPlayerData[i].clientID == id) {
                PlayerData updated = allPlayerData[i];
                updated.playerPlaced = true;
                allPlayerData[i] = updated;
                break;
            }
        }
    }

    public bool GetHasPlayerPlaced(ulong id) {
        foreach (var player in allPlayerData) {
            if (player.clientID == id) {
                return player.playerPlaced;
            }
        }

        return false;
    }

    public int GetScore(ulong id) {
        foreach (var player in allPlayerData) {
            if (player.clientID == id) {
                return player.score;
            }
        }

        return 0;
    }

    public int GetColor(ulong id) {
        foreach (var player in allPlayerData) {
            if (player.clientID == id) {
                return player.currentColor;
            }
        }

        return 0;
    }

    public PlayerData GetPlayerData(ulong id) {
        foreach (var player in allPlayerData) {
            if (player.clientID == id) {
                return player;
            }
        }

        return default;
    }

    public void AddScore(ulong id, int delta) {
        for (int i = 0; i < allPlayerData.Count; i++) {
            if (allPlayerData[i].clientID == id) {
                PlayerData updated = allPlayerData[i];
                updated.score += delta;
                allPlayerData[i] = updated;

                if (updated.score >= WIN_SCORE) {
                    OnPlayerWin?.Invoke(id);
                }

                break;
            }
        }
    }

    public void ResetScore(ulong id) {
        for (int i = 0; i < allPlayerData.Count; i++) {
            if (allPlayerData[i].clientID == id) {
                PlayerData updated = allPlayerData[i];
                updated.score = 0;
                allPlayerData[i] = updated;
                break;
            }
        }
    }

    public void UpdatePlayerColor(ulong id, int newColor) {
        for (int i = 0; i < allPlayerData.Count; i++) {
            if (allPlayerData[i].clientID == id) {
                PlayerData updated = allPlayerData[i];
                updated.currentColor = newColor;
                allPlayerData[i] = updated;
                break;
            }
        }
    }

    private void AddNewClientToList(ulong clientID) {
        if (!IsServer) return;

        foreach (var playerData in allPlayerData) {
            if (playerData.clientID == clientID)
                return;
        }

        PlayerData newPlayer = new PlayerData {
            clientID = clientID,
            score = 0,
            currentColor = UnityEngine.Random.Range(0, 3),
            playerPlaced = false
        };

        allPlayerData.Add(newPlayer);

        PrintAllPlayerList();
    }

    private void PrintAllPlayerList() {
        foreach (var player in allPlayerData) {
            Debug.Log($"Player ID => {player.clientID} | Placed: {player.playerPlaced} | Called by: {NetworkManager.Singleton.LocalClientId}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestColorChangeServerRpc(int newColor, ServerRpcParams rpcParams = default) {
        ulong senderID = rpcParams.Receive.SenderClientId;
        UpdatePlayerColor(senderID, newColor);
        ResetScore(senderID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestAddScoreServerRpc(int delta, ServerRpcParams rpcParams = default) {
        ulong senderID = rpcParams.Receive.SenderClientId;
        AddScore(senderID, delta);
    }
}
