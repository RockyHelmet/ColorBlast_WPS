using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class SharedSpaceNetworkTransform : NetworkBehaviour {
    public Transform sharedSpace; 
    public bool smooth = true;
    public float lerp = 10f;

    private Vector3 targetLocalPos;
    private Quaternion targetLocalRot;

    private NetworkVariable<Vector3> netPos = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<Quaternion> netRot = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    void Awake() {
        if (sharedSpace == null) {
            var ssGo = GameObject.FindGameObjectWithTag("SharedSpace");
            if (ssGo) sharedSpace = ssGo.transform;
        }

        targetLocalPos = transform.localPosition;
        targetLocalRot = transform.localRotation;
    }

    void Update() {
        if (IsOwner) {
            // Owner writes to network variables
            netPos.Value = transform.localPosition;
            netRot.Value = transform.localRotation;
        }
        else {
            // Non-owners lerp to target
            targetLocalPos = netPos.Value;
            targetLocalRot = netRot.Value;

            if (smooth) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * lerp);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRot, Time.deltaTime * lerp);
            }
            else {
                transform.localPosition = targetLocalPos;
                transform.localRotation = targetLocalRot;
            }
        }
    }
}
