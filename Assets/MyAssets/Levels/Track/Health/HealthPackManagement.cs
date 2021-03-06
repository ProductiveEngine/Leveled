﻿using System.Collections;
using UnityEngine;

public class HealthPackManagement : MonoBehaviour
{
    private GameObject custom1;

    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;

    private GameObject[] gos;

    void Start()
    {
        custom1 = (Resources.Load("healthPackPosition") as GameObject);

        GameObject temp1 = (GameObject)Instantiate(custom1, pos1, Quaternion.identity);
        temp1.transform.parent = transform;

        GameObject temp2 = (GameObject)Instantiate(custom1, pos2, Quaternion.identity);
        temp2.transform.parent = transform;

        GameObject temp3 = (GameObject)Instantiate(custom1, pos3, Quaternion.identity);
        temp3.transform.parent = transform;

        GameObject temp4 = (GameObject)Instantiate(custom1, pos4, Quaternion.identity);
        temp4.transform.parent = transform;

    }

    void SpawnNewHealth(Vector3 pos)
    {
        // Waits 10 seconds
        StartCoroutine(Wait());

        gos = GameObject.FindGameObjectsWithTag("healthTag");

        foreach (GameObject go in gos)
        {
            if (go.transform.position == pos)
            {
                return;
            }
        }
        GameObject temp = (GameObject)Instantiate(custom1, pos, Quaternion.identity);
        temp.transform.parent = transform;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(10);
    }
}