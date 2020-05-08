using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    const string MOVE_FORWARDS = "MOVE_FORWARDS";
    const string MOVE_BACKWARDS= "MOVE_BACKWARDS";
    const string TURN_LEFT = "TURN_LEFT";
    const string TURN_RIGHT = "TURN_RIGHT";
    const string WAIT = "WAIT";

    public InputManager im;
    public float CarSpeed = 1f;
    public float RotateSpeed = 5;
    public float CriticalDistance = 1f;
    public string State; // stop/forwards/baackwards/turn_left/turn_right

    private void Start()
    {
        State = WAIT;
    }

    private void Update()
    {
        HandleStates();
        Debug.Log(IsApproaching(im.RightDistanceHistory));
    }

    void Waiting()
    {
        if(im.upDistance >= CriticalDistance)
        {
            State = MOVE_FORWARDS;
        }
        else if(im.leftDistance >= CriticalDistance && im.leftDistance >= im.rightDistance)
        {
            State = TURN_LEFT;
        }
        else if(im.rightDistance >= CriticalDistance && im.rightDistance >= im.leftDistance)
        {
            State = TURN_RIGHT;
        }
    }

    void MoveForwards()
    {
        if(
            im.upDistance <= CriticalDistance
           || IsApproaching(im.RightDistanceHistory)
           || IsApproaching(im.LeftDistanceHistory)
          )
        {
            State = WAIT;
        }
        transform.Translate(Vector2.up * Time.deltaTime * CarSpeed);
    }

    void MoveBackwards()
    {        
        transform.Translate(Vector2.down * Time.deltaTime * CarSpeed);
    }

    void TurnLeft()
    {
        if(im.upDistance >= CriticalDistance)
        {
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + 1 * Time.deltaTime * RotateSpeed);
    }

    void TurnRight()
    {
        if (im.upDistance >= CriticalDistance)
        {
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + -1 * Time.deltaTime * RotateSpeed);
    }

    void HandleStates()
    {
        // This could be implemented as a one liner if state variable was pointer to a function.
        switch (State)
        {
            case MOVE_FORWARDS:
                MoveForwards();
                break;
            case MOVE_BACKWARDS:
                MoveBackwards();
                break;
            case TURN_LEFT:
                TurnLeft();
                break;
            case TURN_RIGHT:
                TurnRight();
                break;
            case WAIT:
                Waiting();
                break;
            default:
                Waiting();
                break;
        }
        //Debug.Log(State);
    }

    bool IsApproaching(float[] DistanceHistory)
    {
        bool IsApproaching = false;
        int SmallerDistanceCounter = 0;
        for(int i = 1; i < DistanceHistory.Length; i++)
        {
            if(DistanceHistory[i] < DistanceHistory[i - 1])
            {
                SmallerDistanceCounter++;
            }
        }
        if(SmallerDistanceCounter >= DistanceHistory.Length / 2)
        {
            IsApproaching = true;
        }

        return IsApproaching;
    }
}
