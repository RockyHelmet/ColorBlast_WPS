using UnityEngine;
using Unity.Netcode;

public class UIController : MonoBehaviour {
    private PlayerMovement localPlayer;
    public GameObject controlPanel;

    public void SetPlayer(PlayerMovement p) {
        if (p != null && p.IsOwner)
            localPlayer = p;
    }

    public void ActivateControlPanel() {
        controlPanel.SetActive(true);
    }

    // Movement
    public void OnMoveForward(bool pressed) => localPlayer?.MoveForward(pressed);
    public void OnMoveBackward(bool pressed) => localPlayer?.MoveBackward(pressed);
    public void OnMoveLeft(bool pressed) => localPlayer?.MoveLeft(pressed);
    public void OnMoveRight(bool pressed) => localPlayer?.MoveRight(pressed);

    // Camera
    public void OnLookUp(bool pressed) => localPlayer?.LookUp(pressed);
    public void OnLookDown(bool pressed) => localPlayer?.LookDown(pressed);
    public void OnLookLeft(bool pressed) => localPlayer?.LookLeft(pressed);
    public void OnLookRight(bool pressed) => localPlayer?.LookRight(pressed);
}
