using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGlobalPosition : MonoBehaviour
{
    [SerializeField] private Transform cameraGlobalFocusPointTransform;
    [SerializeField] private float smoothness = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraGlobalFocusPointTransform = GameObject.FindGameObjectWithTag("CameraGlobalFocusPoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Lerping from current transform position to the camera global focus point transform position + y 20f with lerping movement smoothness * dt to make it smooth overtime
        transform.position = Vector3.Lerp(transform.position, cameraGlobalFocusPointTransform.position + new Vector3(0f, 20f, 20f), smoothness * Time.deltaTime);

        // Now, camera global position object's rotation need to look at camera global focus point object's position
        // Creating the direction towards which the camera has to look, that is, the look direction
        Vector3 lookDirection = (cameraGlobalFocusPointTransform.position - transform.position).normalized; // Can also use --> Vector3.Normalize((cameraGlobalFocusPointTransform.position - transform.position))
        // Now that I got the look direction, making a look rotation. Also making it lerp from current rotation to LookRotation with lerping smoothness * dt to make it smooth overtime
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles, smoothness * Time.deltaTime);
    }
}
