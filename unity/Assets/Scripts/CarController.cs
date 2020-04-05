using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class CarController : MonoBehaviour
{
    public InputManager im;
    public RaycastManager rm;
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    public float strengthCoefficient = 90000f;
    public float steerCoefficientMultiplier = 1.5f;

    public float targetDistance;

    private void Start()
    {
        im = GetComponent<InputManager>();
    }

    void FixedUpdate()
    {
        leftWheel.motorTorque = strengthCoefficient * Time.deltaTime * im.throttle;
        rightWheel.motorTorque = strengthCoefficient * Time.deltaTime * im.throttle;

        if (im.steer != 0)
        {
            leftWheel.motorTorque = im.steer * steerCoefficientMultiplier * strengthCoefficient * Time.deltaTime;
            rightWheel.motorTorque = -im.steer * steerCoefficientMultiplier * strengthCoefficient * Time.deltaTime;
        }

    }

    private void Update()
    {
        Debug.Log(rm.leftDistance + " " + rm.rightDistance + " " + rm.forwardDistance + " " + rm.backwardsDistance);
    }
}
