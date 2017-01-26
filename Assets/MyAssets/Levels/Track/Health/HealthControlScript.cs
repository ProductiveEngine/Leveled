using UnityEngine;

public class HealthControlScript : MonoBehaviour
{
    public GUISkin customSkin;
    public Transform deadCar;
    public GameObject healthMap;

    public float health = 100.0f;
    public float pointsKeeper = 0;
    public int fatalityBonus = 0;

    private bool healthTaken = false;
    private bool userControls = true;

    private bool receivingDamage = false;

    float startTime = 30;
    float timeleft = 0;

    private bool gameIsOver = false;

    private GameObject[] gos;

    void Start()
    {
        healthMap = GameObject.Find("HealthMap");
    }
    //--------------------------------------------------------
    private bool take25 = true;
    private Vector3 tempPos;
    void OnTriggerEnter(Collider other)
    {
        tempPos = other.gameObject.transform.position;

        if (other.tag == "healthTag" && take25)
        {
            take25 = false;

            Destroy(other.gameObject);

            health = health + 25;
            healthTaken = true;
            if (health > 100) { health = 100; }

            healthMap.GetComponent<HealthPackManagement>().SendMessage("SpawnNewHealth", tempPos);

            pointsKeeper = pointsKeeper - 5;

            Invoke("ChangeTake25", 1);
        }

    }
    void ChangeTake25()
    {
        take25 = true;
    }
    //------------------------------------------------------

    void MachineGunDamage(int tpt)
    {
        CancelInvoke();
        SetReceivingDamageTrue();
        Invoke("SetReceivingDamageFalse", 5);

        health = health - 0.5f;
        pointsKeeper = pointsKeeper - 0.5f;

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "obstacleTag" || collision.gameObject.tag == "carTag")
        {
            float tempC = collision.relativeVelocity.magnitude / 10;
            health = health - tempC;

            if (collision.gameObject.tag == "carTag" && health <= 0 && collision.gameObject.GetComponent<ControlScript>().userControl)
            {
                collision.gameObject.GetComponent<HealthControlScript>().fatalityBonusAdder();
            }
        }
        if (collision.gameObject.tag == "homingMissileTag")
        {
            CancelInvoke();
            SetReceivingDamageTrue();
            Invoke("SetReceivingDamageFalse", 5);

            health = health - 10;

            pointsKeeper = pointsKeeper - 10;

            if (collision.transform.GetComponent<HomingMissileScript>().fromUser)
            {
                gos = GameObject.FindGameObjectsWithTag("carTag");
                foreach (GameObject go in gos)
                {
                    if (go.GetComponent<ControlScript>().userControl)
                    {
                        go.GetComponent<HealthControlScript>().IncreasePointsKeeper(10);
                        if (health <= 0)
                        {
                            go.GetComponent<HealthControlScript>().fatalityBonusAdder();
                        }
                        break;
                    }

                }

            }


        }
        if (collision.gameObject.tag == "speedMissileTag")
        {
            CancelInvoke();
            SetReceivingDamageTrue();
            Invoke("SetReceivingDamageFalse", 5);
            health = health - 5;
            pointsKeeper = pointsKeeper - 5;

            if (collision.transform.GetComponent<SpeedMissileScript>().fromUser)
            {
                gos = GameObject.FindGameObjectsWithTag("carTag");
                foreach (GameObject go in gos)
                {
                    if (go.GetComponent<ControlScript>().userControl)
                    {
                        go.GetComponent<HealthControlScript>().IncreasePointsKeeper(5);
                        if (health <= 0)
                        {
                            go.GetComponent<HealthControlScript>().fatalityBonusAdder();
                        }
                        break;
                    }
                }
            }
        }

        if (collision.gameObject.tag == "powerMissileTag")
        {
            CancelInvoke();
            SetReceivingDamageTrue();
            Invoke("SetReceivingDamageFalse", 5);
            health = health - 20;
            pointsKeeper = pointsKeeper - 20;

            if (collision.transform.GetComponent<PowerMissileScript>().fromUser)
            {
                gos = GameObject.FindGameObjectsWithTag("carTag");
                foreach (GameObject go in gos)
                {
                    if (go.GetComponent<ControlScript>().userControl)
                    {
                        go.GetComponent<HealthControlScript>().IncreasePointsKeeper(20);
                        if (health <= 0)
                        {
                            go.GetComponent<HealthControlScript>().fatalityBonusAdder();
                        }
                        break;
                    }
                }
            }
        }
    }
    void Update()
    {
        if (health <= 0)
        {
            if (userControls)
            {
                gameIsOver = true;
                GameObject GUIObject = GameObject.Find("GUI");
                GUIObject.GetComponent<EndGameScript>().SetUserIsDead(true);
                Destroy(gameObject);
            }
            else
            {
                GameObject temp = (GameObject)Instantiate(deadCar, transform.position, Quaternion.identity);
                temp.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, 10, 10), transform.position);
                Destroy(gameObject);
            }
        }
    }

    void DisableUser()
    {
        userControls = false;
    }
    public float GetHealth()
    {
        return health;
    }
    public bool GetHealthTaken()
    {
        return healthTaken;
    }
    public void SetHealthTaken(bool setter)
    {
        healthTaken = setter;

    }
    public bool GetReceivingDamage()
    {
        return receivingDamage;
    }
    public void SetReceivingDamageFalse()
    {
        receivingDamage = false;
    }
    public void SetReceivingDamageTrue()
    {
        receivingDamage = true;
    }
    //------------------------------------------------------------------------------------------------------------------------------
    void IncreasePointsKeeper(int adder)
    {
        pointsKeeper = pointsKeeper + adder;
    }
    void fatalityBonusAdder()
    {
        fatalityBonus++;
    }
    //------------------------------------------------------------------------------------------------------------------------------    
}