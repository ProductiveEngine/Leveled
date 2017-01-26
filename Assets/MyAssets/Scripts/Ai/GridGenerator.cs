using UnityEngine;
using System.Collections;

public class GridGenerator : MonoBehaviour {

float up = 639;
float left = -288;

float down = -269;
float right = 620;

int maxNumberOfColumns = 20;
int maxNumberOfRows = 20;

private float gridWidth;
private float gridHeight;

private float widthStep;
private float heightStep;

Type[] extraTypes ;
extraTypes = new Type[1];
extraTypes[0] = typeof(EdgeInfo);


FIXME_VAR_TYPE grid= new Array(maxNumberOfRows);

private float idCounter =0.0f;

class NodeInfo {

	float id;
	float parentID;
	float priorityQueueID;
		
	bool  accessible ;

	float posX;
	float posY;
	
	float gridPosX;
	float gridPosY;

	float g = 0;
	float h = 0;
	float f = 0;
	
	float parentGridPosX;
	float parentGridPosY;
		
	void  NodeInfo ( float i ,   bool c  ,    float a  ,   float b  ,   float d ,   float e  ){
		id = i;
		accessible = c;
		posX = a;
		posY = b;
		gridPosX = d;
		gridPosY = e;
	}
	
}

class EdgeInfo {

	float id = 0;

	float up = 45.4f;
	float upright = Mathf.Sqrt(2)*45.4f;
	float right = 45.4f;
	float downright = Mathf.Sqrt(2)*45.4f;
	float down = 45.4f;
	float downleft = Mathf.Sqrt(2)*45.4f;
	float left = 45.4f;
	float upleft = Mathf.Sqrt(2)*45.4f;
		
	void  EdgeInfo ( ){
		
	}
	void  EdgeInfo (  float z  ){
		id = z;
	}
}

private FIXME_VAR_TYPE neighbors;

void  Awake (){	
	
	for (i=0; i <maxNumberOfRows; i++)
	{
		grid[i] = new Array(maxNumberOfColumns);
		
	}

	gridWidth = Mathf.Abs(left - right);
	gridHeight = Mathf.Abs(up - down); 

	widthStep = gridWidth / maxNumberOfColumns;
	heightStep = gridHeight / maxNumberOfRows ;
		
	 FIXME_VAR_TYPE layerMask= 1 << 11;
	// This would cast rays only against colliders in layer 8.
	// But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
	layerMask =~ layerMask;
		
	for ( n=0; n < maxNumberOfRows ; n++)
	{
		for( m=0; m<maxNumberOfColumns ; m++)
		{
			 waypoint = GameObject(""+(n+1)+"_"+(m+1)) ;
			 waypoint.transform.parent = transform; 
			 
			 waypoint.transform.position.z = -(n)*heightStep + up ;
			 waypoint.transform.position.x = (m)*widthStep + left;
			 
			 waypoint.transform.position.y = 1;
			  			
			 bool  checkAccessibility = !Physics.CheckSphere(Vector3(waypoint.transform.position.x , waypoint.transform.position.y , waypoint.transform.position.z) , 0.5f , layerMask );
						
			 grid[n][m] = new NodeInfo(idCounter,checkAccessibility,waypoint.transform.position.x,waypoint.transform.position.z,n,m);
			 idCounter = idCounter+1.0f;
		}
		
	}
			
}

}