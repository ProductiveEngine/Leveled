using UnityEngine;

public class Instatiation : MonoBehaviour
{

    public Transform customPrefab;

    void Start()
    {
        Instantiate(customPrefab, new Vector3(504.5801f, 8, -95.47156f), Quaternion.identity);
    }

}