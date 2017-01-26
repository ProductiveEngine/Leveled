using UnityEngine;
using System.Collections;

public class pauseScreen : MonoBehaviour {

GUISkin customSkin;

 bool  isPaused = false;

void  Update (){

	if(Input.GetKey(KeyCode.Escape))
	{
		if(isPaused)
		{
			isPaused = false;
			SetPause(false);
		}
		else if(!isPaused)
		{
			isPaused = true;
			SetPause(true);
		}
		
	}

}

void  SetPause (  bool pause  ){
	 Input.ResetInputAxes();
	Object[] gos = FindObjectsOfType(typeof(GameObject));
	foreach( GameObject go in gos)
		go.SendMessage("DidPause",pause,SendMessageOptions.DontRequireReceiver);
		
	
	transform.position = Vector3.zero;
	
	if(pause)
	{
		Time.timeScale = 0;
	}
	else
	{
		Time.timeScale = 1;
	}
			
}

void  OnGUI (){
	GUI.skin = customSkin;
	
	if(isPaused)
	{
		GUILayout.BeginArea( new Rect(Screen.width/2-50,Screen.height/2-50,200,200));
		
		GUILayout.Label("Game paused");
		
		if(GUILayout.Button("Resume"))
		{
			isPaused = false;
			SetPause(false);
		}
		if(GUILayout.Button("Restart"))
		{
			Time.timeScale = 1;
			Application.LoadLevel( (Application.loadedLevelName) );
		}
		if(GUILayout.Button("Quit"))
		{
			Application.LoadLevel("Go Race");
		}
		
		GUILayout.EndArea();
	
	}
	
	
}


}