using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float HorizontalMovement;
    public float VerticalMovement;

    public float upDistance;
    public float downDistance;
    public float leftDistance;
    public float rightDistance;

    public float rayDistance = 2;

    public LayerMask ObstacleLayerMask;

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement = Input.GetAxis("Horizontal");
        VerticalMovement = Input.GetAxis("Vertical");
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
        Vector2 up = new Vector2(0, 1 * rayDistance);
        Vector2 down = new Vector2(0, -1 * rayDistance);
        Vector2 left = new Vector2(-1 * rayDistance, 0);
        Vector2 right = new Vector2(1 * rayDistance, 0);

        RaycastHit2D upHit = CheckRaycast(up);
        RaycastHit2D downHit = CheckRaycast(down);
        RaycastHit2D leftHit = CheckRaycast(left);
        RaycastHit2D rightHit = CheckRaycast(right);

        upDistance = 0;
        downDistance = 0;
        leftDistance = 0;
        rightDistance = 0;

        if (upHit.collider)
        {
            upDistance = Mathf.Abs(upHit.point.y - transform.position.y);
        }
        if (downHit.collider)
        {
            downDistance = Mathf.Abs(downHit.point.y - transform.position.y);
        }
        if (leftHit.collider)
        {
            leftDistance = Mathf.Abs(leftHit.point.x - transform.position.x);
        }
        if (rightHit.collider)
        {
            rightDistance = Mathf.Abs(rightHit.point.x - transform.position.x);
        }

        Debug.Log(upDistance + " " + downDistance + " " + leftDistance + " " + rightDistance);
    }

}
