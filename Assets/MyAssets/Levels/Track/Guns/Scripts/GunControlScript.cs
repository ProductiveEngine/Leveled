using UnityEngine;

public class GunControlScript : MonoBehaviour
{

    Transform speedMissilePrefab;
    Transform homingMissilePrefab;
    Transform powerMissilePrefab;

    GameObject missileMap;
    GameObject enemyLocked;

    private float radius = 5.0f;
    private float power = 10.0f;

    private BoxCollider bcollider;

    private float reloadTime = 1;
    private float lastShot = -10.0f;

    private bool userControls = true;
    private bool autoFire = false;
    private bool weaponsTaken = false;
    //-------------------------------------------------------------------------------
    ParticleSystem hitParticles;

    public void EnableAutoFire()
    {
        autoFire = true;
    }
    public void DisableAutoFire()
    {
        autoFire = false;
    }
    //-----------------------------------------------------------------------------

    int speedAmount = 0;
    int homingAmount = 0;
    int powerAmount = 0;

    public int GetSpeedAmount()
    {
        return speedAmount;
    }
    public int GetHomingAmount()
    {
        return homingAmount;
    }
    public int GetPowerAmount()
    {
        return powerAmount;
    }
    //-----------------------------------------------------------------------------
    public void Start()
    {
        missileMap = GameObject.Find("MissileMap");

        Transform bh = (transform.Find("collider"));
        bcollider = bh.GetComponent<BoxCollider>();
        //--------------------------------------------------------------------
        hitParticles = GetComponentInChildren<ParticleSystem>();

        if (hitParticles)
            hitParticles.Stop();
    }

    public void Update()
    {
        if (userControls)
        {
            if (Input.GetButtonDown("Fire1") && homingAmount > 0)
            {
                if (enemyLocked)
                {
                    GameObject projectile1 = (GameObject)Instantiate(homingMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                    projectile1.SendMessage("SetTarget", enemyLocked);
                    projectile1.SendMessage("SetFromUserTrue", 5);
                    Physics.IgnoreCollision(projectile1.GetComponent<Collider>(), bcollider);
                    homingAmount--;
                }
            }

            if (Input.GetButtonDown("Fire2") && speedAmount > 0)
            {
                GameObject projectile2 = (GameObject)Instantiate(speedMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                projectile2.SendMessage("SetFromUserTrue", 5);
                Physics.IgnoreCollision(projectile2.GetComponent<Collider>(), bcollider);
                speedAmount--;
            }

            if (Input.GetButtonDown("Fire3"))
            {
                Fire();
            }

            if (Input.GetButtonDown("Fire4") && powerAmount > 0)
            {
                GameObject projectile3 = (GameObject)Instantiate(powerMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                projectile3.SendMessage("SetFromUserTrue", 5);
                Physics.IgnoreCollision(projectile3.GetComponent<Collider>(), bcollider);
                powerAmount--;
            }
        }
        else if (autoFire && (Time.time > reloadTime + lastShot))
        {
            if (homingAmount > 0)
            {
                GameObject projectile4 = (GameObject)Instantiate(homingMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                projectile4.SendMessage("SetTarget", enemyLocked);
                Physics.IgnoreCollision(projectile4.GetComponent<Collider>(), bcollider);
                homingAmount--;
            }

            if (speedAmount > 0)
            {
                GameObject projectile5 = (GameObject)Instantiate(speedMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                Physics.IgnoreCollision(projectile5.GetComponent<Collider>(), bcollider);
                speedAmount--;
            }

            if (powerAmount > 0)
            {
                GameObject projectile6 = (GameObject)Instantiate(powerMissilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                Physics.IgnoreCollision(projectile6.GetComponent<Collider>(), bcollider);
                powerAmount--;
            }
            Fire();
            lastShot = Time.time;
        }
    }

    void Fire()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        RaycastHit hit = new RaycastHit();

        // Bit shift the index of the layer (10) to get a bit mask
        int layerMask = 1 << 10;
        // This would cast rays only against colliders in layer 10.
        // But instead we want to collide against everything except layer 10. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        // Did we hit anything?
        Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Physics.Raycast(origin, direction, out hit, 100f, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            // Apply a force to the rigidbody we hit
            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(10 * direction, hit.point);

            // Place the particle system for spawing out of place where we hit the surface!
            // And spawn a couple of particles
            if (hitParticles)
            {
                hitParticles.transform.position = hit.point;
                hitParticles.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                hitParticles.Play();
            }
            // Send a damage message to the hit object			            
            if (hit.collider.tag == "carColliderTag")
            {
                hit.collider.SendMessageUpwards("MachineGunDamage", 1);
            }
        }
    }
    //----------------------------------------------------------------------------------------------
    private bool take2 = true;
    private Vector3 tempPos;

    public void OnTriggerEnter(Collider other)
    {
        tempPos = other.gameObject.transform.position;

        if (other.tag == "speedM" && take2)
        {
            take2 = false;
            Destroy(other.gameObject);
            speedAmount = speedAmount + 2;
            weaponsTaken = true;
            missileMap.GetComponent<MissilePackManagementScript>().SendMessage("SpawnNewSpeed", tempPos);
            Invoke("ChangeTake2", 1);
        }
        if (other.tag == "homingM" && take2)
        {
            take2 = false;
            Destroy(other.gameObject);
            homingAmount = homingAmount + 2;
            weaponsTaken = true;
            missileMap.GetComponent<MissilePackManagementScript>().SendMessage("SpawnNewHoming", tempPos);
            Invoke("ChangeTake2", 1);
        }

        if (other.tag == "powerM" && take2)
        {
            take2 = false;
            Destroy(other.gameObject);
            powerAmount = powerAmount + 2;
            weaponsTaken = true;
            missileMap.GetComponent<MissilePackManagementScript>().SendMessage("SpawnNewPower", tempPos);
            Invoke("ChangeTake2", 1);
        }

    }
    void ChangeTake2()
    {
        take2 = true;
    }

    void DisableUser()
    {
        userControls = false;
    }

    public bool GetWeaponsTaken()
    {
        return weaponsTaken;
    }

    public void SetWeaponsTaken(bool setter)
    {
        weaponsTaken = setter;
    }
    public void SetEnemyLocked(GameObject enemyTarget)
    {
        enemyLocked = enemyTarget;
    }
}