using System;
using System.Collections;
using UnityEngine;

public partial class ControlScript : MonoBehaviour
{
    #region var

    private float smoothSidewaysSlip = 0.0f;
    private float smoothForwardSlip = 0.0f;
    private float maxDownPressureForce = -1500.0f;
    private bool overrideForHandbrake = false;
    //User controls
    private float brake = 0.0f;
    private float handbrake = 0.0f;
    private float steer = 0.0f;
    public float motor = 0.0f;

    // Wheel geometry
    public Transform FL;
    public Transform FR;
    public Transform RL;
    public Transform RR;

    int currentGear = 0;
    public float engineRPM = 0.0f;
    float wheelRPM = 0.0f;

    int rev = 1;
    //suspension setup
    float suspensionDistance = 0.1f;
    float springs = 1000;
    float dampers = 50;
    float wheelRadius = 0.45f;
    float wheelMass = 15.0f;

    float wheelRadiusInFeet = 0.5833f; //feet
    float wheelRadiusInMeters = 0.1778f; //meters
    float wheelRadiusInInches = 7; //inches
    //center of gravity height - effects tilting in corners
    //float cogY = -1.5f;

    //engine powerband
    float minRPM = 700;
    float maxRPM = 6000;

    float minTorque = 400.0f;
    //maximum Engine Torque
    float maxTorque = 1000.0f;

    //automatic transmission shift points
    float shiftDownRPM = 2500;
    float shiftUpRPM = 5500;

    //gear ratios
    float[] gearRatios = { -2.3f, 2.3f, 1.78f, 1.30f, 1.00f };
    float finalDriveRatio = 3.4f;
    // input steer and input torque are the values substituted out for the player input. The 
    // "NavigateTowardsWaypoint" function determines values to use for these variables to move the car
    // in the desired direction.
    private float inputSteer = 0.0f;
    private float inputTorque = 0.0f;
    float inputMotor = 0.0f;
    float inputBrake = 0.0f;
    float inputHandbrake = 0.0f;

    bool enableControlToWaypoints = true;
    //---------------------------------------------------------------------------------------
    private Wheel[] wheels;
    Vector3 velo = Vector3.zero; //rigidbody velocity
    Vector3 flatVelo = new Vector3();
    float kmPerH = 0;
    private bool onGround;
    private GameObject enemyLocked;
    bool OverrideStart = true;
    //maximal corner and braking acceleration capabilities
    float maxCornerAccel = 5.0f;
    float maxBrakeAccel = 10.0f;

    private float cornerSlip = 0.0f;
    private float driveSlip = 0.0f;

    public bool waitCountDown = false;
    // Approximate motor torque curve with a simple parabola: 
    // torque goes from motorMinTorque at zero RPM to motorMaxTorque at 
    // motorRpmRange/2 RPM then back to motorMinTorque at motorRpmRange. 
    // This is way off the real motors, but works good for a simple car. 

    float motorRpmRange = 1500.0f;

    // Steering: wheels can turn up to lowSpeedSteerAngle when car is still; 
    // and up to highSpeedSteerAngle when car goes at highSpeed km/h. 
    // This is to decrease steering at high velocities so that playing with 
    // plain keyboard is possible. 
    float lowSpeedSteerAngle = 80.0f;
    float highSpeedSteerAngle = 30;
    float highSpeed = 70.0f;
    public float topSpeed = 250;
    // How much we move the mass center vertically. 
    float massCenterY = -0.2f;

    // How much velocity based down-pressure force we apply. 
    float downPressureFactor = 0.5f;

    // Brake and handbrake torques. 
    float brakeStrength = 100.0f;
    float handBrakeStrength = 400.0f;
    #endregion
    IEnumerator WaitRestore()
    {
        yield return new WaitForSeconds(0.5f);
    }

