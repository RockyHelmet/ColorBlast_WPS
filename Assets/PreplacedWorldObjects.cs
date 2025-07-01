using Niantic.Experimental.Lightship.AR.WorldPositioning;
using Niantic.Experimental.Lightship.AR.XRSubsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PreplacedWorldObjects : MonoBehaviour {

    public static PreplacedWorldObjects Instance {  get; private set; }

    [SerializeField] private List<Material> materialsList = new();
    [SerializeField] private List<GameObject> balloons = new();
    [SerializeField] private List<LatLong> latLongs = new();
    [SerializeField] private ARWorldPositioningManager positioningManager;
    [SerializeField] private ARWorldPositioningObjectHelper objectHelper;
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private Button startButton;

    private List<GameObject> instantiatedObjects = new();
    private bool isPlaying;

    private void Awake() {
        Instance = this;

        isPlaying = false;

        startButton.onClick.AddListener(InitiateGame);
    }

    private void Start() {
        //foreach(var gpsCoord in latLongs){
        //    GameObject newObject = Instantiate(balloons[latLongs.IndexOf(gpsCoord) % balloons.Count]);

        //    objectHelper.AddOrUpdateObject(newObject, gpsCoord.latitude, gpsCoord.longitude, 0, Quaternion.identity);

        //    Debug.Log($"Added {newObject.name} at latitude {gpsCoord.latitude} and longitude {gpsCoord.longitude}");
        //}

        //positioningManager.OnStatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(WorldPositioningStatus status) {
        Debug.Log("Status changed to " + status);
    }

    private void Update() {
        if (isPlaying) {

        }
    }

    public (double, double) GetGeographicOffsetFromCameraPosition(Vector3 position) {
        double latitudeOffset = position.z / 111000;
        double longitudeOffset = position.x / (111000 * Mathf.Cos((float) positioningManager.WorldTransform.OriginLatitude * Mathf.Deg2Rad));

        return (latitudeOffset, longitudeOffset);
    }


    private void InitiateGame() {
        isPlaying = true;
        Transform cameraTransform = cameraManager.GetComponent<Transform>();

        for (int i = 0; i < 10; i++) {
            (double latOffset, double longOffset) = GetGeographicOffsetFromCameraPosition(cameraTransform.position + new Vector3(UnityEngine.Random.Range(10f, 100f), UnityEngine.Random.Range(10f, 100f), UnityEngine.Random.Range(10f, 100f)));

            double latitude = positioningManager.WorldTransform.OriginLatitude + latOffset;
            double longitude = positioningManager.WorldTransform.OriginLongitude + longOffset;

            GameObject newObject = Instantiate(balloons[0]);

            newObject.GetComponent<MeshRenderer>().material = materialsList[UnityEngine.Random.Range(0, materialsList.Count)];

            objectHelper.AddOrUpdateObject(newObject, latitude, longitude, 0, Quaternion.identity);
        }

        positioningManager.OnStatusChanged += OnStatusChanged;
    }

    public ARWorldPositioningObjectHelper GetObjectHelper() {
        return objectHelper;
    }

    public ARWorldPositioningManager GetWorldPositioningManager() {
        return positioningManager;
    }

}

[Serializable]
public struct LatLong {
    public double latitude;
    public double longitude;
}