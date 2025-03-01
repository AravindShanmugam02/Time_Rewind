using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    // Controllable Object Container
    GameObject COContainer;

    // List for holding child objects of Controllable Object Container
    List<Transform> listOfChildrenOfControllableObjectContainer;

    // List for holding the controllable objects information
    List<ControllableObjectInfo> listOfControlllableObjectsInfo;

    // List for holding Frames
    List<FrameInfo> listOfFramesInfo;

    void SetToObjInfo(ControllableObjectInfo _objInfo, Transform _childObjTrans, Rigidbody _childObjRigBody, ObjectController _childObjCont)
    {
        _objInfo.posX = _childObjTrans.position.x;
        _objInfo.posY = _childObjTrans.position.y;
        _objInfo.posZ = _childObjTrans.position.z;

        _objInfo.rotX = _childObjTrans.rotation.x;
        _objInfo.rotY = _childObjTrans.rotation.y;
        _objInfo.rotZ = _childObjTrans.rotation.z;

        _objInfo.sclX = _childObjTrans.localScale.x;
        _objInfo.sclY = _childObjTrans.localScale.y;
        _objInfo.sclZ = _childObjTrans.localScale.z;

        _objInfo.objTag = _childObjTrans.tag;

        _objInfo.linearVelocity = _childObjRigBody.linearVelocity;
        _objInfo.angularVelocity = _childObjRigBody.angularVelocity;

        _objInfo.isMoving = _childObjCont.GetIsObjectMovingValue();
        _objInfo.isCameraAssigned = _childObjCont.GetIsCameraAssignedValue();
    }

    void SetFromObjInfo(ControllableObjectInfo _objInfo, Transform _childObjTrans, Rigidbody _childObjRigBody, ObjectController _childObjCont)
    {
        _childObjTrans.position = new Vector3(_objInfo.posX, _objInfo.posY, _objInfo.posZ);
        _childObjTrans.rotation = Quaternion.Euler(_objInfo.rotX, _objInfo.rotY, _objInfo.rotZ);
        _childObjTrans.localScale = new Vector3(_objInfo.sclX, _objInfo.sclY, _objInfo.sclZ);

        _childObjRigBody.linearVelocity = _objInfo.linearVelocity;
        _childObjRigBody.angularVelocity = _objInfo.angularVelocity;

        // We don't set tag, as we would be using tag to check whether the values are being set to the right object.

        // Wouldn't need to set isMoving value as it doesn't matter when rewinding.

        _childObjCont.SetCameraAssignedValue(_objInfo.isCameraAssigned);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        listOfChildrenOfControllableObjectContainer = new List<Transform>();
        listOfControlllableObjectsInfo = new List<ControllableObjectInfo>();
        COContainer = GameObject.FindGameObjectWithTag("ControllableObjectsContainer");

        if(COContainer == null)
        {
            Debug.LogError("GameObject Not Found! - ControllableObjectsContainer");
        }
        else
        {
            int childrenCount = COContainer.transform.childCount;
            for(int i = 0; i < childrenCount; i++)
            {
                // Getting the transforms of all the child controllable Objects.
                listOfChildrenOfControllableObjectContainer.Add(COContainer.transform.GetChild(i));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Input Handle
        if(Input.GetKey(KeyCode.R)) // Rewind
        {

        }
    }

    // Frame Recorder

    // Rewind Time
}
