using UnityEngine;
using System.Collections;

public class roof180 : MonoBehaviour {


void  OnCollisionEnter ( Collision other  ){

	
}

bool  ok = true;
void  OnTriggerEnter ( Collider other  ){

	if(other.tag == "terrainTag")
	{	
	/*
		transform.parent.transform.rotation.z = 0;
		transform.parent.rigidbody.freezeRotation = true;
		transform.parent.rigidbody.velocity = Vector3.zero;
		transform.parent.transform.position.y = 10;
		yield return new WaitForSeconds(1);
		transform.parent.rigidbody.freezeRotation = false;
	*/
		if(ok)
		{
			Invoke("RestoreCar",3);
			ok = false;
		}	
		
	}
}
void  OnTriggerStay ( Collider other  ){

	if(other.tag == "terrainTag")
	{	
		if(ok)
		{
			Invoke("RestoreCar",3);
			ok = false;
		}	
		
	}
	
}
void  OnTriggerExit ( Collider other  ){
	ok = true;
	CancelInvoke("RestoreCar");
	
}

void  RestoreCar (){
	ok = true;
	SendMessageUpwards("Restore",1);
}

}