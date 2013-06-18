using UnityEngine;
using System.Collections;

public class GameStatManager : MonoBehaviour
{
    public tk2dTextMesh Contacts;
    public tk2dTextMesh Input;
    public tk2dTextMesh Velocity;
    public tk2dTextMesh State;

	// Use this for initialization
	void Awake () {
	    Messenger.Default.Register<RunnerStatsMessage>(this, OnGameStatChanged);
	}

    private void OnGameStatChanged(RunnerStatsMessage obj)
    {
        Contacts.text = obj.Contacts.ToString();
        Input.text = obj.CurrentInput.ToString();
        Velocity.text = obj.Velocity.ToString();
        State.text = obj.CurrentState.ToString();
        Contacts.Commit();
        Input.Commit();
        Velocity.Commit();
        State.Commit();
    }
}
