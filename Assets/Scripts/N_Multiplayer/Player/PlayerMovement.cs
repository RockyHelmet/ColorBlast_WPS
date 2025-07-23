using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
    [Header("Movement")]
    public float moveSpeed = 12f;

    [Header("Rotation")]
    public float lookSpeed = 50f;
    public float topClamp = -60f;
    public float bottomClamp = 60f;
    private float xRotation = 0f;

    [Header("References")]
    public CharacterController controller;
    public Transform playerBody;
    public Transform cameraTransform;
    public GameObject ControlsGameObject;

    public float cameraXoffset;
    public float cameraYoffset;
    public float cameraZoffset;

    private Camera playerCamera;

    private bool moveForward, moveBackward, moveLeft, moveRight;
    private bool lookUp, lookDown, lookLeft, lookRight;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        if (controller == null)
            controller = GetComponent<CharacterController>();

        UIController uiController = GameObject.FindGameObjectWithTag("ControlPanelUI").GetComponent<UIController>();
        uiController.ActivateControlPanel();

        ControlsGameObject = GameObject.FindGameObjectWithTag("ControlPanel");
        if (ControlsGameObject != null) Debug.Log("CP found");
        else Debug.Log("CP not found");

        if (IsOwner) {
            if (ControlsGameObject != null)
                ControlsGameObject.SetActive(true);

            StartCoroutine(RegisterToUI());
        }
        else {
            if (ControlsGameObject != null)
                ControlsGameObject.SetActive(false);

            enabled = false;
            controller.enabled = false;
        }
        //Camera
        if (IsOwner) {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x + cameraXoffset, transform.position.y + cameraYoffset, transform.position.z + cameraZoffset);
            playerCamera.transform.SetParent(transform);
            playerBody = transform;
            cameraTransform = playerCamera.transform;
        }
    }
    void Update() {
        if (!IsOwner) return;

        HandleMovement();
        HandleCameraRotation();
    }

    void HandleMovement() {
        Vector3 move = Vector3.zero;

        if (moveForward) move += playerBody.forward;
        if (moveBackward) move -= playerBody.forward;
        if (moveLeft) move -= playerBody.right;
        if (moveRight) move += playerBody.right;

        controller.Move(move.normalized * moveSpeed * Time.deltaTime);
    }

    void HandleCameraRotation() {
        float camY = 0f;
        float camX = 0f;

        if (lookRight) camY += 1f;
        if (lookLeft) camY -= 1f;
        if (lookUp) camX -= 1f;
        if (lookDown) camX += 1f;

        playerBody.Rotate(Vector3.up * camY * lookSpeed * Time.deltaTime);

        xRotation += camX * lookSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private IEnumerator RegisterToUI() {
        UIController ui = null;
        while (ui == null) {
            ui = FindObjectOfType<UIController>();
            yield return null;
        }
        ui.SetPlayer(this);
    }

    // UI button inputs — movement
    public void MoveForward(bool pressed) => moveForward = pressed;
    public void MoveBackward(bool pressed) => moveBackward = pressed;
    public void MoveLeft(bool pressed) => moveLeft = pressed;
    public void MoveRight(bool pressed) => moveRight = pressed;

    // UI button inputs — camera
    public void LookUp(bool pressed) => lookUp = pressed;
    public void LookDown(bool pressed) => lookDown = pressed;
    public void LookLeft(bool pressed) => lookLeft = pressed;
    public void LookRight(bool pressed) => lookRight = pressed;
}
