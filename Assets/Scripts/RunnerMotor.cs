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
    //[NonSerialized]
    public Motor[] Motors;
    private Dictionary<RunnerState, Motor> _motorCache;

    public void Initialize()
    {
        _motorCache = new Dictionary<RunnerState, Motor>
                          {
                              {
                                  RunnerState.Running, new Motor
                                                           {
                                                               Type = RunnerState.Running,
                                                               acceleratingVelocity = new Vector3(15, 0, 0),
                                                               deceleratingVelocity = new Vector3(100, 0, 0),
                                                               maxVelocity = 10,
                                                               forceMode = ForceMode.Force,
                                                               shouldDecelerate = true,
                                                           }
                              },
                              {
                                  RunnerState.Walking, new Motor
                                                           {
                                                               Type = RunnerState.Walking,
                                                               acceleratingVelocity = new Vector3(15, 0, 0),
                                                               deceleratingVelocity = new Vector3(100, 0, 0),
                                                               maxVelocity = 10,
                                                               forceMode = ForceMode.Force,
                                                               shouldDecelerate = true,
                                                           }
                              },
                              {
                                  RunnerState.Jumping, new Motor
                                                           {
                                                               Type = RunnerState.Jumping,
                                                               acceleratingVelocity = new Vector3(1, 2, 0),
                                                               maxVelocity = 200,
                                                               forceMode = ForceMode.VelocityChange,
                                                               shouldDecelerate = false,
                                                           }
                              },
                              {
                                  RunnerState.Dash, new Motor
                                                        {
                                                            Type = RunnerState.Dash,
                                                            acceleratingVelocity = new Vector3(1.5f, 0, 0),
                                                            maxVelocity = 200,
                                                            forceMode = ForceMode.VelocityChange,
                                                            shouldDecelerate = false,
                                                        }
                              },
                              {
                                  RunnerState.GroundDash, new Motor
                                                              {
                                                                  Type = RunnerState.GroundDash,
                                                                  acceleratingVelocity = new Vector3(20f, 0, 0),
                                                                  maxVelocity = 200,
                                                                  forceMode = ForceMode.VelocityChange,
                                                                  shouldDecelerate = false,
                                                              }
                              },
                              {
                                  RunnerState.Sliding, new Motor
                                                           {
                                                               Type = RunnerState.Sliding,
                                                               acceleratingVelocity = new Vector3(1, 0, 0),
                                                               maxVelocity = 200,
                                                               forceMode = ForceMode.VelocityChange,
                                                               shouldDecelerate = false,
                                                           }
                              },
                              {
                                  RunnerState.Dropping, new Motor
                                                            {
                                                                Type = RunnerState.Dropping,
                                                                acceleratingVelocity = new Vector3(0, -10, 0),
                                                                maxVelocity = 200,
                                                                forceMode = ForceMode.VelocityChange,
                                                                shouldDecelerate = false,
                                                            }
                              },
                          };

        Motors = _motorCache.Values.ToArray();
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