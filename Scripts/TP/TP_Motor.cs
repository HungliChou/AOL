using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour 
{
	public static TP_Motor Instance;
	public CharacterController CC;
	public TP_Info playerInfo;
	public TP_Controller playerController;
	public TP_Animator playerAnimator;
	public float ForwardSpeed;
	public float BackwardSpeed;
	public float StrafingSpeed;
	public float RollSpeed;
	//public float JumpSpeed;
	public float Gravity;
	public float TerminalVelocity;//Max downward speed at which gravity can be applied
	public float _verticalVelocity;
	public float moveSpeed;
	public bool isAlignCamera;
	//variable for PushBack
	private float rayDistance;
	
	private Transform myTransform;
	private Vector3 myPos;
	public Vector3 MoveVector { get; set; }
	public float VerticalVelocity { get{return _verticalVelocity;} set{_verticalVelocity = value;} }
	public bool IsRolling { get; set; } 
	public bool IsAlignCamera{get{return isAlignCamera;}set{isAlignCamera = value;}}

	void Awake() 
	{
		Instance = this;
		CC = gameObject.GetComponent<CharacterController>();
		playerInfo = GetComponent<TP_Info>();
		playerController = GetComponent<TP_Controller>();
		playerAnimator = GetComponent<TP_Animator>();
		myTransform = transform;
		myPos = myTransform.position;
		isAlignCamera = true;
		//JumpSpeed = 400f;
		Gravity = 500f;
		TerminalVelocity = 1000f;
	}


	private void UpdateSpeed(float AbiMoveSpeed)
	{
		ForwardSpeed = AbiMoveSpeed;
		BackwardSpeed = AbiMoveSpeed/2;
		StrafingSpeed = AbiMoveSpeed/2;
		if(playerInfo.IsGodPowering)
			RollSpeed = AbiMoveSpeed/1.5f;
		else
			RollSpeed = AbiMoveSpeed;
	}
	public void UpdateMotor(float AbiMoveSpeed) 
	{
		myPos = myTransform.position;


		//Update speed
		UpdateSpeed(AbiMoveSpeed);
		//Facing the Camera Direction
		//SnapAlignCharacterWithCamera();

		if(!playerInfo.isAI)
		{
			if(MoveVector.x != 0 || MoveVector.z != 0)
			{
				SpinSnapAlignCharacterWithCamera();
			}
			if(!isAlignCamera)
			{
				if(playerAnimator.State==TP_Animator.CharacterState.Idle)
					isAlignCamera = true;
				else if(Camera.main.transform.eulerAngles.y != myTransform.eulerAngles.y)
				{
					SpinSnapAlignCharacterWithCamera();
				}
				else 
					isAlignCamera = true;

			}
			//Let it move
			ProcessMotion(AbiMoveSpeed);
		}

		else
			ProcessMotion(AbiMoveSpeed);
		
	}

	public void OnCharacterAlignWithCamera()
	{
		isAlignCamera = false;
	}

	void ProcessMotion(float AbiMoveSpeed)
	{
		// Transform MoveVector to World Space
		MoveVector = myTransform.TransformDirection(MoveVector);
		// Normalize MoveVector if Magnitude > 1
		if(MoveVector.magnitude > 1)
			MoveVector = Vector3.Normalize(MoveVector);
		//if rolling
		if(IsRolling)
		{
			ApplyRoll();
			if(playerAnimator.State!=TP_Animator.CharacterState.Rolling)
				IsRolling = false;
		}
		moveSpeed = MoveSpeed(AbiMoveSpeed);
		// Multiply MoveVector by MoveSpeed
		MoveVector *= moveSpeed;
		//Reapply VerticalVelocity MoveVector.y
		MoveVector = new Vector3(MoveVector.x, _verticalVelocity, MoveVector.z);
		// Apply gravity
		ApplyGravity();
		// Move the Character in World Space
		// Multiply MoveVector by DeltaTime
		playerController.CharacterController.Move(MoveVector * Time.deltaTime);
	}

	void ApplyRoll()
	{
		if(playerAnimator.MoveDirection == TP_Animator.Direction.Left)
			MoveVector -= myTransform.right * (RollSpeed/3);
		else if(playerAnimator.MoveDirection == TP_Animator.Direction.Right)
			MoveVector += myTransform.right * RollSpeed/3;
		else if(playerAnimator.MoveDirection == TP_Animator.Direction.LeftForward)
			MoveVector -= myTransform.right * (RollSpeed/6);
		else if(playerAnimator.MoveDirection == TP_Animator.Direction.RightForward)
			MoveVector += myTransform.right * (RollSpeed/6);
	}

	void ApplyGravity()
	{
		//if y < max V -->  y - gravity
		if(MoveVector.y > -TerminalVelocity)
		{
			if(MoveVector.y<0)
			{
				MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime*1.5f, MoveVector.z);
			}
			else
				MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

		}
		//if grounded and down too fast(<-1) --> y = -1
		if(playerController.CharacterController.isGrounded && MoveVector.y < -1)
		{
			MoveVector = new Vector3(MoveVector.x, -1 , MoveVector.z);
		}
	
	}

	public void Roll()
	{
		IsRolling = true;
	}

	/*public void Jump()
	{
		if(playerController.CharacterController.isGrounded)
			_verticalVelocity = JumpSpeed;
	}*/

	public void SpinSnapAlignCharacterWithCamera()
	{
		Quaternion newRot = new Quaternion(myTransform.rotation.x,
		                                   myTransform.rotation.y,
		                                   myTransform.rotation.z,myTransform.rotation.w);
		float y = Camera.main.transform.eulerAngles.y - myTransform.eulerAngles.y;
		newRot *= Quaternion.Euler(0,y,0);
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, newRot, Time.deltaTime*10);
	}

	float MoveSpeed(float AbiMoveSpeed)
	{
		var moveSpeed = 0f;
		
		switch(playerAnimator.MoveDirection)
		{
			case TP_Animator.Direction.Stationary:
				moveSpeed = 0;
				break;
			case TP_Animator.Direction.Forward:
				moveSpeed = ForwardSpeed;
				break;
			case TP_Animator.Direction.Backward:
				moveSpeed = BackwardSpeed;
				break;
			case TP_Animator.Direction.Left:
				moveSpeed = StrafingSpeed;
				break;
			case TP_Animator.Direction.Right:
				moveSpeed = StrafingSpeed;
				break;
			case TP_Animator.Direction.LeftForward:
				moveSpeed = ForwardSpeed;
				break;
			case TP_Animator.Direction.RightForward:
				moveSpeed = ForwardSpeed;
				break;
			case TP_Animator.Direction.LeftBackward:
				moveSpeed = BackwardSpeed;
				break;
			case TP_Animator.Direction.RightBackward:
				moveSpeed = BackwardSpeed;
				break;
		}
		return moveSpeed;
	}


}
