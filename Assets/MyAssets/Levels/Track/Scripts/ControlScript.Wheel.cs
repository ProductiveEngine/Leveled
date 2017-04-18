using System.Collections.Generic;
using UnityEngine;

public partial class ControlScript
{
    #region var
    private bool playOnce = false;
    bool receivingFire = false;
    bool flagRaised = false;
    private int captureFlagBonus = 0;

    private BoxCollider bcollider;

    public bool userControl = true;
    private int remainingOpp = 7;

    bool activateStuck = false;
    bool activateStuck2 = true;

    bool checkAllEnemies = false;
    #endregion
    #region Utils

    void ResetFlag()
    {
        flagRaised = false;
    }
    void Unstuck()
    {
        if (Mathf.Floor(GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000) < 1)
        {
            transform.Rotate(Vector3.up * 180);
        }
        activateStuck = false;
        activateStuck2 = true;

    }
    void Restore()
    {
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.transform.position = new Vector3(transform.position.x, 15f);
        GetComponent<Rigidbody>().freezeRotation = true;
        StartCoroutine(WaitRestore());
        GetComponent<Rigidbody>().freezeRotation = false;
    }
    public void EnableUser()
    {
        OverrideStart = false;
        userControl = true;


        engineAudio = gameObject.AddComponent<AudioSource>();
        /*engineAudio.loop = true;
        engineAudio.playOnAwake = false;
        engineAudio.clip = engineSound;
        engineAudio.volume = volume;
        */

        turboAudio = gameObject.AddComponent<AudioSource>();
        /*turboAudio.loop = false;
        turboAudio.playOnAwake = false;
        turboAudio.clip = turbo;
        turboAudio.volume = volume;
        */

        ignitionAudio = gameObject.AddComponent<AudioSource>();
        /*ignitionAudio.loop = false;
        ignitionAudio.playOnAwake = true;
        ignitionAudio.clip = ignitionSound;
        ignitionAudio.volume = volume;
        */

        exAudio = gameObject.AddComponent<AudioSource>();
        /*exAudio.loop = false;
        exAudio.playOnAwake = false;
        exAudio.clip = exSound;
        exAudio.volume = volume;
        */


        Camera.main.SendMessage("SetTarget", transform);
        Camera.main.SendMessage("setUpGamePlayGui", transform);

        GameObject go;
        go = GameObject.FindGameObjectWithTag("radarTag");
        go.GetComponent<Radar>().centerObject = transform;
        go.SendMessage("SetTarget", transform);
        go.SendMessage("OpenRadar", 1);
    }
    public void DisableUser()
    {
        userControl = false;
        Invoke("setOverrideStart", 4);
    }
    void setOverrideStart()
    {
        OverrideStart = false;
    }
    #endregion    

