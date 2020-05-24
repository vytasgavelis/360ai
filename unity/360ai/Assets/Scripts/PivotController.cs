using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour
{
    // Chassis states
    const string MOVE_FORWARDS = "MOVE_FORWARDS";
    const string MOVE_BACKWARDS = "MOVE_BACKWARDS";
    const string TURN_LEFT = "TURN_LEFT";
    const string TURN_RIGHT = "TURN_RIGHT";
    const string WAIT = "WAIT";
    const string STOP = "STOP";
    const string PANIC = "PANIC";
    public string State;

    public InputManager im;
    public float CarSpeed = 2f;
    public float RotateSpeed = 2f;
    public float CriticalDistance = 1f;
    public float CriticalDistanceSide = 1.5f; // This var is experimental. Might not be needed in the future.
    public float CriticalDistanceWhenTurning = 1.25f; // This should be about 20-30% larger than CriticalDistance in order to eliminate laggy rotation.
    public float SecondsToPanic = 5f;
    public bool IsApproachingLeft;
    public bool IsApproachingRight;
    public float LastForwardCommand;
    public float LastBackwardsCommand;
    public float TimeToGoBackwards = 0.25f;

    private void Start()
    {
        State = MOVE_FORWARDS;
        LastForwardCommand = 0;
    }

    private void Update()
    {
        HandleStates();
    }

    void Waiting()
    {
        if (im.upDistance >= CriticalDistance &&
            im.leftDistance >= CriticalDistanceSide && 
            im.rightDistance >= CriticalDistanceSide
            )
        {
            State = MOVE_FORWARDS;
        }
    }

    void MoveForwards()
    { 
        if (im.upDistance <= CriticalDistance)
        {
            LastBackwardsCommand = Time.realtimeSinceStartup;
            State = MOVE_BACKWARDS;
        }
        transform.Translate(Vector2.up* Time.deltaTime* CarSpeed);
    }

    void MoveBackwards()
    {
        if (Time.realtimeSinceStartup - TimeToGoBackwards > LastBackwardsCommand)
        {
            State = TURN_LEFT;
        }
        transform.Translate(Vector2.down * Time.deltaTime * CarSpeed);
    }

    void TurnLeft()
    {
        if(im.leftDistance <= CriticalDistanceWhenTurning)
        {
            State = MOVE_FORWARDS;
        }
        transform.Rotate(0, 0, transform.rotation.z + RotateSpeed);
    }

    void TurnRight()
    {
        if (im.leftDistance >= CriticalDistance)
        {
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + -RotateSpeed);
    }

    void Stop()
    {

    }

    void Panic()
    {
        // Make vehicle play beeping sound.
        // Vehicle now shall be controlled by a remote controller.
        // Restore STATE to WAIT after user is done controlling the vehicle.
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
            case PANIC:
                Panic();
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
