using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Entities;

public enum MonsterType{
	None,monsterBoss,monster
}

public class MonsterScript : MonoBehaviour {

	public MonsterType type;
	public Transform myTransform;
	public AIRig aiRig;
	public TP_Controller playerController;
	public TP_Animator playerAnimator;
	public TP_Info playerInfo;
	public Vital playerHealth;
	public GameObject Land;
	public GameObject Enemy;
	public bool Recover;
	public MonsterScript BossScript;
	public List<MonsterScript> minions;

	public string _state;
	public bool inLand;
	public bool _attack;
	public bool _owner;
	public int _speed;
	public float attackTimer;
	public bool islocked;
	public bool SnapToLand;

	void Awake()
	{
		myTransform = transform;
		playerController = GetComponent<TP_Controller>();
		playerAnimator = GetComponent<TP_Animator>();
		playerInfo = GetComponent<TP_Info>();
		islocked = false;
		SnapToLand = false;
	}

	// Use this for initialization
	void Start () {
		myTransform.GetComponent<NJGMapItem>().type = 8;
		if(type==MonsterType.monsterBoss)
		{
			myTransform.parent.GetComponent<WildTrigger>().MonsterBoss = myTransform;
			Land = myTransform.parent.gameObject;
			MonsterScript[] MS = myTransform.parent.GetComponentsInChildren<MonsterScript>();
			foreach(MonsterScript ms in MS)
			{
				if(ms.type==MonsterType.monster)
				{
					minions.Add(ms);
					ms.BossScript = this;
				}
			}
			Reset();
		}
		else if(type==MonsterType.monster)
		{
			int status = -(InRoom_Menu.SP.GetPlayerFromID(myTransform.GetComponent<PhotonView>().viewID).properties.Ai)%3;
			if(status==2)
				Land = myTransform.parent.FindChild("Anchor1").gameObject;
			else if(status==0)
				Land = myTransform.parent.FindChild("Anchor2").gameObject;
		}
		aiRig.AI.WorkingMemory.SetItem("Land",Land);
		aiRig.AI.WorkingMemory.SetItem("faceTarget",myTransform.parent.GetComponent<WildTrigger>().FaceTarget);

		PhotonView playerView = GetComponent<PhotonView>();
		if(playerView.isMine)
			_owner = true;
		else
			_owner = false;
		aiRig.AI.WorkingMemory.SetItem("owner",_owner);

		_speed = playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue;
		aiRig.AI.WorkingMemory.SetItem("forwardSpeed", _speed);

		aiRig.AI.WorkingMemory.SetItem("isLocked",islocked);

		_state = playerAnimator.State.ToString();
		aiRig.AI.WorkingMemory.SetItem("state",_state);
		aiRig.AI.WorkingMemory.SetItem("canAttack",_attack);
		inLand = true;
		aiRig.AI.WorkingMemory.SetItem("inLand",inLand);

		playerInfo.MeleeAttackDistance = 6;
		playerInfo.MeleeAttackAngle = 0.83f;


		aiRig.AI.WorkingMemory.SetItem("recover",Recover);


	}
	
