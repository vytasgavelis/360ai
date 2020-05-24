using System.Collections;
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
    const string PANIC = "PANIC";
    const string TURN_LEFT_AFTER_ROTATION = "TURN_LEFT_AFTER_ROTATION";
    const string TURN_RIGHT_AFTER_ROTATION = "TURN_RIGHT_AFTER_ROTATION";
    public string State;

    public InputManager im;
    public float CarSpeed = 2f;
    public float RotateSpeed = 2f;
    public float CriticalDistance = 1f;
    public float CriticalDistanceSide = 1.5f; // This var is experimental. Might not be needed in the future.
    public float CriticalDistanceWhenTurning = 1.25f; // This should be about 20-30% larger than CriticalDistance in order to eliminate laggy rotation.
    public float SecondsToPanic = 5f;
    public float SecondsToTurnAfterRotation = 0.25f; // Since we cannot detect obstacles after rotating turn for another x amount of seconds just to be sure.
    public float LastTurnLeftCommand = 0f;
    public float LastTurnRightCommand = 0f;
    public bool IsApproachingLeft;
    public bool IsApproachingRight;
    public float LastForwardCommand;

    // Put this into your initialization function.
    private void Start()
    {
        State = MOVE_FORWARDS;
        LastForwardCommand = 0;
    }

    // Put this into your while loop.
    private void Update()
    {
        HandleStates();
        IsApproachingLeft = IsApproaching(im.LeftDistanceHistory);
        IsApproachingRight = IsApproaching(im.RightDistanceHistory);
        if(LastForwardCommand <= Time.realtimeSinceStartup - SecondsToPanic) // You should replace this with micros() function in arduino.
        {
            State = PANIC;
        }
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
        else
        {
            State = PANIC;
        }
    }

    void MoveForwards()
    {
        LastForwardCommand = Time.realtimeSinceStartup; // Replace with micros() function.
        if (
            im.upDistance <= CriticalDistance
           || IsApproachingRight
           || IsApproachingLeft
          )
        {
            State = WAIT;
        }
        transform.Translate(Vector2.up * Time.deltaTime * CarSpeed); // Move vehicle forwards
    }

    void MoveBackwards()
    {
        transform.Translate(Vector2.down * Time.deltaTime * CarSpeed); // Move vehicle backwards
    }

    void TurnLeft()
    {
        if (im.rightDistance >= CriticalDistanceSide && im.upDistance >= CriticalDistanceWhenTurning)
        {
            LastTurnLeftCommand = Time.realtimeSinceStartup;
            State = TURN_LEFT_AFTER_ROTATION;
        }
        transform.Rotate(0, 0, transform.rotation.z + RotateSpeed); // Turn vehicle left
    }

    void TurnRight()
    {
        if (im.leftDistance >= CriticalDistanceSide && im.upDistance >= CriticalDistanceWhenTurning)
        {
            LastTurnRightCommand = Time.realtimeSinceStartup;
            State = TURN_RIGHT_AFTER_ROTATION;
        }
        transform.Rotate(0, 0, transform.rotation.z + -RotateSpeed);
    }

    void TurnLeftAfterRotation(){
        if(
            LastTurnLeftCommand + SecondsToTurnAfterRotation < Time.realtimeSinceStartup
            || im.upDistance <= CriticalDistance
            || im.leftDistance <= CriticalDistance
            || im.rightDistance <= CriticalDistance
        ){
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + RotateSpeed);
    }

    void TurnRightAfterRotation(){
        if(
            LastTurnRightCommand + SecondsToTurnAfterRotation  < Time.realtimeSinceStartup
            || im.upDistance <= CriticalDistance
            || im.leftDistance <= CriticalDistance
            || im.rightDistance <= CriticalDistance
        ){
            State = WAIT;
        }
        transform.Rotate(0, 0, transform.rotation.z + -RotateSpeed); // Turn vehicle right
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
            case TURN_LEFT_AFTER_ROTATION:
                TurnLeftAfterRotation();
                break;
            case TURN_RIGHT_AFTER_ROTATION:
                TurnRightAfterRotation();
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
        // You can find distance history implementation in InputManager.cs line 70.
        // DistanceHistory array may be made longer if lack of sensor precision is creating any issues.
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
