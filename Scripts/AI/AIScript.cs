using UnityEngine;
using System.Collections;
using RAIN.Core;
using RAIN.Entities;

public class AIScript : MonoBehaviour {

	public Transform myTransform;
	public Vector3 myPos;
	public float rayDistance;
	public bool backRayObj;
	public bool rightRayObj;
	public bool leftRayObj;
	public TP_Controller playerController;
	public TP_Animator playerAnimator;
	public string _state;
	public string State{get{return _state;}set{_state = value;}}
	public string _attack;
	public string Attack{get{return _attack;}set{_attack = value;}}
	public TP_Info playerInfo;
	public Vital playerHealth;
	public AIRig aiRig = null;
	public EntityRig ettRig = null;
	public Vector3 _wPNdiecrtion;
	public GameObject _wild = null;
	public GameObject[] wildHarnesses;
	public GameObject AttackTarget;
	public int _task;
	public int _level;
	public int _speed;
	public bool isResetWild;
	public bool _canShopping;
	public bool _needShopping;											//if need -> after shopping Then go out
	public bool _atHome;												//if at home -> can use LightSource
	public bool _atEnemyHome;											//if at Enemy home -> can use LightSource
	public bool _owner;
	public bool _hasEnemy;
	public bool _canChangeState;
	public bool _canUseSkill;
	public bool[] skillState;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		rayDistance = 0.6f;
		backRayObj = false;
		rightRayObj = false;
		leftRayObj = false;

		wildHarnesses = GameObject.Find("GameScript").GetComponent<Game_Manager>().WildHarnesses;

		playerController = GetComponent<TP_Controller>();
		playerAnimator = GetComponent<TP_Animator>();
		playerInfo = GetComponent<TP_Info>();
		playerHealth = playerInfo.GetVital((int)VitalName.Health);
		PhotonView playerView = GetComponent<PhotonView>();
		if(playerView.isMine)
			_owner = true;
		else
			_owner = false;
		aiRig.AI.WorkingMemory.SetItem("owner",_owner);

		aiRig.AI.WorkingMemory.SetItem("changePosTimer",0);
		aiRig.AI.WorkingMemory.SetItem("DyingHealth", (int)(playerHealth.MaxValue*0.1f));
		aiRig.AI.WorkingMemory.SetItem("wild",_wild);
		aiRig.AI.WorkingMemory.SetItem("T",0);
		aiRig.AI.WorkingMemory.SetItem("isChangingPos",false);
		_task = 0; 
		_level = 1;
		_speed = playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue;
		_canShopping = true;
		_needShopping = true;
		_atHome = false;
		_atEnemyHome = false;
		_hasEnemy = false;
		_canChangeState = true;
		_canUseSkill = true;

		aiRig.AI.WorkingMemory.SetItem("canUseSkill",_canUseSkill);
		aiRig.AI.WorkingMemory.SetItem("hasEnemy",_hasEnemy);
		aiRig.AI.WorkingMemory.SetItem("canShopping",_canShopping);
		aiRig.AI.WorkingMemory.SetItem("needShopping",_needShopping);
		aiRig.AI.WorkingMemory.SetItem("forwardSpeed", _speed);
		aiRig.AI.WorkingMemory.SetItem("canUseLS", true);
		aiRig.AI.WorkingMemory.SetItem("SkillTimer",0);
		aiRig.AI.WorkingMemory.SetItem("LSTimer", 0);
		aiRig.AI.WorkingMemory.SetItem("atHome", _atHome);
		aiRig.AI.WorkingMemory.SetItem("atEnemy", _atEnemyHome);
		aiRig.AI.WorkingMemory.SetItem("ready", false);
		aiRig.AI.WorkingMemory.SetItem("TaskNum", _task);
		aiRig.AI.WorkingMemory.SetItem("attackTar",AttackTarget);
		_state = playerAnimator.State.ToString();
		_attack = "";
		aiRig.AI.WorkingMemory.SetItem("state",_state);
		aiRig.AI.WorkingMemory.SetItem("attackWay",_attack);

		_wPNdiecrtion = Vector3.zero;
		aiRig.AI.WorkingMemory.SetItem("WPNdiecrtion",_wPNdiecrtion);
		//_state = TP_Animator.CharacterState.None;
		isResetWild = false;
		aiRig.AI.WorkingMemory.SetItem("reset",isResetWild);

