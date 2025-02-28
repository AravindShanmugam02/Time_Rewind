using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour, ICamera
{
    [Header("Objects")]
    [SerializeField] private float _movementSpeed = 5.0f, _rotationSpeed = 0.5f;
    [SerializeField] private float _rightLeftInput, _forwardBackwardInput;
    [SerializeField] private Vector3 _movementDirectionInput, _directionOfMovement, _movementVelocityOrMotion, _newRotation;
    [SerializeField] private string _myTag; // Removed type 'TagHandle' and used 'string' instead.
    [SerializeField] private bool isObjectMoving = false;
    public bool GetIsObjectMovingValue() { return isObjectMoving; } // Created this function, so that TimeKeeper can use it to access 'isObjectMoving' value on adhoc to set this value every frame.
    private Transform _myTransform;
    private Rigidbody _myRigidBody;
    private CharacterController _myCharacterController;

    [Header("MainCamera")]
    [SerializeField] private bool _isCameraAssigned = false;                                                            //} --> Made these 3 as non-static.
    public void SetCameraAssignedValue(bool isCameraAssigned) { _isCameraAssigned = isCameraAssigned; }                 //}     When declared as static, accessing the function to set the value actually accesses both the ControllableObject's
    // TimeKeeper can use SetCameraAssignedValue(bool isCameraAssigned) on adhoc to set this value every frame.
    public bool GetIsCameraAssignedValue() { return _isCameraAssigned; }                                                //}     function because of the same name and being 'public static'.
    // TimeKeeper can use GetIsCameraAssignedValue() on adhoc to set this value every frame.

    [Header("IControl")]
    [SerializeField] private static List<IControl> iControlSubscribersList;                                             //} --> Making these static also results in accessing all the prefab object's verson of static function.
    public static void SubscribeToIControl(IControl listener) { iControlSubscribersList.Add(listener); }                //}     Remember that this script is intended for a prefab and hence any static function, or public function, would result 
    public static void UnsubscribeFromIControl(IControl listener) { iControlSubscribersList.Remove(listener); }         //}     in whatever that object acessing, getting access to all of the prefab object's version of functions or variables.
                                                                                                                        //}     For Example, MainCamera accesses SubscribeToIControl function, which results in MainCamera accessing all the prefab object's version of that function.
                                                                                                                        //}     In our case, that shouldn't be a problem as of now. But, if any arises, got to ditch public or static functions or variables from prefab objects and find alternate solution.
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
////////////// Calculating Movement, Direction, Movement Velocity (or) Motion, and Applying it to the Character Controller //////////////

            // Movement direction using Vector3 Input
            _movementDirectionInput = new Vector3(_rightLeftInput, 0.0f, _forwardBackwardInput);
            
            // Direction of Movement
            _directionOfMovement = _movementDirectionInput.normalized;

            // Movement Speed
            _movementVelocityOrMotion = _directionOfMovement * _movementSpeed;

            // NOTE: Can also use pure physics to move the object by _myRigidBody.AddForce(_movementVelocityOrMotion);
            // (OR) by not using physics at all by transform.translate.
            _myCharacterController.Move((_movementVelocityOrMotion) * Time.deltaTime);

            // [TODO] Calculate Jump velocity in consideration with gravity.
        }

        // Calculating Rotation - Remember X and Z Rotation should be frozen.
        // Keeping this calculation outside of _isCameraAssigned because, the object shouldn't rotate unnecessarily when not moving or moving.

        // NOTE: "The Object rotates on it's own when moving, even when no rotation or angular force is applied" - This bug is fixed now.
        // Fix - For every rigid object, Unity activates the Automatic Inertia Tensor which applies an Intertia Tensor of about V3(1,1,1)
        // to the object when it moves, especially when it moves in the -90 or 90 from it's Z axis. It is basically an angular inertia.
        // So, I have turned it off in the editor and set the Intertia Tensor to V3(0,0,0). Now, the rotation should be all good to calculate.
        // [TODO - Object rotation based on its movement direction]


        // Can also check this with pure physics by using _myRigidBody.linearVelocity == Vector3.zero
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
