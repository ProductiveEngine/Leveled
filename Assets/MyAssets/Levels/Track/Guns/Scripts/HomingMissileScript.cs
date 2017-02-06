using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HomingMissileScript : MonoBehaviour
{

    GameObject enemyTarget;
    Transform target;

    float blastForce = 0;
    float blastRadius = 10;
    bool implode = true;

    float explosionRadius = 0.0f;
    Collider[] colliders;

    float damping = 1.0f;
    float drivespeed = 60;
    public bool crashed = false;

    private bool homingControl = false;
    private bool fireMissile = false;

    GameObject explosion;
    float timeOut = 20.0f;

    public bool fromUser = false;

    public void Start()
    {
        Invoke("Kill", timeOut);
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public void Update()
    {
        if (!homingControl)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * drivespeed);
            Invoke("ChangeControl", 0.5f);
        }
        else
        {
            blastForce = 50;

            colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            // Update the target point
            if (enemyTarget)
            {
                SetTargetAbsolutePosition(enemyTarget.transform.position);
                absoluteTargetPoint = target.TransformPoint(relativeTargetPoint);
            }
            // Rotate towards the target   

            Vector3 oldDirection = transform.TransformDirection(Vector3.forward);
            Vector3 direction = absoluteTargetPoint - transform.position;

            direction = Vector3.RotateTowards(oldDirection, direction, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 1);
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void ChangeControl()
    {
        homingControl = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    /*
      The homing missile every frame simply rotates towards the target

     The actual physics is done by setting up the drag in the rigidbody (5 works well)
     and a relative force along the zaxis in the constanct force (
     relative.z = 50 works well)
    */
    float rotationSpeed = 50.0f;

    private Vector3 relativeTargetPoint = Vector3.zero;
    private Vector3 absoluteTargetPoint = Vector3.zero;

    void SetTarget(GameObject t)
    {
        enemyTarget = t;
        target = enemyTarget.transform;
    }

    void SetTargetAbsolutePosition(Vector3 position)
    {
        absoluteTargetPoint = position;

        if (target)
        {
            relativeTargetPoint = target.InverseTransformPoint(absoluteTargetPoint);
        }
    }
    void Kill()
    {
        ParticleSystem emitter = GetComponentInChildren<ParticleSystem>();

        if (emitter)
        {
            emitter.Stop();
        }
        Destroy(gameObject);
    }

    void SetFromUserTrue()
    {
        fromUser = true;
    }
}