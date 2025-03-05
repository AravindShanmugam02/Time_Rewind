using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour, ICamera, ITimer
{
    [Header("Objects")]
    [SerializeField] private float movementSpeed = 5.0f, rotationSpeed = 5.0f;
    [SerializeField] private float rightLeftInput, forwardBackwardInput;
    [SerializeField] private Vector3 movementDirectionInput, directionOfMovement, movementVelocityOrMotion, newRotation;
    [SerializeField] private string myTag; // Removed type 'TagHandle' and used 'string' instead.
    [SerializeField] private bool isObjectMoving = false;
    private Transform myTransform;
    private Rigidbody myRigidBody;
    private CharacterController myCharacterController;
    private ControllableObjectInfo myCOI;

    public bool GetIsObjectMovingValue() { return isObjectMoving; } // Created this function, so that TimeKeeper can use it to access 'isObjectMoving' value on adhoc to set this value every frame.
    public string GetObjectTag() { return myTag; }

    [Header("MainCamera")]
    [SerializeField] private bool isCameraAssigned = false;                                                             //} --> Made these 3 as non-static.
    public void SetCameraAssignedValue(bool _isCameraAssigned) { isCameraAssigned = _isCameraAssigned; }                //}     When declared as static, accessing the function to set the value actually accesses both the ControllableObject's
    // TimeKeeper can use SetCameraAssignedValue(bool _isCameraAssigned) on adhoc to set this value every frame.
    public bool GetIsCameraAssignedValue() { return isCameraAssigned; }                                                 //}     function because of the same name and being 'public static'.
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
        myTransform = GetComponent<Transform>();
        myRigidBody = GetComponent<Rigidbody>();
        myCharacterController = GetComponent<CharacterController>();
        myTag = myTransform.tag;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Freeze X and Z rot as we won't be using them and also not freezing them would make the obj rotate in those axis unnecessarily.
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Subscribe to ICamera
        MainCamera.SubscribeToICamera(this);

        // Subscribe to ITimer
        TimeKeeper.SubscribeToITimer(this);
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
        RecordObjInfo();
    }

    void MovementUpdate()
    {
        // Is only controled when camera is assigned to this object
        if(isCameraAssigned)
        {
#if UNITY_ANDROID || UNITY_IOS
            // Movement input
            _rightLeftInput = CrossPlatformInputManager.GetAxis("Horizontal");
            _upDownInput = CrossPlatformInputManager.GetAxis("Vertical");
#else
            // Movement input
            rightLeftInput = Input.GetAxis("Horizontal");
            forwardBackwardInput = Input.GetAxis("Vertical");
#endif
////////////// Calculating Movement, Direction, Movement Velocity (or) Motion, and Applying it to the Character Controller //////////////

            // Movement direction using Vector3 Input
            movementDirectionInput = new Vector3(rightLeftInput, 0.0f, forwardBackwardInput);
            
            // Direction of Movement
            directionOfMovement = movementDirectionInput.normalized;

            // Movement Speed
            movementVelocityOrMotion = directionOfMovement * movementSpeed;

            // NOTE: Can also use pure physics to move the object by myRigidBody.AddForce(movementVelocityOrMotion);
            // (OR) by not using physics at all by transform.translate.
            myCharacterController.Move((movementVelocityOrMotion) * Time.deltaTime);

            // [TODO] Calculate Jump velocity in consideration with gravity.
        }

        // Calculating Rotation - Remember X and Z Rotation should be frozen.
        // Keeping this calculation outside of _isCameraAssigned because, the object shouldn't rotate unnecessarily when not moving or moving.
        /*
        NOTE: "The Object rotates on it's own when moving, even when no rotation or angular force is applied" - This bug is fixed now.
        Fix - For every rigid object, Unity activates the Automatic Inertia Tensor which applies an Intertia Tensor of about V3(1,1,1)
        to the object when it moves, especially when it moves in the -90 or 90 from it's Z axis. It is basically an angular inertia.
        So, I have turned it off in the editor and set the Intertia Tensor to V3(0,0,0). Now, the rotation should be all good to calculate.
        */

        if(directionOfMovement != Vector3.zero) // Keeping logic inside this if so that the object don't snap back to it's original default rotation when not moving.
        {
            newRotation = Quaternion.LookRotation(directionOfMovement, transform.up).eulerAngles; // Quaternion.LookRotation(Direction_to_look_in, direction_up);
            // [TO FIX] A bug is found where - When object is facing forward, that is, Rotation Y is 0 and asked to turn left, rather than just turning left from straight,
            // it turns right, back and then reaches left direction.
            // When noticed what is the reason, I could see that the newRotation value is 270 when turning left, which is a positive value,
            // hence the object does a full clockwise rotation to reach 270 for rotation Y.
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, newRotation, rotationSpeed * Time.deltaTime));
        }

        // Can also check this with pure physics by using myRigidBody.linearVelocity == Vector3.zero
        if (myCharacterController.velocity == Vector3.zero)
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

    // Since this script is for a prefab, this function records information about only this object.
    void RecordObjInfo()
    {
        myCOI = new ControllableObjectInfo();
        myCOI.posX = myTransform.position.x;
        myCOI.posY = myTransform.position.y;
        myCOI.posZ = myTransform.position.z;

        myCOI.rotX = myTransform.rotation.x;
        myCOI.rotY = myTransform.rotation.y;
        myCOI.rotZ = myTransform.rotation.z;

        myCOI.sclX = myTransform.localScale.x;
        myCOI.sclY = myTransform.localScale.y;
        myCOI.sclZ = myTransform.localScale.z;

        myCOI.objTag = myTransform.tag;

        myCOI.linearVelocity = myRigidBody.linearVelocity;
        myCOI.angularVelocity = myRigidBody.angularVelocity;

        myCOI.isMoving = GetIsObjectMovingValue();
        myCOI.isCameraAssigned = GetIsCameraAssignedValue();

        SendMyCOIToIControlSubscribers();
    }

    void RetrieveObjInfo(ControllableObjectInfo _cOI)
    {
        // We are setting information to the right object by checking the tag. This check is needed because of this script is for a controllable object prefab and we get the _cOI through subscribing to ITimer interface.
        // But it is already checked and sent from TimeKeeper class.
        if(myTag == _cOI.objTag)
        {
            myTransform.position = new Vector3(_cOI.posX, _cOI.posY, _cOI.posZ);
            myTransform.rotation = Quaternion.Euler(_cOI.rotX, _cOI.rotY, _cOI.rotZ);
            myTransform.localScale = new Vector3(_cOI.sclX, _cOI.sclY, _cOI.sclZ);

            myRigidBody.linearVelocity = _cOI.linearVelocity;
            myRigidBody.angularVelocity = _cOI.angularVelocity;

            // We don't set tag, as we would be using tag to check whether the values are being set to the right object.

            // Wouldn't need to set isMoving value as it doesn't matter when rewinding.

            SetCameraAssignedValue(_cOI.isCameraAssigned);
        }
    }

    // Pushing the information about this controllable object prefab to subscribers.
    void SendMyCOIToIControlSubscribers()
    {
        foreach (var i in iControlSubscribersList)
        {
            // Sending out IsObjectMoving information
            i.IControlUpdate(isObjectMoving, myTag);

            // Sending out this object's majority of information
            i.IControlObjectInfoUpdate(myCOI);
        }
    }

    // To get update from ICamera about camera assigned object tag
    public void ICameraUpdate(string _cameraAssignedObjectTag)
    {
        if (myTag == _cameraAssignedObjectTag)
        {
            SetCameraAssignedValue(true);
        }
        else
        {
            SetCameraAssignedValue(false);
        }
    }

    // To get update from ICamera about the current camera state
    public void ICameraStateUpdate(MainCamera.CameraState _currentCameraState)
    {
        // Nothing here!
    }

    public void ITimerRewindingUpdate(TimeKeeper.RewindState _rewindState)
    {

    }

    public void ITimerContObjInfoUpdate(ControllableObjectInfo _cOI)
    {
        RetrieveObjInfo(_cOI);
    }
}
