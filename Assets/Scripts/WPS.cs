using Niantic.Experimental.Lightship.AR.WorldPositioning;
using Niantic.Experimental.Lightship.AR.XRSubsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class WPS : MonoBehaviour {

    public static WPS Instance {  get; private set; }

    [SerializeField] private List<LatLong> latLongs = new();
    [SerializeField] private ARWorldPositioningManager positioningManager;
    [SerializeField] private ARWorldPositioningObjectHelper objectHelper;
    [SerializeField] private ARCameraManager cameraManager;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // Will add location validation here to start.
    }

    public GameObject SpawnGameObjectAtLocation(GameObject prefab, Vector3 relativePosition) {
        GameObject prefabObject = Instantiate(prefab);

        (double latOffset, double longOffset) = GetGeographicOffsetFromCameraPosition(relativePosition);

        double latitude = positioningManager.WorldTransform.OriginLatitude + latOffset;
        double longitude = positioningManager.WorldTransform.OriginLongitude + longOffset;
        double altitude = relativePosition.y;

        objectHelper.AddOrUpdateObject(prefabObject, latitude, longitude, altitude, Quaternion.identity);

        return prefabObject;
    }
    public (double, double) GetGeographicOffsetFromCameraPosition(Vector3 position) {
        double latitudeOffset = position.z / 111000;
        double longitudeOffset = position.x / (111000 * Mathf.Cos((float)positioningManager.WorldTransform.OriginLatitude * Mathf.Deg2Rad));

        return (latitudeOffset, longitudeOffset);
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