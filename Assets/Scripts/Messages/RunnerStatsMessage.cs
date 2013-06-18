using UnityEngine;

public class RunnerStatsMessage
{
    public RunnerState CurrentState;
    public Vector3 Velocity;
    public CollisionInfo Contacts;
    public InputState CurrentInput;

    public RunnerStatsMessage(RunnerState currentState, Vector3 velocity, CollisionInfo contacts, InputState currentInput)
    {
        CurrentState = currentState;
        Velocity = velocity;
        Contacts = contacts;
        CurrentInput = currentInput;
    }
}
