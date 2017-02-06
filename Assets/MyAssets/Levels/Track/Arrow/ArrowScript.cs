using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public Transform target;

    private bool ok;

    void Update()
    {
        if (ok)
        {
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
        ok = true;
    }
}
