using Unity.Netcode;
using UnityEngine;

public class PlayerAR : NetworkBehaviour {
    private Camera playerCamera;
    private AudioListener playerAudio;

    void Start() {
        playerCamera = GetComponentInChildren<Camera>(true);
        playerAudio = GetComponentInChildren<AudioListener>(true);

        if (!IsOwner) {
            if (playerCamera) playerCamera.enabled = false;
            if (playerAudio) playerAudio.enabled = false;
        }
        else {
            if (playerCamera) {
                playerCamera.enabled = true;
                playerCamera.tag = "MainCamera";
            }

            if (playerAudio) {
                playerAudio.enabled = true;
            }

            DisableDefaultOriginCamera();
        }
    }

    private void DisableDefaultOriginCamera() {
        // Looks for another camera in scene that's not this one and disables it
        GameObject defaultOrigin = GameObject.FindGameObjectWithTag("XROriginDefault");
        defaultOrigin.SetActive(false);
    }
}

