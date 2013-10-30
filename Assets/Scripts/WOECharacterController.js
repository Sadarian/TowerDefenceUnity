public var jumpSpeed:float	= 8.0;
public var gravity:float =	20.0;
public var walkSpeed:float	= 5.0;
public var trotSpeed:float	= 10.0;
public var runSpeed:float = 20.0;
public var rotateSpeed:float = 150.0;

public var grounded:boolean =	false;
public var moveDirection:Vector3	= Vector3.zero;
public var isWalking:boolean = true;
public var moveStatus:String = "idle";
public var jumping:boolean	= false;
public var jumpingReachedApex:boolean = false;
public var moveSpeed:float	= 0.0;

private var	groundedTimeout =	0.4;
private var	lastGroundedTime = 0.0;
private var	idleTimeStart=-1.0;
private var	walkTimeStart=-1.0;
public var backwards:boolean=false;
public var m_fRotSpeed:float=0;
public var wantLock:boolean=false;

//@script AddComponentMenu("Camera-Control/WoE Camera")

function Update()
{
	var bMouse0:boolean=Input.GetMouseButton(0);
	var bMouse1:boolean=Input.GetMouseButton(1);
	var bJump:boolean=Input.GetButton("Jump");
	var fAxisV:float=Input.GetAxis("Vertical");
	var fAxisH:float=Input.GetAxis("Horizontal");
	var fAxisMX:float=Input.GetAxis("Mouse X");
	var fAxisMY:float=Input.GetAxis("Mouse Y");
	var fAxisMW:float=Input.GetAxis("Mouse ScrollWheel");
		
	if(Input.mousePosition.x<0||Input.mousePosition.y<0||Input.mousePosition.x>Screen.width||Input.mousePosition.y>Screen.height)
	{
		//drop input
		bJump=bMouse1=bMouse0=false;
		fAxisV=fAxisH=fAxisMX=fAxisMY=fAxisMW=0;
	};

	//	Only allow movement and	jumps	while	grounded
	if(grounded)
	{
		lastGroundedTime = Time.time;
		moveDirection = new Vector3((bMouse1 ? fAxisH : 0),0,fAxisV);
		
		if(moveDirection==Vector3.zero&&bMouse0&&bMouse1)
		{
			moveDirection.z=1;
		};
		
		backwards=moveDirection.z<0;
				  
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection.Normalize();
		moveDirection *= isWalking	? walkSpeed	: runSpeed;
		 
		moveStatus = "idle";
		if(moveDirection != Vector3.zero)
		{
			idleTimeStart=-1;
			if(walkTimeStart<0) {walkTimeStart=Time.time;};
			isWalking=((Time.time-walkTimeStart)<1)||backwards;
			moveStatus = isWalking ? "walking" : "running";
		}
		else
		{
			if(idleTimeStart<0) {idleTimeStart=Time.time;};
			if((Time.time-idleTimeStart)>0.25) {walkTimeStart=-1;};
		};
		 
		//	Jump!
		if(bJump)
		{
			moveDirection.y =	jumpSpeed;
			jumpingReachedApex=false;
			jumping=true;
		}
		else
		{
			jumpingReachedApex=false;
			jumping=false;
		};
	};
	
	//	Allow	turning at anytime. Keep the character	facing in the same direction as the	Camera if the right mouse button	is	down.
	var fRY0:float=transform.rotation.eulerAngles.y;
	if(bMouse1)
	{
		transform.rotation=Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0);
	} else {
		transform.Rotate(0,fAxisH * rotateSpeed *	Time.deltaTime, 0);
	};
	var fRY1:float=transform.rotation.eulerAngles.y;
	m_fRotSpeed=fRY1-fRY0;
	while(m_fRotSpeed<-180) {m_fRotSpeed+=360;};
	while(m_fRotSpeed>180) {m_fRotSpeed-=360;};
	m_fRotSpeed/=Time.deltaTime;
	
	if(Mathf.Abs(m_fRotSpeed)>=5)
	{
		idleTimeStart=-1;
	};
	
	wantLock=bMouse1||bMouse0;
	if(!wantLock)
	{
		Screen.lockCursor=false;
	}
	else
	{
		if(fAxisMX!=0||fAxisMY!=0) {Screen.lockCursor=true;};
	};
	
	//	Toggle walking/running with the T key
	/*if(Input.GetAxis("Run") == 1)
	{
		isWalking =	!isWalking;
	};*/
	
	//Apply gravity
	moveDirection.y -= gravity	* Time.deltaTime;
	
	//Move controller
	var controller:CharacterController = GetComponent(CharacterController);
	var flags =	controller.Move(moveDirection	* Time.deltaTime);
	grounded	= (flags	& CollisionFlags.Below)	!=	0;
	
  // if(grounded)	{jumping=false;};
  
	 if (jumping && !jumpingReachedApex	&&	moveDirection.y <= 0.0)
	 {
		  jumpingReachedApex	= true;
	 };
};

function	GetSpeed	()	{
	if	(moveStatus	==	"idle")
		moveSpeed =	0;
	if	(moveStatus	==	"walking")
		moveSpeed =	walkSpeed;
	if	(moveStatus	==	"running")
		moveSpeed =	runSpeed;
	return moveSpeed;
}

function	IsJumping () {
	return jumping;
}

function IsIdle()
{
	 if(idleTimeStart<0) {return false;};
	 return (Time.time-idleTimeStart)>3;
};

function IsIdleDirect()
{
	 if(idleTimeStart<0) {return false;};
	 return (Time.time-idleTimeStart)>0.25;
};

function	HasJumpReachedApex ()
{
	 return jumpingReachedApex;
}

function	IsGroundedWithTimeout ()
{
	 return lastGroundedTime +	groundedTimeout >	Time.time;
}

function	GetWalkSpeed () {
	return walkSpeed;
}
@script RequireComponent(CharacterController) 
