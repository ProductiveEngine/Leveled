using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
	public Transform target;

	private boolean ok;

	void Update () {
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
