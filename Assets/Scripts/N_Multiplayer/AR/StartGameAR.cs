using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Niantic.Lightship.SharedAR.Colocalization;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class StartGameAR : MonoBehaviour
{
    [SerializeField] private SharedSpaceManager _sharedSpaceManager;
    private const int MAX_CLIENTS_ROOM =2;

    [SerializeField] private Texture2D _targetImage;
    [SerializeField] private float _targetImageSize;
    private string roomName = "TestRoom";

    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;
    private bool isHost;

    private static event Action OnStartSharedSpaceHost;
    private static event Action OnJoinSharedSpaceClient;
    public static event Action OnStartGame;
    public static event Action OnStartSharedSpace;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        _sharedSpaceManager.sharedSpaceManagerStateChanged += SharedSpaceManagerOnSharedSPaceManagerChanged;

        StartGameButton.onClick.AddListener(StartGame);
        CreateRoomButton.onClick.AddListener(CreateGameHost);
        JoinRoomButton.onClick.AddListener(JoinGameClient);

        StartGameButton.interactable = false;
    }

    private void SharedSpaceManagerOnSharedSPaceManagerChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs obj) {
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

    }

    void StartSharedSpace() {
        OnStartSharedSpace?.Invoke();
        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.MockColocalization) {

            var mockTrainingArgs = ISharedSpaceTrackingOptions.CreateMockTrackingOptions();
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                MAX_CLIENTS_ROOM,
                "MockColocalizationDemo"
            );

            _sharedSpaceManager.StartSharedSpace(mockTrainingArgs, roomArgs);
            return;
        }
        if (_sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.ImageTrackingColocalization) {

            var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(
                _targetImage, _targetImageSize
                );
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                MAX_CLIENTS_ROOM,
                "ImageColocalizationDemo"
            );

            _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomArgs);
            return;
        }
    }
        void CreateGameHost() {
        isHost = true;
        OnStartSharedSpaceHost?.Invoke();
    }

    void JoinGameClient() {
        isHost = false;
        OnJoinSharedSpaceClient?.Invoke();
    }

}