    #region Unity   
    void OnGUI()
    {
        if (userControl)
        {
            //GUIOverlayTexture
            GUI.depth = 1;
            GUI.skin = customSkin2;
            GUILayout.BeginArea(new Rect(60, Screen.height - (GUIOverlayTexture.height - 20), 200, 50));
            GUILayout.Label("	health : " + Mathf.RoundToInt(health.GetHealth()));
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(50, Screen.height - (GUIOverlayTexture.height - 85), 200, 100));
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + ammo.GetHomingAmount());
            GUILayout.Space(20);
            GUILayout.Label("" + ammo.GetSpeedAmount());
            GUILayout.Space(20);
            GUILayout.Label("" + ammo.GetPowerAmount());

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - 140, Screen.height - 60, 100, 30));
            GUILayout.Label("Gear : " + currentGear);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - 115, Screen.height - 100, 60, 100));
            GUILayout.Label("" + Mathf.RoundToInt(kmPerH / 2));
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - 75, Screen.height - 100, 60, 100));
            GUILayout.Label("km/h");
            GUILayout.EndArea();


            // Enemy Info ----------------------------------------------------------------------
            GUILayout.BeginArea(new Rect(enemySkull.width + 5, 0, 500, 150));
            GUILayout.BeginVertical();

            try
            {
                GUILayout.Label(enemyLocked.name);
                GUILayout.Label("Enemy health : " +
                                Mathf.RoundToInt(enemyLocked.GetComponent<HealthControlScript>().GetHealth()));

                switch (enemyLocked.GetComponent<ControlScript>().showState)
                {
                    case 1:
                        GUILayout.Label("Enemy state : Looking for trouble ...");
                        break;
                    case 2:
                        GUILayout.Label("Enemy state : He can run... but he can't hide...");
                        break;
                    case 3:
                        GUILayout.Label("Enemy state : Bleeding like a stuck pig");
                        break;
                    case 4:
                        GUILayout.Label("Enemy state : Got NO missiles !!!");
                        break;

                }
                GUILayout.Label("Remaining opponents : " + remainingOpp + "/7");
            }
            catch (Exception e)
            {
                ;
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
    void Start()
    {
        ammo = GetComponent<GunControlScript>();
        health = GetComponent<HealthControlScript>();

        muzzleFlash.enabled = false;
        //------------------------------------------------------------------------------
        GameObject supertemp = GameObject.Find("ObstacleDetectort");
        m_dDBox = supertemp.GetComponent<CapsuleCollider>();
        obstacleMap = GameObject.Find("ObstacleMap");
        GetObstacles();

        waypointContainer = GameObject.Find("Waypoints");
        // Call the function to determine the array of waypoints. This sets up the array of points by finding
        // transform components inside of a source container.
        GetWaypoints();

        healthMap = GameObject.Find("HealthMap");
        missileMap = GameObject.Find("MissileMap");

        GetHealthPacks();
        GetMissilePacks();
        //---------------------------------------------------------------------------------------------

        Transform bh = transform.Find("collider");
        bcollider = bh.GetComponent<BoxCollider>();

        //Wheel colliders creation 
        wheels = new Wheel[4];

        for (int i = 0; i < 4; i++)
        {
            wheels[i] = new Wheel();
        }

        //Assign geometry
        wheels[0].geometry = FL;
        wheels[1].geometry = FR;
        wheels[2].geometry = RL;
        wheels[3].geometry = RR;

        //
        wheels[0].maxSteerAngle = 30.0f;
        wheels[1].maxSteerAngle = 30.0f;
        wheels[0].powered = false;
        wheels[1].powered = false;
        wheels[2].powered = true;
        wheels[3].powered = true;
        wheels[2].handbraked = true;
        wheels[3].handbraked = true;

        foreach (Wheel w in wheels)
        {

            w.originalRotation = w.geometry.localRotation;

            GameObject colliderObject = new GameObject("WheelCollider");
            colliderObject.transform.parent = transform;
            colliderObject.transform.position = w.geometry.position;
            colliderObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

            w.coll = colliderObject.AddComponent<WheelCollider>();
            w.coll.suspensionDistance = suspensionDistance;
            w.coll.suspensionSpring = new JointSpring() { damper = dampers, spring = springs };
            w.coll.radius = wheelRadius;
            w.coll.mass = wheelMass;
            //--------------------
            w.coll.forwardFriction = new WheelFrictionCurve() { stiffness = 2.092f };
            w.coll.sidewaysFriction = new WheelFrictionCurve() { stiffness = 0.022f };
        }
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, -0.1f, 0.0f);
        currentGear = 1;
    }
    void FixedUpdate()
    {
        // calculate current speed in km/h 
        kmPerH = GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000;
        velo = GetComponent<Rigidbody>().velocity;

        if (waitCountDown || OverrideStart)
        {
            brake = 0;
            steer = 0;
            motor = 0;
        }
        else
        {
            if (userControl)
            {
                steer = Input.GetAxis("Horizontal");
                motor = Mathf.Clamp01(Input.GetAxis("Vertical"));
                brake = Mathf.Clamp01(-Input.GetAxis("Vertical"));
                handbrake = Input.GetButton("Jump") ? 1.0f : 0.0f;
            }
            else
            {
                steer = inputSteer;
                motor = inputMotor;
                brake = inputBrake;
                handbrake = inputHandbrake;
            }
        }
        // current wheels rpm 
        //float wheelsRpm = ComputeRpmFromWheels();
        // find maximum steer angle (dependent on car velocity) 
        float maxSteer = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, kmPerH / highSpeed);
        float wheelSteer = steer * maxSteer;

        if (userControl && handbrake > 0)
        {
            wheels[0].coll.steerAngle = 5 * steer;
            wheels[1].coll.steerAngle = 5 * steer;
        }
        else
        {
            wheels[0].coll.steerAngle = wheelSteer;
            wheels[1].coll.steerAngle = wheelSteer;
        }
        //
        ApplyDownPressure();
        // motor & brake 
        if (currentGear == 0)
        {
            if (motor > 0 && kmPerH < 5)
            {
                currentGear = 1;
            }

            float tmp = brake;
            brake = motor;
            motor = tmp;
        }
        else if (currentGear == 1)
        {
            if (brake > 0 && kmPerH < 5)
            {

                currentGear = 0;

                float tmp = brake;
                brake = motor;
                motor = tmp;
            }
        }

        float axisTorque = ComputeAxisTorque(Mathf.Abs(motor));

        if (motor > 0.0f)
        {
            if (overrideForHandbrake)
            {
                Invoke("RestoreFriction", 0.1f);
            }
            if (kmPerH > 350)
            {
                wheels[0].coll.motorTorque = 0;
                wheels[2].coll.motorTorque = 0;
                wheels[1].coll.motorTorque = 0;
                wheels[3].coll.motorTorque = 0;
            }
            else
            {
                wheels[0].coll.motorTorque = 1.5f * axisTorque * motor;
                wheels[2].coll.motorTorque = 1.5f * axisTorque * motor;
                wheels[1].coll.motorTorque = 1.5f * axisTorque * motor;

                wheels[3].coll.motorTorque = 1.5f * axisTorque * motor;
            }
        }

        if (brake > 0.0f || handbrake > 0.0f)
        {
            if (userControl && handbrake > 0.0f)
            {
                wheels[2].coll.motorTorque = 0;
                wheels[3].coll.motorTorque = 0;

                ApplyAdditionalSteeringForce(steer);
            }
            else
            {
                flatVelo = new Vector3(velo.x, 0, velo.z);

                float totalbrake = brake + handbrake * 0.5f;

                if (totalbrake > 1.0f)
                {
                    totalbrake = 1.0f;
                }

                Vector3 brakeForce = -flatVelo.normalized * totalbrake * GetComponent<Rigidbody>().mass * maxBrakeAccel;
                GetComponent<Rigidbody>().AddForceAtPosition(brakeForce, transform.position);

                wheels[0].coll.motorTorque = 0;
                wheels[1].coll.motorTorque = 0;
                wheels[2].coll.motorTorque = 0;
                wheels[3].coll.motorTorque = 0;
            }

        }

        //wheel GFX
        UpdateWheels();

        if (!userControl && !OverrideStart)
        {
            if (flagRaised)
            {
                NavigateTowardsWaypoint();

                Vector3 direction = transform.TransformDirection(Vector3.forward);
                RaycastHit hit;
                // Bit shift the index of the layer (8) to get a bit mask
                int layerMask = 1 << 10;
                // This would cast rays only against colliders in layer 8.
                // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                layerMask = ~layerMask;

                // Did we hit anything?
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    direction, out hit, 100, layerMask))
                {
                    if (hit.collider.tag == "carColliderTag")
                    {
                        ammo.EnableAutoFire();
                    }
                    else
                    {
                        ammo.DisableAutoFire();
                    }
                }
                else
                {
                    ammo.DisableAutoFire();
                }
            }
            else
            {
                FuzzyLogic();
            }
            ObstacleDetection();
        }

        /*
        if (userControl && !ignitionAudio.isPlaying)
        {
            if (playOnce)
            {
                engineAudio.pitch = Mathf.Abs(engineRPM / maxRPM) + 1;

                if (engineAudio.pitch > 2.0f)
                {
                    engineAudio.pitch = 2.0f;
                }
            }
            else
            {
                StartEngineSound();
            }
        }
        */

    }
    #endregion
    void UpdateWheels()
    {
        //calculate handbrake slip for traction gfx
        float handbrakeSlip = handbrake * GetComponent<Rigidbody>().velocity.magnitude * 0.1f;

        if (handbrakeSlip > 1)
            handbrakeSlip = 1;

        float totalSlip = 0.0f;
        onGround = false;

        foreach (Wheel w in wheels)
        {
            //rotate wheel
            w.rotation += wheelRPM / 60.0f * rev * 360.0f * Time.fixedDeltaTime;
            //Debug.Log("rpm4 "+wheelRpm);
            w.rotation = Mathf.Repeat(w.rotation, 360.0f);
            w.geometry.localRotation = Quaternion.Euler(w.rotation, w.maxSteerAngle * steer, 0.0f) * w.originalRotation;
            //Debug.Log(w.rotation);
            //check if wheel is on ground
            if (w.coll.isGrounded)
                onGround = true;

            float slip = 1; //cornerSlip + (w.powered?driveSlip:0.0f)+(w.handbraked?handbrakeSlip:0.0f);
            totalSlip += slip;

            WheelHit hit;
            WheelCollider c;
            c = w.coll;
            if (c.GetGroundHit(out hit))
            {
                //if the wheels touches the ground, adjust graphical wheel position to reflect springs
                w.geometry.localPosition.Set(
                    w.geometry.localPosition.x,
                    w.geometry.localPosition.y - (Vector3.Dot(w.geometry.position - hit.point, transform.up) - w.coll.radius),
                    w.geometry.localPosition.z);
            }
        }
    }
    void RestoreFriction()
    {
        wheels[0].coll.sidewaysFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[1].coll.sidewaysFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[2].coll.sidewaysFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[3].coll.sidewaysFriction = new WheelFrictionCurve() { extremumValue = (20000) };

        wheels[0].coll.forwardFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[1].coll.forwardFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[2].coll.forwardFriction = new WheelFrictionCurve() { extremumValue = (20000) };
        wheels[3].coll.forwardFriction = new WheelFrictionCurve() { extremumValue = (20000) };

    }
    void ApplyAdditionalSteeringForce(float addSteer)
    {
        Vector3 steerForce = new Vector3(0, 0, 0);
        Vector3 localPos = new Vector3(0, 0, 0);
        localPos = transform.InverseTransformPoint(GetComponent<Rigidbody>().position);

        steerForce.x = addSteer * 500;

        foreach (Wheel w in wheels)
        {
            if (w.coll.isGrounded)
            {
                onGround = true;
                w.coll.sidewaysFriction = new WheelFrictionCurve() { extremumValue = (1000) };
                w.coll.forwardFriction = new WheelFrictionCurve() { extremumValue = (1000) };
                overrideForHandbrake = true;
            }
        }
    }
    void ApplyDownPressure()
    {
        // apply down pressure to the car (only if any wheel is grounded)         
        float groundedCount = 0;
        float sidewaysSlip = 0.0f;
        float forwardSlip = 0.0f;

        foreach (var w in wheels)
        {
            if (w.coll.isGrounded)
            {
                ++groundedCount;
                WheelHit hit;
                WheelCollider wc = w.coll;
                wc.GetGroundHit(out hit);
                sidewaysSlip += hit.sidewaysSlip;
                forwardSlip += hit.forwardSlip;
                //temp = w.coll.radius;
            }
        }

        if (groundedCount > 0)
        {
            sidewaysSlip /= groundedCount;
            forwardSlip /= groundedCount;
        }

        float smoother = Time.deltaTime * 5.0f;
        smoothSidewaysSlip = Mathf.Lerp(smoothSidewaysSlip, sidewaysSlip, smoother);
        smoothForwardSlip = Mathf.Lerp(smoothForwardSlip, forwardSlip, smoother);

        if (groundedCount > 0)
        {
            Vector3 downPressure = new Vector3(0, 0, 0);
            downPressure.y = -Mathf.Pow(GetComponent<Rigidbody>().velocity.magnitude, 1.2f) * downPressureFactor;
            downPressure.y = Mathf.Max(downPressure.y, maxDownPressureForce);

            Vector3 localPos = new Vector3(0, 0, 0);
            localPos.x = smoothSidewaysSlip / 5.0f;
            localPos.x = -Mathf.Sign(localPos.x) * localPos.x * localPos.x;
            float hwidth = bcollider.size.x / 2.0f; // ------------------------------------------------------
            localPos.x = Mathf.Clamp(localPos.x, -hwidth, hwidth);
            Vector3 worldPos = GetComponent<Rigidbody>().position + GetComponent<Rigidbody>().rotation * localPos;
            downPressure = transform.TransformDirection(downPressure);

            GetComponent<Rigidbody>().AddForceAtPosition(downPressure, worldPos, ForceMode.Acceleration);
            GetComponent<Rigidbody>()
                .AddForceAtPosition(downPressure, new Vector3(worldPos.x + 10, worldPos.y, worldPos.z + 10),
                    ForceMode.Acceleration);
            GetComponent<Rigidbody>()
                .AddForceAtPosition(downPressure, new Vector3(worldPos.x - 10, worldPos.y, worldPos.z + 10),
                    ForceMode.Acceleration);
            GetComponent<Rigidbody>()
                .AddForceAtPosition(downPressure, new Vector3(worldPos.x + 10, worldPos.y, worldPos.z - 10),
                    ForceMode.Acceleration);
            GetComponent<Rigidbody>()
                .AddForceAtPosition(downPressure, new Vector3(worldPos.x - 10, worldPos.y, worldPos.z - 10),
                    ForceMode.Acceleration);
        }
    }
    // A simple parabola to approximate the shape of motor torque 
    // curve. In real cars you probably would do this with explicit 
    // table that maps rpm to torque. 
    float MotorTorqueCurve(float motorrpm)
    {
        float x = (motorrpm / motorRpmRange) * 2 - 1;
        float y = 0.5f * (-x * x + 2) + 500;

        return Mathf.Lerp(minTorque, maxTorque, y);
    }
    // Given acceleration pedal input, computes the torque that 
    // we want to apply to wheel axles. 
    float ComputeAxisTorque(float accelPedal)
    {
        // Compute current motor rpm, based on wheels rpm 
        wheelRPM = Mathf.Abs(ComputeRpmFromWheels());

        ComputeMotorRpm(wheelRPM);
        AutomaticTransmission();
        // Figure out motor torque based on motor rpm 
        //---------------------------------#####
        float motorTorque = accelPedal * MotorTorqueCurve(engineRPM);

        // Compute wheel axle torque from motor torque 
        return motorTorque / gearRatios[currentGear] / finalDriveRatio;
    }
    // Compute motor RPM from wheel RPM 
    void ComputeMotorRpm(float rpm)
    {
        if (currentGear != 0)
        {
            engineRPM = rpm * gearRatios[currentGear] * finalDriveRatio;
            //engineRPM = Mathf.Min( engineRPM, motorRpmRange ); 

        }
        else
        {
            engineRPM = -rpm * gearRatios[currentGear] * finalDriveRatio;
        }



    }
    // Compute average RPM of all motorized wheels 
    float ComputeRpmFromWheels()
    {
        float rpm = 0.0f;
        float count = 0;

        foreach (var w in wheels)
        {
            if (w.powered)
            {
                rpm += w.coll.rpm;
                ++count;
            }
        }

        if (count != 0)
        {
            rpm /= count;
        }

        return rpm;
    }

    //Automatically shift gears
    void AutomaticTransmission()
    {
        if (currentGear > 0)
        {
            if (engineRPM > shiftUpRPM && currentGear < gearRatios.Length - 1)
            {
                currentGear++;
                if (userControl)
                {
                    turboAudio.Play();
                    muzzleFlash.enabled = true;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = false;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = true;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = false;
                }
            }
            if (engineRPM < shiftDownRPM && currentGear > 1)
            {
                currentGear--;
                if (userControl)
                {
                    exAudio.Play();
                    muzzleFlash.enabled = true;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = false;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = true;
                    StartCoroutine(WaitAutoTransmission());
                    muzzleFlash.enabled = false;
                }
            }
        }
    }
    IEnumerator WaitAutoTransmission()
    {
        yield return new WaitForSeconds(0.1f);
    }


}