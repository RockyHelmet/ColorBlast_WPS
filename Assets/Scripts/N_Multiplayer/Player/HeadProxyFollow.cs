using Unity.Netcode;
using UnityEngine;

public class HeadProxyFollow : NetworkBehaviour {
    public Transform arCamera;
    private Transform sharedSpace;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        sharedSpace = GameObject.FindWithTag("SharedSpace")?.transform;
        if (sharedSpace == null) {
            Debug.LogError("[HeadProxyFollow] SharedSpace not found!");
            return;
        }

        // Server sets parent
        if (IsServer)
            transform.SetParent(sharedSpace, false);

        // Owner sets local reference to camera
        if (IsOwner) {
            arCamera = Camera.main ? Camera.main.transform : GameObject.FindWithTag("ARCamera")?.transform;
            if (arCamera == null)
                Debug.LogError("[HeadProxyFollow] Could not find AR Camera in scene!");
        }
    }

    void LateUpdate() {
        if (!IsOwner || arCamera == null || sharedSpace == null) return;

        // Directly map AR camera pose into shared space coordinates
        transform.localPosition = sharedSpace.InverseTransformPoint(arCamera.position);
        transform.localRotation = Quaternion.Inverse(sharedSpace.rotation) * arCamera.rotation;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (arCamera != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(arCamera.position, 0.05f);
        }
    }
#endif
}
