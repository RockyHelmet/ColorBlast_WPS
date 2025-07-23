using System;
using System.Collections.Generic;
using Niantic.Lightship.AR.Settings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceXROrigin : NetworkBehaviour {
    [SerializeField] private GameObject xrOriginPrefab;

    private Camera mainCam;

    public static event Action XROriginPlaced;

    private void Start() {
        mainCam = Camera.main;
    }

    void Update() {
        if (AllPlayerDataManager.Instance != null &&
            AllPlayerDataManager.Instance.GetHasPlayerPlaced(NetworkManager.Singleton.LocalClientId)) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            TouchToRay(Input.mousePosition);
        }
#endif

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began) {

            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            TouchToRay(touch.position);
        }
#endif
    }

    void TouchToRay(Vector3 screenPos) {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 spawnPos = hit.point + Vector3.up * 0.1f;
            SpawnXROriginServerRpc(spawnPos, rotation, NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnXROriginServerRpc(Vector3 position, Quaternion rotation, ulong clientId) {
        GameObject origin = Instantiate(xrOriginPrefab, position, rotation);
        origin.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        AllPlayerDataManager.Instance.AddPlacedPlayer(clientId);

        XROriginPlaced?.Invoke();
    }
}