	// Update is called once per frame
	void Update () {

		if(Enemy!=null)
		{
			aiRig.AI.WorkingMemory.SetItem("Enemy", Enemy);
		}
		if(type==MonsterType.monster)
		{
			if(BossScript!=null)
			{
				Enemy = BossScript.aiRig.AI.WorkingMemory.GetItem<GameObject>("Enemy");
				aiRig.AI.WorkingMemory.SetItem("Enemy", Enemy);
				inLand = BossScript.inLand;
				aiRig.AI.WorkingMemory.SetItem("inLand", inLand);
			}
			else
			{

			}
		}
		else if(type==MonsterType.monsterBoss)
		{
			inLand = aiRig.AI.WorkingMemory.GetItem<bool>("inLand");

		}

		Recover = aiRig.AI.WorkingMemory.GetItem<bool>("recover");
		if(Recover)
		{
			playerInfo.GetVital((int)VitalName.Health).DamageValue = 0;
			myTransform.rotation = new Quaternion(myTransform.rotation.x,0,myTransform.rotation.z,myTransform.rotation.w);
			foreach(MonsterScript ms in minions)
			{
				ms.playerInfo.GetVital((int)VitalName.Health).DamageValue = 0;
				ms.RotateToLand();
			}
		}
		playerHealth = playerInfo.GetVital((int)VitalName.Health);
		aiRig.AI.WorkingMemory.SetItem("health", playerHealth.CurValue);

		if(_speed!=playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue)
		{
			_speed = playerInfo.GetAbility((int)(AbilityName.MoveSpeed)).CurValue;
			aiRig.AI.WorkingMemory.SetItem("forwardSpeed", _speed);
		}


		if(_state=="InLand")
		{
			aiRig.AI.WorkingMemory.SetItem("state", "Idle");
		}

		_state = aiRig.AI.WorkingMemory.GetItem<string>("state");
		TP_Animator.CharacterState lastState = playerAnimator.State;

		if(playerAnimator.CanMove())
		{
			if(islocked)
			{
				islocked = false;
				aiRig.AI.WorkingMemory.SetItem("isLocked",islocked);
			}
		}
		else
		{
			if(!islocked)
			{
				islocked = true;
				aiRig.AI.WorkingMemory.SetItem("isLocked",islocked);
			}
		}

		if(_state=="runningForward")
		{
			if(playerAnimator.CanMove())
			{
				if(playerAnimator.MoveDirection!=TP_Animator.Direction.Forward)
				{
					playerAnimator.MoveDirection = TP_Animator.Direction.Forward;
					playerAnimator.State = TP_Animator.CharacterState.Running;
				}
			}
		}



		if(_state!=playerAnimator.State.ToString())
		{
			/*if(_state == "Dead")
				playerAnimator.Die();*/
			if(lastState==TP_Animator.CharacterState.KnockingDown&&!playerAnimator.LockAnimating)
				playerAnimator.StandUp();
			
			if(lastState!=TP_Animator.CharacterState.Beinghit&&lastState!=TP_Animator.CharacterState.Dizzing&&lastState!=TP_Animator.CharacterState.Freeze&&lastState!=TP_Animator.CharacterState.Dead&&lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp&&lastState!=TP_Animator.CharacterState.Attacking001&&lastState!=TP_Animator.CharacterState.Attacking002&&lastState!=TP_Animator.CharacterState.Attacking003)
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
		if(attackTimer == 0)
		{
			if(!playerAnimator.LockAttacking&&!playerAnimator.LockAnimating)
			{
				_attack = aiRig.AI.WorkingMemory.GetItem<bool>("canAttack");
				if(_attack)
				{
					if(lastState!=TP_Animator.CharacterState.KnockingDown&&lastState!=TP_Animator.CharacterState.StandingUp&&lastState!=TP_Animator.CharacterState.EnergyStoring)
					{
						attackTimer = 1;
						playerAnimator.LockAttacking = true;
						playerAnimator.LockAnimating = true;
						int way = 1;
						if(type==MonsterType.monsterBoss)
						{
							way = (int)(Random.Range(1,4-0.01f));
						}
						else if(type==MonsterType.monster)
						{
							way = (int)(Random.Range(1,3-0.01f));
						}
						if(way==1)
						{
							playerAnimator.Attack001();
						}
						else if(way==2)
						{
							playerAnimator.Attack002();
						}
						else
						{
							playerAnimator.Attack003();
						}
					}
				}
			}
		}
	}

	void FixedUpdate()
	{
		if(attackTimer<0)
			{attackTimer=0;}
		else
			attackTimer -= Time.fixedDeltaTime;
	}

	public void RotateToLand()
	{
		Quaternion newRot = new Quaternion(myTransform.rotation.x,
		                                   myTransform.rotation.y,
		                                   myTransform.rotation.z,myTransform.rotation.w);
		float rot = Land.transform.eulerAngles.y - myTransform.eulerAngles.y;

		newRot *= Quaternion.Euler(0,rot,0);
		
		myTransform.rotation = newRot;

	}

	void Reset()
	{
		playerInfo.GetVital((int)VitalName.Health).DamageValue = 0;
		myTransform.localRotation = new Quaternion(myTransform.rotation.x,0,myTransform.rotation.z,myTransform.rotation.w);
		foreach(MonsterScript ms in minions)
		{
			ms.playerInfo.GetVital((int)VitalName.Health).DamageValue = 0;
			if(ms.Land!=null)
			{
				ms.RotateToLand();
				ms.myTransform.localPosition = ms.Land.transform.localPosition;
			}
		}
	}
}
