
var custom1 : Transform;


var pos1 : Vector3;
var pos2 : Vector3;
var pos3 : Vector3;
var pos4 : Vector3;

private var gos : GameObject[];

function Start()
{
	
	
	var temp1 = Instantiate(custom1,pos1, Quaternion.identity);
	temp1.transform.parent = transform;
	
	var temp2 = Instantiate(custom1,pos2, Quaternion.identity);
	temp2.transform.parent = transform;
	
	var temp3 = Instantiate(custom1,pos3, Quaternion.identity);
	temp3.transform.parent = transform;
	
	var temp4 = Instantiate(custom1,pos4, Quaternion.identity);
	temp4.transform.parent = transform;
	
}

function Update () 
{
	
	
}


function SpawnNewHealth(pos : Vector3)
{
	// Waits 10 seconds
	yield new WaitForSeconds (10);
	gos = GameObject.FindGameObjectsWithTag("healthTag");
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

