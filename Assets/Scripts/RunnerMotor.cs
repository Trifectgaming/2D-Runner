using System;
using System.Linq;
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

    public void Move(RunnerState runnerState, Rigidbody rigidbody)
    {
        if (Motors != null)
        {
            var motor = Motors.FirstOrDefault(m => m.Type == runnerState);
            if (motor != null)
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
    }
}

[Serializable]
public class StateTransition
{
    public RunnerState currentState;
    public InputState command;
    public bool below;
    public bool above;
    public bool front;
    public bool behind;
    public float minSpeedX = float.NegativeInfinity;
    public float minSpeedY = float.NegativeInfinity;
    public float maxSpeedX = float.PositiveInfinity;
    public float maxSpeedY = float.PositiveInfinity;
}

[Serializable]
public class StateTranitionTo
{
    public StateTransition currentState;
    public RunnerState nextState;
}

[Serializable]
public class RunnerStateMachine
{
    public RunnerState currentState = RunnerState.Running;
    public StateTranitionTo[] transitions;

    public RunnerState Transition(InputState action, bool touchingPlatform, Vector3 currentVelocity)
    {
        if (transitions != null)
        {
            var allowedTransition = transitions.FirstOrDefault(t =>
                t.currentState.currentState == currentState &&
                t.currentState.command == action &&
                t.currentState.below == touchingPlatform &&
                (currentVelocity.x > t.currentState.minSpeedX) && (t.currentState.maxSpeedX > currentVelocity.x)
                );
            if (allowedTransition != null)
            {
                currentState = allowedTransition.nextState;
            }
        }
        
        return currentState;
    }
}

public enum RunnerState
{
    Walking,
    Running,
    Jumping,
    Jumped,
    Diving,
    Dived,
    Backflipping,
    Backflipped,
    Sliding,
    Slid,
    Falling
}
