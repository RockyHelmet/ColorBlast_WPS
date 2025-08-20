using Unity.Netcode;
using UnityEngine;

public class PlayerAR : NetworkBehaviour {
    [SerializeField] private float cameraZoffset = 0f;

    private Camera arCam;

    public  void ConnectPlayer(ulong clientId) {
        Debug.Log("Connecting camera.");
        if (!IsOwner) return;
        arCam = GetComponentInChildren<Camera>();
        arCam.enabled = true;
        arCam.tag = "MainCamera";
        Debug.Log("Camera assigned to player: " +  clientId.ToString());
        arCam.transform.position += new Vector3(0, 0, cameraZoffset);
        AllPlayerDataManager.Instance?.AddPlacedPlayer(clientId);
    }
}
