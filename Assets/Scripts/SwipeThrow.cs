using UnityEngine;

public class SwipeThrow : MonoBehaviour {

    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform throwPoint;
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private bool isSwiping = false;
    [SerializeField] private float forceMultiplier = 0.005f;

    private void Update() {
        if (Input.touchSupported && Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch.phase, touch.position);
        } else if (Input.GetMouseButtonDown(0)) {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
        } else if (Input.GetMouseButtonUp(0) && isSwiping) {
            swipeEndPos = Input.mousePosition;
            ShootFromSwipe();
            isSwiping = false;
        }
    }

    private void HandleTouch(TouchPhase phase, Vector2 position) {
        switch (phase) {
            case TouchPhase.Began:
                swipeStartPos = position;
                break;
            case TouchPhase.Ended:
                swipeEndPos = position;
                ShootFromSwipe();
                break;
        }
    }

    private void ShootFromSwipe() {
        Vector2 swipeVector = swipeEndPos - swipeStartPos;
        float swipeLength = swipeVector.magnitude;

        if (swipeLength < 15f) return; // Ignore short swipes

        Vector3 direction = new Vector3(swipeVector.x, swipeVector.y, swipeLength).normalized;
        Vector3 forceDir = Camera.main.transform.TransformDirection(direction);

        //GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);

        GameObject stone = WPS.Instance.SpawnGameObjectAtLocation(stonePrefab, throwPoint.position);

        Rigidbody rb = stone.GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.AddForce(forceDir * swipeLength * forceMultiplier, ForceMode.Impulse);
    }
}