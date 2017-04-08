using System.Collections;
using UnityEngine;

public class InstantiateCars : MonoBehaviour
{
    private bool playerWon = false;
    public GUISkin customSkin;

    private Vector3 positionOne;
    private Vector3 positionTwo;
    private Vector3 positionThree;
    private Vector3 positionFour;
    private Vector3 positionFive;
    private Vector3 positionSix;
    private Vector3 positionSeven;
    private Vector3 positionEight;

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

    private int countDown = 3;
    bool ok = false;
    private bool ok1 = false;
    private bool ok2 = false;
    private bool ok3 = false;
    private bool ok4 = false;

    void OnGUI()
    {
        GUI.skin = customSkin;

        float downTime = Mathf.Floor(Time.timeSinceLevelLoad);

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 200));
        GUILayout.BeginVertical();

        if (!ok)
        {
            if (countDown == 3)
            {
                GUILayout.Label("Ready : " + countDown);
            }
            if (countDown == 2)
            {
                GUILayout.Label("Steady : " + countDown);
            }
            if (countDown == 1)
            {
                GUILayout.Label("Steady : " + countDown);
            }
            if (countDown == 0)
            {
                GUILayout.Label("GO!!! ");
                myCarPrefab.GetComponent<ControlScript>().waitCountDown = false;
            }
            if (countDown == -1)
            {
                ok = true;
            }

        }
        if (downTime >= 2 && !ok && !ok1)
        {
            countDown--;
            ok1 = true;
        }
        else if (downTime >= 3 && !ok && !ok2)
        {
            countDown--;
            ok2 = true;
        }
        else if (downTime >= 4 && !ok && !ok3)
        {
            countDown--;
            ok3 = true;
        }
        else if (downTime >= 5 && !ok && !ok4)
        {
            countDown--;
            ok4 = true;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();


        if (playerWon)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 200, 200));

            GUILayout.Label("You won !!!");

            if (GUILayout.Button("Restart"))
            {
                Time.timeScale = 1;
                Application.LoadLevel((Application.loadedLevelName));
            }
            if (GUILayout.Button("Quit"))
            {
                Application.LoadLevel("Go Race");
            }

            GUILayout.EndArea();
        }

    }

    void PlayerWon()
    {
        playerWon = true;
    }

    GameObject myCarPrefab;

    private void showTheCar()
    {
        myCarPrefab = GameObject.Instantiate(Resources.Load("CarPrefab") as GameObject, positionTwo, Quaternion.identity);
        myCarPrefab.GetComponent<ControlScript>().waitCountDown = true;
        myCarPrefab.GetComponent<ControlScript>().EnableUser();

        for (int i = 1; i < 9; i++)
        {
            if (i == 2)
            {
                continue;
            }

            waitOne();
            GameObject enemyCar = Instantiate(Resources.Load("CarPrefab") as GameObject, getPosition(i), Quaternion.identity);
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
