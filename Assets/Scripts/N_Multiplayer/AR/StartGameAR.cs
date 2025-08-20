using System;
using Niantic.Lightship.SharedAR.Colocalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAR : MonoBehaviour {
    [SerializeField] private SharedSpaceManager _sharedSpaceManager;
    private const int MAX_AMOUNT_CLIENTS_ROOM = 2;

    [SerializeField] private Texture2D _targetImage;
    [SerializeField] private float _targetImageSize;

    private string roomName = "TestRoom";

    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;
    [SerializeField] private GameObject UIPanel;

    private bool isHost;
    private bool networkStarted = false;

    public static event Action OnStartSharedSpaceHost;
    public static event Action OnJoinSharedSpaceClient;
    public static event Action OnStartGame;
    public static event Action OnStartSharedSpace;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        UIPanel.SetActive(true);

        // Subscribe to SharedSpace state
        _sharedSpaceManager.sharedSpaceManagerStateChanged += SharedSpaceManagerOnStateChanged;

        // Button listeners
        CreateRoomButton.onClick.AddListener(CreateGameHost);
        JoinRoomButton.onClick.AddListener(JoinGameClient);
    }

    private void SharedSpaceManagerOnStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs args) {
        if (args.Tracking) {
            CreateRoomButton.interactable = false;
            JoinRoomButton.interactable = false;

            // Start network only once, when tracking is ready
            if (!networkStarted) {
                networkStarted = true;
                StartGame();
            }
        }
        else {
            // Optionally re-enable buttons if tracking lost
            CreateRoomButton.interactable = true;
            JoinRoomButton.interactable = true;
        }
    }

    private void StartSharedSpace() {
        OnStartSharedSpace?.Invoke();

        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.MockColocalization) {
            var mockTrackingArgs = ISharedSpaceTrackingOptions.CreateMockTrackingOptions();
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                MAX_AMOUNT_CLIENTS_ROOM,
                "MockColocalizationDemo"
            );
            _sharedSpaceManager.StartSharedSpace(mockTrackingArgs, roomArgs);
            return;
        }

        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.ImageTrackingColocalization) {
            var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(
                _targetImage, _targetImageSize
            );
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                MAX_AMOUNT_CLIENTS_ROOM,
                "ImageColocalization"
            );
            _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomArgs);
        }
    }

    private void StartGame() {
        OnStartGame?.Invoke();

        if (isHost)
            NetworkManager.Singleton.StartHost();
        else
            NetworkManager.Singleton.StartClient();

        UIPanel.SetActive(false);
    }

    private void CreateGameHost() {
        isHost = true;
        OnStartSharedSpaceHost?.Invoke();
        StartSharedSpace();
        // Do NOT call StartGame() here. It will start after tracking is ready
    }

    private void JoinGameClient() {
        isHost = false;
        OnJoinSharedSpaceClient?.Invoke();
        StartSharedSpace();
        // Do NOT call StartGame() here. It will start after tracking is ready
    }
}
