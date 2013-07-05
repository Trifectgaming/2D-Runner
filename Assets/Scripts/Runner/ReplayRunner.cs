using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ReplayRunner : AutomaticRunner
{
    private Queue<InputRecord> _inputQueue;
    private InputRecord _currentRecord;

    public void LoadInputQueue(List<InputRecord> replay)
    {
        _inputQueue = new Queue<InputRecord>(replay);
        SetCurrentRecord();
        Debug.Log(name + " loaded with " + _inputQueue.Count + " actions.");
    }

    private void SetCurrentRecord()
    {
        if (_inputQueue.Count > 0)
        {
            _currentRecord = _inputQueue.Dequeue();
        }
        else
        {
            _currentRecord = new InputRecord(Vector3.zero, InputState.None);
        }
    }

    protected override void Update()
    {
        if (_currentRecord.Position.x <= Transform.position.x)
        {
            Debug.Log(_currentRecord.Position.x + "is <= " + Transform.position.x);
            ((AutomaticRunnerInput)inputController).ActionQueue.Enqueue(_currentRecord.InputState);
            Debug.Log(name + " executed " + _currentRecord.InputState + " remaining actions " + _inputQueue.Count);            
            SetCurrentRecord();
        }
        base.Update();
    }
}
