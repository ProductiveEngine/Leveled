
var custom1 : Transform;
var custom2 : Transform;
var custom3 : Transform;

var pos1 : Vector3;
var pos2 : Vector3;
var pos3 : Vector3; 
var pos4 : Vector3;
var pos5 : Vector3;
var pos6 : Vector3;
var pos7 : Vector3;
var pos8 : Vector3;
var pos9 : Vector3;

private var gos : GameObject[];

function Start()
{
	
	
	var temp = Instantiate(custom1,pos1, Quaternion.identity);
	temp.transform.parent = transform;
	//temp.transform.position = Vector3(200,1,500);
	
	temp = Instantiate(custom2,pos2, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom1,pos3, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom2,pos4, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom1,pos5, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom2,pos6, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom3,pos7, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom3,pos8, Quaternion.identity);
	temp.transform.parent = transform;
	
	temp = Instantiate(custom3,pos9, Quaternion.identity);
	temp.transform.parent = transform;
	
}

function Update () 
{
	
	
}


function SpawnNewSpeed(pos : Vector3)
{
	
	// Waits 10 seconds
	yield new WaitForSeconds (10);
	gos = GameObject.FindGameObjectsWithTag("speedM");
	for (var go : GameObject in gos) 
	{
		if(go.transform.position == pos)
		{
			return;
		}
		
	}
	var temp = Instantiate(custom1,pos, Quaternion.identity);
	temp.transform.parent = transform;

}

function SpawnNewHoming(pos : Vector3)
{
	// Waits 10 seconds
	
	yield new WaitForSeconds (10);
	gos = GameObject.FindGameObjectsWithTag("homingM");
	for (var go : GameObject in gos) 
	{
		if(go.transform.position == pos)
		{
			return;
		}
		
	}
	var temp = Instantiate(custom2,pos, Quaternion.identity);
	temp.transform.parent = transform;

}
function SpawnNewPower(pos : Vector3)
{
	// Waits 10 seconds
	yield new WaitForSeconds (10);
	gos = GameObject.FindGameObjectsWithTag("powerM");
	for (var go : GameObject in gos) 
	{
		if(go.transform.position == pos)
		{
			return;
		}
		
	}
	var temp = Instantiate(custom3,pos, Quaternion.identity);
	temp.transform.parent = transform;

}





