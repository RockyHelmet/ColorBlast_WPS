using UnityEngine;

public class SwipeThrow : MonoBehaviour {

    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform throwPoint;
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private bool isSwiping = false;
    [SerializeField] private float forceMultiplier = 0.005f;

    void Update() {
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

    void HandleTouch(TouchPhase phase, Vector2 position) {
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

    void ShootFromSwipe() {
        Vector2 swipeVector = swipeEndPos - swipeStartPos;
        float swipeLength = swipeVector.magnitude;

        if (swipeLength < 15f) return; // Ignore short swipes

        Vector3 direction = new Vector3(swipeVector.x, swipeVector.y, swipeLength).normalized;
        Vector3 forceDir = Camera.main.transform.TransformDirection(direction);

        //GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
        GameObject stone = Instantiate(stonePrefab);

        Rigidbody rb = stone.GetComponent<Rigidbody>();

        (double latOffset, double longOffset) = PreplacedWorldObjects.Instance.GetGeographicOffsetFromCameraPosition(throwPoint.position);

        double latitude = PreplacedWorldObjects.Instance.GetWorldPositioningManager().WorldTransform.OriginLatitude + latOffset;
        double longitude = PreplacedWorldObjects.Instance.GetWorldPositioningManager().WorldTransform.OriginLongitude + longOffset;

        PreplacedWorldObjects.Instance.GetObjectHelper().AddOrUpdateObject(stone, latitude, longitude, 0, Quaternion.identity);

        rb.isKinematic = false;
        rb.AddForce(forceDir * swipeLength * forceMultiplier, ForceMode.Impulse);
    }
}