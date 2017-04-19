using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColiderTest : MonoBehaviour {

    public WheelCollider coll;

    public float testm;
    // Use this for initialization
    void Start ()
    {
        coll = GetComponent<WheelCollider>();

    }
	
	// Update is called once per frame
	void Update ()
	{
	    coll.motorTorque = testm;

        float motor = Mathf.Clamp01(Input.GetAxis("Vertical"));
    }
}
