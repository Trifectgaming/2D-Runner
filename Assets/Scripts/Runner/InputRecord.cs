using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class InputRecord {

    public InputRecord()
    {
        
    }

    public InputRecord(Vector3 position, InputState inputState)
    {
        Position = position;
        InputState = inputState;
    }

    public Vector3 Position { get; set; }
    public InputState InputState { get; set; }
}
