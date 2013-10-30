using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/WoE Camera")]

public class WOECamera : MonoBehaviour
{
	public Transform target;
   
	public float targetHeight =	1.7f;
	public float distance =	5.0f;

	public float maxDistance = 20;
	public float minDistance = .6f;

	public float xSpeed	= 250.0f;
	public float ySpeed	= 120.0f;

	public int yMinLimit = -80;
	public int yMaxLimit = 80;

	public int zoomRate	= 40;

	public float rotationDampening = 3.0f;
	public float zoomDampening = 5.0f;

	public float x = 0.0f;
	public float y = 0.0f;
	public float currentDistance;
	public float desiredDistance;
	public float correctedDistance;

   void	Start ()
	{
		Vector3	angles = transform.eulerAngles;
		x =	angles.x;
		//y	= angles.y;
		y=25;

		currentDistance	= distance;
		desiredDistance	= distance;
		correctedDistance =	distance;

		// Make	the	rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
   }
   
	/**
	 * Camera logic	on LateUpdate to only update after all character movement logic	has	been handled.
	 */
   void	LateUpdate ()
	{
	   // Don't	do anything	if target is not defined
		if (!target) {return;};
		
		bool bMouse0=Input.GetMouseButton(0);
		bool bMouse1=Input.GetMouseButton(1);
		float fAxisV=Input.GetAxis("Vertical");
		float fAxisH=Input.GetAxis("Horizontal");
		float fAxisMX=Input.GetAxis("Mouse X");
		float fAxisMY=Input.GetAxis("Mouse Y");
		float fAxisMW=Input.GetAxis("Mouse ScrollWheel");
		
		if(Input.mousePosition.x<0||Input.mousePosition.y<0||Input.mousePosition.x>Screen.width||Input.mousePosition.y>Screen.height)
		{
			//drop input
			bMouse1=bMouse0=false;
			fAxisV=fAxisH=fAxisMX=fAxisMY=fAxisMW=0;
		};

		// If either mouse buttons are down, let the mouse govern camera position
		if (bMouse0||bMouse1)
		{
			x +=fAxisMX*xSpeed*0.02f;
			y -=fAxisMY*ySpeed*0.02f;
		}
		// otherwise, ease behind the target if	any	of the directional keys	are	pressed
		else if	(fAxisV!=0||fAxisH!=0)
		{
			float targetRotationAngle =	target.eulerAngles.y;
			float currentRotationAngle = transform.eulerAngles.y;
			x =	Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
		}

		y =	ClampAngle(y, yMinLimit, yMaxLimit);

		// set camera rotation
		Quaternion rotation	= Quaternion.Euler(y, x, 0);

		// calculate the desired distance
		desiredDistance	-= fAxisMW *	Time.deltaTime * zoomRate *	Mathf.Abs(desiredDistance);
		desiredDistance	= Mathf.Clamp(desiredDistance, minDistance,	maxDistance);
		correctedDistance =	desiredDistance;

		// calculate desired camera	position
		Vector3	position = target.position - (rotation * Vector3.forward * desiredDistance + new Vector3(0,	-targetHeight, 0));

		// check for collision using the true target's desired registration	point as set by	user using height
		//RaycastHit collisionHit;
		//Vector3	trueTargetPosition = new Vector3(target.position.x,	target.position.y +	targetHeight, target.position.z);

		// if there	was	a collision, correct the camera	position and calculate the corrected distance
		bool isCorrected = false;
		/*if (Physics.Linecast(trueTargetPosition, position, out collisionHit))
		{
			position = collisionHit.point;
			correctedDistance =	Vector3.Distance(trueTargetPosition, position);
			isCorrected	= true;
		}*/
		
		// For smoothing, lerp distance	only if	either distance	wasn't corrected, or correctedDistance is more than	currentDistance
		currentDistance	= !isCorrected || correctedDistance	> currentDistance ?	Mathf.Lerp(currentDistance,	correctedDistance, Time.deltaTime *	zoomDampening) : correctedDistance;

		// recalculate position	based on the new currentDistance
		position = target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0,	-targetHeight, 0));
		
		
		float fScapeH=Terrain.activeTerrain.SampleHeight(position)+2;
		if(position.y<fScapeH)
		{
			float fY=y;
			while(position.y<fScapeH&&fY<yMaxLimit) //fix angle based on terrain
			{
				fY+=0.05f;
				rotation=Quaternion.Euler(fY, x, 0);
				position=target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0,	-targetHeight, 0));
				fScapeH=Terrain.activeTerrain.SampleHeight(position)+2;
			};
			y=y*0.5f+fY*0.5f;
		};


		transform.rotation = rotation;
		transform.position = position;
   }

	private	static float ClampAngle(float angle, float min,	float max)
	{
		if (angle <	-360)
			angle += 360;
		if (angle >	360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}
