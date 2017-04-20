using UnityEngine;

public class CarMovementScript : MonoBehaviour
{

    public float MotorForce;

    public WheelCollider FR;
    public WheelCollider FL;
    public WheelCollider RR;
    public WheelCollider RL;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float v = MotorForce;

        RL.motorTorque = v;
        RR.motorTorque = v;
    }
}
