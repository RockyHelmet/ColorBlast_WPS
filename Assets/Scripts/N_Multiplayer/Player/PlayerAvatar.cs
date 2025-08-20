using Unity.Netcode;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour {
    [Header("References")]
    public Transform headProxy; 

    void Start() {
        if (IsOwner) {
            // Hide local mesh for first-person view
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
        }

        // Parent under shared space
        var ssGo = GameObject.FindGameObjectWithTag("SharedSpace");
        if (ssGo) transform.SetParent(ssGo.transform, true);

        // Auto-find head proxy if not assigned
        if (headProxy == null)
            headProxy = GameObject.FindWithTag("HeadProxy")?.transform;
    }

    void Update() {
        if (headProxy == null) return;


        Vector3 targetPos = headProxy.position;
        transform.position = targetPos;

        Vector3 euler = transform.eulerAngles;
        euler.y = headProxy.eulerAngles.y;
        transform.eulerAngles = euler;
    }
}
