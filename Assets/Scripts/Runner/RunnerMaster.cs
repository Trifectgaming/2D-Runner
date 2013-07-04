using UnityEngine;
using System.Collections;

public class RunnerMaster : MonoBehaviour
{
    public float gameSpeed =1;
    public int FPS =30;

	// Use this for initialization
    void Awake()
    {

    }
    
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Time.timeScale = gameSpeed;
	    Application.targetFrameRate = FPS;
	}
}
