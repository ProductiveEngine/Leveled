using System.Collections;
using UnityEngine;

public class MissilePackManagementScript : MonoBehaviour
{
    private GameObject custom1;
    private GameObject custom2;
    private GameObject custom3;

    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;
    public Vector3 pos5;
    public Vector3 pos6;
    public Vector3 pos7;
    public Vector3 pos8;
    public Vector3 pos9;

    private GameObject[] gos;

    void Start()
    {
        custom1 = Resources.Load("HomingMissilePackPosition") as GameObject;
        custom2 = Resources.Load("PowerMissilePackPosition") as GameObject;
        custom3 = Resources.Load("SpeedMissilePackPosition") as GameObject;

        GameObject temp = (GameObject)Instantiate(custom1, pos1, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom2, pos2, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom1, pos3, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom2, pos4, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom1, pos5, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom2, pos6, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom3, pos7, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom3, pos8, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (GameObject)Instantiate(custom3, pos9, Quaternion.identity);
        temp.transform.parent = transform;

    }

    IEnumerator SpawnNewSpeed(Vector3 pos)
    {
        yield return new WaitForSeconds(10);
        gos = GameObject.FindGameObjectsWithTag("speedM");

        bool ok = true;

        foreach (GameObject go in gos)
        {
            if (go.transform.position == pos)
            {
                ok = false;
            }
        }

        if (ok)
        {
            GameObject temp = (GameObject)Instantiate(custom1, pos, Quaternion.identity);
            temp.transform.parent = transform;
        }
    }

    IEnumerator SpawnNewHoming(Vector3 pos)
    {
        yield return new WaitForSeconds(10);
        gos = GameObject.FindGameObjectsWithTag("homingM");

        bool ok = true;

        foreach (GameObject go in gos)
        {
            if (go.transform.position == pos)
            {
                ok = false;
            }
        }

        if (ok)
        {
            GameObject temp = (GameObject)Instantiate(custom2, pos, Quaternion.identity);
            temp.transform.parent = transform;
        }
    }
    IEnumerator SpawnNewPower(Vector3 pos)
    {
        // Waits 10 seconds
        yield return new WaitForSeconds(10);
        gos = GameObject.FindGameObjectsWithTag("powerM");

        bool ok = true;

        foreach (GameObject go in gos)
        {
            if (go.transform.position == pos)
            {
                ok = false;
            }

        }

        if (ok)
        {
            GameObject temp = (GameObject)Instantiate(custom3, pos, Quaternion.identity);
            temp.transform.parent = transform;
        }

    }






}