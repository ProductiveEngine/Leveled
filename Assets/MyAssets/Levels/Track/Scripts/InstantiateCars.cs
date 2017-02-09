using System.Collections;
using UnityEngine;

public class InstantiateCars : MonoBehaviour
{
    private Vector3 positionOne;
    private Vector3 positionTwo;
    private Vector3 positionThree;
    private Vector3 positionFour;
    private Vector3 positionFive;
    private Vector3 positionSix;
    private Vector3 positionSeven;
    private Vector3 positionEight;

    public Transform carPrefab;

    public void Start()
    {
        positionOne = new Vector3(-200, 8, 90);
        positionTwo = new Vector3(-200, 8, -120);
        positionThree = new Vector3(490, 8, 600);
        positionFour = new Vector3(300, 8, 600);
        positionFive = new Vector3(-100, 8, 600);
        positionSix = new Vector3(500, 8, 0);
        positionSeven = new Vector3(159, 8, 249);
        positionEight = new Vector3(207, 8, 447);

        showTheCar();
    }

    private void showTheCar()
    {
        Transform myCar = Instantiate(carPrefab, positionTwo, Quaternion.identity);
        myCar.GetComponent<ControlScript>().EnableUser();


        for (int i = 1; i < 9; i++)
        {
            waitOne();
            Transform enemyCar = Instantiate(carPrefab, getPosition(i), Quaternion.identity);
            enemyCar.GetComponent<HealthControlScript>().DisableUser();
            enemyCar.GetComponent<GunControlScript>().DisableUser();
            enemyCar.GetComponent<ControlScript>().DisableUser();
        }

    }

    IEnumerator waitOne()
    {
        yield return new WaitForSeconds(1);
    }


    private Vector3 getPosition(int i)
    {
        Vector3 result;

        switch (i)
        {
            case 1:
                result = positionOne;
                break;
            case 2:
                result = positionTwo;
                break;
            case 3:
                result = positionThree;
                break;
            case 4:
                result = positionFour;
                break;
            case 5:
                result = positionFive;
                break;
            case 6:
                result = positionSix;
                break;
            case 7:
                result = positionSeven;
                break;
            case 8:
                result = positionEight;
                break;
            default:
                result = positionOne;
                break;
        }

        return result;
    }

    void Update()
    {

        GameObject[] checkSpeedMissiles = GameObject.FindGameObjectsWithTag("speedMissileTag");

        foreach (var checkSpeedMissile in checkSpeedMissiles)
        {
            if (checkSpeedMissile.GetComponent<SpeedMissileScript>().crashed)
            {

                Destroy(checkSpeedMissile);
            }
        }

        GameObject[] checkHomingMissiles = GameObject.FindGameObjectsWithTag("homingMissileTag");

        foreach (var checkHomingMissile in checkHomingMissiles)
        {

            if (checkHomingMissile.GetComponent<HomingMissileScript>().crashed)
            {
                Destroy(checkHomingMissile);
            }
        }

        GameObject[] checkPowerMissiles = GameObject.FindGameObjectsWithTag("powerMissileTag");

        foreach (var checkPowerMissile in checkPowerMissiles)
        {

            if (checkPowerMissile.GetComponent<PowerMissileScript>().crashed)
            {
                Destroy(checkPowerMissile);
            }
        }
    }
}
