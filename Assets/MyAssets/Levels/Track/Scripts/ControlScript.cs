using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{

    //-- Sound ---------------------------------------------------------------------------------
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

    //-- GUI related variables    
    public GUISkin customSkin2;

    public Texture GUIOverlayTexture;
    public Texture speedometer;
    public Texture enemySkull;
    public Renderer muzzleFlash;
    //------------------------------ A. I. -------------------------------------------------------------------------------
    bool receivingFire = false;
    GunControlScript ammo;
    HealthControlScript health;

    bool flagRaised = false;

    //states
    int chase = 1;
    int evade = 2;
    int loadHealth = 3;
    int loadWeapons = 4;

    private int state = 1;

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

    int showState = 0;
    private GameObject enemy;
    //---------- FUZZY LOGIC ------------------------------------------------------------
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
    #region Calculate Health Priority
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
    //--------------------------------------------------------------------------------------------------------------------
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

    private BoxCollider bcollider;
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
    //---------------------------------------------------------------------
    //                       CAR SPECS

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

    // Here's all the variables for the AI, the waypoints are determined in the "GetWaypoints" function.
    // the waypoint container is used to search for all the waypoints in the scene, and the current
    // waypoint is used to determine which waypoint in the array the car is aiming for.
    private GameObject waypointContainer;
    private List<Transform> waypoints;
    private int currentWaypoint = 0;

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

    class wheel
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

    private wheel[] wheels;
    private GameObject obstacleMap;
    private List<Transform> obstacles;
    private List<SphereCollider> obstacleColliders;
    private int currentObstacle = 0;

    private CapsuleCollider m_dDBox;
    private Transform criticalDistance;
    private bool probeCheck = false;

    GunControlScript guns;
    public bool userControl = true;
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
        //go.GetComponent<"radar">().centerObject = transform;
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
    private GameObject healthMap;
    private List<Transform> healthPacks;
    private int closestHealthPack = 0;
    private GameObject missileMap;
    private List<Transform> missilePacks;
    private int closestMissilePack = 0;

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
        wheels = new wheel[4];

        for (int i = 0; i < 4; i++)
        {
            wheels[i] = new wheel();
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

        foreach (wheel w in wheels)
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

    //Update wheel status
    void UpdateWheels()
    {
        //calculate handbrake slip for traction gfx
        float handbrakeSlip = handbrake * GetComponent<Rigidbody>().velocity.magnitude * 0.1f;

        if (handbrakeSlip > 1)
            handbrakeSlip = 1;

        float totalSlip = 0.0f;
        onGround = false;

        foreach (wheel w in wheels)
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

    Vector3 velo = Vector3.zero; //rigidbody velocity
    Vector3 flatVelo = new Vector3();
    float kmPerH = 0;

    private bool onGround;
    private GameObject enemyLocked;

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

    bool OverrideStart = true;

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
    private bool playOnce = false;
    void StartEngineSound()
    {
        engineAudio.Play();

        float timer = 0;
        float fadeTime = 2;
        playOnce = true;
    }
    private float smoothSidewaysSlip = 0.0f;
    private float smoothForwardSlip = 0.0f;
    private float maxDownPressureForce = -1500.0f;
    private bool overrideForHandbrake = false;

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

        foreach (wheel w in wheels)
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
    private int remainingOpp = 7;
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
            catch (Exception)
            {
                ;
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
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
    void populateWaypoints(List<Transform> path)
    {
        waypoints.Clear();

        waypoints = new List<Transform>() { path[0] };
        currentWaypoint = 0;
        flagRaised = true;
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
    IEnumerator WaitRestore()
    {
        yield return new WaitForSeconds(0.5f);
    }
    void ResetFlag()
    {
        flagRaised = false;
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
    bool activateStuck = false;
    bool activateStuck2 = true;
    void Unstuck()
    {
        if (Mathf.Floor(GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000) < 1)
        {
            transform.Rotate(Vector3.up * 180);
        }
        activateStuck = false;
        activateStuck2 = true;

    }
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
    bool checkAllEnemies = false;
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
    private int captureFlagBonus = 0;
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
}