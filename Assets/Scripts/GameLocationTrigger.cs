using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[ RequireComponent(typeof(ARTrackedImageManager))]
public class GameLocationTrigger : MonoBehaviour
{

    [SerializeField] GameManager gameManager;
    ARTrackedImageManager aRRTrackedImageManager;

    private void Awake()
    {
        aRRTrackedImageManager = GetComponent<ARTrackedImageManager>();


    }
    private void OnEnable()
    {
        aRRTrackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        aRRTrackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }


    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                HandleTrackedImageScanned(trackedImage.referenceImage.name);
                return;
            }

        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                HandleTrackedImageScanned(trackedImage.referenceImage.name);
                return;
            }

        }

    }

    void HandleTrackedImageScanned(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) { return; }
        gameManager.PlaceGame(imageName);

    }

}
