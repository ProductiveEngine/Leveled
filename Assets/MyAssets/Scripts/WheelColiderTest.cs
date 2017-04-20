using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColiderTest : MonoBehaviour {

    public WheelCollider coll;
    public Transform t;
    public float Rot;
    public float testm;
    // Use this for initialization
    void Start ()
    {
        coll = GetComponent<WheelCollider>();
        t = GetComponent<Transform>();

    }

    float rotationsPerMinute = 10;

	// Update is called once per frame
	void Update ()
	{
	    coll.motorTorque = testm;

        float motor = 1;

	    transform.Rotate((float) 6.0 * rotationsPerMinute * Time.deltaTime,0,0);

    }
}
