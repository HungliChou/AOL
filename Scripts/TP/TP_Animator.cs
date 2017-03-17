using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HitSound{
	None, Sword1, Sword2, Fist, Slap
}

public class TP_Animator : Photon.MonoBehaviour 
{
	public enum HitWays
	{
		BeHit,KnockDown,None
	}

	public enum Skilltype
	{
		Attack,Darkman_Spurt,LaunchAttack,AddHealth,Theia_AddDefense,Darkman_Freeze,Darkman_AddDefense,Mars_Stun,Mars_AddAttack,
		Persia_Curse, Steropi_AddAttack,Hyperion_Unbeatable, Hyperion_AddDefense, Steropi_RollingStrike, Mars_Assault
	}

	public enum BuffSkillType
	{
		None,Speed,Power
	}

	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}
	
	public enum CharacterState
	{
		Idle, Running, Rolling, Dead, RunningAttacking001, //Jumping, Falling, Landing,
		Attacking001, Attacking002, Attacking003, Attacking004, Attacking005,
		Attacking1Final, Attacking2Final,Attacking3Final,Attacking4Final,Attacking5Final, 
		MagicAttacking1, MagicStoring, MagicStoredAttacking,
		Skilling01, Skilling02, Skilling03, Skilling04, SuperSkilling,
		ThrowingBigRock, ThrowingFireWork,
		EnergyStoring,
		Freeze,Beinghit,Dizzing,KnockingDown,StandingUp,
		None,ShowIdle
		//WalkingBackwards, StrafingLeft, StrafingRight
	}

	public enum CharacterMode
	{
		None, Skilling
	}

	public static TP_Animator Instance;
	public TP_Controller playerController;
	public TP_Motor playerMotor;
	public TP_Info playerInfo;
	public PhotonView playerView;
	public TP_Camera playerCam;
	public InRoom_Menu roomScript;
	public Animation targetLookAtAnim;
	public Attack playerAttack;
	public RangeColliderAttack playerRCA;
	public Transform spine;				//bone transform for mixing running attack animation
	public bool lockAnimating;
	public TP_Info.CharacterType type;

	public GameObject[] SkillRange;
	public AudioSource mainAudio;
	public AudioSource walkAudio;
	public GameObject audioPrefab;
	public bool[] finalAttackSoundBool;

	public CharacterMode mode;
	public CharacterState _state;
	public Direction _moveDirection;
	public AnimationClip currentAnimation;
	public AnimationClip currentAttack;

	public CharacterState nextState;	//to determine Attacking

	public bool _onComboTimer;
	public float _comboAttackTimer;
	public float _runningAttackTimer;
	public float _runningAttackSetTime;
	public bool _checkRunningAttackTimer;

	public int _multitudeAttacktime;	//to determine how many times can be executed to cause damage
	public int _multitudeAttackTimer;   //to calculate how many times of the attack has been executed to cause damage



	#region Running Attack CD time
	//public bool lockJumping;			//for other clients
	public bool lockRolling;
	public float rollTimer;
	public bool lockAttacking;			//true->can not attack
	private bool lockAttckingTiming;    //timer trigger
	public float attackTimer;			//timer
	public GameObject _currentSkillRange;
	#endregion

	#region Getter and Setter
	//public bool LockJumping {get{return lockJumping;} set{lockJumping = value;}}
	public bool LockAnimating {get{return lockAnimating;} set{lockAnimating = value;}}
	public bool LockAttacking {get{return lockAttacking;} set{lockAnimating = value;}}
	public GameObject CurrentSkillRange{get{return _currentSkillRange;} set{_currentSkillRange = value;}}
	public AnimationClip CurrentAnimation{get{return currentAnimation;} set{currentAnimation = value;}}
	public Direction MoveDirection { get{return _moveDirection;} set{_moveDirection = value;} }
	public CharacterState State { get{return _state;} set{_state = value;} }
	public CharacterMode Mode {get{return mode;} set{mode = value;}}
	#endregion

	#region animation state declaration
	private AnimationState goLeft;
	private AnimationState goRight;
	private AnimationState goBack;
	private AnimationState goForward;
	private AnimationState goRightForward;
	private AnimationState goLeftForward;
	private AnimationState goRightBack;
	private AnimationState goLeftBack;
	private AnimationState rollLeft;
	private AnimationState rollRight;
	//private AnimationState jump;
	//private AnimationState jumpLand;
	private AnimationState showIdle;
	private AnimationState idle;
	private AnimationState storeEnergy;
	//private AnimationState spinIdle;
	private AnimationState magicAttack;
	private AnimationState storeMagic;
	private AnimationState storedMagicAttack;
	private AnimationState attack01;
	private AnimationState attack02;
	private AnimationState attack03;
	private AnimationState attack04;
	private AnimationState attack05;
	private AnimationState attack1_Final;
	private AnimationState attack2_Final;
	private AnimationState attack3_Final;
	private AnimationState attack4_Final;
	private AnimationState attack5_Final;
	private AnimationState skill01;
	private AnimationState skill02;
	private AnimationState skill03;
	private AnimationState skill04;
	private AnimationState superSkill;
	private AnimationState runAttackUpper;
	private AnimationState runForwardAttack;
	private AnimationState runBackAttack;
	private AnimationState runLeftAttack;
	private AnimationState runRightAttack;
	private AnimationState runLeftForwardAttack;
	private AnimationState runRightForwardAttack;
	private AnimationState runLeftBackAttack;
	private AnimationState runRightBackAttack;
	private AnimationState freeze;
	private AnimationState dizzy;
	private AnimationState beHit;
	private AnimationState knockDown;
	private AnimationState standUp;
	private AnimationState die;
	#endregion

	#region animation state declaration
	public AudioClip runningForwardSound;
	public AudioClip runningBackSound;
	public AudioClip rollSound;
	public AudioClip idleSound;
	public AudioClip storeEnergySound;
	public AudioClip magicAttackSound;
	public AudioClip storeMagicSound;
	public AudioClip storedMagicAttackSound;
	public AudioClip normalAttackSound;
	public AudioClip attack1_FinalSound;
	public AudioClip attack2_FinalSound;
	public AudioClip attack3_FinalSound;
	public AudioClip attack4_FinalSound;
	public AudioClip attack5_FinalSound;
	public AudioClip skill01Sound;
	public AudioClip skill02Sound;
	public AudioClip skill03Sound;
	public AudioClip skill04Sound;
	public AudioClip superSkillSound;
	public AudioClip runAttackUpperSound;
	/*private AnimationState runForwardAttack;
	private AnimationState runBackAttack;
	private AnimationState runLeftAttack;
	private AnimationState runRightAttack;
	private AnimationState runLeftForwardAttack;
	private AnimationState runRightForwardAttack;
	private AnimationState runLeftBackAttack;
	private AnimationState runRightBackAttack;*/
	//private AnimationState freeze;
	//private AnimationState dizzy;
	public AudioClip beHitSound;
	public AudioClip knockDownSound;
	public AudioClip standUpSound;
	public AudioClip dieSound;
	#endregion

	#region
	public bool openTrail;
	public GameObject trail;
	public GameObject StoreEnergyEff;
	public GameObject GodPowerEff;
	public List<GameObject> effect = new List<GameObject>();
	#endregion


	void Awake()
	{
		Instance = this;
		playerController = GetComponent<TP_Controller>();
		playerMotor = GetComponent<TP_Motor>();
		playerInfo = GetComponent<TP_Info>();
		roomScript = GameObject.FindGameObjectWithTag("RoomMenu").GetComponent<InRoom_Menu>();
		playerView = GetComponent<PhotonView>();
		if(gameObject.transform.Find("_TargetLookAtAnchor/targetLookAt")!=null)
			targetLookAtAnim = gameObject.transform.Find("_TargetLookAtAnchor/targetLookAt").GetComponent<Animation>();

		type = playerInfo.Type;
		DetermineCharacterAnimation();
		SetRunningAttackTime();
		nextState = CharacterState.None;
		mode = CharacterMode.None;
		//lockJumping = false;
		lockRolling = false;
		lockAnimating = false;
		lockAttacking = false;
		lockAttckingTiming = false;
		rollTimer = 0;
		attackTimer = 0;
		_onComboTimer = false;
		_comboAttackTimer = 0;
		_multitudeAttacktime = 0;
		_multitudeAttackTimer = 0;
		openTrail = false;
	}

	void SetRunningAttackTime()
	{
		switch(playerInfo.Role)
		{
		case TP_Info.CharacterRole.Mars:
			_runningAttackSetTime = 0.65f;
			break;
		case TP_Info.CharacterRole.DarkMan:
			_runningAttackSetTime = 0.62f;
			break;
		case TP_Info.CharacterRole.Theia:
			_runningAttackSetTime = 0.75f;
			break;
		case TP_Info.CharacterRole.Persia:
			_runningAttackSetTime = 0.6f;
			break;
		case TP_Info.CharacterRole.Hyperion:
			_runningAttackSetTime = 0.55f;
			break;
		case TP_Info.CharacterRole.Steropi:
			_runningAttackSetTime = 0.65f;
			break;
		default:
			_runningAttackSetTime = 1.5f;
			break;
		}
	}

	void Start ()
	{

	}

	public void OnPlayerAttack()
	{
		playerAttack = GetComponentInChildren<Attack>();
	}

	void Update () 
	{
		if(playerAttack==null)
			playerAttack = GetComponentInChildren<Attack>();
		DetermineCurrentState();
		ProcessCurrentState();
	}

	void FixedUpdate()
	{

		if(lockRolling)
			RollTimer();
		if(lockAttacking)
			RunAttackTimer();
		if(_onComboTimer)
		{
			if(_comboAttackTimer > 0)
			{
				CountDownComboAttackTimer();
			}
			else
			{
				if(nextState != CharacterState.None)
				{
					//if(currentAttack != idle.clip)
					//{
						nextState = CharacterState.None;
						currentAttack = idle.clip;
						_onComboTimer = false;
					//}

				}
			}
		}
		if(_checkRunningAttackTimer)
		{
			if(_runningAttackTimer<_runningAttackSetTime)
			{
				_runningAttackTimer += Time.fixedDeltaTime;
			}
			else
			{
				if(playerCam!=null)
				{
					if(playerCam.LockMouseInput)
					{
						playerCam.LockMouseInput = false;
					}
				}
				/*if(ShouldLockAttackCheck())
					lockAttacking = true;
				else
					lockAttacking = false;*/
				if(_moveDirection==Direction.Stationary)
					State = CharacterState.Idle;
				else
					State = CharacterState.Running;
				nextState = CharacterState.None;
				currentAttack = idle.clip;

				if(trail!=null)
				{
					if(openTrail)
					{
						trail.GetComponent<MeleeWeaponTrail>().Emit = false;
						openTrail = false;
					}
				}

				_runningAttackTimer = 0;
				_checkRunningAttackTimer = false;
			}
		}
	}

	void CountDownComboAttackTimer()
	{
		_comboAttackTimer -= Time.fixedDeltaTime;
		if(_comboAttackTimer<0)
			_comboAttackTimer = 0;
	}

	void MixAnimation(TP_Info.CharacterRole role)
	{
		switch(role)
		{
		case TP_Info.CharacterRole.Mars:
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Mars_RunForwardBackAttack"].clip,"Mars_Run_FB_AttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Mars_RunLeftForwardRightBackAttack"].clip,"Mars_Run_LFRB_AttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Mars_RunRightForwardLeftBackAttack"].clip,"Mars_Run_RFLB_AttackUpper");
			
			GetComponent<Animation>()["Mars_Run_FB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Mars_Run_FB_AttackUpper"].layer = 1;
			GetComponent<Animation>()["Mars_Run_LFRB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Mars_Run_LFRB_AttackUpper"].layer = 1;
			GetComponent<Animation>()["Mars_Run_RFLB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Mars_Run_RFLB_AttackUpper"].layer = 1;
			break;
		case TP_Info.CharacterRole.DarkMan:
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunAttack01"].clip,"Darkman_RunAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunBackAttack"].clip,"Darkman_RunBackAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunLeftAttack"].clip,"Darkman_RunLeftAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunRightAttack"].clip,"Darkman_RunRightAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunLeftForwardAttack"].clip,"Darkman_RunLeftForwardAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunRightForwardAttack"].clip,"Darkman_RunRightForwardAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Darkman_RunLeftBackAttack"].clip,"Darkman_RunLeftBackAttackUpper");
			GetComponent<Animation>()["Darkman_RunAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunBackAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunBackAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunLeftAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunLeftAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunRightAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunRightAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunLeftForwardAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunLeftForwardAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunRightForwardAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunRightForwardAttackUpper"].layer = 1;
			GetComponent<Animation>()["Darkman_RunLeftBackAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Darkman_RunLeftBackAttackUpper"].layer = 1;
			break;
		case TP_Info.CharacterRole.Theia:
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Theia_RunMagicAttack"].clip,"Theia_RunAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Theia_RunForwardMagicAttack"].clip,"Theia_RunForwardMagicAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Theia_RunBackMagicAttack"].clip,"Theia_RunBackMagicAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Theia_RunLeftForwardMagicAttack"].clip,"Theia_RunLeftForwardMagicAttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Theia_RunRightBackMagicAttack"].clip,"Theia_RunRightBackMagicAttackUpper");
			GetComponent<Animation>()["Theia_RunAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Theia_RunAttackUpper"].layer = 1;
			GetComponent<Animation>()["Theia_RunForwardMagicAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Theia_RunForwardMagicAttackUpper"].layer = 1;
			GetComponent<Animation>()["Theia_RunBackMagicAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Theia_RunBackMagicAttackUpper"].layer = 1;
			GetComponent<Animation>()["Theia_RunLeftForwardMagicAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Theia_RunLeftForwardMagicAttackUpper"].layer = 1;
			GetComponent<Animation>()["Theia_RunRightBackMagicAttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Theia_RunRightBackMagicAttackUpper"].layer = 1;
			break;
		case TP_Info.CharacterRole.Steropi:
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Steropi_RunForwardBackAttack"].clip,"Steropi_Run_FB_AttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Steropi_RunRightForwardLeftBackAttack"].clip,"Steropi_Run_RFLB_AttackUpper");

			GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"].layer = 1;
			GetComponent<Animation>()["Steropi_Run_RFLB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Steropi_Run_RFLB_AttackUpper"].layer = 1;
			break;
		case TP_Info.CharacterRole.Hyperion:
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Hyperion_RunForwardBackAttack"].clip,"Hyperion_Run_FB_AttackUpper");
			GetComponent<Animation>().AddClip(GetComponent<Animation>()["Hyperion_RunRightForwardLeftBackAttack"].clip,"Hyperion_Run_RFLB_AttackUpper");
			
			GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"].layer = 1;
			GetComponent<Animation>()["Hyperion_Run_RFLB_AttackUpper"].AddMixingTransform(spine);
			GetComponent<Animation>()["Hyperion_Run_RFLB_AttackUpper"].layer = 1;
			break;
		}

	}

	void DetermineCharacterAnimation()
	{
		switch(playerInfo.Role)
		{
		case TP_Info.CharacterRole.Mars:
			//create new animation for running attack
			MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Mars_GoLeft"];
			goRight = GetComponent<Animation>()["Mars_GoRight"];
			goBack = GetComponent<Animation>()["Mars_GoBack"];
			goForward = GetComponent<Animation>()["Mars_GoForward"];
			goRightForward = GetComponent<Animation>()["Mars_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Mars_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Mars_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Mars_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Mars_RollLeft"];
			rollRight = GetComponent<Animation>()["Mars_RollRight"];
			showIdle = GetComponent<Animation>()["Mars_ShowIdle"];
			idle = GetComponent<Animation>()["Mars_Idle"];
			storeEnergy = GetComponent<Animation>()["Mars_StoreEnergy"];

			attack01 = GetComponent<Animation>()["Mars_Attack01"];
			attack02 = GetComponent<Animation>()["Mars_Attack02"];
			attack03 = GetComponent<Animation>()["Mars_Attack03"];
			attack04 = GetComponent<Animation>()["Mars_Attack04"];
			attack05 = GetComponent<Animation>()["Mars_Attack05"];
			attack1_Final = GetComponent<Animation>()["Mars_Attack02"];
			attack2_Final = GetComponent<Animation>()["Mars_Attack2_Final"];
			attack3_Final = GetComponent<Animation>()["Mars_Attack04"];
			attack4_Final = GetComponent<Animation>()["Mars_Attack4_Final"];
			attack5_Final = GetComponent<Animation>()["Mars_Attack5_Final"];

			runAttackUpper = GetComponent<Animation>()["Mars_Run_FB_AttackUpper"];
			runForwardAttack = GetComponent<Animation>()["Mars_Run_FB_AttackUpper"];
			runBackAttack = GetComponent<Animation>()["Mars_Run_FB_AttackUpper"];
			runLeftAttack = GetComponent<Animation>()["Mars_Run_FB_AttackUpper"];
			runRightAttack = GetComponent<Animation>()["Mars_Run_FB_AttackUpper"];
			runLeftForwardAttack = GetComponent<Animation>()["Mars_Run_LFRB_AttackUpper"];
			runRightForwardAttack = GetComponent<Animation>()["Mars_Run_RFLB_AttackUpper"];
			runLeftBackAttack = GetComponent<Animation>()["Mars_Run_RFLB_AttackUpper"];
			runRightBackAttack = GetComponent<Animation>()["Mars_Run_LFRB_AttackUpper"];
			skill01 = GetComponent<Animation>()["Mars_Assault"];
			skill02 = GetComponent<Animation>()["Mars_Stun"];
			skill03 = GetComponent<Animation>()["Mars_FightLight"];
			skill04 = GetComponent<Animation>()["Mars_AresBlessing"];
			superSkill = GetComponent<Animation>()["Mars_HolyRedeemer"];
			freeze = GetComponent<Animation>()["Mars_Freeze"];
			dizzy = GetComponent<Animation>()["Mars_Dizzy"];
			beHit = GetComponent<Animation>()["Mars_Behit"];
			knockDown = GetComponent<Animation>()["Mars_KnockDown"];
			standUp = GetComponent<Animation>()["Mars_StandUp"];
			die = GetComponent<Animation>()["Mars_Die"];
			
			//Change Animation Speed
			//goForward.speed = 1.1f;
			runAttackUpper.speed = 2.5f;
			runForwardAttack.speed = 2.5f;
			runLeftAttack.speed = 2.5f;
			runBackAttack.speed = 2.5f;
			runRightAttack.speed = 2.5f;
			runLeftForwardAttack.speed = 2.5f;
			runRightForwardAttack.speed = 2.5f;
			runLeftBackAttack.speed = 2.5f;
			runRightBackAttack.speed = 2.5f;
			beHit.speed = 1.5f;
			dizzy.speed = 2f;
			storeEnergy.speed = 2;
			knockDown.speed = 2;
			standUp.speed = 3;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			attack01.speed = 2.7f;
			attack02.speed = 2.7f;
			attack03.speed = 2.7f;
			attack04.speed = 2.7f;
			attack05.speed = 2.7f;
			attack1_Final.speed = 2.7f;
			attack2_Final.speed = 1.3f;
			attack3_Final.speed = 2.7f;
			attack4_Final.speed = 2.7f;
			attack5_Final.speed = 2.7f;
			skill01.speed = 1.3f;
			skill02.speed = 2f;
			skill03.speed = 2f;
			skill04.speed = 2f;
			superSkill.speed = 2f;
			break;
		case TP_Info.CharacterRole.DarkMan:
			//create new animation for running attack
			MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Darkman_GoLeft"];
			goRight = GetComponent<Animation>()["Darkman_GoRight"];
			goBack = GetComponent<Animation>()["Darkman_GoBack"];
			goForward = GetComponent<Animation>()["Darkman_GoForward"];
			goRightForward = GetComponent<Animation>()["Darkman_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Darkman_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Darkman_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Darkman_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Darkman_RollLeft"];
			rollRight = GetComponent<Animation>()["Darkman_RollRight"];
			//jump = animation["Darkman_Jump"];
			//jumpLand = animation["Darkman_JumpLand"];
			//spinIdle = animation["Darkman"]
			showIdle = GetComponent<Animation>()["Darkman_ShowIdle"];
			idle = GetComponent<Animation>()["Darkman_Idle"];
			storeEnergy = GetComponent<Animation>()["Darkman_StoreEnergy"];
			//spinIdle = animation["Darkman_SpinIdle"];
			attack01 = GetComponent<Animation>()["Darkman_Attack01"];
			attack02 = GetComponent<Animation>()["Darkman_Attack02"];
			attack03 = GetComponent<Animation>()["Darkman_Attack03"];
			attack04 = GetComponent<Animation>()["Darkman_Attack04"];
			attack05 = GetComponent<Animation>()["Darkman_Attack05"];
			attack1_Final = GetComponent<Animation>()["Darkman_Attack1_Final"];
			attack2_Final = GetComponent<Animation>()["Darkman_Attack2_Final"];
			attack3_Final = GetComponent<Animation>()["Darkman_Attack3_Final"];
			attack4_Final = GetComponent<Animation>()["Darkman_Attack4_Final"];
			attack5_Final = GetComponent<Animation>()["Darkman_Attack5_Final"];
			runAttackUpper = GetComponent<Animation>()["Darkman_RunAttackUpper"];
			runForwardAttack = GetComponent<Animation>()["Darkman_RunAttackUpper"];
			runBackAttack = GetComponent<Animation>()["Darkman_RunBackAttackUpper"];
			runLeftAttack = GetComponent<Animation>()["Darkman_RunLeftAttackUpper"];
			runRightAttack = GetComponent<Animation>()["Darkman_RunRightAttackUpper"];
			runLeftForwardAttack = GetComponent<Animation>()["Darkman_RunLeftForwardAttackUpper"];
			runRightForwardAttack = GetComponent<Animation>()["Darkman_RunRightForwardAttackUpper"];
			runLeftBackAttack = GetComponent<Animation>()["Darkman_RunLeftBackAttackUpper"];
			runRightBackAttack = GetComponent<Animation>()["Darkman_RunAttackUpper"];
			skill01 = GetComponent<Animation>()["Darkman_Spurt"];
			skill02 = GetComponent<Animation>()["Darkman_IceFreeze"];
			skill03 = GetComponent<Animation>()["Darkman_TornadoLightning"];
			skill04 = GetComponent<Animation>()["Darkman_DemonProtection"];
			superSkill = GetComponent<Animation>()["Darkman_DepravedCrush"];
			freeze = GetComponent<Animation>()["Darkman_Freeze"];
			dizzy = GetComponent<Animation>()["Darkman_Dizzy"];
			beHit = GetComponent<Animation>()["Darkman_Behit"];
			knockDown = GetComponent<Animation>()["Darkman_KnockDown"];
			standUp = GetComponent<Animation>()["Darkman_StandUp"];
			die = GetComponent<Animation>()["Darkman_Die"];

			//Change Animation Speed
			goForward.speed = 1.1f;
			runAttackUpper.speed = 2.5f;
			runForwardAttack.speed = 2.5f;
			runLeftAttack.speed = 2.5f;
			runBackAttack.speed = 2.5f;
			runRightAttack.speed = 2.5f;
			runLeftForwardAttack.speed = 2.5f;
			runRightForwardAttack.speed = 2.5f;
			runLeftBackAttack.speed = 2.5f;
			runRightBackAttack.speed = 2.5f;
			beHit.speed = 1.5f;
			storeEnergy.speed = 2;
			knockDown.speed = 2;
			standUp.speed = 3;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			attack01.speed = 2.5f;
			attack02.speed = 2.5f;
			attack03.speed = 2.5f;
			attack04.speed = 2.5f;
			attack05.speed = 2.5f;
			attack1_Final.speed = 2.5f;
			attack2_Final.speed = 2.5f;
			attack3_Final.speed = 3f;
			attack4_Final.speed = 2f;
			attack5_Final.speed = 2f;
			skill01.speed = 1.3f;
			skill02.speed = 2f;
			skill03.speed = 2f;
			skill04.speed = 2f;
			superSkill.speed = 3.5f;
			break;
		case TP_Info.CharacterRole.Theia:
			//create new animation for running attack
			MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Theia_GoLeft"];
			goRight = GetComponent<Animation>()["Theia_GoRight"];
			goBack = GetComponent<Animation>()["Theia_GoBack"];
			goForward = GetComponent<Animation>()["Theia_GoForward"];
			goRightForward = GetComponent<Animation>()["Theia_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Theia_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Theia_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Theia_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Theia_RollLeft"];
			rollRight = GetComponent<Animation>()["Theia_RollRight"];
			showIdle = GetComponent<Animation>()["Theia_ShowIdle"];
			idle = GetComponent<Animation>()["Theia_Idle"];
			storeEnergy = GetComponent<Animation>()["Theia_StoreEnergy"];
			magicAttack = GetComponent<Animation>()["Theia_MagicAttack"];
			storeMagic = GetComponent<Animation>()["Theia_StoreMagic"];
			storedMagicAttack = GetComponent<Animation>()["Theia_StoredMagicAttack"];
			runAttackUpper = GetComponent<Animation>()["Theia_RunAttackUpper"];
			runForwardAttack = GetComponent<Animation>()["Theia_RunForwardMagicAttackUpper"];
			runBackAttack = GetComponent<Animation>()["Theia_RunBackMagicAttackUpper"];
			runLeftAttack = GetComponent<Animation>()["Theia_RunAttackUpper"];
			runRightAttack = GetComponent<Animation>()["Theia_RunAttackUpper"];
			runLeftForwardAttack = GetComponent<Animation>()["Theia_RunLeftForwardMagicAttackUpper"];
			runRightForwardAttack = GetComponent<Animation>()["Theia_RunAttackUpper"];
			runLeftBackAttack = GetComponent<Animation>()["Theia_RunAttackUpper"];
			runRightBackAttack = GetComponent<Animation>()["Theia_RunRightBackMagicAttackUpper"];
			skill01 = GetComponent<Animation>()["Theia_CrossFire"];
			skill02 = GetComponent<Animation>()["Theia_Meteorite"];
			skill03 = GetComponent<Animation>()["Theia_Pray"];
			skill04 = GetComponent<Animation>()["Theia_Crusade"];
			superSkill = GetComponent<Animation>()["Theia_Doom"];
			freeze = GetComponent<Animation>()["Theia_Freeze"];
			dizzy = GetComponent<Animation>()["Theia_Dizzy"];
			beHit = GetComponent<Animation>()["Theia_Behit"];
			knockDown = GetComponent<Animation>()["Theia_KnockDown"];
			standUp = GetComponent<Animation>()["Theia_StandUp"];
			die = GetComponent<Animation>()["Theia_Die"];

			runAttackUpper.speed = 2;
			runForwardAttack.speed = 2;
			runLeftAttack.speed = 2;
			runBackAttack.speed = 2;
			runRightAttack.speed = 2;
			runLeftForwardAttack.speed = 2;
			runRightForwardAttack.speed = 2;
			runLeftBackAttack.speed = 2;
			runRightBackAttack.speed = 2;
			storeMagic.speed = 2;
			storedMagicAttack.speed = 2.5f;
			skill01.speed = 4;
			skill02.speed = 4;
			skill03.speed = 3;
			skill04.speed = 5;
			superSkill.speed = 4.5f;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			knockDown.speed = 2;
			standUp.speed = 2f;
			die.speed = 1.5f;
			break;
		case TP_Info.CharacterRole.Persia:
			//create new animation for running attack
			//MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Persia_GoLeft"];
			goRight = GetComponent<Animation>()["Persia_GoRight"];
			goBack = GetComponent<Animation>()["Persia_GoBack"];
			goForward = GetComponent<Animation>()["Persia_GoForward"];
			goRightForward = GetComponent<Animation>()["Persia_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Persia_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Persia_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Persia_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Persia_RollLeft"];
			rollRight = GetComponent<Animation>()["Persia_RollRight"];
			showIdle = GetComponent<Animation>()["Persia_ShowIdle"];
			idle = GetComponent<Animation>()["Persia_Idle"];
			storeEnergy = GetComponent<Animation>()["Persia_StoreEnergy"];
			magicAttack = GetComponent<Animation>()["Persia_MagicAttack"];
			storeMagic = GetComponent<Animation>()["Persia_StoreMagic"];
			storedMagicAttack = GetComponent<Animation>()["Persia_StoredMagicAttack"];
			runAttackUpper = GetComponent<Animation>()["Persia_MagicAttack"];
			runForwardAttack = GetComponent<Animation>()["Persia_MagicAttack"];
			runBackAttack = GetComponent<Animation>()["Persia_MagicAttack"];
			runLeftAttack = GetComponent<Animation>()["Persia_MagicAttack"];
			runRightAttack = GetComponent<Animation>()["Persia_MagicAttack"];
			runLeftForwardAttack = GetComponent<Animation>()["Persia_RunLeftForward_RightBackMagicAttack"];
			runRightForwardAttack = GetComponent<Animation>()["Persia_RightForward_LeftBackMagicAttack"];
			runLeftBackAttack = GetComponent<Animation>()["Persia_RightForward_LeftBackMagicAttack"];
			runRightBackAttack = GetComponent<Animation>()["Persia_RunLeftForward_RightBackMagicAttack"];
			skill01 = GetComponent<Animation>()["Persia_ThunderousWave"];
			skill02 = GetComponent<Animation>()["Persia_Hail"];
			skill03 = GetComponent<Animation>()["Persia_GroundedLightning"];
			skill04 = GetComponent<Animation>()["Persia_Curse"];
			superSkill = GetComponent<Animation>()["Persia_ScatterSpear"];
			freeze = GetComponent<Animation>()["Persia_Freeze"];
			dizzy = GetComponent<Animation>()["Persia_Dizzy"];
			beHit = GetComponent<Animation>()["Persia_Behit"];
			knockDown = GetComponent<Animation>()["Persia_KnockDown"];
			standUp = GetComponent<Animation>()["Persia_StandUp"];
			die = GetComponent<Animation>()["Persia_Die"];
			
			//runAttackUpper.speed = 2;
			runForwardAttack.speed = 2;
			runLeftAttack.speed = 2;
			runBackAttack.speed = 2;
			runRightAttack.speed = 2;
			runLeftForwardAttack.speed = 2;
			runRightForwardAttack.speed = 2;
			runLeftBackAttack.speed = 2;
			runRightBackAttack.speed = 2;
			storeMagic.speed = 2;
			storedMagicAttack.speed = 3;
			skill01.speed = 3;
			skill02.speed = 4;
			skill03.speed = 3;
			skill04.speed = 5;
			superSkill.speed = 2;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			knockDown.speed = 1.5f;
			standUp.speed = 1.3f;
			superSkill.speed = 4;
			//die.speed = 1.5f;
			break;

		case TP_Info.CharacterRole.Hyperion:
			//create new animation for running attack
			MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Hyperion_GoLeft"];
			goRight = GetComponent<Animation>()["Hyperion_GoRight"];
			goBack = GetComponent<Animation>()["Hyperion_GoBack"];
			goForward = GetComponent<Animation>()["Hyperion_GoForward"];
			goRightForward = GetComponent<Animation>()["Hyperion_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Hyperion_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Hyperion_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Hyperion_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Hyperion_RollLeft"];
			rollRight = GetComponent<Animation>()["Hyperion_RollRight"];
			//showIdle = animation["Hyperion_ShowIdle"];
			idle = GetComponent<Animation>()["Hyperion_Idle"];
			storeEnergy = GetComponent<Animation>()["Hyperion_StoreEnergy"];
			attack01 = GetComponent<Animation>()["Hyperion_Attack01"];
			attack02 = GetComponent<Animation>()["Hyperion_Attack02"];
			attack03 = GetComponent<Animation>()["Hyperion_Attack03"];
			attack04 = GetComponent<Animation>()["Hyperion_Attack04"];
			attack05 = GetComponent<Animation>()["Hyperion_Attack05"];
			attack1_Final = GetComponent<Animation>()["Hyperion_Attack02"];
			attack2_Final = GetComponent<Animation>()["Hyperion_Attack2_Final"];
			attack3_Final = GetComponent<Animation>()["Hyperion_Attack3_Final"];
			attack4_Final = GetComponent<Animation>()["Hyperion_Attack4_Final"];
			attack5_Final = GetComponent<Animation>()["Hyperion_Attack5_Final"];
			runAttackUpper = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runForwardAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runBackAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runLeftAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runRightAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runLeftForwardAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			runRightForwardAttack = GetComponent<Animation>()["Hyperion_Run_RFLB_AttackUpper"];
			runLeftBackAttack = GetComponent<Animation>()["Hyperion_Run_RFLB_AttackUpper"];
			runRightBackAttack = GetComponent<Animation>()["Hyperion_Run_FB_AttackUpper"];
			skill01 = GetComponent<Animation>()["Hyperion_HotRock"];
			skill02 = GetComponent<Animation>()["Hyperion_Unbeatable"];
			skill03 = GetComponent<Animation>()["Hyperion_BombSpreading"];
			skill04 = GetComponent<Animation>()["Hyperion_Intimadate"];
			superSkill = GetComponent<Animation>()["Hyperion_SpiningLight"];
			freeze = GetComponent<Animation>()["Hyperion_Freeze"];
			dizzy = GetComponent<Animation>()["Hyperion_Dizzy"];
			beHit = GetComponent<Animation>()["Hyperion_Behit"];
			knockDown = GetComponent<Animation>()["Hyperion_KnockDown"];
			standUp = GetComponent<Animation>()["Hyperion_StandUp"];
			die = GetComponent<Animation>()["Hyperion_Die"];

			goLeft.speed = 1.2f;
			goRight.speed = 1.2f;
			goForward.speed = 1.2f;
			goBack.speed = 1.2f;
			goLeftForward.speed = 1.2f;
			goLeftBack.speed = 1.2f;
			goRightForward.speed = 1.2f;
			goRightBack.speed = 1.2f;

			//attack01.speed = 2f;
			attack02.speed = 1.5f;
			attack03.speed = 1.5f;
			attack04.speed = 1.5f;
			attack05.speed = 1.5f;
			attack1_Final.speed = 1.5f;
			attack2_Final.speed = 2f;
			attack3_Final.speed = 2f;
			//attack4_Final.speed = 1f;
			attack5_Final.speed = 1.5f;
			
			runAttackUpper.speed = 2.5f;
			runForwardAttack.speed = 2.5f;
			runLeftAttack.speed = 2.5f;
			runBackAttack.speed = 2.5f;
			runRightAttack.speed = 2.5f;
			runLeftForwardAttack.speed = 2.5f;
			runRightForwardAttack.speed = 2.5f;
			runLeftBackAttack.speed = 2.5f;
			runRightBackAttack.speed = 2.5f;
			skill01.speed = 5f;
			skill02.speed = 2.8f;
			skill03.speed = 3.5f;
			skill04.speed = 3;
			superSkill.speed = 5;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			knockDown.speed = 2.5f;
			standUp.speed = 2.2f;
			die.speed = 3f;
			break;

		case TP_Info.CharacterRole.Steropi:
			//create new animation for running attack
			MixAnimation(playerInfo.Role);
			//Get AnimationState
			goLeft = GetComponent<Animation>()["Steropi_GoLeft"];
			goRight = GetComponent<Animation>()["Steropi_GoRight"];
			goBack = GetComponent<Animation>()["Steropi_GoBack"];
			goForward = GetComponent<Animation>()["Steropi_GoForward"];
			goRightForward = GetComponent<Animation>()["Steropi_GoRightForward"];
			goLeftForward = GetComponent<Animation>()["Steropi_GoLeftForward"];
			goRightBack = GetComponent<Animation>()["Steropi_GoRightBack"];
			goLeftBack = GetComponent<Animation>()["Steropi_GoLeftBack"];
			rollLeft = GetComponent<Animation>()["Steropi_RollLeft"];
			rollRight = GetComponent<Animation>()["Steropi_RollRight"];
			showIdle = GetComponent<Animation>()["Steropi_ShowIdle"];
			idle = GetComponent<Animation>()["Steropi_Idle"];
			storeEnergy = GetComponent<Animation>()["Steropi_StoreEnergy"];
			attack01 = GetComponent<Animation>()["Steropi_Attack01"];
			attack02 = GetComponent<Animation>()["Steropi_Attack02"];
			attack03 = GetComponent<Animation>()["Steropi_Attack03"];
			attack04 = GetComponent<Animation>()["Steropi_Attack04"];
			attack05 = GetComponent<Animation>()["Steropi_Attack05"];
			attack1_Final = GetComponent<Animation>()["Steropi_Attack02"];
			attack2_Final = GetComponent<Animation>()["Steropi_Attack2_Final"];
			attack3_Final = GetComponent<Animation>()["Steropi_Attack3_Final"];
			attack4_Final = GetComponent<Animation>()["Steropi_Attack4_Final"];
			attack5_Final = GetComponent<Animation>()["Steropi_Attack5_Final"];
			runAttackUpper = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runForwardAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runBackAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runLeftAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runRightAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runLeftForwardAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			runRightForwardAttack = GetComponent<Animation>()["Steropi_Run_RFLB_AttackUpper"];
			runLeftBackAttack = GetComponent<Animation>()["Steropi_Run_RFLB_AttackUpper"];
			runRightBackAttack = GetComponent<Animation>()["Steropi_Run_FB_AttackUpper"];
			skill01 = GetComponent<Animation>()["Steropi_SnowBall"];
			skill02 = GetComponent<Animation>()["Steropi_RollingStrike"];
			skill03 = GetComponent<Animation>()["Steropi_SlamCrush"];
			skill04 = GetComponent<Animation>()["Steropi_Roar"];
			superSkill = GetComponent<Animation>()["Steropi_LightningStorm"];
			freeze = GetComponent<Animation>()["Steropi_Freeze"];
			dizzy = GetComponent<Animation>()["Steropi_Dizzy"];
			beHit = GetComponent<Animation>()["Steropi_Behit"];
			knockDown = GetComponent<Animation>()["Steropi_KnockDown"];
			standUp = GetComponent<Animation>()["Steropi_StandUp"];
			die = GetComponent<Animation>()["Steropi_Die"];

			goLeft.speed = 1.2f;
			goRight.speed = 1.2f;
			goForward.speed = 1.2f;
			goBack.speed = 1.2f;
			goLeftForward.speed = 1.2f;
			goLeftBack.speed = 1.2f;
			goRightForward.speed = 1.2f;
			goRightBack.speed = 1.2f;

			attack01.speed = 2f;
			attack02.speed = 2f;
			attack03.speed = 2f;
			attack04.speed = 2f;
			attack05.speed = 2f;
			attack1_Final.speed = 2f;
			attack2_Final.speed = 2f;
			attack3_Final.speed = 2f;
			attack4_Final.speed = 1f;
			attack5_Final.speed = 2.5f;

			runAttackUpper.speed = 2f;
			runForwardAttack.speed = 2f;
			runLeftAttack.speed = 2f;
			runBackAttack.speed = 2f;
			runRightAttack.speed = 2f;
			runLeftForwardAttack.speed = 2f;
			runRightForwardAttack.speed = 2f;
			runLeftBackAttack.speed = 2f;
			runRightBackAttack.speed = 2f;
			skill01.speed = 4;
			skill02.speed = 1.7f;
			skill03.speed = 3;
			skill04.speed = 5;
			superSkill.speed = 3;
			rollLeft.speed = 2;
			rollRight.speed = 2;
			knockDown.speed = 2.5f;
			standUp.speed = 2.2f;
			die.speed = 3f;
			break;
		case TP_Info.CharacterRole.Monster:
			idle = GetComponent<Animation>()["Monster_Idle"];
			goForward = GetComponent<Animation>()["Monster_Walk"];
			die = GetComponent<Animation>()["Monster_Die"];
			beHit = GetComponent<Animation>()["Monster_Behit"];
			attack01 = GetComponent<Animation>()["Monster_Attack01"];
			attack02 = GetComponent<Animation>()["Monster_Attack02"];
			freeze = GetComponent<Animation>()["Monster_Freeze"];
			dizzy = GetComponent<Animation>()["Monster_Dizzy"];
			knockDown = GetComponent<Animation>()["Monster_KnockDown"];
			standUp = GetComponent<Animation>()["Monster_StandUp"];
			break;
		case TP_Info.CharacterRole.MonsterBoss:
			idle = GetComponent<Animation>()["MonsterBoss_Idle"];
			goForward = GetComponent<Animation>()["MonsterBoss_Walk"];
			die = GetComponent<Animation>()["MonsterBoss_Die"];
			beHit = GetComponent<Animation>()["MonsterBoss_Behit"];
			attack01 = GetComponent<Animation>()["MonsterBoss_Attack01"];
			attack02 = GetComponent<Animation>()["MonsterBoss_Attack02"];
			attack03 = GetComponent<Animation>()["MonsterBoss_Attack03"];
			freeze = GetComponent<Animation>()["MonsterBoss_Freeze"];
			dizzy = GetComponent<Animation>()["Monster_Dizzy"];
			knockDown = GetComponent<Animation>()["Monster_KnockDown"];
			standUp = GetComponent<Animation>()["Monster_StandUp"];
			break;
		}
	}

	//Check if player can move
	public bool CanMove()
	{
		var canmove = true;
		if( 
		   //State == CharacterState.Falling|| 
		  // State == CharacterState.Jumping|| 
	      // State == CharacterState.Landing||
		   State == CharacterState.Dead||
		   State == CharacterState.Freeze||
		   State == CharacterState.Beinghit||
		   State == CharacterState.Dizzing||
		   State == CharacterState.KnockingDown||
		   State == CharacterState.StandingUp||
		   State == CharacterState.Rolling||
	  	   State == CharacterState.Attacking001||
		   State == CharacterState.Attacking002||
		   State == CharacterState.Attacking003||
		   State == CharacterState.Attacking004||
		   State == CharacterState.Attacking005||
		   State == CharacterState.Attacking1Final||
		   State == CharacterState.Attacking2Final||
		   State == CharacterState.Attacking3Final||
		   State == CharacterState.Attacking4Final||
		   State == CharacterState.Attacking5Final||
		   State == CharacterState.MagicStoring||
		   State == CharacterState.MagicStoredAttacking||
		   State == CharacterState.Skilling01||
		   State == CharacterState.Skilling02||
		   State == CharacterState.Skilling03||
		   State == CharacterState.Skilling04||
		   State == CharacterState.SuperSkilling||
		   State == CharacterState.ThrowingBigRock||
		   State == CharacterState.ThrowingFireWork||
		   State == CharacterState.EnergyStoring
		   )
			canmove = false;
		return canmove;
	}

	public bool CanAttack()
	{
		var canAttack = true;
		if(
		   State == CharacterState.Dead||
		   State == CharacterState.Freeze||
		   State == CharacterState.Beinghit||
		   State == CharacterState.Dizzing||
		   State == CharacterState.KnockingDown||
		   State == CharacterState.StandingUp||
		   State == CharacterState.Rolling
		   )
			canAttack = false;
		return canAttack;

	}

	void BeinghitSound()
	{
		if(beHitSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != beHitSound)
			{
				mainAudio.clip = beHitSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void KnockingDownSound()
	{
		if(knockDownSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != knockDownSound)
			{
				mainAudio.clip = knockDownSound;;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void StandingUpSound()
	{
		if(standUpSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != standUpSound)
			{
				mainAudio.clip = standUpSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void IdleSound()
	{
		if(walkAudio!=null)
		{
			if(idleSound==null)
				walkAudio.clip = null;
			else
			{
				if(walkAudio.clip != idleSound)
				{
					walkAudio.clip = idleSound;
					walkAudio.Play();
					for(int cnt =0;cnt<finalAttackSoundBool.Length;cnt++)
					{finalAttackSoundBool[cnt] = false;}
					mainAudio.clip = null;
				}
			}
		}
	}
	void RunningForwardSound()
	{
		if(walkAudio!=null)
		{
			if(runningForwardSound==null)
				walkAudio.clip = null;
			else
			{
				if(walkAudio.clip != runningForwardSound)
				{
					walkAudio.clip = runningForwardSound;
					walkAudio.Play();
					mainAudio.clip = null;
				}
				}
		}
	}
	void RunningBackSound()
	{
		if(runningBackSound==null)
			walkAudio.clip = null;
		else
		{
			if(walkAudio.clip != runningBackSound)
			{
				walkAudio.clip = runningBackSound;
				walkAudio.Play();
				mainAudio.clip = null;
			}
		}
	}
	void RollingSound()
	{
		if(rollSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != rollSound)
			{
				mainAudio.clip = rollSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void DyingSound()
	{
		if(dieSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != dieSound)
			{
				mainAudio.clip = dieSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void UpperAttackingSound()
	{
		if(runAttackUpperSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != runAttackUpperSound)
			{
				mainAudio.clip = runAttackUpperSound;
				mainAudio.Play();
			}
		}
		//CheckWalkAudio();
	}

	void NormalAttackingSound()
	{
		if(normalAttackSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != normalAttackSound)
			{
				mainAudio.clip = normalAttackSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}

	void Attacking1FinalSound()
	{
		if(attack1_FinalSound!=null)
		//	mainAudio.clip = null;
		//else
		{
			if(!finalAttackSoundBool[0])
			{
				mainAudio.clip = null;
				finalAttackSoundBool[0] = true;
				if(audioPrefab!=null)
				{
					GameObject sound = GameObject.Instantiate(audioPrefab,transform.position,transform.rotation)as GameObject;
					AudioSource soundSource = sound.GetComponent<AudioSource>();
					soundSource.clip = attack1_FinalSound;
					soundSource.Play();
					Destroy(sound,3);
				}
			}
			/*if(mainAudio.clip != attack1_FinalSound)
			{
				mainAudio.clip = attack1_FinalSound;
				mainAudio.Play();
			}*/
		}
		CheckWalkAudio();
	}

	void Attacking2FinalSound()
	{
		if(attack2_FinalSound!=null)
		//	mainAudio.clip = null;
		//else
		{
			if(!finalAttackSoundBool[1])
			{
				mainAudio.clip = null;
				finalAttackSoundBool[1] = true;
				if(audioPrefab!=null)
				{
					GameObject sound = GameObject.Instantiate(audioPrefab,transform.position,transform.rotation)as GameObject;
					AudioSource soundSource = sound.GetComponent<AudioSource>();
					soundSource.clip = attack2_FinalSound;
					soundSource.Play();
					Destroy(sound,3);
				}
			}
		}
		CheckWalkAudio();
	}
	void Attacking3FinalSound()
	{
		if(attack3_FinalSound!=null)
		{
			if(!finalAttackSoundBool[2])
			{
				mainAudio.clip = null;
				finalAttackSoundBool[2] = true;
				if(audioPrefab!=null)
				{
					GameObject sound = GameObject.Instantiate(audioPrefab,transform.position,transform.rotation)as GameObject;
					AudioSource soundSource = sound.GetComponent<AudioSource>();
					soundSource.clip = attack3_FinalSound;
					soundSource.Play();
					Destroy(sound,3);
				}
			}
		}
		CheckWalkAudio();
	}
	void Attacking4FinalSound()
	{
		if(attack4_FinalSound!=null)
		
		{
			if(!finalAttackSoundBool[3])
			{
				mainAudio.clip = null;
				finalAttackSoundBool[3] = true;
				if(audioPrefab!=null)
				{
					GameObject sound = GameObject.Instantiate(audioPrefab,transform.position,transform.rotation)as GameObject;
					AudioSource soundSource = sound.GetComponent<AudioSource>();
					soundSource.clip = attack4_FinalSound;
					soundSource.Play();
					Destroy(sound,3);
				}
			}
			/*if(mainAudio.clip != attack1_FinalSound)
			{
				mainAudio.clip = attack1_FinalSound;
				mainAudio.Play();
			}*/
		}
		CheckWalkAudio();
	}
	void Attacking5FinalSound()
	{
		if(attack5_FinalSound!=null)
		
		{
			if(!finalAttackSoundBool[4])
			{
				mainAudio.clip = null;
				finalAttackSoundBool[4] = true;
				if(audioPrefab!=null)
				{
					GameObject sound = GameObject.Instantiate(audioPrefab,transform.position,transform.rotation)as GameObject;
					AudioSource soundSource = sound.GetComponent<AudioSource>();
					soundSource.clip = attack5_FinalSound;
					soundSource.Play();
					Destroy(sound,5);
				}
			}
		}
		CheckWalkAudio();
	}
	void MagicAttackingSound()
	{
		if(magicAttackSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != magicAttackSound)
			{
				mainAudio.clip = magicAttackSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}

	void StoredMagicAttackingSound()
	{
		if(storedMagicAttackSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != storedMagicAttackSound)
			{
				mainAudio.clip = storedMagicAttackSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}

	void MagicStoringSound()
	{
		if(storeMagicSound==null)
			walkAudio.clip = null;
		else
		{
			if(walkAudio.clip != storeMagicSound)
			{
				walkAudio.clip = storeMagicSound;
				walkAudio.Play();
			}
		}
	}
	void Skilling1Sound()
	{
		if(skill01Sound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != skill01Sound)
			{
				mainAudio.clip = skill01Sound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void Skilling2Sound()
	{
		if(skill02Sound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != skill02Sound)
			{
				mainAudio.clip = skill02Sound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void Skilling3Sound()
	{
		if(skill03Sound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != skill03Sound)
			{
				mainAudio.clip = skill03Sound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void Skilling4Sound()
	{
		if(skill04Sound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != skill04Sound)
			{
				mainAudio.clip = skill04Sound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void SuperSkillingSound()
	{
		if(superSkillSound==null)
			mainAudio.clip = null;
		else
		{
			if(mainAudio.clip != superSkillSound)
			{
				mainAudio.clip = superSkillSound;
				mainAudio.Play();
			}
		}
		CheckWalkAudio();
	}
	void StoringEnergySound()
	{
		if(storeEnergySound=null)
			mainAudio.clip = null;
		/*else
			mainAudio.clip = storeEnergySound;
		mainAudio.Play();*/
		CheckWalkAudio();
	}
	
	void CheckWalkAudio()
	{
		if(_moveDirection==Direction.Stationary)
		{
			walkAudio.clip = null;
			//walkAudio.Play();
		}
	}

	void CheckMainAudio()
	{
		if(_moveDirection!=Direction.Stationary)
		{
			mainAudio.clip = null;
		}
	}

	//Call method based on character state
	void ProcessCurrentState()
	{
		if((photonView.isMine&&!playerInfo.isAI)||(playerInfo.isAI&&PhotonNetwork.isMasterClient))
		{
		switch(State)
		{
		case CharacterState.Beinghit:
			Beinghit();
				BeinghitSound();
			break;
		case CharacterState.MagicStoring:
				MagicStoringSound();
				break;
		case CharacterState.KnockingDown:
			KnockingDown();
				KnockingDownSound();
			break;
		case CharacterState.StandingUp:
			StandingUp();
				StandingUpSound();
			break;
		case CharacterState.Freeze:
			GetComponent<Animation>().CrossFade(freeze.name);
			break;
		case CharacterState.Dizzing:
			GetComponent<Animation>().CrossFade(dizzy.name);
			break;
		case CharacterState.Idle:
			Idle();
				IdleSound();
			break;
		case CharacterState.ShowIdle:
			GetComponent<Animation>().CrossFade(showIdle.name);
			break;
		case CharacterState.Running:
			lockAnimating = false;//To avoid lockanimation error
			if(playerInfo.isMonster)
			{
				GetComponent<Animation>().CrossFade(goForward.name);
				RunningForwardSound();
			}
			else
				Running();
			break;
		case CharacterState.Rolling:
			Rolling();
				RollingSound();
			break;
		case CharacterState.Dead:
			Dying();
				DyingSound();
			break;
		case CharacterState.RunningAttacking001:
			Attacking();
				UpperAttackingSound();
			break;
		case CharacterState.Attacking001:
			Attacking();
				NormalAttackingSound();
			break;
		case CharacterState.Attacking002:
			Attacking();
			break;
		case CharacterState.Attacking003:
			Attacking();
			break;
		case CharacterState.Attacking004:
			Attacking();
			break;
		case CharacterState.Attacking005:
			Attacking();
			break;
		case CharacterState.Attacking1Final:
			Attacking();
				Attacking1FinalSound();
			break;
		case CharacterState.Attacking2Final:
			Attacking();
				Attacking2FinalSound();
			break;
		case CharacterState.Attacking3Final:
			Attacking();
				Attacking3FinalSound();
			break;
		case CharacterState.Attacking4Final:
			Attacking();
				Attacking4FinalSound();
			break;	
		case CharacterState.Attacking5Final:
			Attacking();
				Attacking5FinalSound();
			break;	
		case CharacterState.MagicAttacking1:
			Attacking();
				MagicAttackingSound();
			break;
		case CharacterState.MagicStoredAttacking:
			Attacking();
				StoredMagicAttackingSound();
			break;
		case CharacterState.Skilling01:
			Attacking();
				Skilling1Sound();
			break;
		case CharacterState.Skilling02:
			Attacking();
				Skilling2Sound();
			break;
		case CharacterState.Skilling03:
			Attacking();
				Skilling3Sound();
			break;
		case CharacterState.Skilling04:
			Attacking();
				Skilling4Sound();
			break;
		case CharacterState.SuperSkilling:
			Attacking();
				SuperSkillingSound();
			break;
		case CharacterState.ThrowingBigRock:
			Attacking();
			break;
		case CharacterState.ThrowingFireWork:
			Attacking();
			break;
		}
		}
		else
		{
			switch(State)
			{

			case CharacterState.EnergyStoring:
				GetComponent<Animation>().CrossFade(storeEnergy.name);
				break;
			case CharacterState.MagicStoring:
				GetComponent<Animation>().CrossFade(storeMagic.name);
				MagicStoringSound();
				break;
			case CharacterState.Beinghit:
				Beinghit();
				BeinghitSound();
				break;
			case CharacterState.KnockingDown:
				KnockingDown();
				KnockingDownSound();
				break;
			case CharacterState.StandingUp:
				StandingUp();
				StandingUpSound();
				break;
			case CharacterState.Freeze:
				GetComponent<Animation>().CrossFade(freeze.name);
				break;
			case CharacterState.Dizzing:
				GetComponent<Animation>().CrossFade(dizzy.name);
				break;
			case CharacterState.Idle:
				Idle();
				IdleSound();
				break;
			case CharacterState.Running:
				lockAnimating = false;//To avoid lockanimation error
				Running();
				break;
			case CharacterState.Rolling:
				if(_moveDirection == Direction.Left||_moveDirection == Direction.LeftForward)
				{
					currentAttack = rollLeft.clip;
					GetComponent<Animation>().CrossFade(rollLeft.name);
				}
				else if(_moveDirection == Direction.Right||_moveDirection == Direction.RightForward)
				{
					currentAttack = rollRight.clip;
					GetComponent<Animation>().CrossFade(rollRight.name);
				}
				RollingSound();
				break;
			case CharacterState.Dead:
				GetComponent<Animation>().CrossFade(die.name);
				DyingSound();
				break;
			case CharacterState.RunningAttacking001:
				switch(MoveDirection)
				{
				case Direction.Forward:
					GetComponent<Animation>().CrossFade(runForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.Backward:
					GetComponent<Animation>().CrossFade(runBackAttack.name);
					RunningBackSound();
					break;
				case Direction.Left:
					GetComponent<Animation>().CrossFade(runLeftAttack.name);
					RunningBackSound();
					break;
				case Direction.Right:
					GetComponent<Animation>().CrossFade(runRightAttack.name);
					RunningBackSound();
					break;
				case Direction.LeftForward:
					GetComponent<Animation>().CrossFade(runLeftForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.RightForward:
					GetComponent<Animation>().CrossFade(runRightForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.LeftBackward:
					GetComponent<Animation>().CrossFade(runLeftBackAttack.name);
					RunningBackSound();
					break;
				case Direction.RightBackward:
					GetComponent<Animation>().CrossFade(runRightBackAttack.name);
					RunningBackSound();
					break;
				}
				UpperAttackingSound();
				break;
			case CharacterState.Attacking001:
				GetComponent<Animation>().CrossFade(attack01.name);
				NormalAttackingSound();
				break;
			case CharacterState.Attacking002:
				GetComponent<Animation>().CrossFade(attack02.name);
				break;
			case CharacterState.Attacking003:
				GetComponent<Animation>().CrossFade(attack03.name);
				break;
			case CharacterState.Attacking004:
				GetComponent<Animation>().CrossFade(attack04.name);
				break;
			case CharacterState.Attacking005:
				GetComponent<Animation>().CrossFade(attack05.name);
				break;
			case CharacterState.Attacking1Final:
				GetComponent<Animation>().CrossFade(attack1_Final.name);
				Attacking1FinalSound();
				break;
			case CharacterState.Attacking2Final:
				GetComponent<Animation>().CrossFade(attack2_Final.name);
				Attacking2FinalSound();
				break;
			case CharacterState.Attacking3Final:
				GetComponent<Animation>().CrossFade(attack3_Final.name);
				Attacking3FinalSound();
				break;
			case CharacterState.Attacking4Final:
				GetComponent<Animation>().CrossFade(attack4_Final.name);
				Attacking4FinalSound();
				break;	
			case CharacterState.Attacking5Final:
				GetComponent<Animation>().CrossFade(attack5_Final.name);
				Attacking5FinalSound();
				break;	
			case CharacterState.MagicAttacking1:
				switch(MoveDirection)
				{
				case Direction.Stationary:
					GetComponent<Animation>().CrossFade(runAttackUpper.name);
					break;
				case Direction.Forward:
					GetComponent<Animation>().CrossFade(runForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.Backward:
					GetComponent<Animation>().CrossFade(runBackAttack.name);
					RunningBackSound();
					break;
				case Direction.Left:
					GetComponent<Animation>().CrossFade(runLeftAttack.name);
					RunningBackSound();
					break;
				case Direction.Right:
					GetComponent<Animation>().CrossFade(runRightAttack.name);
					RunningBackSound();
					break;
				case Direction.LeftForward:
					GetComponent<Animation>().CrossFade(runLeftForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.RightForward:
					GetComponent<Animation>().CrossFade(runRightForwardAttack.name);
					RunningForwardSound();
					break;
				case Direction.LeftBackward:
					GetComponent<Animation>().CrossFade(runLeftBackAttack.name);
					RunningBackSound();
					break;
				case Direction.RightBackward:
					GetComponent<Animation>().CrossFade(runRightBackAttack.name);
					RunningBackSound();
					break;
				}
				MagicAttackingSound();
				break;
			case CharacterState.MagicStoredAttacking:
				GetComponent<Animation>().CrossFade(storedMagicAttack.name);
				StoredMagicAttackingSound();
				break;
			case CharacterState.Skilling01:
				if(skill01!=null)
				GetComponent<Animation>().CrossFade(skill01.name);
				Skilling1Sound();
				break;
			case CharacterState.Skilling02:
				if(skill02!=null)
				GetComponent<Animation>().CrossFade(skill02.name);
				Skilling2Sound();
				break;
			case CharacterState.Skilling03:
				if(skill03!=null)
				GetComponent<Animation>().CrossFade(skill03.name);
				Skilling3Sound();
				break;
			case CharacterState.Skilling04:
				if(skill04!=null)
				GetComponent<Animation>().CrossFade(skill04.name);
				Skilling4Sound();
				break;
			case CharacterState.SuperSkilling:
				if(superSkill!=null)
				GetComponent<Animation>().CrossFade(superSkill.name);
				SuperSkillingSound();
				break;
			case CharacterState.ThrowingBigRock:
				//crossfade throwingBigRock
				break;
			case CharacterState.ThrowingFireWork:
				//crossfade throwingFireWork
				break;
			}
		}

	}

	void ShakeCamera()
	{
		targetLookAtAnim.PlayQueued("CameraShake",QueueMode.PlayNow);
	}

	#region method of Input
	public void MeleeAttackInput(int mouseType)
	{
		if(mouseType==0)
		{
			if(MoveDirection!=Direction.Stationary && State!=CharacterState.RunningAttacking001)
				RunningAttack001();
			else if(MoveDirection==Direction.Stationary)
			{
				switch(DetermineMeleeAttack(mouseType))
				{
				case CharacterState.Attacking001:
					Attack001();
					break;
					
				case CharacterState.Attacking002:
					Attack002();
					break;
					
				case CharacterState.Attacking003:
					Attack003();
					break;
					
				case CharacterState.Attacking004:
					Attack004();
					break;
					
				case CharacterState.Attacking005:
					Attack005();
					break;
				}
			}
		}
		else if(mouseType==1)
		{
			switch(DetermineMeleeAttack(mouseType))
			{
			case CharacterState.Attacking1Final:
				Attack1_Final();
				break;
			case CharacterState.Attacking2Final:
				Attack2_Final();
				break;
			case CharacterState.Attacking3Final:
				Attack3_Final();
				break;
			case CharacterState.Attacking4Final:
				Attack4_Final();
				break;
			case CharacterState.Attacking5Final:
				Attack5_Final();
				break;
			}
		}
	}

	public void GodPowerInput(bool Godmode)
	{
		GodPower(Godmode);
	}

	public void ArtifactInput(bool mode)
	{
		if(mode)
		{
			GameObject godWeapon = GameObject.Instantiate(InRoom_Menu.SP.effectPrefabs[(int)CharacterRoleEff.Other-1].effectPrefs[5].effectPref,Vector3.zero,Quaternion.identity) as GameObject;
			godWeapon.GetComponent<Rigidbody>().useGravity = false;
			Transform godWeaponTrans = godWeapon.transform;
			godWeaponTrans.parent = playerCam.TargetLookAt;
			godWeaponTrans.localPosition = Vector3.zero;
			godWeaponTrans.localRotation = Quaternion.identity;
			godWeaponTrans.localScale = Vector3.one*0.5f;
			Game_Manager.SP.showGodWeapon = false;
		}
		else
		{
			Transform godWeapon = playerCam.TargetLookAt.Find("GodWeapon");
			Destroy(godWeapon.gameObject);
		}
	}

	public void UseLightSourceInput(bool active)
	{
		if(active)
		{
			EnergyStore();
		}
		else
		{
			playerInfo.EnergyStoreInput(false);
			_state = CharacterState.Idle;
			currentAttack = idle.clip;
		}
	}

	public void EnergyStoreInput(int mouseType)
	{
		if(mouseType==0)
			EnergyStore();
		else
		{
			playerInfo.EnergyStoreInput(false);
			State = CharacterState.Idle;
			currentAttack = idle.clip;
		}
	}

	public void MagicAttackInput(int mouseType)
	{
		if(mouseType==0)
		{
			//if(MoveDirection!=Direction.Stationary && State!=CharacterState.RunningAttacking001)
				//RunningAttack001();
			//else
				MagicAttack001();
		}
		else if(mouseType==1)//storing
			MagicStore();
		else if(mouseType==2)
			MagicStoredAttack();
	}

	public void PreSkillInput(int skillType)
	{
		PreUseSkill(skillType);
	}

	public void SkillInput(int skillType)
	{
		switch(skillType)
		{
		case 5:
			Skill_Super();
			break;
		/*case 5:
			ThrowInput(playerInfo._throwingWeapon);
			break;*/
		default:
			UseSkill(skillType);
			break;
		}
	}

	public void BuffSkillInput(BuffSkillName buffSkillType)
	{
		switch(buffSkillType)
		{
		case BuffSkillName.Buff_Power:
			Debug.Log("Use BuffPower");
			break;
		case BuffSkillName.Buff_Speed:
			Debug.Log("Use BuffSpeed");
			break;
		}
		playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Buff,(int)buffSkillType);
	}

	public void ThrowInput(TP_Info.ThrowingWeapons throwWeaponType)
	{
		switch(throwWeaponType)
		{
		case TP_Info.ThrowingWeapons.BigRock:
			ThrowBigRock();
			break;
		case TP_Info.ThrowingWeapons.Firework:
			ThrowFirework();
			break;
		}
	}
	#endregion

	//TPcontroller will call this method when "Fire1" has been pressed to determine what attackstate will be
	private CharacterState DetermineMeleeAttack(int mouseType)
	{
		_onComboTimer = false;
		var	attackState = CharacterState.None;
		if(mouseType==0)
		{
			//if(currentAttack!=null)
			//Debug.Log("currentAtt:" + currentAttack.name + "time:" + animation[currentAttack.name].time + "Length:" + animation[currentAttack.name].length);
			if(State != CharacterState.Attacking001 && State != CharacterState.Attacking002 && State != CharacterState.Attacking003 && 
			   State != CharacterState.Attacking004 && State != CharacterState.Attacking005 &&
			   State != CharacterState.Attacking1Final && State != CharacterState.Attacking2Final && State != CharacterState.Attacking3Final && 
			   State != CharacterState.Attacking4Final && State != CharacterState.Attacking5Final && (currentAttack == idle.clip||currentAttack==null))
			{
				if(nextState == CharacterState.None)
					nextState = CharacterState.Attacking001;
				else if(nextState == CharacterState.Attacking001)
					attackState = CharacterState.Attacking001;
			}
			else if(State == CharacterState.Attacking001 || currentAttack == attack01.clip)
			{
				if(nextState != CharacterState.Attacking002)
					nextState = CharacterState.Attacking002;
				else if(nextState == CharacterState.Attacking002)
				{
					if(State!=CharacterState.Idle)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
							attackState = CharacterState.Attacking002;
					}
					else
					{
						attackState = CharacterState.Attacking002;
					}
				}

			}
			else if(State == CharacterState.Attacking002 || currentAttack == attack02.clip)
			{
				if(nextState != CharacterState.Attacking003)
					nextState = CharacterState.Attacking003;
				else if(nextState == CharacterState.Attacking003)
				{
					if(State!=CharacterState.Idle)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.9f)
							attackState = CharacterState.Attacking003;
					}
					else
						attackState = CharacterState.Attacking003;
				}
					
				
			}
			else if(State == CharacterState.Attacking003 || currentAttack == attack03.clip)
			{
				if(nextState != CharacterState.Attacking004)
					nextState = CharacterState.Attacking004;
				else if(nextState == CharacterState.Attacking004)
				{
					if(State!=CharacterState.Idle)
					{

						if(GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
							attackState = CharacterState.Attacking004;
					}
					else
						attackState = CharacterState.Attacking004;
				}
				
			}
			else if(State == CharacterState.Attacking004 || currentAttack == attack04.clip)
			{
				if(nextState != CharacterState.Attacking005)
					nextState = CharacterState.Attacking005;
				else if(nextState == CharacterState.Attacking005)
				{
					if(State!=CharacterState.Idle)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
							attackState = CharacterState.Attacking005;
					}
					else
						attackState = CharacterState.Attacking005;
				}
			}
		}
		else if(mouseType==1)
		{
			if(State == CharacterState.Attacking001)
			{
				if(nextState != CharacterState.Attacking1Final)
					nextState = CharacterState.Attacking1Final;
				else if(nextState == CharacterState.Attacking1Final && GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
					attackState = CharacterState.Attacking1Final;	
			}
			if(State == CharacterState.Attacking002)
			{
				if(nextState != CharacterState.Attacking2Final)
					nextState = CharacterState.Attacking2Final;
				else if(nextState == CharacterState.Attacking2Final && GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
					attackState = CharacterState.Attacking2Final;	
			}
			if(State == CharacterState.Attacking003)
			{
				if(nextState != CharacterState.Attacking3Final)
					nextState = CharacterState.Attacking3Final;
				else if(nextState == CharacterState.Attacking3Final && GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
					attackState = CharacterState.Attacking3Final;	
			}
			if(State == CharacterState.Attacking004)
			{
				if(nextState != CharacterState.Attacking4Final)
					nextState = CharacterState.Attacking4Final;
				else if(nextState == CharacterState.Attacking4Final && GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
					attackState = CharacterState.Attacking4Final;	
			}
			if(State == CharacterState.Attacking005)
			{
				if(nextState != CharacterState.Attacking5Final)
					nextState = CharacterState.Attacking5Final;
				else if(nextState == CharacterState.Attacking5Final && GetComponent<Animation>()[currentAttack.name].time>GetComponent<Animation>()[currentAttack.name].length*0.95f)
					attackState = CharacterState.Attacking5Final;	
			}
		}
		return attackState;
	}

	#region Start Action Method
	public void Die()
	{
		if(State==CharacterState.Dead)
		{
			if(die!=null)
				GetComponent<Animation>().CrossFade(die.name);
		}
		else if(State!=CharacterState.Dead)
		{
			playerView.RPC("TellTeammateKilled",PhotonTargets.All,playerInfo.Team,-1);
			if(InRoom_Menu.SP.GetPlayerFromID(playerView.viewID).team>0)
				InRoom_Menu.SP.UseLightSourceToReviveInput(playerInfo.Team,playerView.viewID);
			State = CharacterState.Dead;
			if(die!=null)
			GetComponent<Animation>().CrossFade(die.name);
		}
	}

	private void Skill_Super()
	{
		if(SkillRange[4]!=null)
		{
			_currentSkillRange = SkillRange[4];
			_currentSkillRange.SetActive(true);
		}

		if(superSkill!=null)
		{
			if(_state!=CharacterState.SuperSkilling)
			{
				switch(playerInfo.Role)
				{
				case TP_Info.CharacterRole.Mars:
					SetMultiTimes(1);
					break;
				case TP_Info.CharacterRole.DarkMan:
					SetMultiTimes(5);
					break;
				case TP_Info.CharacterRole.Theia:
					SetMultiTimes(1);
					break;
				case TP_Info.CharacterRole.Persia:
					SetMultiTimes(1);
					break;
				case TP_Info.CharacterRole.Steropi:
					SetMultiTimes(1);
					break;
				case TP_Info.CharacterRole.Hyperion:
					SetMultiTimes(3);
					break;
				}
			}
			State = CharacterState.SuperSkilling;
			lockAnimating = true;
			GetComponent<Animation>().CrossFade(superSkill.name);
			currentAttack = superSkill.clip;
		}
	}

	private void PreUseSkill(int skillType)
	{
		if(skillType>0 && skillType<6)
		{
			if(SkillRange[skillType-1]!=null)
			{
				_currentSkillRange = SkillRange[skillType-1];
				_currentSkillRange.SetActive(true);
				//_currentSkillRange.transform.rotation = Quaternion.identity;
				playerRCA = _currentSkillRange.GetComponent<RangeColliderAttack>();
				if(!playerInfo.isAI)
					playerRCA.OnModel();
			}
		}
	}

	//set times of using skill or attack
	void SetMultiTimes(int times)
	{
		if(times==0)
			playerAttack.LockAttackCheck = false;
		_multitudeAttacktime = times;
		_multitudeAttackTimer = 0;
	}
	
	private void UseSkill(int skillType)
	{
		if(playerRCA!=null)
			playerRCA.OffModel();

		switch(skillType)
		{
		case 1:
			if(skill01!=null)
			{
				if(_state!=CharacterState.Skilling01)
				{
					switch(playerInfo.Role)
					{
					case TP_Info.CharacterRole.Mars:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.DarkMan:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Theia:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Persia:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Steropi:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Hyperion:
						SetMultiTimes(1);
						break;
					}
				}

				State = CharacterState.Skilling01;
				lockAnimating = true;
				GetComponent<Animation>().CrossFade(skill01.name);
				currentAttack = skill01.clip;

			}
			break;
		case 2:
			if(skill02!=null)
			{
				if(_state!=CharacterState.Skilling02)
				{
					switch(playerInfo.Role)
					{
					case TP_Info.CharacterRole.Mars:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.DarkMan:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Theia:
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Theia,2);
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Persia:
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Persia,2);
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Steropi:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Hyperion:
						SetMultiTimes(1);
						break;
					}
				}

				State = CharacterState.Skilling02;
				lockAnimating = true;
				GetComponent<Animation>().CrossFade(skill02.name);
				currentAttack = skill02.clip;
			}
			break;
		case 3:
			if(skill03!=null)
			{
				if(_state!=CharacterState.Skilling03)
				{
					switch(playerInfo.Role)
					{
					case TP_Info.CharacterRole.Mars:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.DarkMan:
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.DarkMan,3);
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Theia:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Persia:
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Persia,3);
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Steropi:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Hyperion:
						SetMultiTimes(1);
						break;
					}

				}

				State = CharacterState.Skilling03;
				lockAnimating = true;
				GetComponent<Animation>().CrossFade(skill03.name);
				currentAttack = skill03.clip;
			}
			break;
		case 4:
			if(skill04!=null)
			{
				if(_state!=CharacterState.Skilling04)
				{
					switch(playerInfo.Role)
					{
					case TP_Info.CharacterRole.Mars:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.DarkMan:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Theia:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Persia:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Steropi:
						SetMultiTimes(1);
						break;
					case TP_Info.CharacterRole.Hyperion:
						SetMultiTimes(1);
						break;
					}

				}

				State = CharacterState.Skilling04;
				lockAnimating = true;
				GetComponent<Animation>().CrossFade(skill04.name);
				currentAttack = skill04.clip;
			}
			break;
		}
	}

	private void ThrowBigRock()
	{
		State = CharacterState.ThrowingBigRock;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack3_Final.name);
		currentAttack = attack3_Final.clip;
		Debug.Log("Throw BigRock");
	}

	private void ThrowFirework()
	{
		State = CharacterState.ThrowingFireWork;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack4_Final.name);
		currentAttack = attack4_Final.clip;
		Debug.Log("Throw Firework");
	}

	private void GodPower(bool Godmode)
	{
		if(GodPowerEff!=null)
		{
			GodPowerEff.SetActive(Godmode);
		}
	}

	public void UseWaterInput(int waterType)
	{
		playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Other,waterType+1);
	}

	private void UseArtifact(bool mode)
	{
		if(mode)
		{
			//Creat Particle
			Debug.Log("Use Artifact");
		}
		else
		{
			//Destroy Particle
			Debug.Log("No Artifact");
		}
	}

	private void EnergyStore()
	{
		State = CharacterState.EnergyStoring;
		lockAnimating = true;
		if(storeEnergy!=null)
		GetComponent<Animation>().CrossFade(storeEnergy.name);
		currentAttack = storeEnergy.clip;
		playerInfo.EnergyStoreInput(true);
	}

	private void MagicAttack001()
	{
		if(_state!=CharacterState.MagicAttacking1)
			playerAttack.LockAttackCheck = false;
		LockAttackCheck(1,HitWays.BeHit);
		_checkRunningAttackTimer = true;
		ShakeCamera();
		State = CharacterState.MagicAttacking1;
		lockAnimating = true;
		lockAttacking = true;
		switch(MoveDirection)
		{
		case Direction.Stationary:
			GetComponent<Animation>().CrossFade(runAttackUpper.name);
			currentAttack = runAttackUpper.clip;
			break;
		case Direction.Forward:
			GetComponent<Animation>().CrossFade(runForwardAttack.name);
			currentAttack = runForwardAttack.clip;
			break;
		case Direction.Backward:
			GetComponent<Animation>().CrossFade(runBackAttack.name);
			currentAttack = runBackAttack.clip;
			break;
		case Direction.Left:
			GetComponent<Animation>().CrossFade(runLeftAttack.name);
			currentAttack = runLeftAttack.clip;
			break;
		case Direction.Right:
			GetComponent<Animation>().CrossFade(runRightAttack.name);
			currentAttack = runRightAttack.clip;
			break;
		case Direction.LeftForward:
			GetComponent<Animation>().CrossFade(runLeftForwardAttack.name);
			currentAttack = runLeftForwardAttack.clip;
			break;
		case Direction.RightForward:
			GetComponent<Animation>().CrossFade(runRightForwardAttack.name);
			currentAttack = runRightForwardAttack.clip;
			break;
		case Direction.LeftBackward:
			GetComponent<Animation>().CrossFade(runLeftBackAttack.name);
			currentAttack = runLeftBackAttack.clip;
			break;
		case Direction.RightBackward:
			GetComponent<Animation>().CrossFade(runRightBackAttack.name);
			currentAttack = runRightBackAttack.clip;
			break;
		}
	}

	private void MagicStore()
	{
		if(_state!=CharacterState.MagicStoring)
		{
			playerInfo.StoreMagicInput();
			State = CharacterState.MagicStoring;
			lockAnimating = true;
			GetComponent<Animation>().CrossFade(storeMagic.name);
			currentAttack = storeMagic.clip;
		}
	}

	private void MagicStoredAttack()
	{
		if(_state!=CharacterState.MagicStoredAttacking)
			playerAttack.LockAttackCheck = false;
		LockAttackCheck(1,HitWays.BeHit);
		State = CharacterState.MagicStoredAttacking;
		lockAnimating = true;
		//lockAttacking = true;
		GetComponent<Animation>().CrossFade(storedMagicAttack.name);
		currentAttack = storedMagicAttack.clip;
	}

	public void RunningAttack001()
	{
		if(_state!=CharacterState.RunningAttacking001)
			playerAttack.LockAttackCheck = false;
		LockAttackCheck(0.95f,HitWays.BeHit);
		State = CharacterState.RunningAttacking001;
		_checkRunningAttackTimer = true;
		lockAnimating = true;
		lockAttacking = true;
		switch(MoveDirection)
		{
		/*case Direction.Stationary:
			break;*/
		case Direction.Forward:
			GetComponent<Animation>().CrossFade(runForwardAttack.name);
			currentAttack = runForwardAttack.clip;
			break;
		case Direction.Backward:
			GetComponent<Animation>().CrossFade(runBackAttack.name);
			currentAttack = runBackAttack.clip;
			break;
		case Direction.Left:
			GetComponent<Animation>().CrossFade(runLeftAttack.name);
			currentAttack = runLeftAttack.clip;
			break;
		case Direction.Right:
			GetComponent<Animation>().CrossFade(runRightAttack.name);
			currentAttack = runRightAttack.clip;
			break;
		case Direction.LeftForward:
			GetComponent<Animation>().CrossFade(runLeftForwardAttack.name);
			currentAttack = runLeftForwardAttack.clip;
			break;
		case Direction.RightForward:
			GetComponent<Animation>().CrossFade(runRightForwardAttack.name);
			currentAttack = runRightForwardAttack.clip;
			break;
		case Direction.LeftBackward:
			GetComponent<Animation>().CrossFade(runLeftBackAttack.name);
			currentAttack = runLeftBackAttack.clip;
			break;
		case Direction.RightBackward:
			GetComponent<Animation>().CrossFade(runRightBackAttack.name);
			currentAttack = runRightBackAttack.clip;
			break;
		}
	}

	public void Attack001()
	{
		if(_state!=CharacterState.Attacking001)
		{
			SetMultiTimes(0);
		}
		LockAttackCheck(1f,HitWays.BeHit);

		State = CharacterState.Attacking001;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack01.name);
		currentAttack = attack01.clip;
	}
	
	public void Attack002()
	{
		if(_state!=CharacterState.Attacking002)
		{
			SetMultiTimes(0);
		}
		LockAttackCheck(1.025f,HitWays.BeHit);

		State = CharacterState.Attacking002;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack02.name);
		currentAttack = attack02.clip;
	}
	
	public void Attack003()
	{	
		if(_state!=CharacterState.Attacking003)
		{
			SetMultiTimes(0);
		}
		LockAttackCheck(1.05f,HitWays.BeHit);

		State = CharacterState.Attacking003;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack03.name);
		currentAttack = attack03.clip;
	}

	public void Attack004()
	{	
		if(_state!=CharacterState.Attacking004)
		{
			SetMultiTimes(0);
		}
		LockAttackCheck(1.075f,HitWays.BeHit);

		State = CharacterState.Attacking004;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack04.name);
		currentAttack = attack04.clip;
	}

	public void Attack005()
	{	
		if(_state!=CharacterState.Attacking005)
		{
			SetMultiTimes(0);
		}
		LockAttackCheck(1.1f,HitWays.KnockDown);

		State = CharacterState.Attacking005;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack05.name);
		currentAttack = attack05.clip;
	}
	
	public void Attack1_Final()
	{	
		if(_state!=CharacterState.Attacking1Final)
		{
			SetMultiTimes(0);
			LockAttackCheck(1.025f,HitWays.BeHit);
		}

		State = CharacterState.Attacking1Final;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack1_Final.name);
		currentAttack = attack1_Final.clip;
	}

	public void Attack2_Final()
	{	
		if(_state!=CharacterState.Attacking2Final)
		{
			SetMultiTimes(0);
			LockAttackCheck(1.05f,HitWays.BeHit);
		}

		State = CharacterState.Attacking2Final;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack2_Final.name);
		currentAttack = attack2_Final.clip;
	}

	public void Attack3_Final()
	{	
		if(_state!=CharacterState.Attacking3Final)
		{
			switch(playerInfo.Role)
			{
			case TP_Info.CharacterRole.Mars:
				SetMultiTimes(1);
					break;
			case TP_Info.CharacterRole.DarkMan:
				SetMultiTimes(1);
				break;
			case TP_Info.CharacterRole.Hyperion:
				SetMultiTimes(1);
				break;
			case TP_Info.CharacterRole.Steropi:
				SetMultiTimes(1);
				break;
			}
		}

		State = CharacterState.Attacking3Final;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack3_Final.name);
		currentAttack = attack3_Final.clip;
	}

	public void Attack4_Final()
	{	
		if(_state!=CharacterState.Attacking4Final)
		{
			switch(playerInfo.Role)
			{
			case TP_Info.CharacterRole.Mars:
				SetMultiTimes(2);
				break;
			case TP_Info.CharacterRole.DarkMan:
				SetMultiTimes(2);
				break;
			case TP_Info.CharacterRole.Hyperion:
				SetMultiTimes(1);
				break;
			case TP_Info.CharacterRole.Steropi:
				SetMultiTimes(1);
				break;
			}
		}

		State = CharacterState.Attacking4Final;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack4_Final.name);
		currentAttack = attack4_Final.clip;
	}

	public void Attack5_Final()
	{	
		if(_state!=CharacterState.Attacking5Final)
		{
			switch(playerInfo.Role)
			{
			case TP_Info.CharacterRole.Mars:
				SetMultiTimes(1);
				break;
			case TP_Info.CharacterRole.DarkMan:
				SetMultiTimes(2);
				break;
			case TP_Info.CharacterRole.Hyperion:
				SetMultiTimes(1);
				break;
			case TP_Info.CharacterRole.Steropi:
				SetMultiTimes(1);
				break;
			}
		}

		State = CharacterState.Attacking5Final;
		lockAnimating = true;
		GetComponent<Animation>().CrossFade(attack5_Final.name);
		currentAttack = attack5_Final.clip;
	}

	public void Roll()
	{
		State = CharacterState.Rolling;
		lockAnimating = true;
		if(rollLeft!=null&&rollRight!=null)
		{
			if(_moveDirection == Direction.Left||_moveDirection == Direction.LeftForward)
			{
				currentAttack = rollLeft.clip;
				GetComponent<Animation>().CrossFade(rollLeft.name);
			}
			else if(_moveDirection == Direction.Right||_moveDirection == Direction.RightForward)
			{
				currentAttack = rollRight.clip;
				GetComponent<Animation>().CrossFade(rollRight.name);
			}
		}
	}

	public void Behit()
	{
		State = CharacterState.Beinghit;
		_onComboTimer = true;
		_comboAttackTimer = 0.5f;
		lockAnimating = true;
		currentAnimation = beHit.clip;
		beHit.time = 0;
		GetComponent<Animation>().CrossFade(beHit.name);
	}

	public void Freeze()
	{
		State = CharacterState.Freeze;
		lockAnimating = true;
		currentAttack = freeze.clip;
		GetComponent<Animation>().CrossFade(freeze.name);
	}

	public void KnockDown()
	{
		State = CharacterState.KnockingDown;
		lockAnimating = true;
			currentAttack = knockDown.clip;
		GetComponent<Animation>().CrossFade(knockDown.name);
	}

	public void StandUp()
	{
		State = CharacterState.StandingUp;
		lockAnimating = true;
		currentAttack = standUp.clip;
		GetComponent<Animation>().CrossFade(standUp.name);
	}

	public void Dizzy()
	{
		State = CharacterState.Dizzing;
		lockAnimating = true;
		currentAttack = dizzy.clip;
		GetComponent<Animation>().CrossFade(dizzy.name);
	}
	/*public void Jump()
	{
		if(!playerController.CharacterController.isGrounded /*||IsDead || State == CharacterState.Jumping)
			return;
		lastState = State;
		State = CharacterState.Jumping;
		lockAnimating = true;
		animation.CrossFade(jump.name);
	}*/
	#endregion

	#region EffectRPC
	[RPC]
	public void SkillEffect(int role, int skill)
	{
		float destroyTime = 3;
		GameObject prefab;
		GameObject clone;
		Vector3 pos;
		Quaternion rot;
		prefab = roomScript.effectPrefabs[role-1].effectPrefs[skill-1].effectPref;
		clone = (GameObject)Instantiate(prefab,transform.position,transform.rotation);

		switch(skill)
		{
		case 1:
			switch((CharacterRoleEff)role)
			{
			case CharacterRoleEff.Mars:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,1.5f,0);
				destroyTime = 1.5f;
				break;
			case CharacterRoleEff.DarkMan:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,50,80);
				destroyTime = 1f;
				break;
			}
			break;
		case 2:
			switch((CharacterRoleEff)role)
			{
			case CharacterRoleEff.Mars:
				clone.transform.parent = playerInfo.HUDTextTarget;
				clone.transform.localPosition = Vector3.zero;
				break;
			case CharacterRoleEff.DarkMan:
				clone.transform.parent = transform;
				break;
			case CharacterRoleEff.Theia:
				clone.transform.parent = transform;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localRotation = Quaternion.identity;
				clone.transform.localPosition = new Vector3(0,0,15);
				clone.transform.parent = null;
				break;
			case CharacterRoleEff.Persia:
				clone.transform.parent = transform;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localRotation = Quaternion.identity;
				clone.transform.localPosition = new Vector3(0,0,23);
				clone.transform.parent = null;
				break;
			case CharacterRoleEff.Steropi:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,1.5f,0);
				destroyTime = 1.5f;
				break;
			case CharacterRoleEff.Hyperion:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,1.5f,0);
				destroyTime = 5;
				break;
			}
			break;
		case 3:
			switch((CharacterRoleEff)role)
			{
			case CharacterRoleEff.DarkMan:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,1.5f,-3);
				break;
			case CharacterRoleEff.Persia:
				clone.transform.parent = transform;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localRotation = Quaternion.identity;
				clone.transform.localPosition = new Vector3(0,0,15);
				clone.transform.parent = null;
				break;
			}
			break;
		case 4:
			switch((CharacterRoleEff)role)
			{
			}
			break;
		case 5:
			switch((CharacterRoleEff)role)
			{
			case CharacterRoleEff.DarkMan:
				destroyTime = 2;
				break;
			case CharacterRoleEff.Persia:
				clone.transform.parent = transform;
				clone.transform.localPosition += new Vector3(0,5,0);
				clone.transform.Rotate(new Vector3(0,-90,0));
				break;
			case CharacterRoleEff.Hyperion:
				destroyTime = 2;
				break;
			}

			break;
		}


		Destroy(clone,destroyTime);
	}
	#endregion

	bool IsAIButNotMasterClients()
	{
		if(playerInfo.isAI&&!PhotonNetwork.isMasterClient)
			return true;
		else
			return false;
	}

	//Method for using skill and checking it to be corresponding with the animation
	void SkillAttack()
	{
		if(_multitudeAttackTimer<_multitudeAttacktime)
		{
			switch(playerInfo.Role)
			{
			case TP_Info.CharacterRole.Mars:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Mars,1);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[0].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(0,Skilltype.Mars_Assault,RCA,HitWays.None);
					}
					break;
					
				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Mars_Stun,RCA,HitWays.None);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1,HitWays.BeHit);
						_multitudeAttackTimer++;
						_currentSkillRange.SetActive(false);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Mars,4);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Mars_AddAttack,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.2f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Mars,5);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[4].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				}
				break;

			case TP_Info.CharacterRole.DarkMan:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.DarkMan,1);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[0].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(0,Skilltype.Darkman_Spurt,RCA,HitWays.None);
					}
					break;
					
				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Darkman_Freeze,RCA,HitWays.None);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[2].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(2,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.DarkMan,4);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Darkman_AddDefense,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					RangeColliderAttack RCAs = SkillRange[4].GetComponent<RangeColliderAttack>();
					if(_multitudeAttackTimer==_multitudeAttacktime-5)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.5f)
						{
							if(!IsAIButNotMasterClients())
								playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.DarkMan,5);
							playerAttack.LockAttackCheck = false;

								SkillAttackCheck(4,Skilltype.Attack,RCAs,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-4)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.55f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCAs,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-3)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.6f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCAs,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-2)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.65f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCAs,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-1)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.7f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCAs,HitWays.KnockDown);
						}
					}
					break;
				}
				break;
			case TP_Info.CharacterRole.Theia:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.5f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Theia,1);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[0].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(0,Skilltype.Attack,RCA,HitWays.BeHit);
					}
					break;

				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.6f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[2].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(2,Skilltype.AddHealth,RCA,HitWays.None);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Theia_AddDefense,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.5f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Theia,5);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[4].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				}
				break;
			case TP_Info.CharacterRole.Persia:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.2f)
					{
						_currentSkillRange.SetActive(false);
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1,HitWays.BeHit);
						_multitudeAttackTimer++;
					}
					break;
					
				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.5f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[2].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(2,Skilltype.Attack,RCA,HitWays.BeHit);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Persia_Curse,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.8f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Persia,5);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[4].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				}
				break;
			case TP_Info.CharacterRole.Hyperion:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.7f)
					{
						_currentSkillRange.SetActive(false);
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1,HitWays.BeHit);
						_multitudeAttackTimer++;

					}
					break;
					
				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.65f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Hyperion,2);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Hyperion_Unbeatable,RCA,HitWays.None);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.7f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Hyperion,3);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[2].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(2,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Hyperion,4);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Hyperion_AddDefense,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					RangeColliderAttack RCA = SkillRange[4].GetComponent<RangeColliderAttack>();
					if(_multitudeAttackTimer==_multitudeAttacktime-3)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
						{
							if(!IsAIButNotMasterClients())
								playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Hyperion,5);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-2)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.35f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.BeHit);
						}
					}
					if(_multitudeAttackTimer==_multitudeAttacktime-1)
					{
						if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.4f)
						{
							if(!_currentSkillRange.activeSelf)
								_currentSkillRange.SetActive(true);
							playerAttack.LockAttackCheck = false;
							SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.KnockDown);
						}
					}
					break;
				}
				break;
			case TP_Info.CharacterRole.Steropi:
				switch(State)
				{
				case CharacterState.Skilling01:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.2f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1,HitWays.BeHit);
						_multitudeAttackTimer++;
						_currentSkillRange.SetActive(false);
					}
					break;
					
				case CharacterState.Skilling02:
					if(GetComponent<Animation>()[currentAttack.name].time>0)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Steropi,2);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[1].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(1,Skilltype.Steropi_RollingStrike,RCA,HitWays.None);
					}
					break;
				case CharacterState.Skilling03:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.4f && GetComponent<Animation>()[currentAttack.name].time<(GetComponent<Animation>()[currentAttack.name].length)*0.8f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Steropi,3);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[2].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(2,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				case CharacterState.Skilling04:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Steropi,4);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[3].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(3,Skilltype.Steropi_AddAttack,RCA,HitWays.None);
					}
					break;
				case CharacterState.SuperSkilling:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.3f)
					{
						if(!IsAIButNotMasterClients())
							playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Steropi,5);
						playerAttack.LockAttackCheck = false;
						RangeColliderAttack RCA = SkillRange[4].GetComponent<RangeColliderAttack>();
						SkillAttackCheck(4,Skilltype.Attack,RCA,HitWays.KnockDown);
					}
					break;
				}
				break;
			}
		}
	}

	//Method for causing multitude damage and checking it to be corresponding with the animation
	void MultitudeAttack()
	{
		if(_multitudeAttackTimer<_multitudeAttacktime)
		{
			switch(playerInfo.Role)
			{
			case TP_Info.CharacterRole.Mars:
				switch(State)
				{
				case CharacterState.Attacking3Final:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.075f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking4Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.2f && GetComponent<Animation>()[currentAttack.name].time < 0.5f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.1f,HitWays.BeHit);
						}
					}
					else if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.7f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.1f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking5Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.5f && GetComponent<Animation>()[currentAttack.name].time < 0.8f)
					{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.125f,HitWays.BeHit);
					}

					break;
				}
				break;
			case TP_Info.CharacterRole.DarkMan:
				switch(State)
				{
				case CharacterState.Attacking3Final:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.05f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking4Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.2f && GetComponent<Animation>()[currentAttack.name].time < 0.5f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.075f,HitWays.BeHit);
						}
					}
					else if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.7f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.075f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking5Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.5f && GetComponent<Animation>()[currentAttack.name].time < 0.8f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.1f,HitWays.BeHit);
						}
					}
					else if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.1f,HitWays.KnockDown);
					}
					break;
				}
				break;
			case TP_Info.CharacterRole.Hyperion:
				switch(State)
				{
				case CharacterState.Attacking3Final:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.05f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking4Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.2f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.1f,HitWays.BeHit);
						}
					}
					break;
				case CharacterState.Attacking5Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.5f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.125f,HitWays.BeHit);
						}
					}
					break;
					/*else if(animation[currentAttack.name].time>(animation[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.7f,HitWays.KnockDown);
					}
					break;*/
				}
				break;
			case TP_Info.CharacterRole.Steropi:
				switch(State)
				{
				case CharacterState.Attacking3Final:
					if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.05f,HitWays.KnockDown);
					}
					break;
				case CharacterState.Attacking4Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.2f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
								playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.1f,HitWays.BeHit);
						}
					}
					break;
				case CharacterState.Attacking5Final:
					if(GetComponent<Animation>()[currentAttack.name].time > 0.5f)
					{
						if(_multitudeAttackTimer<_multitudeAttacktime-1)
						{
							playerAttack.LockAttackCheck = false;
							LockAttackCheck(1.125f,HitWays.BeHit);
						}
					}
					/*else if(animation[currentAttack.name].time>(animation[currentAttack.name].length)*0.9f)
					{
						playerAttack.LockAttackCheck = false;
						LockAttackCheck(1.7f,HitWays.KnockDown);
					}
					break;*/
					break;
				}
				break;
			}
		}
	}

	#region Character State Methods
	void Attacking()
	{
		//Debug.Log("Time: " + animation[currentAttack.name].time + " Length: " + animation[currentAttack.name].length + "NT:" + animation[currentAttack.name].normalizedTime + "W:" + animation[currentAttack.name].weight);
		if(trail!=null)
		{
			if(!openTrail)
			{
				trail.GetComponent<MeleeWeaponTrail>().Emit = true;
				openTrail = true;
			}
		}

		if(playerCam!=null)
		{
			if(State!=CharacterState.MagicAttacking1)
				playerCam.LockMouseInput = true;
		}
		if(State == CharacterState.RunningAttacking001||State == CharacterState.MagicAttacking1)
		{
			if(playerInfo.Role!=TP_Info.CharacterRole.Persia)
				Running();//change running attack direction
		}

		if(State == CharacterState.Attacking3Final||State == CharacterState.Attacking4Final || State== CharacterState.Attacking5Final)
		{
			MultitudeAttack();
		}
		if(State == CharacterState.Skilling01||State == CharacterState.Skilling02||State == CharacterState.Skilling03||State == CharacterState.Skilling04||State == CharacterState.SuperSkilling)
		{
			SkillAttack();
			CheckCurrentAttack();
		}
		if((GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length)))
		{
			//animation[currentAttack.name].time = 0;
			if(mode == CharacterMode.Skilling)
			{
				mode = CharacterMode.None;
				playerController.SkillType = 0;
			}
			if(playerCam!=null)
			{
				if(playerCam.LockMouseInput)
				{
						playerCam.LockMouseInput = false;
				}
			}
			if(ShouldLockAttackCheck())
				lockAttacking = true;
			else
				lockAttacking = false;

			State = CharacterState.Idle;
			nextState = CharacterState.None;
			currentAttack = idle.clip;

			if(trail!=null)
			{
				if(openTrail)
				{
					trail.GetComponent<MeleeWeaponTrail>().Emit = false;
					openTrail = false;
				}
			}
		}
	}

	void Dying()
	{
		if(!GetComponent<Animation>().IsPlaying(die.name))
		{
			playerInfo.DoDead();
		}
	}

	void CheckCurrentAttack()
	{
		if(currentAttack!=skill01.clip||currentAttack!=skill02.clip||currentAttack!=skill03.clip||currentAttack!=skill04.clip||currentAttack!=superSkill.clip)
		{
			switch(_state)
			{
			case CharacterState.Skilling01:
				currentAttack = skill01.clip;
				break;
			case CharacterState.Skilling02:
				currentAttack = skill02.clip;
				break;
			case CharacterState.Skilling03:
				currentAttack = skill03.clip;
				break;
			case CharacterState.Skilling04:
				currentAttack = skill04.clip;
				break;
			case CharacterState.SuperSkilling:
				currentAttack = superSkill.clip;
				break;
			}
		}
	}

	public void Idle()
	{
		/*if(!photonView.isMine)
			lockJumping = false;*/
		//if(!animation.IsPlaying(spinIdle.name))
			GetComponent<Animation>().CrossFade(idle.name);
		//animation.PlayQueued(spinIdle.name,QueueMode.CompleteOthers,PlayMode.StopAll);//.CrossFade(spinIdle.name);.
		if(playerCam!=null)
		{
			if(currentAttack!=idle.clip)
				currentAttack = idle.clip;
			if(playerCam.LockMouseInput)
			{
				playerCam.LockMouseInput = false;
			}
		}

		if(lockAnimating)
			lockAnimating = false;

		if(playerAttack!=null)
		{
			if(playerAttack.LockAttackCheck)
			{
				playerAttack.LockAttackCheck = false;
				_multitudeAttacktime = 0;
				_multitudeAttackTimer = 0;
			}
		}
	}

	void Running()
	{
		switch(MoveDirection)
		{
		case Direction.Stationary:
			break;
		case Direction.Forward:
			GetComponent<Animation>().CrossFade(goForward.name);
			RunningForwardSound();
			break;
		case Direction.Backward:
			GetComponent<Animation>().CrossFade(goBack.name);
			RunningBackSound();
			break;
		case Direction.Left:
			GetComponent<Animation>().CrossFade(goLeft.name);
			RunningBackSound();
			break;
		case Direction.Right:
			GetComponent<Animation>().CrossFade(goRight.name);
			RunningBackSound();
			break;
		case Direction.LeftForward:
			GetComponent<Animation>().CrossFade(goLeftForward.name);
			RunningForwardSound();
			break;
		case Direction.RightForward:
			GetComponent<Animation>().CrossFade(goRightForward.name);
			RunningForwardSound();
			break;
		case Direction.LeftBackward:
			GetComponent<Animation>().CrossFade(goLeftBack.name);
			RunningBackSound();
			break;
		case Direction.RightBackward:
			GetComponent<Animation>().CrossFade(goRightBack.name);
			RunningBackSound();
			break;
		}
		if(_state == CharacterState.Running)
			CheckMainAudio();
	}

	void Rolling()
	{
		if(GetComponent<Animation>()[currentAttack.name].weight>0.1f)
		{
			if(!playerMotor.IsRolling)
				playerMotor.Roll();
		}
		if(GetComponent<Animation>()[currentAttack.name].weight==1)
		{
			playerMotor.IsRolling = false;
			State = CharacterState.Idle;
			lockAnimating = false;
			lockRolling = true;
		}
	}

	void Beinghit()
	{
		if(GetComponent<Animation>()[currentAnimation.name].normalizedTime>(GetComponent<Animation>()[currentAnimation.name].length))
		{
			State = CharacterState.Idle;
			lockAnimating = false;
		}
	}

	void KnockingDown()
	{
		if(currentAttack!=knockDown.clip)
			currentAttack = knockDown.clip;
		if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length))
		{
			lockAnimating = false;
		}
	}

	void StandingUp()
	{
		if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length))
		{
			State = CharacterState.Idle;
			lockAnimating = false;
		}
	}

	void Freezing()
	{
		if(GetComponent<Animation>()[currentAttack.name].time>(GetComponent<Animation>()[currentAttack.name].length))
		{
			State = CharacterState.Idle;
			lockAnimating = false;
		}
	}

	/*void Jumping()
	{
		if(!photonView.isMine)
		{
			//if(playerController.CharacterController.isGrounded)
			//{
			if(!lockJumping)
			{
				animation.CrossFade(jump.name);
				lockJumping = true;
			}
			else
			{
				if(!animation.isPlaying)
					_state = CharacterState.Landing;
			}
				//State = CharacterState.Landing;
			//}
		}
		else
		{
			if((!animation.isPlaying && playerController.CharacterController.isGrounded) ||
			   playerController.CharacterController.isGrounded)
			{
				/*if(lastState == CharacterState.Running)
				animation.CrossFade("RunLand")
			  else
			  	animation.CrossFade("Land")*/
				//animation.CrossFade(jump.name);
				//State = CharacterState.Landing;
			//}
			/*else if(!animation.IsPlaying(jump.name))
			{
			State = CharacterState.Falling;
			animation.CrossFade("Darkman_Idle");
			}
			else
			{
				State = CharacterState.Jumping;
				//Help determine if we fell too far
			}
		}
	}*/
	
	//void Falling(){}
	
	/*void Landing()
	{
		if(!photonView.isMine)
			lockJumping = false;
		//if(lastState == CharacterState.Running)
		//{
		//	State = CharacterState.Running;
		//	animation.CrossFade(goForward.name);
		//}
		//else
		//{
			if(!animation.IsPlaying(jumpLand.name))
			{
				State = CharacterState.Idle;
				animation.Play(jumpLand.name);
			}
		//}
		lockAnimating = false;
	}*/
	#endregion

	#region Determine Direction and State
	//Determine move direction
	public void DetermineCurrentMoveDirection()
	{
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;
		if(playerMotor.MoveVector.z > 0)
			forward = true;
		if(playerMotor.MoveVector.z < 0)
			backward = true;
		if(playerMotor.MoveVector.x > 0)
			right = true;
		if(playerMotor.MoveVector.x < 0)
			left = true;
		
		if(forward)
		{
			if(left)
				MoveDirection = Direction.LeftForward;
			else if(right)
				MoveDirection = Direction.RightForward;
			else
				MoveDirection = Direction.Forward;
		}
		else if(backward)
		{
			if(left)
				MoveDirection = Direction.LeftBackward;
			else if(right)
				MoveDirection = Direction.RightBackward;
			else
				MoveDirection = Direction.Backward;
		}
		else if(left)
			MoveDirection = Direction.Left;
		else if(right)
			MoveDirection = Direction.Right;
		else
			MoveDirection = Direction.Stationary;
	}
	
	//Determine current state
	void DetermineCurrentState()
	{
		if(State == CharacterState.Dead)
			return;
		if(!WholeGameManager.SP.InGame)
		{
			return;
		}
		/*if(!playerController.CharacterController.isGrounded)
		{
			if (State != CharacterState.Falling && 
			    State != CharacterState.Jumping && 
			    State != CharacterState.Landing)
			{
				//We should be falling
			}
		}*/
		
		if (CanMove()&&State!=CharacterState.RunningAttacking001&&State!=CharacterState.MagicAttacking1)
		{
			if(MoveDirection==Direction.Stationary)
				State = CharacterState.Idle;
			else if(MoveDirection!=Direction.Stationary)
			{
				//if(TP_Controller.Instance.IsRunning)
				State = CharacterState.Running;
				//else
				//State = CharacterState.Walking;
			}		
		}
	}
	#endregion

	//Attack CD timer
	void RunAttackTimer()
	{
		float Rate;
		if(playerInfo.isAI)
			Rate = 150;
		else
			Rate = 100;
		float AttackTime = Rate/playerInfo.GetAbility((int)AbilityName.AttackSpeed).CurValue;

		if(attackTimer<AttackTime)
			attackTimer += Time.fixedDeltaTime;
		else
		{
			lockAttacking = false;
			attackTimer = 0;
		}
	}

	//Roll CD timer
	void RollTimer()
	{
		float RollTime = 100/playerInfo.GetAbility((int)AbilityName.AttackSpeed).CurValue;
		if(rollTimer<RollTime)
			rollTimer += Time.fixedDeltaTime;
		else
		{
			lockRolling = false;
			rollTimer = 0;
		}
	}

	//if true->lockAttack,else do not lockattack
	bool ShouldLockAttackCheck()
	{
		bool check = true;
		if(State == CharacterState.Skilling01||State == CharacterState.Skilling02||State == CharacterState.Skilling03||State == CharacterState.Skilling04||State == CharacterState.SuperSkilling)
		{
			check = false;
		}
		if(playerInfo.isAI)
			check = true;
		return check;
	}

	#region Ways of Attack Check
	//normal attack
 	void LockAttackCheck(float enforcement,HitWays hitWays)
	{
		if(!playerAttack.LockAttackCheck)
		{

			float force = playerInfo.GetAbility((int)AbilityName.MeleeAttack).CurValue * enforcement;
			if(type == TP_Info.CharacterType.Melee)
			{
				if(playerInfo.Role==TP_Info.CharacterRole.Steropi && State==CharacterState.Skilling01)
				{
					force = playerInfo.GetMagicSkill(0).CurValue;
					playerAttack.Steropi_SnowBallInput((int)force);
				}
				else if(playerInfo.Role==TP_Info.CharacterRole.Hyperion && State==CharacterState.Skilling01)
				{
					force = playerInfo.GetMagicSkill(0).CurValue;
					playerAttack.Hyperion_HotRockInput((int)force);
				}
				else if(playerInfo.Role==TP_Info.CharacterRole.Mars && State==CharacterState.Skilling03)
				{
					force = playerInfo.GetMagicSkill(2).CurValue;
					playerAttack.Mars_FightLightInput((int)force);
				}
				else
				{
					playerAttack.MeleeAttackInput(playerInfo.MeleeAttackDistance, playerInfo.MeleeAttackAngle, (int)force, hitWays);
					_multitudeAttackTimer++;
				}
				
			}
			else if(type == TP_Info.CharacterType.Magic)
			{
				if(State==CharacterState.MagicStoring)
				{
					force *= (1 + playerInfo.MagicAttackPower * 0.2f);
					Debug.Log("F:"+force + " M:" + playerInfo.MagicAttackPower);
					StartCoroutine(playerAttack.StoredMagicAttackInput((int)force));

				}
				else 
				{
					if(playerInfo.Role==TP_Info.CharacterRole.Persia && State==CharacterState.Skilling01)
					{
						force = playerInfo.GetMagicSkill(0).CurValue;
						playerAttack.Persia_ThunderousWaveInput((int)force);
					}
					else
						StartCoroutine(playerAttack.MagicAttackInput((int)force));

				}
			}
			playerAttack.LockAttackCheck = true;
		}
	}

	void SkillAttackCheck(int skillNum,Skilltype skillType, RangeColliderAttack RCA,HitWays hitWay)
	{
		if(!playerAttack.LockAttackCheck)
		{
			int force = 0;
			if(State==CharacterState.SuperSkilling)
			{
				force = (int)(playerInfo.GetSuperSkill().CurValue);
				if(playerInfo.Role==TP_Info.CharacterRole.DarkMan)
					force = (int)(force-205f);
				else if(playerInfo.Role==TP_Info.CharacterRole.Hyperion)
					force = (int)(force-175f);
			}
			else
				force = (int)(playerInfo.GetMagicSkill(skillNum).CurValue);
			playerAttack.RangeSkillInput(skillType,RCA,force,hitWay);
			
			_multitudeAttackTimer++;

			playerAttack.LockAttackCheck = true;
		}
	}
	#endregion
}
