using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public struct ControllableObjectTimeGapInfo
{
    public string objTag;
    public int timeGapStartFrame;
    public int timeGapStopFrame;
}

public struct FrameInfo
{
    public List<ControllableObjectInfo> ControllableObjectsInfoList;
}

public struct StringTagGameObject
{
    public string stgoKey;  // Removed type 'TagHandle' and used 'string' instead for storing game object tags.
    public GameObject stgoValue;
}