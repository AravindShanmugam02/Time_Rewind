using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour, ICamera
{
    [Header("Objects")]
    [SerializeField] private float _movementSpeed = 1.0f;
    [SerializeField] private float _rightLeftInput, _forwardBackwardInput;
    [SerializeField] private Vector3 _movementInput, _directionOfMovement, _movementVelocityOrMotion;
    [SerializeField] private bool isObjectMoving = false;
    [SerializeField] private string _myTag; // Removed type 'TagHandle' and used 'string' instead.
    private Transform _myTransform;
    private Rigidbody _myRigidBody;
    private CharacterController _myCharacterController;
    private float _gravityForce = -9.81f;

    [Header("MainCamera")]
    [SerializeField] private bool _isCameraAssigned = false;                                                            //} --> Made these 3 as non-static.
    public void SetCameraAssignedValue(bool isCameraAssigned) { _isCameraAssigned = isCameraAssigned; }                 //}     When declared as static, accessing the function to set the value actually accesses both the ControllableObject's
    public bool GetCameraAssignedValue() { return _isCameraAssigned; }                                                  //}     function because of the same name and being 'public static'.

    [Header("IControl")]
    [SerializeField] private static List<IControl> iControlSubscribersList;
    public static void SubscribeToIControl(IControl listener) { iControlSubscribersList.Add(listener); }
    public static void UnsubscribeFromIControl(IControl listener) { iControlSubscribersList.Remove(listener); }

    void Awake()
    {
        // Initialising the list
        iControlSubscribersList = new List<IControl>();

        // Get components
        _myTransform = GetComponent<Transform>();
        _myRigidBody = GetComponent<Rigidbody>();
        _myCharacterController = GetComponent<CharacterController>();
        _myTag = _myTransform.tag;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCamera.SubscribeToICamera(this);

        _myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
        PushInfoToIControlSubscribers();
    }

    void MovementUpdate()
    {
        // Is only controled when camera is assigned to this object
        if(_isCameraAssigned)
        {
#if UNITY_ANDROID || UNITY_IOS
            // Movement input
            _rightLeftInput = CrossPlatformInputManager.GetAxis("Horizontal");
            _upDownInput = CrossPlatformInputManager.GetAxis("Vertical");
#else
            // Movement input
            _rightLeftInput = Input.GetAxis("Horizontal");
            _forwardBackwardInput = Input.GetAxis("Vertical");
#endif
            // Calculating Movement, Direction, Movement Velocity (or) Motion, and Applying it to the Character Controller.
            _movementInput = new Vector3(_rightLeftInput, 0.0f, _forwardBackwardInput);
            _directionOfMovement = _movementInput.normalized;
            _movementVelocityOrMotion = _directionOfMovement * _movementSpeed * Time.deltaTime;
            _myCharacterController.Move(_movementVelocityOrMotion);
        }

        if (_myCharacterController.velocity == Vector3.zero)
        {
            // Not moving...
            isObjectMoving = false;
        }
        else
        {
            // Moving...
            isObjectMoving = true;
        }
    }

    void PushInfoToIControlSubscribers()
    {
        foreach (var i in iControlSubscribersList)
        {
            i.IControlUpdate(isObjectMoving, _myTag);
        }
    }

    public void ICameraUpdate(string cameraAssignedObjectTag)
    {
        if (_myTag == cameraAssignedObjectTag)
        {
            SetCameraAssignedValue(true);
        }
        else
        {
            SetCameraAssignedValue(false);
        }
    }
}
