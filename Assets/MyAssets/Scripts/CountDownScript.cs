using UnityEngine;
using System.Collections;

public class CountDownScript : MonoBehaviour {
//

private bool  playerWon = false;

Transform customprefab;
CONTROL carControlExternalScript;
GUISkin customSkin;

void  setUpCountDown ( Transform t   ){
	customprefab = t;
}
void  Start (){
	
	while(true)
	{
		yield return 0;
		break;
	}
	carControlExternalScript = customprefab.GetComponent<"CONTROL">();
	carControlExternalScript.waitCountDown = true;
	
	
}

private int countDown = 3;
bool  ok = false;
private bool  ok1 = false;
private bool  ok2 = false;
private bool  ok3 = false;
private bool  ok4 = false;

void  OnGUI (){
	GUI.skin = customSkin;
	
	FIXME_VAR_TYPE downTime= Mathf.Floor(Time.timeSinceLevelLoad);
	
	GUILayout.BeginArea( new Rect(Screen.width/2-100,Screen.height/2-50,200,200));
	GUILayout.BeginVertical();

	if(!ok)
	{
		if(countDown ==3)
		{
			GUILayout.Label("Ready : "+countDown);
		}
		if(countDown==2)
		{
			GUILayout.Label("Steady : "+countDown);
		}
		if(countDown==1)
		{
			GUILayout.Label("Steady : "+countDown);
		}
		if(countDown==0)
		{
			GUILayout.Label("GO!!! ");
			carControlExternalScript.waitCountDown = false;
		}
		if(countDown == -1)
		{
			ok = true;
		}
		
	}
	if (downTime >= 2 && !ok && !ok1)
	{
		countDown--;
		ok1 = true;
	}
	else if(downTime >= 3 && !ok && !ok2)
	{
		countDown--;
		ok2 = true;
	}
	else if(downTime >= 4 && !ok && !ok3)
	{
		countDown--;
		ok3 = true;
	}
	else if(downTime >= 5 && !ok && !ok4)
	{
		countDown--;
		ok4 = true;
	}
	
	GUILayout.EndVertical();
	GUILayout.EndArea();

	
	if(playerWon)
	{
		GUILayout.BeginArea( new Rect(Screen.width/2-50,Screen.height/2-50,200,200));
		
		GUILayout.Label("You won !!!");
		
		if(GUILayout.Button("Continue"))
		{
			Time.timeScale = 1;
			Application.LoadLevel("Hall of fame");
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

void  PlayerWon (){
	playerWon=true;
}










}