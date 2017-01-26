var target : Transform;

var radius = 30;
var power = 1000000.0;
var damping = 1.0;
var drivespeed = 170;
var crashed : boolean = false;

private var fireMissile : boolean = false;
private var direction : Vector3;

var explosion : GameObject;
var timeOut = 20.0;

public var fromUser : boolean = false;

function Start()
{

	Invoke("Kill",timeOut);
	
}

function Update () 
{

		transform.Translate(Vector3.forward * Time.deltaTime * drivespeed);

}

function OnTriggerEnter()
{
	transform.Translate(Vector3.forward * Time.deltaTime * drivespeed);

}

function OnCollisionEnter(collision : Collision) 
{
	/*
	var contact : ContactPoint = collision.contacts[0];
	var rotation = Quaternion.FromToRotation(Vector3.up,contact.normal);
	Instantiate(explosion,contact.point,rotation);
	
	crashed = true;
	//Debug.Log("Speed missile crashed");
	Kill();
	*/
	Instantiate(explosion,transform.position,Quaternion.identity);
	Destroy(gameObject);
}

function Kill()
{
	var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
	if(emitter)
	{
		emitter.emit = false;
	}
	//transform.DetachChildren();
	
	Destroy(gameObject);

}

function SetFromUserTrue()
{
	fromUser = true;
}





@script RequireComponent (Rigidbody)






