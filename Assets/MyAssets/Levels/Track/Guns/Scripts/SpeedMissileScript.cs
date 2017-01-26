using UnityEngine;

public class SpeedMissileScript : MonoBehaviour
{
    Transform target;

    float radius = 30;
    float power = 1000000.0f;
    float damping = 1.0f;
    float drivespeed = 170;
    bool crashed = false;

    private bool fireMissile = false;
    private Vector3 direction;

    GameObject explosion;
    float timeOut = 20.0f;

    public bool fromUser = false;

    void Start()
    {
        Invoke("Kill", timeOut);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * drivespeed);
    }

    void OnTriggerEnter()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * drivespeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Kill()
    {
        ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
        if (emitter)
        {
            emitter.emit = false;
        }

        Destroy(gameObject);
    }

    void SetFromUserTrue()
    {
        fromUser = true;
    }
}