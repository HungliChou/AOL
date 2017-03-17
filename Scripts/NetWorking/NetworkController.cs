using UnityEngine;
using System.Collections;

public class NetworkController : Photon.MonoBehaviour {

	public TP_Motor playerMotor;
	public TP_Animator playerAnimator;
	public TP_Info playerInfo;

	private bool isloaded;
	public bool Isloaded{get{return isloaded;} set{isloaded = value;}}
	// Use this for initialization
	void Awake () {
		isloaded = false;

	}
	
	void OnAllScripts()
	{
		playerMotor = GetComponent<TP_Motor>();
		playerAnimator = GetComponent<TP_Animator>();
		playerInfo = GetComponent<TP_Info>();
		gameObject.name = gameObject.name + photonView.viewID;

		/*playerCamera = transform.FindChild("Camera").gameObject;// transform.FindChild("Camera").gameObject;
		playerAttack = transform.FindChild("AttackRangeCol").gameObject;
		if(photonView.isMine)
		{
			playerCamera.SetActive(true);
			playerAttack.SetActive(true);
			playerCamera.transform.parent = transform.FindChild("targetLookAt").transform;
			playerController.OnPlayerAttack();
			playerAnimator.OnPlayerAttack();
		}
		else
		{
			playerCamera.gameObject.SetActive(false);
			playerController.OnPlayerAttack();
			playerAnimator.OnPlayerAttack();
			playerAttack.gameObject.SetActive(true);
		}
		playerController.SetIsLocalPlayer(photonView.isMine);*/
	}

	void Start()
	{
	}

	//Now to synchronize the position/rotation etc.

	private Vector3 correctPosition = Vector3.zero;
	private Quaternion correctRotation = Quaternion.identity;
	private float correctSpeed = 0;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			if(playerMotor!=null)
			{
				stream.SendNext((Vector3)playerMotor.MoveVector);
				stream.SendNext(playerMotor.moveSpeed);
				stream.SendNext(playerMotor.RollSpeed);
				stream.SendNext((float)playerMotor.VerticalVelocity);
			}
				//stream.SendNext((bool)playerAnimator.LockAnimating);
				//stream.SendNext((int)playerAnimator.Mode);
			if(playerAnimator!=null)
			{
				stream.SendNext((int)playerAnimator.MoveDirection);
				stream.SendNext((int)playerAnimator.State);
			}
			if(playerInfo!=null)
			{
				stream.SendNext((int)playerInfo.GetVital((int)VitalName.Health).CurValue);
				//stream.SendNext((int)playerInfo.GetVital((int)VitalName.Mana).CurValue);
			}
		}
		else
		{
				correctPosition = (Vector3)stream.ReceiveNext();
				correctRotation = (Quaternion)stream.ReceiveNext();
			if(playerMotor!=null)
			{
				playerMotor.MoveVector = (Vector3)stream.ReceiveNext();
				playerMotor.moveSpeed = (float)stream.ReceiveNext();
				playerMotor.RollSpeed = (float)stream.ReceiveNext();
				playerMotor.VerticalVelocity = (float)stream.ReceiveNext();
			}
				//playerAnimator.LockAnimating = (bool)stream.ReceiveNext();
				//playerAnimator.Mode = (TP_Animator.CharacterMode)stream.ReceiveNext();
			if(playerAnimator!=null)
			{
				playerAnimator.MoveDirection = (TP_Animator.Direction)stream.ReceiveNext();
				playerAnimator.State = (TP_Animator.CharacterState)stream.ReceiveNext();
			}
			if(playerInfo!=null)
			{
				playerInfo.GetVital((int)VitalName.Health).CurValue = (int)stream.ReceiveNext();
			//	playerInfo.GetVital((int)VitalName.Mana).CurValue = (int)stream.ReceiveNext();
			}
		}
	}

	void Update()
	{
		if(WholeGameManager.SP.InGame&&!isloaded)
		{
			OnAllScripts();
			isloaded = true;
		}
		if(!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, correctPosition, Time.deltaTime * 10);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctRotation, Time.deltaTime * 10);
		}
	}


}