		skillState = new bool[5];
	}

	void CheckSkillState()
	{
		if(playerInfo.GetVital((int)VitalName.Energy).CurValue==TP_Info.MAX_VITAL_VALUE_ENERGY)
			skillState[0] = true;
		else
			skillState[0] = false;

		for(int cnt=1;cnt<5;cnt++)
		{
			if(playerController.CheckSkillCDTime(cnt))
			{
				skillState[cnt] = true;
			}
			else
				skillState[cnt] = false;
		}
	}

	void CheckCanMove()
	{
		if(!playerAnimator.CanMove())
			aiRig.AI.WorkingMemory.SetItem("forwardSpeed", 0);
		else
			aiRig.AI.WorkingMemory.SetItem("forwardSpeed", _speed);
	}

	// Update is called once per frame
	void Update () {

		CheckCanMove();

		CheckSkillState();

		TP_Animator.CharacterState lastState = playerAnimator.State;

		playerHealth = playerInfo.GetVital((int)VitalName.Health);
		
		aiRig.AI.WorkingMemory.SetItem("DyingHealth", (int)(playerHealth.MaxValue*0.1f));
		aiRig.AI.WorkingMemory.SetItem("health", playerHealth.CurValue);
		if(playerInfo.Level!=_level)
			aiRig.AI.WorkingMemory.SetItem("level", playerInfo.Level);
		if(_speed!=playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue)
		{
			_speed = playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue;
			aiRig.AI.WorkingMemory.SetItem("forwardSpeed", _speed);
		}

		rayCastTest();

		_state = aiRig.AI.WorkingMemory.GetItem<string>("state");
		_attack = aiRig.AI.WorkingMemory.GetItem<string>("attackWay");

		if(_canShopping!=playerInfo.CanShopping)
		{
			_canShopping = playerInfo.CanShopping;
			aiRig.AI.WorkingMemory.SetItem("canShopping",_canShopping);
		}

		if(_atHome!=aiRig.AI.WorkingMemory.GetItem<bool>("atHome"))
			aiRig.AI.WorkingMemory.SetItem("atHome",_atHome);
		if(_atEnemyHome!=aiRig.AI.WorkingMemory.GetItem<bool>("atEnemy"))
			aiRig.AI.WorkingMemory.SetItem("atEnemy", _atEnemyHome);

		if(_state=="runningForward")
		{
			if(lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp && playerAnimator.CanMove())
			{
				if(playerAnimator.MoveDirection!=TP_Animator.Direction.Forward)
				{
					playerAnimator.MoveDirection = TP_Animator.Direction.Forward;
					playerAnimator.State = TP_Animator.CharacterState.Running;
				}
			}
		}

		if(_state=="runningBack")
		{
			if(lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp&&playerAnimator.CanMove())
			{
				playerAnimator.MoveDirection = TP_Animator.Direction.Backward;
				playerAnimator.State = TP_Animator.CharacterState.Running;
			}
		}

		if(_state == "StoreEnergy")
		{
			if(!aiRig.AI.WorkingMemory.ItemExists("storeEnergyTimer"))
				aiRig.AI.WorkingMemory.SetItem("storeEnergyTimer",0);
			float SETimer = aiRig.AI.WorkingMemory.GetItem<float>("storeEnergyTimer");
			if(SETimer<5)
			{
				SETimer += Time.deltaTime;
				aiRig.AI.WorkingMemory.SetItem("storeEnergyTimer",SETimer);
				playerController.EnergyStoreInput(0);
			}
			else
			{
				_state = "Idle";
				aiRig.AI.WorkingMemory.SetItem("Task",0);
				aiRig.AI.WorkingMemory.RemoveItem("storeEnergyTimer");
				playerAnimator.EnergyStoreInput(1);
			}
		}

		if(_state!=playerAnimator.State.ToString())
		{
			if(_state == "Dead")
				playerAnimator.Die();
			if(lastState==TP_Animator.CharacterState.KnockingDown&&!playerAnimator.LockAnimating)
				playerAnimator.StandUp();

			if(lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp&&lastState!=TP_Animator.CharacterState.EnergyStoring&&playerAnimator.Mode!=TP_Animator.CharacterMode.Skilling)
			{
				switch(_state)
				{
				case "Idle":
					playerAnimator.MoveDirection = TP_Animator.Direction.Stationary;
					playerAnimator.State = TP_Animator.CharacterState.Idle;
					break;
				}
			}

		}

		if(_attack=="MagicAttack")
		{
			if(!playerAnimator.LockAttacking&&!playerAnimator.LockAnimating)
			{
				if(lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp&&lastState!=TP_Animator.CharacterState.EnergyStoring)
				{
					playerAnimator.LockAttacking = true;
					playerController.MagicAttackInput(0);
				}
			}
		}
	}

	private void DrawHelperAtCenter(Vector3 direction, Color color, float scale)
	{
		Vector3 destination = myPos + direction * scale;
		Debug.DrawLine(myPos + direction * 0.25f, destination, color);
	}

	void rayCastTest()
	{
		myPos = myTransform.position;
		myPos += Vector3.up;
		RaycastHit hitF;
		RaycastHit hitR;
		RaycastHit hitB;
		RaycastHit hitL;



		float shoulderMultiplier = 0.25f;
		//Vector3 leftRayPos = myPos - (myTransform.right * shoulderMultiplier);
		//Vector3 rightRayPos = myPos + (myTransform.right * shoulderMultiplier);

		if(Physics.Raycast(myPos + myTransform.forward*shoulderMultiplier, myTransform.forward, out hitF, rayDistance))
		{
			if(hitF.transform.tag == "Ground")
			{
				DrawHelperAtCenter(myTransform.forward, Color.red, rayDistance);
	
				if(!isResetWild)
					StartCoroutine(ResetWild());
			}
		}
		else
			DrawHelperAtCenter(myTransform.forward, Color.yellow, rayDistance);

		if(Physics.Raycast(myPos + myTransform.right*shoulderMultiplier, myTransform.right, out hitR, rayDistance))
		{
			if(hitR.transform.tag == "Ground")
			{
				if(!rightRayObj)
				{
					rightRayObj = true;
					aiRig.AI.WorkingMemory.SetItem("RObj", rightRayObj);
				}
				DrawHelperAtCenter(myTransform.right, Color.red, rayDistance);
			}
		}
		else
		{
			if(rightRayObj)
			{
				rightRayObj = false;
				aiRig.AI.WorkingMemory.SetItem("RObj", rightRayObj);
			}
			DrawHelperAtCenter(myTransform.right, Color.yellow, rayDistance);
		}

		if(Physics.Raycast(myPos - myTransform.right*shoulderMultiplier, -myTransform.right, out hitL, rayDistance))
		{
			if(hitL.transform.tag == "Ground")
			{
				if(!leftRayObj)
				{
					leftRayObj = true;
					aiRig.AI.WorkingMemory.SetItem("LObj", leftRayObj);
				}
				DrawHelperAtCenter(-myTransform.right, Color.red, rayDistance);
			}
		}
		else
		{
			if(leftRayObj)
			{
				leftRayObj = false;
				aiRig.AI.WorkingMemory.SetItem("LObj", leftRayObj);
			}
			DrawHelperAtCenter(-myTransform.right, Color.yellow, rayDistance);
		}

		if(Physics.Raycast(myPos - myTransform.forward*shoulderMultiplier, -myTransform.forward, out hitB, rayDistance))
		{
			if(hitB.transform.tag == "Ground")
			{
				if(!backRayObj)
				{
					backRayObj = true;
					aiRig.AI.WorkingMemory.SetItem("BObj", backRayObj);
				}
				DrawHelperAtCenter(-myTransform.forward, Color.red, rayDistance);
			}
		}
		else
		{
			if(backRayObj)
			{
				backRayObj = false;
				aiRig.AI.WorkingMemory.SetItem("BObj", backRayObj);
			}
			DrawHelperAtCenter(-myTransform.forward, Color.yellow, rayDistance);
		}
	}
	
	IEnumerator ResetWild()
	{
		isResetWild = true;
		yield return new WaitForSeconds(1);
		aiRig.AI.WorkingMemory.SetItem("Task",3);
		CharacterController CC = GetComponent<CharacterController>();
		CC.Move(-myTransform.forward);
		isResetWild = false;
	}

	public void AssignTeam(int _team)
	{
		aiRig.AI.WorkingMemory.SetItem("team",_team);
		string str = "team" + _team.ToString();
		ettRig.Entity.EntityName = str;
	}
}
