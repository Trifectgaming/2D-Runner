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
            ((AutomaticRunnerInput)inputController).ActionQueue.Enqueue(_currentRecord.InputState);        
            SetCurrentRecord();
        }
        base.Update();
    }
}
