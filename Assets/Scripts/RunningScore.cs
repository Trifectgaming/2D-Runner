using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dTextMesh))]
public class RunningScore : MonoBehaviour
{
    public Vector3 offset;
    private tk2dTextMesh _mesh;
    private float _orthoSize;

    Transform __transform; // cache transform locally
    Transform _transform
    {
        get
        {
            if (__transform == null) __transform = transform;
            return __transform;
        }
    }

	// Use this for initialization
	void Start ()
	{
	    _mesh = GetComponent<tk2dTextMesh>();
        _orthoSize = Camera.mainCamera.orthographicSize;
	    UpdateTransform();
	}

    private void UpdateTransform()
    {
        var resolution = Camera.mainCamera.GetScreenWidth() / Camera.mainCamera.GetScreenHeight();
        _transform.localPosition = new Vector3(
            -resolution * _orthoSize + offset.x,
            _transform.localPosition.y + offset.y,
            _transform.localPosition.z + offset.z);
    }

    // Update is called once per frame
	void Update ()
	{
        UpdateTransform();
        _mesh.text = "Ran: " + Runner.DistanceTraveled.ToString("0000000");
	    _mesh.Commit();
	}
}
