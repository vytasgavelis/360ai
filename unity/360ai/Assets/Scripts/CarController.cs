using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public InputManager im;
    public float CarSpeed = 100f;


    private void Update()
    { 
        transform.Translate(Vector2.up * Time.deltaTime * im.VerticalMovement * CarSpeed);
        transform.Translate(Vector2.right * Time.deltaTime * im.HorizontalMovement * CarSpeed);

        // Put obstacle avoidance logic here
    }

    void MoveForwards()
    {

    }

    void MoveBackwards()
    {

    }

    void TurnLeft()
    {

    }

    void TurnRight()
    {

    }
}
