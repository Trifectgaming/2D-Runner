using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour
{
    private HashSet<string> Colliders = new HashSet<string>(); 
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Colliders.Add(other.name);
        DetermineCollision();
    }

    private void DetermineCollision()
    {
        ColliderCount = Colliders.Count;
        Colliding = Colliders.Count > 0;
    }

    void OnTriggerStay(Collider other)
    {
        Colliders.Add(other.name);
        DetermineCollision();
    }

    private void OnTriggerExit(Collider other)
    {
        Colliders.RemoveWhere(t => t == other.name);
        DetermineCollision();
    }


    void OnCollisionEnter(Collision other)
    {
        Colliding = true;
        //Debug.Log(name + " collided with " + other.gameObject.name);
    }

    void OnCollisionStay(Collision other)
    {
        Colliding = true;
        //Debug.Log(name + " collided" + other.gameObject.name);
    }

    void OnCollisionExit(Collision other)
    {
        Colliding = false;
    }
    
    public bool Colliding;
    public int ColliderCount;
}
