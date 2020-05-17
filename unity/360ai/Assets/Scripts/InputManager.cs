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

    public float rayDistance = 50;

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

    private RaycastHit2D CheckRaycast(Vector2 direction)
    {
        Vector2 startingPosition = new Vector2(transform.position.x, transform.position.y);

        Debug.DrawRay(startingPosition, direction, Color.green);
        return Physics2D.Raycast(startingPosition, direction, rayDistance, ObstacleLayerMask.value);
    }

    private void RaycastCheckUpdate()
    {
        Vector2 up = transform.up * rayDistance;
        Vector2 left = -transform.right * rayDistance;
        Vector2 right = transform.right * rayDistance;

        RaycastHit2D upHit = CheckRaycast(up);
        RaycastHit2D leftHit = CheckRaycast(left);
        RaycastHit2D rightHit = CheckRaycast(right);

        upDistance = 100;
        leftDistance = 100;
        rightDistance = 100;

        if (upHit.collider)
        {
            upDistance = Mathf.Abs(upHit.point.y - transform.position.y);
        }
        if (leftHit.collider)
        {
            leftDistance = Mathf.Abs(leftHit.point.x - transform.position.x);
        }
        if (rightHit.collider)
        {
            rightDistance = Mathf.Abs(rightHit.point.x - transform.position.x);
        }

        // Keep track of previous distances.
        RightDistanceHistory[HistoryCounter] = rightDistance;
        LeftDistanceHistory[HistoryCounter] = leftDistance;
        HistoryCounter++;
        if(HistoryCounter >= 5)
        {
            HistoryCounter = 0;
        }

        Debug.Log(upDistance + " " + leftDistance + " " + rightDistance);
    }

}
