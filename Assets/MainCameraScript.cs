using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour, IControl
{
    [Header("Objects")]
    [SerializeField] private GameObject controllableObject00;
    [SerializeField] private GameObject controllableObject01;
    [SerializeField] private Dictionary<GameObject, string> dictionaryGameObjectWithTagHandle; // Removed type '<GameObject, TagHandle>' and used '<GameObject, string>' instead.
    [SerializeField] private string cameraAssignedObjectTag; // Removed type 'TagHandle'.

    [Header("MainCamera")]
    [SerializeField] private Vector3 _currentPosition, _newPosition, _targetPosition, _currentRotation;
    [SerializeField] private float _cameraMovementSmoothness = 0.5f;
    private Vector3 offSetForCameraPos, offSetForCameraRot;
    private Transform _myTransform;

    [Header("ICamera")]
    [SerializeField] private static List<ICamera> iCameraSubscribersList;
    public static void SubscribeToICamera(ICamera listener) { iCameraSubscribersList.Add(listener); }
    public static void UnsubscribeFromICamera(ICamera listener) { iCameraSubscribersList.Remove(listener); }

    void Awake()
    {
        _myTransform = GetComponent<Transform>();

        // Initialising lists and dictionaries
        iCameraSubscribersList = new List<ICamera>();
        dictionaryGameObjectWithTagHandle = new Dictionary<GameObject, string>();
        dictionaryGameObjectWithTagHandle.Add(controllableObject00, controllableObject00.transform.tag); // Assigning GameObjects and their 'string tag' in Dictionary
        dictionaryGameObjectWithTagHandle.Add(controllableObject01, controllableObject01.transform.tag);

        // By Default, assigning MainCamera to object 00.
        cameraAssignedObjectTag = dictionaryGameObjectWithTagHandle[controllableObject00];

        // Assigning value
        offSetForCameraPos = new Vector3(0f, 7f, -10);
        offSetForCameraRot = new Vector3(25f, 0f, 0f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectControl.SubscribeToIControl(this);

        _currentPosition = transform.position;
        _newPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CameraUpdate();
        PushInfoToICameraSubscribers();
    }

    void CameraUpdate()
    {
        foreach (KeyValuePair<GameObject, string> pair in dictionaryGameObjectWithTagHandle)
        {
            if (cameraAssignedObjectTag == pair.Value)
            {
                _targetPosition = pair.Key.transform.GetChild(0).transform.position;
                break;
            }
        }

        _newPosition = _targetPosition + offSetForCameraPos;
        _currentPosition = transform.position;
        _currentRotation = transform.rotation.eulerAngles;
        transform.position = Vector3.Lerp(_currentPosition, _newPosition, 0.5f);
        transform.rotation.SetLookRotation(Vector3.Lerp(_currentRotation, offSetForCameraRot, 0.5f));
    }

    void PushInfoToICameraSubscribers()
    {
        foreach(var i in iCameraSubscribersList)
        {
            i.ICameraUpdate(cameraAssignedObjectTag);
        }
    }

    public void IControlUpdate(bool isObjectMoving, string objectTag)
    {

    }
}
