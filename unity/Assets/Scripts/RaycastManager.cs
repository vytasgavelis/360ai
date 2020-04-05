using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public float forwardDistance;
    public float backwardsDistance;
    public float leftDistance;
    public float rightDistance;

    public float avoidDistance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        Vector3 backwards = transform.TransformDirection(Vector3.back) * 10;
        Debug.DrawRay(transform.position, backwards, Color.green);

        Vector3 left = transform.TransformDirection(Vector3.left) * 10;
        Debug.DrawRay(transform.position, left, Color.green);

        Vector3 right = transform.TransformDirection(Vector3.right) * 10;
        Debug.DrawRay(transform.position, right, Color.green);

        forwardDistance = 0;
        backwardsDistance = 0;
        rightDistance = 0;
        leftDistance = 0;

        if (Physics.Raycast(transform.position, forward, out hit))
        {
            forwardDistance = hit.distance;
        }
        if (Physics.Raycast(transform.position, backwards, out hit))
        {
            backwardsDistance = hit.distance;
        }
        if (Physics.Raycast(transform.position, left, out hit))
        {
            leftDistance = hit.distance;
        }
        if (Physics.Raycast(transform.position, right, out hit))
        {
            rightDistance = hit.distance;
        }


        DrawRays();
    }

    void DrawRays()
    { 
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        Vector3 backwards = transform.TransformDirection(Vector3.back) * 10;
        Debug.DrawRay(transform.position, backwards, Color.green);

        Vector3 left = transform.TransformDirection(Vector3.left) * 10;
        Debug.DrawRay(transform.position, left, Color.green);

        Vector3 right = transform.TransformDirection(Vector3.right) * 10;
        Debug.DrawRay(transform.position, right, Color.green);
    }
}