    void populateWaypoints(List<Transform> path)
    {
        waypoints.Clear();

        waypoints = new List<Transform>() { path[0] };
        currentWaypoint = 0;
        flagRaised = true;
    }
    void GetObstacles()
    {
        Transform[] potentialObstacles = obstacleMap.GetComponentsInChildren<Transform>();

        obstacles = new List<Transform>();
        obstacleColliders = new List<SphereCollider>();

        foreach (Transform potentialObstacle in potentialObstacles)
        {
            if (potentialObstacle != obstacleMap.transform)
            {
                obstacles.Add(potentialObstacle);
            }
        }
        SphereCollider[] potentialObstacleColliders = obstacleMap.GetComponentsInChildren<SphereCollider>();

        foreach (SphereCollider potentialObstacleCollider in potentialObstacleColliders)
        {
            if (potentialObstacleCollider != obstacleMap.GetComponent<SphereCollider>())
            {
                obstacleColliders.Add(potentialObstacleCollider);
            }
        }
    }
    #region Navigation
    void ObstacleDetection()
    {
        if (activateStuck && Mathf.Floor(GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000) < 1 && activateStuck2)
        {
            activateStuck2 = false;
            Invoke("Unstuck", 2);
        }

        if (probeCheck)
        {
            //Closest Intersecting Obstacle
            SphereCollider closestIntersectingObject = null;
            //this will be used to track the distance to the CIB
            float distToClosestIP = Mathf.Infinity; //maxDouble;
            //this will record the transformed local coordinates of the CIB
            Vector3 localPosOfClosestObstacle = new Vector3(0, 0, 0);

            m_dDBox.height = 25;
            m_dDBox.center = new Vector3(m_dDBox.center.x, m_dDBox.center.y, 10.59f);

            int i = 0;
            currentObstacle = 0;

            for (i = 0; i < obstacles.Count; i++)
            {
                Vector3 relativeObstaclePosition =
                    transform.InverseTransformPoint(new Vector3(obstacles[currentObstacle].position.x,
                        transform.position.y, obstacles[currentObstacle].position.z));

                if (relativeObstaclePosition.z >= 0)
                {
                    //if the distance from the z axis to the object's position is less than it's radius 
                    // + half the width of the detection box then there is a potential intersection
                    float expandedRadius = m_dDBox.radius +
                                           obstacleColliders[currentObstacle].radius + 20;

                    //Debug.Log(" realtive : "+Mathf.Abs(relativeObstaclePosition.x)+"    expandedRadius : "+expandedRadius);
                    if (Mathf.Abs(relativeObstaclePosition.x) < expandedRadius)
                    {
                        //now to do a line/circle intersection test. The center of the circle
                        //is represented by (cZ,cX).The intersection points are given by the formula 
                        // z = cZ +/-sqrt(r^2-cX^2) for x=0.
                        //We only need to look at the smallest positive value of z because that will be the closest
                        //point of intersection.

                        float cZ = relativeObstaclePosition.z;
                        float cX = relativeObstaclePosition.x;

                        //we only need to calculate the sqrt part of the above equation once
                        float sqrtPart = Mathf.Sqrt(expandedRadius * expandedRadius - cX * cX);

                        float ip = cZ - sqrtPart;

                        if (ip <= 0)
                        {
                            ip = cZ + sqrtPart;
                        }
                        //test to see if this is the closest so far.If it is, keep a 
                        //record of the obstacle and its local coordinates
                        if (ip < distToClosestIP)
                        {
                            distToClosestIP = ip;

                            closestIntersectingObject = obstacleColliders[currentObstacle];
                            localPosOfClosestObstacle = relativeObstaclePosition;

                            //Debug.Log("CIB "+Vector3( 	obstacles[currentObstacle].position.x, transform.position.y, obstacles[currentObstacle].position.z ));
                        }
                    }
                }
                currentObstacle++;
            }

            //if we have found an intersecting obstacle, calculate a steering 
            //force away from it
            Vector3 steeringForce;

            //---------------------------------------------------------------------------------------------------------------------------------

            if (closestIntersectingObject != null)
            {

                //the closer the agent is to an object, the stronger the steering force
                //should be
                float multiplier = 1.0f + (m_dDBox.height - localPosOfClosestObstacle.z) / m_dDBox.height;

                //calculate the lateral force
                steeringForce.x = (closestIntersectingObject.radius - localPosOfClosestObstacle.x) * multiplier * 100;

                //inputSteer = steeringForce.x;//localPosOfClosestObstacle.x/localPosOfClosestObstacle.magnitude ;
                //apply a braking force proportional to the obstacle's distance from
                //the vehicle
                float brakingWeight = 100;

                Vector3 gmt = transform.position;
                gmt.z = gmt.z + 5;
                steeringForce.z = (closestIntersectingObject.radius - localPosOfClosestObstacle.z) * brakingWeight;

                inputHandbrake = 0;
                inputSteer = Mathf.Clamp(steeringForce.x, -1.0f, 1.0f); //Mathf.Sign(steeringForce.x);//
                inputBrake = 0;
                inputMotor = 0.5f;

                return;
            }
        }
        return;

    }
    void NavigateTowardsWaypoint()
    {
        // now we just find the relative position of the waypoint from the car transform,
        // that way we can determine how far to the left and right the waypoint is.
        //Debug.Log("w "+currentWaypoint);

        if (currentWaypoint < 0 || currentWaypoint >= waypoints.Count)
        {
            Debug.Log("flag status : " + flagRaised + "   " + currentWaypoint);
        }
        Vector3 RelativeWaypointPosition =
            transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y,
                waypoints[currentWaypoint].position.y));

        // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
        if (enableControlToWaypoints)
        {
            inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

            // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...

            if (Mathf.Abs(inputSteer) < 0.1f) //  || rigidbody.velocity.magnitude * 3600/1000 <30 ) 
            {
                //inputTorque = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude ;
                inputHandbrake = 0;
                inputMotor = 1;
                inputBrake = 0;
                //Debug.Log("test");
            }
            else if (GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000 < 30)
            {
                inputHandbrake = 0;
                inputBrake = 0;
                inputMotor = 1;
            }
            else
            {
                inputHandbrake = 1;
                inputBrake = 0;
                inputMotor = 0;
                //Debug.Log("test2");
            }
        }

        // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
        // next in the list.
        if (RelativeWaypointPosition.magnitude < 20)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Count - 1)
            {
                currentWaypoint = waypoints.Count - 1;
            }
        }
    }
    void NavigateTowardsSpecificWaypoint(Vector3 target)
    {
        if (enableControlToWaypoints)
        {
            Vector3 RelativeWaypointPosition =
                transform.InverseTransformPoint(new Vector3(target.x, transform.position.y, target.z));
            // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
            inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
            // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...

            if (Mathf.Abs(inputSteer) < 0.6f) //  || rigidbody.velocity.magnitude * 3600/1000 <30 ) 
            {
                //inputTorque = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude ;
                inputHandbrake = 0;
                inputMotor = 1;
                inputBrake = 0;
            }
            else if (GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000 < 30)
            {
                inputHandbrake = 0;
                inputBrake = 0;
                inputMotor = 1;
            }
            else
            {
                inputHandbrake = 1;
                inputBrake = 0;
                inputMotor = 0;
            }
        }
    }
    #endregion

    // Destroy everything that enters the trigger
    void OnTriggerEnter(Collider other)
    {

        //Debug.Log("enter collider");

        if (other.tag == "obstacleTag" || other.tag == "wallTag")
        {
            probeCheck = true;
            enableControlToWaypoints = false;
        }
        if (other.tag == "flagTag")
        {
            captureFlagBonus++;
        }

    }
    void OnTriggerExit(Collider other)
    {

        //Debug.Log("exit trigger");

        if (other.tag == "obstacleTag" || other.tag == "wallTag")
        {
            Invoke("waiter", 1);
        }


    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "obstacleTag" || other.tag == "wallTag")
        {
            probeCheck = true;
            enableControlToWaypoints = false;
        }
    }
    void waiter()
    {
        probeCheck = false;
        enableControlToWaypoints = true;
        activateStuck = false;
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "homingMissileTag" || collision.gameObject.tag == "speedMissileTag" ||
            collision.gameObject.tag == "powerMissileTag" || collision.gameObject.tag == "machineGunTag")
        {
            //receivingFire = true;
        }
        if ((collision.gameObject.tag == "wallTag" || collision.gameObject.tag == "obstacleTag") && !activateStuck)
        {
            activateStuck = true;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "homingMissileTag" || collision.gameObject.tag == "speedMissileTag" ||
            collision.gameObject.tag == "powerMissileTag" || collision.gameObject.tag == "machineGunTag")
        {
            //receivingFire = true;
        }
        if ((collision.gameObject.tag == "wallTag" || collision.gameObject.tag == "obstacleTag") && !activateStuck)
        {
            activateStuck = true;
        }

    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "homingMissileTag" || collision.gameObject.tag == "speedMissileTag" ||
            collision.gameObject.tag == "powerMissileTag" || collision.gameObject.tag == "machineGunTag")
        {
            //yield return new WaitForSeconds(3);
            //receivingFire = false;
        }

    }

    private GameObject obstacleMap;
    private List<Transform> obstacles;
    private List<SphereCollider> obstacleColliders;
    private int currentObstacle = 0;

    private CapsuleCollider m_dDBox;
    private Transform criticalDistance;
    private bool probeCheck = false;

    GunControlScript guns;

    // Here's all the variables for the AI, the waypoints are determined in the "GetWaypoints" function.
    // the waypoint container is used to search for all the waypoints in the scene, and the current
    // waypoint is used to determine which waypoint in the array the car is aiming for.
    private GameObject waypointContainer;
    private List<Transform> waypoints;
    private int currentWaypoint = 0;

    int showState = 0;
    private GameObject enemy;
    #region Sound
    void StartEngineSound()
    {
        engineAudio.Play();

        float timer = 0;
        float fadeTime = 2;
        playOnce = true;
    }

    public AudioClip engineSound;
    public AudioClip ignitionSound;
    public AudioClip turbo;
    public AudioClip exSound;
    static float volume = 1;

    private AudioSource engineAudio;
    private AudioSource turboAudio;
    private AudioSource ignitionAudio;
    private AudioSource exAudio;

    static void SetVolume(float v)
    {
        volume = v;
        //engineAudio.volume = volume;
        //turboAudio.volume = volume;
        //ignitionAudio.volume = volume;
    }
    #endregion
    #region GUI       
    //-- GUI related variables    
    public GUISkin customSkin2;

    public Texture GUIOverlayTexture;
    public Texture enemySkull;
    public Renderer muzzleFlash;
    #endregion
    #region HealthAndMissiles
    private GameObject healthMap;
    private List<Transform> healthPacks;
    private int closestHealthPack = 0;
    private GameObject missileMap;
    private List<Transform> missilePacks;
    private int closestMissilePack = 0;

    GunControlScript ammo;
    HealthControlScript health;

    #endregion

    //states
    int chase = 1;
    int evade = 2;
    int loadHealth = 3;
    int loadWeapons = 4;

    private int state = 1;

    private class Wheel
    {
        public bool powered = false;
        public Transform geometry;
        public WheelCollider coll;
        public Quaternion originalRotation;

        public float rotation = 0.0f;
        public float maxSteerAngle = 0.0f;
        public float lastSkidMark = -1;
        public bool handbraked = false;
    }

    void Update()
    {
        if (userControl)
        {
            enemyLocked = FindClosestEnemy();
            ammo.SetEnemyLocked(enemyLocked);

            if (Input.GetButtonDown("Restore"))
            {
                Restore();
            }
            /*
            GetComponent<AudioSource>().pitch = Mathf.Abs(engineRPM / maxRPM) + 1;

            if (GetComponent<AudioSource>().pitch > 2.0f)
            {
                GetComponent<AudioSource>().pitch = 2.0f;
            }
            */
        }
    }
    void GetHealthPacks()
    {
        Transform[] potentialHealthPacks = healthMap.GetComponentsInChildren<Transform>();
        healthPacks = new List<Transform>();

        foreach (Transform potentialHealthPack in potentialHealthPacks)
        {
            if (potentialHealthPack != healthMap.transform)
            {
                healthPacks.Add(potentialHealthPack);
            }
        }

    }
    void GetMissilePacks()
    {
        Transform[] potentialMissilePacks = missileMap.GetComponentsInChildren<Transform>();
        missilePacks = new List<Transform>();

        foreach (Transform potentialMissilePack in potentialMissilePacks)
        {
            if (potentialMissilePack != missileMap.transform)
            {
                missilePacks.Add(potentialMissilePack);
            }
        }

    }
    void GetWaypoints()
    {
        // Now, this function basically takes the container object for the waypoints, then finds all of the transforms in it,
        // once it has the transforms, it checks to make sure it's not the container, and adds them to the array of waypoints.
        Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();
        waypoints = new List<Transform>();

        foreach (Transform potentialWaypoint in potentialWaypoints)
        {
            if (potentialWaypoint != waypointContainer.transform)
            {
                waypoints.Add(potentialWaypoint);
            }
        }
    }
    // Find the name of the closest enemy
    GameObject FindClosestEnemy()
    {
        // Find all game objects with tag Enemy
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("carTag");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        remainingOpp = gos.Length - 1;

        if (userControl && gos.Length == 8)
        {
            checkAllEnemies = true;

        }
        if (gos.Length == 1 && checkAllEnemies && userControl)
        {
            float finalScore = health.pointsKeeper + 100 * health.fatalityBonus + health.GetHealth() +
                               200 * captureFlagBonus;
            OverrideStart = true;
            userControl = false;
            Camera.main.SendMessage("PlayerWon", 5);
        }

        // Iterate through them and find the closest one
        foreach (GameObject go in gos)
        {
            if (go.transform.position != transform.position && !go.transform.IsChildOf(transform))
            {
                Vector3 diff = (go.transform.position - position);
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }

        return closest;
    }
    Vector3 FindClosestHealthPack()
    {
        GetHealthPacks();

        float threshold = Mathf.Infinity;

        int i = -1;
        bool confirm = false;

        for (i = 0; i < healthPacks.Count; i++)
        {
            Vector3 RelativeHealthPackPosition =
                transform.InverseTransformPoint(new Vector3(healthPacks[i].position.x, transform.position.y,
                    healthPacks[i].position.z));

            float temp;
            temp = Mathf.Sqrt(Mathf.Pow(RelativeHealthPackPosition.x, 2) + Mathf.Pow(RelativeHealthPackPosition.z, 2));

            if (temp < threshold)
            {
                threshold = temp;
                closestHealthPack = i;
                confirm = true;
            }

        }

        if (closestHealthPack >= 0 && confirm)
        {
            confirm = false;
            //Debug.Log("closestHealthPack : "+healthPacks[closestHealthPack].position);
            return healthPacks[closestHealthPack].position;
        }
        else
        {
            confirm = false;
            return new Vector3(-1, -1, -1);
        }

    }
    Vector3 FindClosestMissilePack()
    {
        GetMissilePacks();

        float threshold = Mathf.Infinity;

        int i = -1;
        bool confirm = false;

        for (i = 0; i < missilePacks.Count; i++)
        {
            Vector3 RelativeMissilePackPosition =
                transform.InverseTransformPoint(new Vector3(missilePacks[i].position.x, transform.position.y,
                    missilePacks[i].position.z));

            float temp;
            temp = Mathf.Sqrt(Mathf.Pow(RelativeMissilePackPosition.x, 2) + Mathf.Pow(RelativeMissilePackPosition.z, 2));

            if (temp < threshold)
            {
                threshold = temp;
                closestMissilePack = i;
                confirm = true;
            }


        }

        if (closestMissilePack >= 0 && confirm)
        {
            confirm = false;
            return new Vector3(missilePacks[closestMissilePack].position.x, missilePacks[closestMissilePack].position.y,
                missilePacks[closestMissilePack].position.z);
        }
        else
        {
            confirm = false;
            return new Vector3(-1, -1, -1);
        }
    }
    #region FUZZY LOGIC
    void FuzzyLogic()
    {
        float tempHealth = health.GetHealth();
        int tempAmmo = ammo.GetSpeedAmount() + ammo.GetHomingAmount() + ammo.GetPowerAmount();

        GameObject tempEnemy = FindClosestEnemy();
        if (tempEnemy != null)
        {
            enemy = tempEnemy;
        }

        if (enemy == null)
        {
            return;
        }
        ammo.SetEnemyLocked(enemy);

        GunControlScript enemyMissileScript = enemy.GetComponent<GunControlScript>();
        HealthControlScript enemyHealthScript = enemy.GetComponent<HealthControlScript>();

        float enemyHealth = enemyHealthScript.GetHealth();

        int enemyAmmo = enemyMissileScript.GetSpeedAmount() + enemyMissileScript.GetHomingAmount();
        Vector3 enemyTransformPosition = enemy.transform.position;

        Vector3 closestHealthPack = FindClosestHealthPack();
        Vector3 closestMissilePack = FindClosestMissilePack();

        float healthPriority = CalcHealthPriority(tempHealth, closestHealthPack);
        float missilePriority = CalcMissilePriority(tempAmmo, closestMissilePack);
        float chasePriority = CalcChasePriority(enemyHealth, tempAmmo, enemyTransformPosition);
        float evadePriority = CalcEvadePriority(tempHealth, enemyAmmo, enemyTransformPosition);

        showState = MaximumPriority(healthPriority, missilePriority, chasePriority, evadePriority);
        switch (showState)
        {
            case 1:
                ChaseFL(enemy);
                break;
            case 2:
                EvadeFL(enemy);
                break;
            case 3:
                LoadHealthFL(closestHealthPack);
                break;
            case 4:
                LoadWeaponsFL(closestMissilePack);
                break;

            default:
                break;


        }
    }
    int MaximumPriority(float healthPriority, float missilePriority, float chasePriority, float evadePriority)
    {
        float temp = Mathf.Max(Mathf.Max(Mathf.Max(healthPriority, missilePriority), chasePriority), evadePriority);

        if (temp == chasePriority)
        {
            return 1;
        }
        else if (temp == evadePriority)
        {
            return 2;
        }
        else if (temp == healthPriority)
        {
            return 3;
        }
        else if (temp == missilePriority)
        {
            return 4;
        }

        return -1;
    }
    //---------------------------------------------------------------------------------------------
    void ChaseFL(GameObject enemy)
    {

        NavigateTowardsSpecificWaypoint(new Vector3(enemy.transform.position.x + 10, enemy.transform.position.y,
            enemy.transform.position.z + 10));

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        RaycastHit hit = new RaycastHit();

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 10;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        // Did we hit anything?
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), direction,
            out hit, 100, layerMask))
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
    void EvadeFL(GameObject enemy)
    {
        ammo.DisableAutoFire();
        Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(enemy.transform.position);
        // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
        inputSteer = -RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

        inputHandbrake = 0;
        inputMotor = 1;
        inputBrake = 0;
    }
    void LoadHealthFL(Vector3 closestHealthPack)
    {
        ammo.DisableAutoFire();
        NavigateTowardsSpecificWaypoint(closestHealthPack);
    }
    void LoadWeaponsFL(Vector3 closestMissilePack)
    {
        ammo.DisableAutoFire();
        NavigateTowardsSpecificWaypoint(closestMissilePack);
    }
    //---------- Calculate Health Priority-----------------------------------------------    
    float CalcHealthPriority(float tempHealth, Vector3 closestHealthPack)
    {
        float lowHealth = FuzzyReverseGrade(tempHealth, 15, 30);
        float mediumHealth = FuzzyTrapezoid(tempHealth, 15, 30, 60, 75);
        float goodHealth = FuzzyGrade(tempHealth, 60, 75);

        float distance = Vector3.Distance(closestHealthPack, transform.position);

        float closeDistance = FuzzyReverseGrade(distance, 25, 50);
        float mediumDistance = FuzzyTriangle(distance, 25, 50, 75);
        float farDistance = FuzzyGrade(distance, 50, 75);

        float rule1 = FuzzyAND(lowHealth, closeDistance); //critical
        float rule2 = FuzzyAND(lowHealth, mediumDistance); //high
        float rule3 = FuzzyAND(lowHealth, farDistance); //high
        float rule4 = FuzzyAND(mediumHealth, closeDistance); //normal
        float rule5 = FuzzyAND(mediumHealth, mediumDistance); //low
        float rule6 = FuzzyAND(mediumHealth, farDistance); //low
        float rule7 = FuzzyAND(goodHealth, closeDistance); //low
        float rule8 = FuzzyAND(goodHealth, mediumDistance); //low
        float rule9 = FuzzyAND(goodHealth, farDistance); //low

        float critical = 0;
        float high = 0;
        float normal = 0;
        float low = 0;

        // --- Critical
        critical = rule1;

        // --- High
        high = FuzzyOR(high, rule2);
        high = FuzzyOR(high, rule3);

        // --- Normal
        normal = rule4;

        // --- Low
        low = FuzzyOR(low, rule5);
        low = FuzzyOR(low, rule6);
        low = FuzzyOR(low, rule7);
        low = FuzzyOR(low, rule8);
        low = FuzzyOR(low, rule9);


        float representativeValue1 = DefuzzyReverseGrade(low, 20, 30);
        float representativeValue2 = DefuzzyTriangle(normal, 20, 30, 50);
        float representativeValue3 = DefuzzyTrapezoid(high, 30, 50, 60, 80);
        float representativeValue4 = DefuzzyGrade(critical, 60, 80, 100);

        return ((representativeValue1 * low + representativeValue2 * normal + representativeValue3 * high +
                 representativeValue4 * critical) / low + normal + high + critical);

    }
    float CalcMissilePriority(int tempAmmo, Vector3 closestMissilePack)
    {

        float lowAmmo = FuzzyReverseGrade(tempAmmo, 0, 4);
        float okAmmo = FuzzyTrapezoid(tempAmmo, 0, 4, 8, 14);
        float loadsAmmo = FuzzyGrade(tempAmmo, 8, 14);

        float distance = Vector3.Distance(closestMissilePack, transform.position);

        float closeDistance = FuzzyReverseGrade(distance, 25, 50);
        float mediumDistance = FuzzyTriangle(distance, 25, 50, 75);
        float farDistance = FuzzyGrade(distance, 50, 75);

        float rule1 = FuzzyAND(lowAmmo, closeDistance); //critical
        float rule2 = FuzzyAND(lowAmmo, mediumDistance); //high
        float rule3 = FuzzyAND(lowAmmo, farDistance); //high
        float rule4 = FuzzyAND(okAmmo, closeDistance); //normal
        float rule5 = FuzzyAND(okAmmo, mediumDistance); //low
        float rule6 = FuzzyAND(okAmmo, farDistance); //low
        float rule7 = FuzzyAND(loadsAmmo, closeDistance); //low
        float rule8 = FuzzyAND(loadsAmmo, mediumDistance); //low
        float rule9 = FuzzyAND(loadsAmmo, farDistance); //low

        float critical = 0;
        float high = 0;
        float normal = 0;
        float low = 0;


        // --- Critical
        critical = rule1;

        // --- High
        high = FuzzyOR(high, rule2);
        high = FuzzyOR(high, rule3);

        // --- Normal
        normal = rule4;

        // --- Low
        low = FuzzyOR(low, rule5);
        low = FuzzyOR(low, rule6);
        low = FuzzyOR(low, rule7);
        low = FuzzyOR(low, rule8);
        low = FuzzyOR(low, rule9);


        float representativeValue1 = DefuzzyReverseGrade(low, 20, 30);
        float representativeValue2 = DefuzzyTriangle(normal, 20, 30, 50);
        float representativeValue3 = DefuzzyTrapezoid(high, 30, 50, 60, 80);
        float representativeValue4 = DefuzzyGrade(critical, 60, 80, 100);

        return ((representativeValue1 * low + representativeValue2 * normal + representativeValue3 * high +
                 representativeValue4 * critical) / low + normal + high + critical);
    }
    float CalcChasePriority(float enemyHealth, int tempAmmo, Vector3 enemyTransformPosition)
    {
        // --- Enemy Health
        float lowHealth = FuzzyReverseGrade(enemyHealth, 15, 30);
        float mediumHealth = FuzzyTrapezoid(enemyHealth, 15, 30, 60, 75);
        float goodHealth = FuzzyGrade(enemyHealth, 60, 75);

        // --- Our Ammo
        float lowAmmo = FuzzyReverseGrade(tempAmmo, 0, 4);
        float okAmmo = FuzzyTrapezoid(tempAmmo, 0, 4, 8, 14);
        float loadsAmmo = FuzzyGrade(tempAmmo, 8, 14);

        // --- Distance
        float distance = Vector3.Distance(enemyTransformPosition, transform.position);

        float closeDistance = FuzzyReverseGrade(distance, 25, 50);
        float mediumDistance = FuzzyTriangle(distance, 25, 50, 75);
        float farDistance = FuzzyGrade(distance, 50, 75);

        // ----- Rules
        float critical = 0;
        float high = 0;
        float normal = 0;
        float low = 0;

        float rule1 = FuzzyAND3(lowAmmo, lowHealth, closeDistance); //high
        float rule2 = FuzzyAND3(lowAmmo, lowHealth, mediumDistance); //normal
        float rule3 = FuzzyAND3(lowAmmo, lowHealth, farDistance); //low
        float rule4 = FuzzyAND3(lowAmmo, mediumHealth, closeDistance); //normal
        float rule5 = FuzzyAND3(lowAmmo, mediumHealth, mediumDistance); //normal 
        float rule6 = FuzzyAND3(lowAmmo, mediumHealth, farDistance); //low
        float rule7 = FuzzyAND3(lowAmmo, goodHealth, closeDistance); //low
        float rule8 = FuzzyAND3(lowAmmo, goodHealth, mediumDistance); //low
        float rule9 = FuzzyAND3(lowAmmo, goodHealth, farDistance); //low

        float rule10 = FuzzyAND3(okAmmo, lowHealth, closeDistance); //critical
        float rule11 = FuzzyAND3(okAmmo, lowHealth, mediumDistance); //critical
        float rule12 = FuzzyAND3(okAmmo, lowHealth, farDistance); //high
        float rule13 = FuzzyAND3(okAmmo, mediumHealth, closeDistance); //critical
        float rule14 = FuzzyAND3(okAmmo, mediumHealth, mediumDistance); //high
        float rule15 = FuzzyAND3(okAmmo, mediumHealth, farDistance); //normal
        float rule16 = FuzzyAND3(okAmmo, goodHealth, closeDistance); //high
        float rule17 = FuzzyAND3(okAmmo, goodHealth, mediumDistance); //normal
        float rule18 = FuzzyAND3(okAmmo, goodHealth, farDistance); //normal

        float rule19 = FuzzyAND3(loadsAmmo, lowHealth, closeDistance); //critical
        float rule20 = FuzzyAND3(loadsAmmo, lowHealth, mediumDistance); //critical
        float rule21 = FuzzyAND3(loadsAmmo, lowHealth, farDistance); //high
        float rule22 = FuzzyAND3(loadsAmmo, mediumHealth, closeDistance); //high
        float rule23 = FuzzyAND3(loadsAmmo, mediumHealth, mediumDistance); //high
        float rule24 = FuzzyAND3(loadsAmmo, mediumHealth, farDistance); //normal
        float rule25 = FuzzyAND3(loadsAmmo, goodHealth, closeDistance); //normal
        float rule26 = FuzzyAND3(loadsAmmo, goodHealth, mediumDistance); //normal
        float rule27 = FuzzyAND3(loadsAmmo, goodHealth, farDistance); //normal

        // --- Critical
        critical = FuzzyOR(critical, rule10);
        critical = FuzzyOR(critical, rule11);
        critical = FuzzyOR(critical, rule13);
        critical = FuzzyOR(critical, rule19);
        critical = FuzzyOR(critical, rule20);

        // --- High
        high = FuzzyOR(high, rule1);
        high = FuzzyOR(high, rule12);
        high = FuzzyOR(high, rule14);
        high = FuzzyOR(high, rule16);
        high = FuzzyOR(high, rule21);
        high = FuzzyOR(high, rule22);
        high = FuzzyOR(high, rule23);

        // --- Normal
        normal = FuzzyOR(normal, rule2);
        normal = FuzzyOR(normal, rule4);
        normal = FuzzyOR(normal, rule5);
        normal = FuzzyOR(normal, rule15);
        normal = FuzzyOR(normal, rule17);
        normal = FuzzyOR(normal, rule18);
        normal = FuzzyOR(normal, rule24);
        normal = FuzzyOR(normal, rule25);
        normal = FuzzyOR(normal, rule26);
        normal = FuzzyOR(normal, rule27);

        // --- Low
        low = FuzzyOR(low, rule3);
        low = FuzzyOR(low, rule6);
        low = FuzzyOR(low, rule7);
        low = FuzzyOR(low, rule8);
        low = FuzzyOR(low, rule9);

        float representativeValue1 = DefuzzyReverseGrade(low, 20, 30);
        float representativeValue2 = DefuzzyTriangle(normal, 20, 30, 50);
        float representativeValue3 = DefuzzyTrapezoid(high, 30, 50, 60, 80);
        float representativeValue4 = DefuzzyGrade(critical, 60, 80, 100);

        return ((representativeValue1 * low + representativeValue2 * normal + representativeValue3 * high +
                 representativeValue4 * critical) / low + normal + high + critical);
    }
    float CalcEvadePriority(float tempHealth, int enemyAmmo, Vector3 enemyTransformPosition)
    {
        // ---- Our Health
        float lowHealth = FuzzyReverseGrade(tempHealth, 15, 30);
        float mediumHealth = FuzzyTrapezoid(tempHealth, 15, 30, 60, 75);
        float goodHealth = FuzzyGrade(tempHealth, 60, 75);

        // ---- Enemy Ammo
        float lowAmmo = FuzzyReverseGrade(enemyAmmo, 0, 4);
        float okAmmo = FuzzyTrapezoid(enemyAmmo, 0, 4, 8, 14);
        float loadsAmmo = FuzzyGrade(enemyAmmo, 8, 14);

        // ----- Distance
        float distance = Vector3.Distance(enemyTransformPosition, transform.position);

        float closeDistance = FuzzyReverseGrade(distance, 25, 50);
        float mediumDistance = FuzzyTriangle(distance, 25, 50, 75);
        float farDistance = FuzzyGrade(distance, 50, 75);

        // ----- Rules
        float critical = 0;
        float high = 0;
        float normal = 0;
        float low = 0;

        float rule1 = FuzzyAND3(lowAmmo, lowHealth, closeDistance); //normal
        float rule2 = FuzzyAND3(lowAmmo, lowHealth, mediumDistance); //normal
        float rule3 = FuzzyAND3(lowAmmo, lowHealth, farDistance); //low
        float rule4 = FuzzyAND3(lowAmmo, mediumHealth, closeDistance); //normal
        float rule5 = FuzzyAND3(lowAmmo, mediumHealth, mediumDistance); //low 
        float rule6 = FuzzyAND3(lowAmmo, mediumHealth, farDistance); //low
        float rule7 = FuzzyAND3(lowAmmo, goodHealth, closeDistance); //low
        float rule8 = FuzzyAND3(lowAmmo, goodHealth, mediumDistance); //low
        float rule9 = FuzzyAND3(lowAmmo, goodHealth, farDistance); //low

        float rule10 = FuzzyAND3(okAmmo, lowHealth, closeDistance); //high
        float rule11 = FuzzyAND3(okAmmo, lowHealth, mediumDistance); //high
        float rule12 = FuzzyAND3(okAmmo, lowHealth, farDistance); //normal
        float rule13 = FuzzyAND3(okAmmo, mediumHealth, closeDistance); //normal
        float rule14 = FuzzyAND3(okAmmo, mediumHealth, mediumDistance); //normal
        float rule15 = FuzzyAND3(okAmmo, mediumHealth, farDistance); //low
        float rule16 = FuzzyAND3(okAmmo, goodHealth, closeDistance); //low
        float rule17 = FuzzyAND3(okAmmo, goodHealth, mediumDistance); //low
        float rule18 = FuzzyAND3(okAmmo, goodHealth, farDistance); //low

        float rule19 = FuzzyAND3(loadsAmmo, lowHealth, closeDistance); //critical
        float rule20 = FuzzyAND3(loadsAmmo, lowHealth, mediumDistance); //high
        float rule21 = FuzzyAND3(loadsAmmo, lowHealth, farDistance); //normal
        float rule22 = FuzzyAND3(loadsAmmo, mediumHealth, closeDistance); //normal
        float rule23 = FuzzyAND3(loadsAmmo, mediumHealth, mediumDistance); //normal
        float rule24 = FuzzyAND3(loadsAmmo, mediumHealth, farDistance); //normal
        float rule25 = FuzzyAND3(loadsAmmo, goodHealth, closeDistance); //low
        float rule26 = FuzzyAND3(loadsAmmo, goodHealth, mediumDistance); //low
        float rule27 = FuzzyAND3(loadsAmmo, goodHealth, farDistance); //low

        // --- Critical
        critical = rule19;

        // --- High
        high = FuzzyOR(high, rule10);
        high = FuzzyOR(high, rule11);
        high = FuzzyOR(high, rule20);

        // --- Normal
        normal = FuzzyOR(normal, rule1);
        normal = FuzzyOR(normal, rule2);
        normal = FuzzyOR(normal, rule4);
        normal = FuzzyOR(normal, rule12);
        normal = FuzzyOR(normal, rule13);
        normal = FuzzyOR(normal, rule14);
        normal = FuzzyOR(normal, rule21);
        normal = FuzzyOR(normal, rule22);
        normal = FuzzyOR(normal, rule23);
        normal = FuzzyOR(normal, rule24);

        // --- Low
        low = FuzzyOR(low, rule3);
        low = FuzzyOR(low, rule5);
        low = FuzzyOR(low, rule6);
        low = FuzzyOR(low, rule7);
        low = FuzzyOR(low, rule8);
        low = FuzzyOR(low, rule9);
        low = FuzzyOR(low, rule15);
        low = FuzzyOR(low, rule16);
        low = FuzzyOR(low, rule17);
        low = FuzzyOR(low, rule18);
        low = FuzzyOR(low, rule25);
        low = FuzzyOR(low, rule26);
        low = FuzzyOR(low, rule27);

        float representativeValue1 = DefuzzyReverseGrade(low, 20, 30);
        float representativeValue2 = DefuzzyTriangle(normal, 20, 30, 50);
        float representativeValue3 = DefuzzyTrapezoid(high, 30, 50, 60, 80);
        float representativeValue4 = DefuzzyGrade(critical, 60, 80, 100);

        return ((representativeValue1 * low + representativeValue2 * normal + representativeValue3 * high +
                 representativeValue4 * critical) / low + normal + high + critical);
    }
    float FuzzyGrade(float value, float x0, float x1)
    {
        float result = 0;
        float x = value;

        if (x <= x0)
        {
            result = 0;
        }
        else if (x >= x1)
        {
            result = 1;
        }
        else
        {
            result = (x / (x1 - x0)) - (x0 / (x1 - x0));
        }

        return result;
    }
    float DefuzzyGrade(float C1, float x0, float x1, float maxXValue)
    {
        float a = IntersectionX(0.0f, 1.0f, C1, (-1 / (x1 - x0)), 1, (-x0 / (x1 - x0)));

        return (a + maxXValue) / 2;
    }
    float FuzzyReverseGrade(float value, float x0, float x1)
    {
        float result = 0;
        float x = value;

        if (x <= x0)
        {
            result = 1;
        }
        else if (x >= x1)
        {
            result = 0;
        }
        else
        {
            result = (-x / (x1 - x0)) + (x1 / (x1 - x0));
        }

        return result;
    }
    float DefuzzyReverseGrade(float C1, float x0, float x1)
    {
        float b = IntersectionX(0.0f, 1.0f, C1, (1 / (x1 - x0)), 1, x1 / (x1 - x0));

        return b / 2;
    }
    float FuzzyTriangle(float value, float x0, float x1, float x2)
    {
        float result = 0;
        float x = value;

        if (x <= x0)
        {
            result = 0;
        }
        else if (x == x1)
        {
            result = 1;
        }
        else if ((x > x0) && (x < x1))
        {
            result = (x / (x1 - x0)) - (x0 / (x1 - x0));
        }
        else
        {
            result = (-x / (x2 - x1)) + (x2 / (x2 - x1));
        }

        return result;
    }
    float DefuzzyTriangle(float C1, float x0, float x1, float x2)
    {
        float a = IntersectionX(0.0f, 1.0f, C1, (-1 / (x1 - x0)), 1, (-x0 / (x1 - x0)));
        float b = IntersectionX(0.0f, 1.0f, C1, (1 / (x2 - x1)), 1, x2 / (x2 - x1));

        return (a + b) / 2;
    }
    float FuzzyTrapezoid(float value, float x0, float x1, float x2, float x3)
    {
        float result = 0;
        float x = value;
        if (x <= x0)
        {
            result = 0;
        }
        else if ((x >= x1) && (x <= x2))
        {
            result = 1;
        }
        else if ((x > x0) && (x < x1))
        {
            result = (x / (x1 - x0)) - (x0 / (x1 - x0));
        }
        else
        {
            result = (-x / (x3 - x2)) + (x3 / (x3 - x2));
        }

        return result;
    }
    float DefuzzyTrapezoid(float C1, float x0, float x1, float x2, float x3)
    {
        float a = IntersectionX(0.0f, 1.0f, C1, (-1 / (x1 - x0)), 1, (-x0 / (x1 - x0)));
        float b = IntersectionX(0.0f, 1.0f, C1, (1 / (x3 - x2)), 1, x3 / (x3 - x2));

        return (a + b) / 2;

    }
    float FuzzyAND(float a, float b)
    {
        return Mathf.Min(a, b);
    }
    float FuzzyAND3(float a, float b, float c)
    {
        float d = Mathf.Min(a, b);
        return Mathf.Min(c, d);
    }
    float FuzzyOR(float a, float b)
    {
        return Mathf.Max(a, b);
    }
    float FuzzyOR3(float a, float b, float c)
    {
        float d = Mathf.Max(a, b);
        return Mathf.Max(c, d);
    }
    float FuzzyNOT(float a)
    {
        return 1.0f - a;
    }
    float IntersectionX(float A1, float B1, float C1, float A2, float B2, float C2)
    {
        float det = A1 * B2 - A2 * B1;
        if (det == 0)
        {
            //Lines are parallel
            return -1;
        }
        else
        {
            return (B2 * C1 - B1 * C2) / det;
        }
    }
    float IntersectionY(float A1, float B1, float C1, float A2, float B2, float C2)
    {
        float det = A1 * B2 - A2 * B1;
        if (det == 0)
        {
            //Lines are parallel
            return -1;
        }
        else
        {
            return (A1 * C2 - A2 * C1) / det;
        }

    }
    #endregion
}