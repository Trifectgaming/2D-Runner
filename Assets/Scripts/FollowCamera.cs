using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public GameObject chasee;
    public Vector3 offset;
    private Transform _myTransform;
    private Transform _chaseeTransform;

    // Use this for initialization
	void Start ()
	{
	    _myTransform = gameObject.transform;
	    _chaseeTransform = chasee.transform;
	}
	
	// Update is called once per frame
	void Update () {
        _myTransform.position = new Vector3(_chaseeTransform.position.x + offset.x, _chaseeTransform.position.y + offset.y, _myTransform.position.z + offset.z);
	}
}
