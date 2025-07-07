using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SwipeThrow_m : NetworkBehaviour {
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private float forceMultiplier = 0.05f;

    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private bool isSwiping = false;

    [SerializeField] private Transform throwPoint;

    private ulong clientID;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            clientID = NetworkManager.LocalClientId;
        }
    }

    void Start() {
        if (!IsOwner) return;

        if (throwPoint == null)
            throwPoint = transform.Find("ThrowPoint");
    }

    void Update() {
        if (!IsOwner) return;

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch.phase, touch.position);
        }
#else
        if (Input.GetMouseButtonDown(0)) {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping) {
            swipeEndPos = Input.mousePosition;
            ShootFromSwipe();
            isSwiping = false;
        }
#endif
    }

    void HandleTouch(TouchPhase phase, Vector2 position) {
        if (phase == TouchPhase.Began) {
            swipeStartPos = position;
        }
        else if (phase == TouchPhase.Ended) {
            swipeEndPos = position;
            ShootFromSwipe();
        }
    }

    void ShootFromSwipe() {
        Vector2 swipeVector = swipeEndPos - swipeStartPos;
        float swipeLength = swipeVector.magnitude;

        if (swipeLength < 15f) return;

        Vector3 direction = new Vector3(swipeVector.x, swipeVector.y, swipeLength).normalized;
        Vector3 forceDir = Camera.main.transform.TransformDirection(direction);

        ThrowStoneServerRpc(forceDir * swipeLength * forceMultiplier);
    }

    [ServerRpc]
    void ThrowStoneServerRpc(Vector3 force, ServerRpcParams rpcParams = default) {
        if (stonePrefab == null || throwPoint == null) {
            Debug.LogError("Stone prefab or throw point not assigned!");
            return;
        }

        GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
        NetworkObject netObj = stone.GetComponent<NetworkObject>();

        netObj.Spawn();
        stone.GetComponent<StoneData>().SetOwnershipServerRpc(clientID);

        Rigidbody rb = stone.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
