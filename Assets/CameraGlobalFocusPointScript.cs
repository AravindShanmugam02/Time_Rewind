using System.Collections.Generic;
using UnityEngine;

public class CameraGlobalFocusPoint : MonoBehaviour
{
    // To hold the transforms of the children of controllable objects container
    [SerializeField] private List<Transform> controllableObjectsList;
    [SerializeField] private float smoothness = 5.0f;

    void Awake()
    {
        // Initialising list
        controllableObjectsList = new List<Transform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject tempGameObject = GameObject.FindGameObjectWithTag("ControllableObjectsContainer");
        if(tempGameObject == null)
        {
            Debug.LogAssertion("tempGameObject is null in CameraGlobalFocusPoint class.");
        }

        for(int i = 0; i < tempGameObject.transform.childCount; i++)
        {
            controllableObjectsList.Add(tempGameObject.transform.GetChild(i).transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 globalFocusPoint = new Vector3(0,0,0);

        // Since there won't be too many objects in my prototype, using for loop
        for(int i = 0; i < controllableObjectsList.Count; i++)
        {
            globalFocusPoint += controllableObjectsList[i].position;
        }

        // Finding a center point or centroid for given coordinates is (V1 + V2 + V3)/number of coordinates given => centroid Vc
        globalFocusPoint /= controllableObjectsList.Count;

        // Lerping between current position to the new centroid position with y 1f with lerping movement smoothness * dt to make it smooth overtime
        transform.position = Vector3.Lerp(transform.position, new Vector3(globalFocusPoint.x, 1, globalFocusPoint.z), smoothness * Time.deltaTime);
    }
}
