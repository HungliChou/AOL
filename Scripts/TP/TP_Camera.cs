using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour 
{
	public static TP_Camera Instance;
	public Transform myTransform;

	public TP_Animator playerAnimator;
	public TP_Motor playerMotor;
	public InRoom_Menu roomMenu;
	public Texture2D AimCircle;
	public Texture2D AimCircleRed;

	public Transform TargetLookAt;
	//from Camera to targetLookAt
	public float Distance = 7f;
	//near limit
	public float DistanceMin = 4f;
	//far limit
	public float DistanceMax = 10f;
	
	public float DistanceSmooth = 0.05f;
	//Camera Occluded Resume
	public float DistanceResumeSmooth = 2f;
	//Sensitivity values to be multiplied by inputs
	public float X_MouseSensitivity = 2f;
	public float Y_MouseSensitivity = 2f;
	public float MouseWheelSensitivity = 10f;
	
	public float X_Smooth = 0.05f;
	public float Y_Smooth = 0.1f;
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	
	
	//Define limits of Y rotation
	public float X_MinLimit = -30f;
	public float X_MaxLimit = 30f;
	public float Y_MinLimit = -40f;
	public float Y_MaxLimit = 80f;
	
	//Defind OcclusionField
	public float OcclusionDistanceStep = 0.5f;
	public int MaxOcclusionCheck = 10;
	
	//rotation about each axis
	public float lastmouseX = 0;
	public float mouseX = 90f;
	private float mouseY = 0f;
	private float velDistance = 0f;
	//validated start distance
	private float startDistance = 0f;
	private Vector3 position = Vector3.zero;
	public Vector3 desiredPosition = Vector3.zero;
	//distance we want to move
	private float desiredDistance = 0f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f;

	public bool _lockMouseInput;
	public bool LockMouseInput{get{return _lockMouseInput;}set{_lockMouseInput = value;}}
	/*public bool shakeSwitch;
	public const float SetshakeValue = 5;
	public float shakeValue;*/

	void Awake() 
	{
		Instance = this;
		myTransform = transform;
	}
	
	void Start()
	{
		//Screen.lockCursor = true;
		//Screen.showCursor = false;

		roomMenu = GameObject.FindGameObjectWithTag("RoomMenu").GetComponent<InRoom_Menu>();

		//Clamped
		Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
		startDistance = Distance;
		Reset();
	}

	/*public void OnShakeCamera()
	{
		shakeValue = SetshakeValue;
		_lockMouseInput = false;
		shakeSwitch = true;
	}

	public IEnumerator ShakeCamera(int length,float shake)
	{
		/*Vector3 shakePos = new Vector3(0,Random.Range(0,shakeValue * 1) - shakeValue, 0);
		shakePos /= 100;
		Debug.Log(shakePos);
		myTransform.position += shakePos;
		shakeValue = shakeValue / 1.05f;

		if(shakeValue < 0.05f)
		{
			shakeValue = 0;
			shakeSwitch = false;
		}

---------------------------
		StartCoroutine(HoldWait());
		shakeSwitch = true;
		mouseX = 0;
		mouseY = 0;
		for(int cnt = length; cnt>0 ; cnt--)
		{
			myTransform.localPosition += new Vector3(0,shake,0);
			yield return new WaitForSeconds(0.02f);
			myTransform.localPosition -= new Vector3(0,shake,0);
			yield return new WaitForSeconds(0.02f);
		}
		shakeSwitch = false;
	}

	IEnumerator HoldWait(){
		while(!playerMotor.IsAlignCamera)
			yield return new WaitForSeconds(0.1f);
	}*/

	/*void OnGUI()
	{
		Texture2D aimCircle;
		if(roomMenu.isAimed)
			aimCircle = AimCircleRed;
		else
			aimCircle = AimCircle;
		
		Rect rect = new Rect(((Screen.width - AimCircle.width)/2),((Screen.height - AimCircle.height)/2)-30,AimCircle.width,AimCircle.height);
		
		GUI.DrawTexture(rect,aimCircle);
	}*/

	void Update()
	{
		/*if(shakeSwitch)
			ShakeCamera();*/
	}

	void LateUpdate()
	{
		if(TargetLookAt == null)
			return;
		if(WholeGameManager.SP.InGame)
		{
			if(Game_Manager.SP.MyGameState==GameState.Esc||GameUIManager.SP.PressControlling)
				return;
		}
		if(!_lockMouseInput)
			HandlePlayerInput();	

		var count = 0;
		do//use do to do at least once
		{
			CalculateDesiredPosition();
			count++;
		}while(CheckIfOccluded(count));


		UpdatePosition();

	}
	
	//process mouse input
	void HandlePlayerInput()
	{
		var deadZone = 0.01f;
		
		//if(Input.GetMouseButton(1))
		//{
			//The RMB is down get mouse Axis input
		mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
		//}
		
		//This is where we will limit mouseY
		mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

		/*if(playerAnimator.MoveDirection==TP_Animator.Direction.Stationary)
		{
			float mouseXnum = mouseX - lastmouseX;
			if(mouseXnum>X_MinLimit&&mouseXnum<X_MaxLimit)
				spine.rotation *= Quaternion.Euler(0,Input.GetAxis("Mouse X") * X_MouseSensitivity,0);
				//spine.Rotate(0,Input.GetAxis("Mouse X") * X_MouseSensitivity,0,Space.World);
			else
			{
				if(playerMotor.MoveVector.x==0&&playerMotor.MoveVector.z==0)
				{
					playerMotor.SpinSnapAlignCharacterWithCamera();
					//Turn Animation
					spine.rotation *= Quaternion.Euler(0,-mouseXnum,0);
					lastmouseX = mouseX;
				}
			}
		}
		else
				lastmouseX = mouseX;*/

		
		if(Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
		{
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
			preOccludedDistance = desiredDistance;
			distanceSmooth = DistanceSmooth;
		}
	}
	
	//use data from HPI to calculate the position we want to go 
	void CalculateDesiredPosition()
	{
		ResetDesiredDistance();
		//Evaluate distance
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmooth);
		//Calculate desired position
		desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
	}
	
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0,0, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction;
	}
	
	bool CheckIfOccluded(int count)
	{
		var isOccluded = false;
		
		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);
		
		if(nearestDistance != -1)
		{
			if(count < MaxOcclusionCheck)
			{
					isOccluded = true;
					Distance -= OcclusionDistanceStep;
			}
			else
			{

				Distance = nearestDistance - Camera.main.nearClipPlane;
			}

			if(Distance<1)
				Distance = 1;
			desiredDistance = Distance;
			distanceSmooth = DistanceResumeSmooth;
		}

		return isOccluded;
	}
	
	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		var nearestDistance = -1f;
		
		RaycastHit hitInfo;
		
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlanAtNear(to);
		
		// Draw lines in the editer to make it easier to visualize
		/*Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);
		
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);*/
		
		if(Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Enemy")
			nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Enemy")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Enemy")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Enemy")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Enemy")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		return nearestDistance;
	}
	
	
	void ResetDesiredDistance()
	{
		if(desiredDistance < preOccludedDistance)
		{
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			
			var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
			
			if(nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}
	//smooth transition from current location to desired position
	void UpdatePosition()
	{

		var posX = Mathf.SmoothDamp(position.x, desiredPosition.x,ref velX, X_Smooth);
		var posY = Mathf.SmoothDamp(position.y, desiredPosition.y,ref velY, Y_Smooth);
		var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z,ref velZ, X_Smooth);
		position = new Vector3(posX, posY, posZ);

		transform.position = position;

		transform.LookAt(TargetLookAt);
		
	}
	
	public void Reset()
	{
		//set mouseX mouseY to default values
		//magic number : X=0-> behind the character Y=10-> just above the character
		mouseX = 0;
		mouseY = 10;
		Distance = startDistance;
		desiredDistance = Distance;
		preOccludedDistance = Distance;
		_lockMouseInput = false;
	}



	public void UseExistingOrCreateNewMainCamera(Transform playerTran)
	{
		/*GameObject tempCamera;
		Transform targetLookAt;
		TP_Camera myCamera;*/
		
		/*if(Camera.main != null)
		{
			tempCamera = Camera.main.gameObject;
		}*/
		//else
		//{
			//tempCamera = new GameObject("Main Camera");
			//tempCamera.AddComponent("Camera");
			//tempCamera.tag = "MainCamera";
		//}

		//tempCamera.AddComponent("TP_Camera");
		//myCamera = tempCamera.GetComponent("TP_Camera") as TP_Camera;
		//targetLookAt = playerTran.FindChild("targetLookAt");
		//targetLookAt = GameObject.Find("targetLookAt") as GameObject;
		
		/*if(targetLookAt == null)
		{
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.parent = GameObject.FindGameObjectWithTag("Player").transform;
			targetLookAt.position = Vector3.zero;		
		}*/
		
		//myCamera.TargetLookAt = targetLookAt;
	}
}
