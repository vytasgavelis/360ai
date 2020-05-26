using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float upDistance;
    public float leftDistance;
    public float rightDistance;
    public float[] RightDistanceHistory;
    public float[] LeftDistanceHistory;
    private int HistoryCounter = 0;

    public float rayDistanceUp = 0.75f;
    public float rayDistanceSide = 0.63f;

    public LayerMask ObstacleLayerMask;

    private void Start()
    {
        RightDistanceHistory = new float[5];
        LeftDistanceHistory = new float[5];
        for(int i = 0; i < 5; i++)
        {
           RightDistanceHistory[i] = 100;
           LeftDistanceHistory[i] = 100;
        }
    }

    private void FixedUpdate()
    {
        RaycastCheckUpdate();
    }

    private RaycastHit2D CheckRaycast(Vector2 direction, float rayDistance)
    {
        Vector2 startingPosition = new Vector2(transform.position.x, transform.position.y);

        Debug.DrawRay(startingPosition, direction, Color.green);
        return Physics2D.Raycast(startingPosition, direction, rayDistance, ObstacleLayerMask.value);
    }

    private void RaycastCheckUpdate()
    {
        Vector2 up = transform.up * rayDistanceUp;
        Vector2 left = -transform.right * rayDistanceSide;
        Vector2 right = transform.right * rayDistanceSide;

        RaycastHit2D upHit = CheckRaycast(up, rayDistanceUp);
        RaycastHit2D leftHit = CheckRaycast(left, rayDistanceSide);
        RaycastHit2D rightHit = CheckRaycast(right, rayDistanceSide);

        upDistance = 100;
        leftDistance = 100;
        rightDistance = 100;

        if (upHit.collider)
        {
            upDistance = upHit.distance;
        }
        if (leftHit.collider)
        { 
            leftDistance = leftHit.distance;
        }
        if (rightHit.collider)
        { 
            rightDistance = rightHit.distance;
        }

        // Keep track of previous distances.
        RightDistanceHistory[HistoryCounter] = rightDistance;
        LeftDistanceHistory[HistoryCounter] = leftDistance;
        HistoryCounter++;
        if(HistoryCounter >= 5)
        {
            HistoryCounter = 0;
        }
    }

}
