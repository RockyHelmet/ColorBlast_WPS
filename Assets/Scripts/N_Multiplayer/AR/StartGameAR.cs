using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject SpawnPanel;
    private bool isHost;

    public static event Action OnStartSharedSpaceHost;
    public static event Action OnJoinSharedSpaceClient;
    public static event Action OnStartGame;
    public static event Action OnStartSharedSpace;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        UIPanel.SetActive(true);
        SpawnPanel.SetActive(false);
        _sharedSpaceManager.sharedSpaceManagerStateChanged += SharedSpaceManagerOnsharedSpaceManagerStateChanged;

        StartGameButton.onClick.AddListener(StartGame);
        CreateRoomButton.onClick.AddListener(CreateGameHost);
        JoinRoomButton.onClick.AddListener(JoinGameClient);

        StartGameButton.interactable = false;

    }

    private void SharedSpaceManagerOnsharedSpaceManagerStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs obj) {
        if (obj.Tracking) {
            StartGameButton.interactable = true;
            CreateRoomButton.interactable = false;
            JoinRoomButton.interactable = false;
        }
    }

    void StartGame() {
        OnStartGame?.Invoke();

        if (isHost) {
            NetworkManager.Singleton.StartHost();
        }
        else {
            NetworkManager.Singleton.StartClient();
        }
        UIPanel.SetActive(false);
        SpawnPanel.SetActive(true);

    }


    void StartSharedSpace() {
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
            return;
        }

    }
    void CreateGameHost() {
        isHost = true;
        OnStartSharedSpaceHost?.Invoke();
        StartSharedSpace();
    }

    void JoinGameClient() {
        isHost = false;
        OnJoinSharedSpaceClient?.Invoke();
        StartSharedSpace();
    }

}