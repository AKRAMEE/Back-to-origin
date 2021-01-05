using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
#if UNITY_EDITOR
using input = GoogleARCore.InstantPreviewInput;
#endif

public class ARController : MonoBehaviour
{
    private List<TrackedPlane> m__NewDetectedPlanes= new List<TrackedPlane>();
    public GameObject GridPrefab;
    public GameObject Portal;
    public GameObject ARCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //chech ARCore session states
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        Session.GetTrackables<TrackedPlane>(m__NewDetectedPlanes,TrackableQueryFilter.New);
        //instantiate a grid for each detectedplane in m_newdetected planes
        for (int i = 0; i < m__NewDetectedPlanes.Count; ++i)
        {
            GameObject grid = Instantiate(GridPrefab,Vector3.zero,Quaternion.identity,transform);
            //this fuction will set the position of grid and modify the vertices of the attached mesh
            grid.GetComponent<GridVisualiser>().Initialize(m__NewDetectedPlanes[i]);
          
        }
        //check if the user totch the screen
        Touch touch;
        if (Input.touchCount<1||(touch = Input.GetTouch(0)).phase!=TouchPhase.Began)
        {
            return;
        }
        //lets check if the user touched any of the Detectedplanes
        TrackableHit hit;
        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            //lets place the prtal on top of the detectedplane that we touch

            //enable the portal
            Portal.SetActive(true);

            //create new anchor
            Anchor anchor=hit.Trackable.CreateAnchor(hit.Pose);

            //set the position of the portal to be the same as the hit position
            Portal.transform.position=hit.Pose.position;
            Portal.transform.rotation=hit.Pose.rotation;

            //we want the portal to face the camera
            Vector3 CameraPosition = ARCamera.transform.position;

            //the portal should only rotate around the Y axis 
            CameraPosition.y = hit.Pose.position.y;

            //Rotate the poral to face the camera 
            Portal.transform.LookAt(CameraPosition,Portal.transform.up);
            //ARCore will keep understanding the world
            Portal.transform.parent=anchor.transform;


        }
    }
}
