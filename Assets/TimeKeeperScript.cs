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

    void SetToObjInfo(ControllableObjectInfo objInfo, Transform childObjTrans, Rigidbody childObjRigBody, ObjectController childObjCont)
    {
        objInfo.posX = childObjTrans.position.x;
        objInfo.posY = childObjTrans.position.y;
        objInfo.posZ = childObjTrans.position.z;

        objInfo.rotX = childObjTrans.rotation.x;
        objInfo.rotY = childObjTrans.rotation.y;
        objInfo.rotZ = childObjTrans.rotation.z;

        objInfo.sclX = childObjTrans.localScale.x;
        objInfo.sclY = childObjTrans.localScale.y;
        objInfo.sclZ = childObjTrans.localScale.z;

        objInfo.objTag = childObjTrans.tag;

        objInfo.linearVelocity = childObjRigBody.linearVelocity;
        objInfo.angularVelocity = childObjRigBody.angularVelocity;

        objInfo.isMoving = childObjCont.GetIsObjectMovingValue();
        objInfo.isCameraAssigned = childObjCont.GetIsCameraAssignedValue();
    }

    void SetFromObjInfo(ControllableObjectInfo objInfo, Transform childObjTrans, Rigidbody childObjRigBody, ObjectController childObjCont)
    {
        childObjTrans.position = new Vector3(objInfo.posX, objInfo.posY, objInfo.posZ);
        childObjTrans.rotation = Quaternion.Euler(objInfo.rotX, objInfo.rotY, objInfo.rotZ);
        childObjTrans.localScale = new Vector3(objInfo.sclX, objInfo.sclY, objInfo.sclZ);

        childObjRigBody.linearVelocity = objInfo.linearVelocity;
        childObjRigBody.angularVelocity = objInfo.angularVelocity;

        // We don't set tag, as we would be using tag to check whether the values are being set to the right object.

        // Wouldn't need to set isMoving value as it doesn't matter when rewinding.

        childObjCont.SetCameraAssignedValue(objInfo.isCameraAssigned);
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
    }

    // Frame Recorder

    // Rewind Time
}
