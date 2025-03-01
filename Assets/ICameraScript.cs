using UnityEngine;

public interface ICamera
{
    void ICameraUpdate(/*TagHandle*/string _cameraAssignedObjectTag); // Not going to use TagHandle because cannot do 'GetComponent<TagHandle>()' because it throws exception. Thus, using 'string transform.tag'.
}
