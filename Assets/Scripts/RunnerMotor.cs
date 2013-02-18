using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

[Serializable]
public class Motor
{
    public RunnerState Type;
    public Vector3 acceleratingVelocity;
    public bool shouldDecelerate;
    public Vector3 deceleratingVelocity;
    public float maxVelocity;
    public ForceMode forceMode;
}

[Serializable]
public class RunnerMotor
{
    public Motor[] Motors;
    private Dictionary<RunnerState, Motor> _motorCache;

    public void Initialize()
    {
        _motorCache = new Dictionary<RunnerState, Motor>(Motors.Length);
        foreach (var motor in Motors)
        {
            _motorCache.Add(motor.Type, motor);
        }
    }

    public void Move(RunnerState runnerState, Rigidbody rigidbody)
    {
        if (_motorCache != null)
        {
            Motor motor;
            var found = _motorCache.TryGetValue(runnerState, out motor);
            if (found)
            {
                if (rigidbody.velocity.x < motor.maxVelocity)
                {
                    rigidbody.AddForce(motor.acceleratingVelocity, motor.forceMode);
                }
                else if (motor.shouldDecelerate && rigidbody.velocity.x > motor.maxVelocity)
                {
                    rigidbody.AddForce(-motor.deceleratingVelocity, motor.forceMode);
                }
            }
        }
        else
        {
            Debug.LogError("Motor is not initialized!");            
        }
    }
}