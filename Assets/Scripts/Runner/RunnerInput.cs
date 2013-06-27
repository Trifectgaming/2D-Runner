using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[Serializable]
public abstract class RunnerInput
{
    public InputWrapper state;
    public InputState lastState;
    //public Queue<InputState> QueuedStates = new Queue<InputState>();
    //public Queue<InputWrapper> RequeuedStates = new Queue<InputWrapper>();
    public abstract void Update();

    public InputState Try()
    {
        state = state.Try();
        return state.input;
    }

    public void Complete()
    {
        state = InputState.None;
    }
}

public struct InputWrapper
{
    public static InputWrapper Empty = new InputWrapper();
    public int attempts;
    public InputState input;

    public InputWrapper(int attempts, InputState input)
    {
        this.attempts = attempts;
        this.input = input;
    }

    public InputWrapper Try()
    {
        if (--attempts > 0)
            return new InputWrapper(attempts, input);
        return Empty;
    }

    public static implicit operator InputWrapper(InputState value)
    {
        return new InputWrapper(value == InputState.None ? 0 : 6, value);
    }

    public override string ToString()
    {
        
        return "(" + input + ", " + attempts + ")";
    }
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
    public override void Update()
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
            //state = InputState.None;            
        }
        //if (state != InputState.None)
        {
            //lastState = state;
        }
        //QueuedStates.Enqueue(state);
        //return state;
    }
}
