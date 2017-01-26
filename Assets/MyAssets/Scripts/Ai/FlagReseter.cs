using UnityEngine;
using System.Collections;

public class FlagReseter : MonoBehaviour {

private bool  ok = true;

void  OnTriggerEnter ( Collider other  ){

	if(ok && other.tag == "roofTag" )
	{
		ok = false;	
		FindFarestEnemy().GetComponent<"HealthControlScript">().health = 0;

		GameObject go;
		go = GameObject.FindGameObjectWithTag("flagTag");

		go.GetComponent<"FlagScript">().ResetFlag();
		Destroy(gameObject);
	}
	
}

// Find the name of the farest enemy
GameObject FindFarestEnemy (){
	// Find all game objects with tag Enemy
	GameObject[] gos;
	gos = GameObject.FindGameObjectsWithTag("carTag");	
	GameObject farest;
	FIXME_VAR_TYPE distance= 0;
	FIXME_VAR_TYPE position= transform.position;

	// Iterate through them and find the farest one
	foreach(GameObject go in gos) 
	{
		if(go.transform.position != transform.position && !go.transform.IsChildOf(transform)  )
		{
			FIXME_VAR_TYPE diff= (go.transform.position - position);
			FIXME_VAR_TYPE curDistance= diff.sqrMagnitude;

			if (curDistance > distance) 
			{
				farest = go;
				distance = curDistance;
			}
		}
	}

	return farest;
}
}