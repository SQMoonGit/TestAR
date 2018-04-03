using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour
{
    public GameObject trackedPlanePrefab;
    public GameObject firstPersonCamera;
    public ScoreboardController scoreboard;
    public SnakeController snakeController;

    // Use this for initialization
    void Start ()
    {
        QuitOnConnectionErrors();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ProcessNewPlanes();
        ProcessTouches();
        scoreboard.SetScore(snakeController.GetLength());
	}

    void QuitOnConnectionErrors()
    {
        //Do not update if ARCore is not tracking.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            //covers variety of errors.
            StartCoroutine(CodelabUtils.ToastAndExit(
                "ARCore encountered a problem connecting. Please restart the app.",5));
        }
    }

    void ProcessNewPlanes()
    {
        List<TrackedPlane> planes = new List<TrackedPlane>();
        Session.GetTrackables(planes, TrackableQueryFilter.New);

        for (int i = 0; i < planes.Count; i++)
        {
            //Instantiate a plane visualization prefab and set it to track the new plane.
            //The transform is set to the origin with an identity rotation since the mesh
            //for our prefab is updated in Unity World coordinates
            GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);
        }
    }

    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            return;

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            SetSelectedPlane(hit.Trackable as TrackedPlane);
    }

    void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        Debug.Log("Selected plane centered at " + selectedPlane.CenterPose.position);
        scoreboard.SetSelectedPlane(selectedPlane);
        snakeController.SetPlane(selectedPlane);
        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }    
}
