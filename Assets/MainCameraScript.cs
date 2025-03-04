using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour, IControl
{
    public enum CameraState
    {
        NoState,
        Object,
        Global
    }

    [Header("Objects")]
    [SerializeField] private GameObject controllableObjectsContainer;
    [SerializeField] private Dictionary<int, StringTagGameObject> intStringTagGameObjectDictionary; // For storing GameObject and its tag with some sort of index.
    [SerializeField] private Dictionary<string, GameObject> gameObjectTagGameObjectDictionary;
    [SerializeField] private string cameraAssignedObjectTag; // Removed type 'TagHandle'.

    [Header("MainCamera")]
    [SerializeField] private Vector3 currentPosition, newPosition, targetPosition, currentRotation;
    private float cameraMovementSmoothness = 5.0f;
    private Vector3 offSetForCameraPos, offSetForCameraRot;
    private Transform myTransform;
    private CameraState myCameraState;

    [Header("ICamera")]
    [SerializeField] private static List<ICamera> iCameraSubscribersList;
    public static void SubscribeToICamera(ICamera listener) { iCameraSubscribersList.Add(listener); }
    public static void UnsubscribeFromICamera(ICamera listener) { iCameraSubscribersList.Remove(listener); }

    void Awake()
    {
        // Get transform
        myTransform = GetComponent<Transform>();
        
        // Initialising lists
        iCameraSubscribersList = new List<ICamera>();
        
        // Initialising dictionaries
        intStringTagGameObjectDictionary = new Dictionary<int, StringTagGameObject>();
        gameObjectTagGameObjectDictionary = new Dictionary<string, GameObject>();

        // Assigning value for variables
        offSetForCameraPos = new Vector3(0f, 7f, -10); // Hard value just got by trail & error in the editor.
        offSetForCameraRot = new Vector3(25f, 0f, 0f);
        currentPosition = transform.position;
        newPosition = transform.position;

        // Assigning camera state
        myCameraState = CameraState.NoState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Finding Controllable Objects Container
        controllableObjectsContainer = GameObject.FindGameObjectWithTag("ControllableObjectsContainer");
        if (controllableObjectsContainer == null)
        {
            Debug.LogError("GameObject Not Found! - ControllableObjectsContainer");
        }
        // Storing controllable objects into dictionary
        for(int i = 0; i < controllableObjectsContainer.transform.childCount; i++)
        {
            StringTagGameObject stgo = new StringTagGameObject();
            stgo.stgoKey = controllableObjectsContainer.transform.GetChild(i).tag;
            stgo.stgoValue = controllableObjectsContainer.transform.GetChild(i).gameObject;
            intStringTagGameObjectDictionary.Add(i, stgo);
            gameObjectTagGameObjectDictionary.Add(stgo.stgoKey, stgo.stgoValue);
        }

        // By Default, assigning MainCamera to object 0.
        // Dictionary[i] --> Gives the Value of the Key, that is, 'i' in the dictionary.
        // Hence, Dictionary[i].stgoKey is actually Dictionary Value of Key ['i'].stgoKey --> which is the 'string' in the StringTagGameObject struct.
        cameraAssignedObjectTag = intStringTagGameObjectDictionary[0].stgoKey;
        
        // Assigning camera state
        if (cameraAssignedObjectTag != "None") myCameraState = CameraState.Object;

        // Subscribe to IControl
        ObjectController.SubscribeToIControl(this);
    }

    // Update is called once per frame
    void Update()
    {
        CameraInput();
        CameraUpdate();
        PushInfoToICameraSubscribers();
    }

    void CameraInput()
    {
#if UNITY_ANDROID || UNITY_IOS
#else
        if(!Input.GetKey(KeyCode.R)) // When not rewinding
        {
            if (Input.GetKeyDown(KeyCode.I)) // Switch Camera
            {
                // If camera is assigned to the last ControllableObject in the list, On next switch the camera switches to Global state.
                if (cameraAssignedObjectTag == intStringTagGameObjectDictionary[(intStringTagGameObjectDictionary.Count) - 1].stgoKey)
                {
                    // Changing the camera assigned object tag to "None" as a way to represent the camera is not assigned to any object.
                    cameraAssignedObjectTag = "None";
                }
            }
        }
#endif
        if(cameraAssignedObjectTag == "None")
        {

        }
    }

    // Updates Camera's position and assigns Camera to target.
    void CameraUpdate()
    {
        // Having foreach loop to iterate over dictionary items with no properly indexed Key is not a good way of updating camera for every frame.
        // Think what will happen if there are 100s of items in the dictionary. It is very inefficient. Hence, having a dictionary with gameObjectTag (which is a string) as KEY is good option.
        // So that we can use 'Dictionary.ContainsKey'.
        if(gameObjectTagGameObjectDictionary.ContainsKey(cameraAssignedObjectTag))
        {
            // Here we are getting the position of the child object (which is its 'Face' object) of the controllable object that is mapped to the tag that is equal to cameraAssignedObjectTag.
            targetPosition = gameObjectTagGameObjectDictionary[cameraAssignedObjectTag].transform.GetChild(0).transform.position;
        }

        newPosition = targetPosition + offSetForCameraPos;
        currentPosition = transform.position;
        currentRotation = transform.rotation.eulerAngles;
        transform.position = Vector3.Lerp(currentPosition, newPosition, 0.5f);
        transform.rotation.SetLookRotation(Vector3.Lerp(currentRotation, offSetForCameraRot, cameraMovementSmoothness * Time.deltaTime));
    }

    void PushInfoToICameraSubscribers()
    {
        // Inform the subscribers about the object to which the camera is current assigned to.
        foreach(var i in iCameraSubscribersList)
        {
            i.ICameraUpdate(cameraAssignedObjectTag);
            i.ICameraStateUpdate(myCameraState);
        }
    }

    public void IControlUpdate(bool _isObjectMoving, string _objectTag)
    {

    }

    public void IControlObjectInfoUpdate(ControllableObjectInfo _cOI)
    {
        // Nothing here!
    }
}
