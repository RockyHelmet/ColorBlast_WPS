using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SharedSpaceFromTrackedImage : MonoBehaviour {
    [SerializeField] ARTrackedImageManager manager;
    [SerializeField] Transform sharedSpace;

    void OnEnable() => manager.trackedImagesChanged += OnChanged;
    void OnDisable() => manager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs args) {
        foreach (var img in args.added)
            Snap(img);
        foreach (var img in args.updated)
            if (img.trackingState == TrackingState.Tracking) Snap(img);
    }

    void Snap(ARTrackedImage img) {
        sharedSpace.SetPositionAndRotation(img.transform.position, img.transform.rotation);
    }
}
