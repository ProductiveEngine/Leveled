
var customSkin : GUISkin;

var deadCar : Transform;

var healthMap : GameObject;

var health : float = 100.0;

var pointsKeeper : float = 0;

var fatalityBonus : int = 0;


private var healthTaken : boolean = false;
private var userControls : boolean = true;

private var receivingDamage : boolean = false;

var startTime : float = 30;
var timeleft : float = 0;

private var gameIsOver : boolean = false;

private var gos : GameObject[];

function Start()
{
	healthMap = GameObject.Find("HealthMap");
}
//--------------------------------------------------------
private var take25 : boolean = true;
private var tempPos : Vector3;
function OnTriggerEnter (other : Collider) 
{
	tempPos = other.gameObject.transform.position;
	
	if(other.tag == "healthTag" && take25)
	{
		take25 = false;
		
		Destroy(other.gameObject);
		
		health = health + 25;
		healthTaken = true;
		if (health > 100){ health = 100;}
		
		healthMap.GetComponent("healthPackManagement").SendMessage("SpawnNewHealth",tempPos);
		
		pointsKeeper = pointsKeeper-5;
		
		Invoke("ChangeTake25",1);		
	}

}
function ChangeTake25()
{
	take25 = true;
}
//------------------------------------------------------

function MachineGunDamage( tpt : int )
{
	CancelInvoke();
	SetReceivingDamageTrue();
	Invoke("SetReceivingDamageFalse",5);
		
	health = health - 0.5;
	pointsKeeper = pointsKeeper-0.5;
	
}
function OnCollisionEnter(collision : Collision) 
{

	//var tag : String = collision.gameObject.tag;
	
	
	
	if(collision.gameObject.tag == "obstacleTag" || collision.gameObject.tag == "carTag" )
	{
		var tempC = collision.relativeVelocity.magnitude/10;
		health = health - tempC;
		
		
		if(collision.gameObject.tag == "carTag" && health<=0 && collision.gameObject.GetComponent(CONTROL).userControl)
		{
			
			collision.gameObject.GetComponent(HealthControlScript).fatalityBonusAdder();
			
		}
		
		
	}
	if(collision.gameObject.tag == "homingMissileTag" )
	{
		CancelInvoke();
		SetReceivingDamageTrue();
		Invoke("SetReceivingDamageFalse",5);
		
		health = health - 10;
		
		pointsKeeper = pointsKeeper-10;
		
		
		
		if(collision.transform.GetComponent(HomingMissileScript).fromUser)
		{
			gos = GameObject.FindGameObjectsWithTag("carTag");
			for (var go : GameObject in gos) 
			{
				if(go.GetComponent("CONTROL").userControl)
				{
					go.GetComponent(HealthControlScript).IncreasePointsKeeper(10);
					if(health<=0)
					{
						go.GetComponent(HealthControlScript).fatalityBonusAdder();
					}
					break;
				}
				
			}
			
		}
		
		
	}	
	if(collision.gameObject.tag == "speedMissileTag" )
	{
		CancelInvoke();
		SetReceivingDamageTrue();
		Invoke("SetReceivingDamageFalse",5);
		
		health = health - 5;
		
		pointsKeeper = pointsKeeper-5;
		
		
		
		if(collision.transform.GetComponent(SpeedMissileScript).fromUser)
		{
			gos = GameObject.FindGameObjectsWithTag("carTag");
			for (var go : GameObject in gos) 
			{
				if(go.GetComponent("CONTROL").userControl)
				{
					go.GetComponent(HealthControlScript).IncreasePointsKeeper(5);
					if(health<=0)
					{
						go.GetComponent(HealthControlScript).fatalityBonusAdder();
					}
					break;
				}
			
			}
			
		}
	}	
	if(collision.gameObject.tag == "powerMissileTag" )
	{
		CancelInvoke();
		SetReceivingDamageTrue();
		Invoke("SetReceivingDamageFalse",5);
		
		health = health - 20;
		
		pointsKeeper = pointsKeeper-20;
		
		
		
		if( collision.transform.GetComponent(PowerMissileScript).fromUser)
		{
			gos = GameObject.FindGameObjectsWithTag("carTag");
			for (var go : GameObject in gos) 
			{
				if(go.GetComponent("CONTROL").userControl)
				{
					go.GetComponent(HealthControlScript).IncreasePointsKeeper(20);
					if(health<=0)
					{
						go.GetComponent(HealthControlScript).fatalityBonusAdder();
					}
					break;
				}
				
			}
			
		}
	}	

}
function Update()
{
	if(health <= 0)
	{
		if(userControls)
		{		
			//temp = Instantiate(deadCar,transform.position, Quaternion.identity);
			//temp.GetComponent(Rigidbody).AddForceAtPosition(Vector3(0,10,10),transform.position);
			
			
			gameIsOver = true;
			var GUIObject = GameObject.Find("GUI");	
			GUIObject.GetComponent("EndGame").SetUserIsDead(true);
			Destroy(gameObject);
		}
		else
		{
			

			temp = Instantiate(deadCar,transform.position, Quaternion.identity);
			temp.GetComponent(Rigidbody).AddForceAtPosition(Vector3(0,10,10),transform.position);
			Destroy(gameObject);
			
		}
	
	}

}

function DisableUser()
{
	userControls = false;
}
function GetHealth() : float
{
	return health;
}
function GetHealthTaken() : boolean
{
	return healthTaken;
}
function SetHealthTaken( setter : boolean)
{
	healthTaken = setter;

} 
function GetReceivingDamage() : boolean
{
	return receivingDamage;
}
function SetReceivingDamageFalse()
{
	receivingDamage = false;
}
function SetReceivingDamageTrue()
{
	receivingDamage = true;
}


//------------------------------------------------------------------------------------------------------------------------------
function IncreasePointsKeeper(adder : int)
{
	pointsKeeper = pointsKeeper + adder;
}
function fatalityBonusAdder()
{
	fatalityBonus++;
}
//------------------------------------------------------------------------------------------------------------------------------







