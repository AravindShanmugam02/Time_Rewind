using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour, IControl, ICamera
{
    public enum RewindState
    {
        NoState,
        Object,
        Global
    }

    // Controllable Object Container
    private GameObject COContainer;

    // List for holding child objects of Controllable Object Container
    private List<Transform> listOfChildrenOfControllableObjectContainer;

    // List for holding the controllable objects information
    private List<ControllableObjectInfo> listOfControlllableObjectsInfo;

    // List for holding Frames
    private List<FrameInfo> listOfFramesInfo;

    // Variable to hold current camera state
    private MainCamera.CameraState currentCameraState;

    // Variable to hold rewind state
    private TimeKeeper.RewindState currentRewindState;

    void Awake()
    {
        // Initializing lists
        listOfChildrenOfControllableObjectContainer = new List<Transform>();
        listOfControlllableObjectsInfo = new List<ControllableObjectInfo>();
        listOfFramesInfo = new List<FrameInfo>();

        // Assigning current camera state
        currentCameraState = MainCamera.CameraState.NoState;

        // Assigning current rewind state
        currentRewindState = TimeKeeper.RewindState.NoState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Finding Controllable Objects Container
        COContainer = GameObject.FindGameObjectWithTag("ControllableObjectsContainer");

        if(COContainer == null)
        {
            Debug.LogError("GameObject Not Found! - ControllableObjectsContainer");
        }

        int childrenCount = COContainer.transform.childCount;
        for(int i = 0; i < childrenCount; i++)
        {
            // Getting the transforms of all the child controllable Objects.
            listOfChildrenOfControllableObjectContainer.Add(COContainer.transform.GetChild(i));
        }

        ObjectController.SubscribeToIControl(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Input Handle
        if(Input.GetKey(KeyCode.R)) // Rewind
        {
            RewindTime();
        }
    }

    // Frame Recorder
    void FrameRecorder()
    {
        // If rewinding globally, stop recording frames
        if (currentRewindState != RewindState.Global)
        {
            // Checking if the list of conrtrollable object info has the same number of entries as the list of all the child objects of controllable object container.
            if (listOfControlllableObjectsInfo.Count == listOfChildrenOfControllableObjectContainer.Count)
            {
                // Create a new FrameInfo object.
                FrameInfo fI = new FrameInfo();

                // Assign the list of controllable object info to fI object's list.
                fI.ControllableObjectsInfoList = listOfControlllableObjectsInfo;

                // Clearing the list of controllable object info. Because when the IControl observer functions give values of ControllableObjectInfo, we got to add them to this list.
                listOfControlllableObjectsInfo.Clear();

                // Add the fI object to the list of frame info.
                listOfFramesInfo.Add(fI);
            }
        }
    }

    // Rewind Time
    void RewindTime()
    {
        if(currentCameraState == MainCamera.CameraState.Global)
        {
            // Setting rewind state variable value to global
            currentRewindState = RewindState.Global;
        }

        // Time Manipulation
        ManipulateTime();
    }

    void ManipulateTime()
    {
        // Global Time
        if(currentRewindState == RewindState.Global)
        {
            FrameInfo fI = listOfFramesInfo[listOfFramesInfo.Count - 1];
            List<ControllableObjectInfo> loCOI = fI.ControllableObjectsInfoList;
        }

        // Local Time
        if(currentRewindState == RewindState.Object)
        {

        }
    }

    public void IControlUpdate(bool _isObjectMoving, string _objectTag)
    {
        // Nothing here!
    }

    public void IControlObjectInfoUpdate(ControllableObjectInfo _cOI)
    {
        if(!listOfControlllableObjectsInfo.Contains(_cOI))
        {
            // WARNING: Not sure which of the copies of prefab in the scene would send this update in which order.
            listOfControlllableObjectsInfo.Add(_cOI);
        }

        FrameRecorder();
    }

    // To get update from ICamera about camera assigned object tag
    public void ICameraUpdate(string _cameraAssignedObjectTag)
    {

    }

    // To get update from ICamera about the current camera state
    public void ICameraStateUpdate(MainCamera.CameraState _currentCameraState)
    {
        currentCameraState = _currentCameraState;
    }
}
