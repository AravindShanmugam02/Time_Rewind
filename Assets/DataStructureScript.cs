using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To hold gameobject's information for every frame
public struct ControllableObjectInfo
{
    public string objTag;
    public bool isCameraAssigned;
    public bool isMoving;

    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;

    public float sclX;
    public float sclY;
    public float sclZ;

    public Vector3 linearVelocity;
    public Vector3 angularVelocity;
}

// To hold gameobject tag and its time gap information
public struct ControllableObjectTimeGapInfo
{
    public string objTag;
    
    // This will be the frame from which the rewind has begun. This takes the frame number from the list of FrameInfo. This info will be unique for each prefab gameobject.
    public int timeGapStartFrame;

    // This will be the frame to which the rewind has ended. This takes the frame number from the list of FrameInfo. This info will be unique for each prefab gameobject.
    public int timeGapStopFrame;
}

// To hold list of gameobject's information that was stored for a frame
public struct FrameInfo
{
    public List<ControllableObjectInfo> ControllableObjectsInfoList;
}

// To hold gameobject tag and gameobject
public struct StringTagGameObject
{
    public string stgoKey;  // Removed type 'TagHandle' and used 'string' instead for storing game object tags.
    public GameObject stgoValue;
}