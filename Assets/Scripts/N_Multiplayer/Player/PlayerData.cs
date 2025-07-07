using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections; 

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
    public ulong clientID;
    public int score;
    public bool playerPlaced;
    public int currentColor;


    public PlayerData(ulong clientID, int score, bool playerPlaced, int currentColor) {
        this.clientID = clientID;
        this.score = score;
        this.playerPlaced = playerPlaced;
        this.currentColor = currentColor;
    }

    public int GetCurrentColor() {
        return currentColor;
    }
    public int GetScore() { return score; }
    public void AddScore(int s) {  score += s; }
    public void ResetScore() {  score = 0; }
    public void UpdateColor(int newColor) { currentColor = newColor; }

    public bool Equals(PlayerData other) {
        return (
            other.playerPlaced == playerPlaced &&
            other.score == score &&
            other.clientID == clientID &&
            other.currentColor == currentColor
        );
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref playerPlaced);
        serializer.SerializeValue(ref currentColor);
    }
}