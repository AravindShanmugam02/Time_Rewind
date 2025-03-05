using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject COContainer;

    // List for holding child objects of Controllable Object Container
    [SerializeField] private List<Transform> listOfChildrenOfControllableObjectContainer;

    // List for holding the controllable objects information
    [SerializeField] private List<ControllableObjectInfo> listOfControlllableObjectsInfo;

    // List for holding Frames
    [SerializeField] private List<FrameInfo> listOfFramesInfo;

    // Variable to hold current camera state
    [SerializeField] private MainCamera.CameraState currentCameraState;

    // Variable to hold rewind state
    [SerializeField] private TimeKeeper.RewindState currentRewindState;

    // Variable to hold camera assigned object tag
    [SerializeField] private string currentCameraAssignedObjectTag;

    // Variable for indexing frames
    private int frameIndex;

    // ITimer Subscription
    public static List<ITimer> iTimerSubscribersList;
    public static void SubscribeToITimer(ITimer listener) { iTimerSubscribersList.Add(listener); }
    public static void UnsubscribeFromITimer(ITimer listener) { iTimerSubscribersList.Remove(listener); }

    // Rewind Pre & Post Setup variables
    bool isRewindSetupDone = false;

    void Awake()
    {
        // Initializing lists
        iTimerSubscribersList = new List<ITimer>();
        listOfChildrenOfControllableObjectContainer = new List<Transform>();
        listOfControlllableObjectsInfo = new List<ControllableObjectInfo>();
        listOfFramesInfo = new List<FrameInfo>();

        // Assigning current camera state
        currentCameraState = MainCamera.CameraState.NoState;

        // Assigning current rewind state
        currentRewindState = TimeKeeper.RewindState.NoState;

        // Assigning camera assigned object tag variable
        currentCameraAssignedObjectTag = "NoObject";

        // Assigning value to frame index
        frameIndex = 0;
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
        MainCamera.SubscribeToICamera(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Input Handle
        if(Input.GetKey(KeyCode.R)) // Rewind
        {
            if(!isRewindSetupDone)
            {
                // Setup for Rewinding time
                PreRewindSetup();
            }

            // Time Manipulation
            ManipulateTime();
        }
        else
        {
            // [TODO] Should implement what happens when R is not pressed?
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

            // [TODO] Have to implement checks to keep the list of Frames under certain count. This time, don't use fixed number of frames (like used in older Time_Trail prototype project) as this might vary from system to system.
        }
    }

    // Setup before rewinding time
    void PreRewindSetup()
    {
        if (currentCameraState == MainCamera.CameraState.Global)
        {
            // Setting rewind state variable value to global
            currentRewindState = RewindState.Global;
        }

        // Sending out the current rewind state to listeners
        foreach (var i in iTimerSubscribersList)
        {
            i.ITimerRewindingUpdate(currentRewindState);
        }

        isRewindSetupDone = true;
    }

    // Cleanup after rewinding time
    void PostRewindCleanup()
    {
        // [TODO] Post rewind cleanup
    }

    // Actual function where frames get used for rewinding and later manipulated
    void ManipulateTime()
    {
        if(isRewindSetupDone)
        {
            // Global Time
            if (currentRewindState == RewindState.Global)
            {
                frameIndex = listOfFramesInfo.Count - 1;
                FrameInfo fI = listOfFramesInfo[frameIndex];
                List<ControllableObjectInfo> listOfCOI = fI.ControllableObjectsInfoList;

                // Usually the controllable object prefab gameobjects are inside the iTimerSubscriberList.
                foreach (var i in iTimerSubscribersList)
                {
                    // What and Why am I doing this?
                    // I am trying to seperate the ControllableObjectInfo object from the list and send the relevant update to only that controllable object by checking tags.
                    // So that only the relavant controllable object's info is sent to itself.

                    // Casting ITimer to ObjectController as it is the original type
                    ObjectController o = (ObjectController)i;

                    // Q. How can I get the object's ControllableObjectInfo without using for loop?
                    // A. Maybe using LINQ functionality

                    // listOfCOI list contains n number of ControllableObjectInfo.
                    // I want to choose the one from the list that is equal to the o's game object tag.

                    // Commenting the below out as it is obvious that using list<T>.Any would return true in this condition as any object in listOfCOI will have a tag that is same as o's tag,
                    // because listOfCOI contains the ControllableObjectInfo of all controllable object prefabs.
                    // if(listOfCOI.Any(x => x.objTag == o.GetObjectTag()))
                    // {
                    // }

                    // Using LINQ query instead of for or foreach loop to fetch the ControllableObjectInfo that has a matching tag as o's.
                    // But using LINQ query gives an IEnumerable<T> result, T being the type we are fetching.
                    var cOIIEnumerable = (from obj in listOfCOI select obj).Where(obj => obj.objTag == o.GetObjectTag());

                    // Checking if IEnumerable is empty or null
                    if (cOIIEnumerable == null || !cOIIEnumerable.Any())
                    {
                        Debug.LogError("cOIEnumerable is empty in ManipulateTime() in TimeKeeper class!");
                    }

                    // In order to get the actual ControllableObjectInfo object from the IEnumerable<T> result, we got to iterate through it using foreach or any suitable iterator.
                    // Since as per our LINQ query condition to fetch a ControllableObjectInfo object,
                    // we can be assured that only one object will be fetched due to the unique tag names for the prefab objects. Hence, taking the first element.
                    ControllableObjectInfo cOI = cOIIEnumerable.First();
                    i.ITimerContObjInfoUpdate(cOI);
                }

                // Since it is a global rewind, we remove the information of frames that were sent out to ObjectCOntroller class for rewinding.
                listOfFramesInfo.RemoveAt(frameIndex);

                // [TODO] handle out of bound error for listOfFramesInfo.

                // Decreasing frameIndex to iterate backwards.
                // Since frameIndex is getting value at the beginning from listOfFramesInfo.Count - 1, we don't need to decrement because anyways we are removing the index of frame that is used for rewinding.
                // Hence commenting.
                // frameIndex--;
            }
            // Local Time
            else if (currentRewindState == RewindState.Object)
            {
                // How do I handle local rewind???
                // Use camera assigned object tag to identify which object is trying to do a local rewind.
                // currentCameraAssignedObjectTag --> will be used.
                // Then I need to separate the obj from the lists and push that to observers/listeners.

            }
            else
            {
                // Nothing here!
            }
        }
    }

    #region IControl functions
    public void IControlUpdate(bool _isObjectMoving, string _objectTag)
    {
        // Nothing here!
    }

    // To get update from IControl that each prefab object send, and add them to a list, so that the list can be recorded for this frame.
    public void IControlObjectInfoUpdate(ControllableObjectInfo _cOI)
    {
        if(!listOfControlllableObjectsInfo.Contains(_cOI))
        {
            // WARNING: Not sure which of the copies of prefab in the scene would send this update in which order.
            // So, the list can have number of gameobject prefabs in random order too. Hence, when extracting use gameobject tag to identify the object.
            listOfControlllableObjectsInfo.Add(_cOI);
        }

        FrameRecorder();
    }
    #endregion

    #region ICamera functions
    // To get update from ICamera about camera assigned object tag
    public void ICameraUpdate(string _cameraAssignedObjectTag)
    {
        currentCameraAssignedObjectTag = _cameraAssignedObjectTag;
    }

    // To get update from ICamera about the current camera state
    public void ICameraStateUpdate(MainCamera.CameraState _currentCameraState)
    {
        currentCameraState = _currentCameraState;
    }
    #endregion
}
