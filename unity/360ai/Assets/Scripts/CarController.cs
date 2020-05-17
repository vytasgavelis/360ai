﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Chassis states
    const string MOVE_FORWARDS = "MOVE_FORWARDS";
    const string MOVE_BACKWARDS = "MOVE_BACKWARDS";
    const string TURN_LEFT = "TURN_LEFT";
    const string TURN_RIGHT = "TURN_RIGHT";
    const string WAIT = "WAIT";
    const string STOP = "STOP";

    public InputManager im;
    public float CarSpeed = 2f;
    public float RotateSpeed = 2f;
    public float CriticalDistance = 1f;
    public float CriticalDistanceSide = 1.5f; // This var is experimental. Might not be needed in the future.
    public float CriticalDistanceWhenTurning = 1.25f; // This should be about 20-30% larger than CriticalDistance in order to eliminate laggy rotation.
    public string State; // stop/forwards/baackwards/turn_left/turn_right
    public bool IsApproachingLeft;
    public bool IsApproachingRight;

    private void Start()
    {
        State = MOVE_FORWARDS;
    }

    private void Update()
    {
        HandleStates();
        IsApproachingLeft = IsApproaching(im.LeftDistanceHistory);
        IsApproachingRight = IsApproaching(im.RightDistanceHistory);
    }

    void Waiting()
    {
        if (im.upDistance >= CriticalDistance && !IsApproachingRight && !IsApproachingLeft)
        {
            State = MOVE_FORWARDS;
        }
        else if (IsApproachingRight)
        {
            State = TURN_LEFT;
        }
        else if (IsApproachingLeft)
        {
            State = TURN_RIGHT;
        }
        else if (im.upDistance <= CriticalDistance)
        {
            State = (im.leftDistance <= im.rightDistance) ? TURN_RIGHT : TURN_LEFT;
        }
        // else{
        //     State = MOVE_BACKWARDS;
        // }
    }

    void MoveForwards()
    {
        if (
            im.upDistance <= CriticalDistance
           || IsApproachingRight
           || IsApproachingLeft
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
        if (im.rightDistance >= CriticalDistanceSide && im.upDistance >= CriticalDistanceWhenTurning)
        {
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + RotateSpeed);
    }

    void TurnRight()
    {
        if (im.leftDistance >= CriticalDistance && im.upDistance >= CriticalDistanceWhenTurning)
        {
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + -RotateSpeed);
    }

    void Stop()
    {

    }

    void HandleStates()
    {
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
                Stop();
                break;
        }
    }

    bool IsApproaching(float[] DistanceHistory)
    {
        bool IsApproaching = false;
        int SmallerDistanceCounter = 0;
        for (int i = 1; i < DistanceHistory.Length; i++)
        {
            if (DistanceHistory[i] <= CriticalDistanceSide && DistanceHistory[i] < DistanceHistory[i - 1])
            {
                SmallerDistanceCounter++;
            }
        }
        if (SmallerDistanceCounter >= DistanceHistory.Length / 2)
        {
            IsApproaching = true;
        }

        return IsApproaching;
    }
}
