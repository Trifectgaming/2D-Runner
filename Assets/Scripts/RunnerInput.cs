using System;
using UnityEngine;
using System.Collections;

[Serializable]
public abstract class RunnerInput
{
    public InputState state;
    public InputState lastState; 
    public abstract InputState Update();
}

public enum InputState
{
    None,
    SwipeLeft,
    SwipeRight,
    SwipeUp,
    SwipeDown
}

[Serializable]
public class KeyboardRunnerInput : RunnerInput
{
    public override InputState Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            state = InputState.SwipeUp;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            state = InputState.SwipeRight;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            state = InputState.SwipeLeft;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            state = InputState.SwipeDown;
        }
        else
        {
            state = InputState.None;            
        }
        if (state != InputState.None)
        {
            Debug.Log("Pressed " + state);
            lastState = state;
        }
        return state;
    }
}
