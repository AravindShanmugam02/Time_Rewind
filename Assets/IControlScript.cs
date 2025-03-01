using UnityEngine;

public interface IControl
{
    void IControlUpdate(bool _isObjectMoving, /*TagHandle*/string _objectTag); // Not going to use TagHandle because cannot do 'GetComponent<TagHandle>()' because it throws exception. Thus, using 'string transform.tag'.
}
