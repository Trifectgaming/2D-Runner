using UnityEngine;
using System.Collections;

public class GuidedRunner : AutomaticRunner
{
    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.name);
        base.OnTriggerEnter(other);
        if (other.gameObject.tag == "Action")
        {
            ((AutomaticRunnerInput)inputController).ActionQueue.Enqueue(other.GetComponent<ActionBlock>().InputState);
        }
    }
}