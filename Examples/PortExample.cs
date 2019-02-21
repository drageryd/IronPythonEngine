using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortExample : ComPort {

    Vector3 force;

    // Use this for initialization
    void Start () {
        force = Vector3.zero;

        InitCommands();
        AddCommand("test");
        AddCommand("test2");
    }
	
	// Update is called once per frame
	void Update () {
		if (force != Vector3.zero)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(force);
            force = Vector3.zero;
        }
	}

    //These will be run by another thread so we cant use all unity functionality
    public string test(float f)//string[] args)
    {
        //Queue in mainthread
        force += Vector3.up * f;
        return "This is a test function ";
    }

    public string test2(float f)
    {
        //Queue in mainthread
        force += Vector3.left * f;
        return "This is a test function ";
    }
}
