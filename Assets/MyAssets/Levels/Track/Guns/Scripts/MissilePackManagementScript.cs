using System.Collections;
using UnityEngine;

public class MissilePackManagementScript : MonoBehaviour
{
    Transform custom1;
    Transform custom2;
    Transform custom3;

    Vector3 pos1;
    Vector3 pos2;
    Vector3 pos3;
    Vector3 pos4;
    Vector3 pos5;
    Vector3 pos6;
    Vector3 pos7;
    Vector3 pos8;
    Vector3 pos9;

    private GameObject[] gos;

    void Start()
    {
        Transform temp = (Transform)Instantiate(custom1, pos1, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom2, pos2, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom1, pos3, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom2, pos4, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom1, pos5, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom2, pos6, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom3, pos7, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom3, pos8, Quaternion.identity);
        temp.transform.parent = transform;

        temp = (Transform)Instantiate(custom3, pos9, Quaternion.identity);
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
            Transform temp = (Transform)Instantiate(custom1, pos, Quaternion.identity);
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
            Transform temp = (Transform)Instantiate(custom2, pos, Quaternion.identity);
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
            Transform temp = (Transform)Instantiate(custom3, pos, Quaternion.identity);
            temp.transform.parent = transform;
        }

    }






}