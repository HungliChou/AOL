using UnityEngine;
using System.Collections;
[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (TP_Animator))]
[RequireComponent (typeof (TP_Motor))]
[RequireComponent (typeof (TP_Info))]
public class TP_Controller : Photon.MonoBehaviour 
{
	public bool iskilltest = false;

	public CharacterController CharacterController;
	public TP_Animator playerAnimator;
	public TP_Motor playerMotor;
	public TP_Info playerInfo;
	public InRoom_Menu roomMenu;
	public Attack playerAttack;
	public PhotonView myPhotonView;
	public static TP_Controller Instance;

	public bool IsRunning;

	public float UselightSourceTimer; 

	public int skillType;
	public int SkillType{ get{return skillType;} set{skillType = value;} }

	public TP_Animator.BuffSkillType[] buffSkillType;

	private bool isLocalPlayer = false;

	public void SetIsLocalPlayer(bool var)
	{
		isLocalPlayer = var;
	}

	void Awake() 
	{
		IsRunning = true;
		CharacterController = GetComponent("CharacterController") as CharacterController;
		playerAnimator = GetComponent<TP_Animator>();
		playerMotor = GetComponent<TP_Motor>();
		playerInfo = GetComponent<TP_Info>();
		roomMenu = GameObject.FindGameObjectWithTag("RoomMenu").GetComponent<InRoom_Menu>(); 

		myPhotonView = GetComponent<PhotonView>();
		Instance = this;

		UselightSourceTimer = 0;

		buffSkillType = new TP_Animator.BuffSkillType[2];

	}

	public void OnPlayerAttack()
	{
		playerAttack = GetComponentInChildren<Attack>();
	}

	void Start()
	{
		skillType = 0;
	}

	/*void Update() 
	{
		if(Input.GetKey(KeyCode.F12))
		{
			playerAnimator.State = TP_Animator.CharacterState.Idle;
			playerAnimator.LockAnimating = false;
			playerAnimator.LockAttacking = false;
		}

		//if(photonView.isMine){isLocalPlayer = true;}
		if(playerInfo.isAI) 
		{
			playerMotor.UpdateMotor(CalculateMoveSpeed());
			return;
		}
		if(!isLocalPlayer) return;
		if(Camera.main == null)
			return;
		if(playerAnimator.State==TP_Animator.CharacterState.Dead||playerAnimator.State==TP_Animator.CharacterState.Freeze||playerAnimator.State==TP_Animator.CharacterState.Dizzing)
			return;
	

	}*/

	void FixedUpdate()
	{
		if(playerInfo.isAI) 
		{
			playerMotor.UpdateMotor(CalculateMoveSpeed());
			return;
		}
		if(WholeGameManager.SP.InGame)
		{
			if(Game_Manager.SP.MyGameState==GameState.Esc||GameUIManager.SP.PressControlling)
				return;
		}
		if(!isLocalPlayer) return;
		if(Camera.main == null)
			return;
		if(playerAnimator.State==TP_Animator.CharacterState.Dead||playerAnimator.State==TP_Animator.CharacterState.Freeze||playerAnimator.State==TP_Animator.CharacterState.Dizzing)
			return;
		//if(playerInfo.isAI) return;
		//if(!isLocalPlayer) return;
//-------------------------
		GetLocomotionInput();
		HandleActionInput();
		//CheckRunning();
		playerMotor.UpdateMotor(CalculateMoveSpeed());
		
		
		
		if(Input.GetKeyUp(KeyCode.Z))
		{
			UselightSourceTimer = 0;
			playerAnimator.UseLightSourceInput(false);
		}
//*******************
		if(Input.GetKey(KeyCode.Z))
		{
			if(UselightSourceTimer<1)
				UselightSourceTimer += Time.fixedDeltaTime;
			else if(UselightSourceTimer==1)
			{	
				if(playerAnimator.State==TP_Animator.CharacterState.Idle||playerAnimator.State==TP_Animator.CharacterState.EnergyStoring)
				{
					playerInfo.LightSourceInput();
				}
				else
				{
					GameUIManager.SP.HudWarningLabel.Add("You Have To Stop Moving To Use LightSource!",Color.yellow,1f);
				}

				UselightSourceTimer = 0;
			}
			if(UselightSourceTimer>1)
				UselightSourceTimer = 1;

			if(playerAnimator.State==TP_Animator.CharacterState.Idle||playerAnimator.State==TP_Animator.CharacterState.EnergyStoring)
			{
				playerAnimator.UseLightSourceInput(true);	
			}

		}

		if(playerAnimator.State==TP_Animator.CharacterState.EnergyStoring)
		{
			if(Input.GetMouseButton(2))
			{
				EnergyStoreInput(0);
			}
			else if(Input.GetMouseButtonUp(2))
			{
				EnergyStoreInput(1);
			}
		}

		if(playerAnimator.LockAttacking==false)
		{
		//	if(playerAnimator.State==TP_Animator.CharacterState.Idle)
		//	{
				if(playerAnimator.LockAnimating==false)
				{
					if(Input.GetMouseButton(2))
					{
						EnergyStoreInput(0);
					}
				}
			}
		//}	
	}
	
	private int CalculateMoveSpeed()
	{
		int movespeed;
		
		movespeed = playerInfo.GetAbility((int)AbilityName.MoveSpeed).CurValue;
		
		return movespeed;
	}

	void GetLocomotionInput()
	{
		var deadZone = 0.1f;//can be change depends on our need
		
		playerMotor.VerticalVelocity = playerMotor.MoveVector.y;
		playerMotor.MoveVector = Vector3.zero;
		if(playerAnimator.CanMove())//||playerAnimator.State ==TP_Animator.CharacterState.Jumping)//can move when jump
		{
			if(Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
				playerMotor.MoveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
		
			if(Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
			{
				playerMotor.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			}
			playerAnimator.DetermineCurrentMoveDirection();
		}
	}

	public bool CheckMana(int skill)
	{
		if(playerInfo.GetVital((int)VitalName.Mana).CurValue>playerInfo.GetMagicSkill(skill-1).Mana)
		{
			return true;
		}
		else
			return false;
	}

	void HandleActionInput()
	{
		//test code
		/*if(!iskilltest)
		{
			if(Input.GetKey(KeyCode.K))
			{
				playerInfo.testKill(roomMenu.localPlayer.ID,3001,3);
				iskilltest = true;
			}
		}*/

		/*if(Input.GetKey(KeyCode.F1))
			playerAnimator.Behit();
		if(Input.GetKey(KeyCode.F2))
			playerAnimator.KnockDown();
		if(Input.GetKey(KeyCode.F4))
			playerAnimator.Freeze();
		if(Input.GetKey(KeyCode.F5))
			playerAnimator.Dizzy();*/
		/*if(Input.GetKey(KeyCode.F6))
			playerAnimator.Die();*/
		//---------------------------
		/*if(Input.GetKey(KeyCode.F2))
			playerAnimator.KnockDown();*/
		if(!playerInfo.IsGodPowering)
		{
			if(Input.GetKey(KeyCode.LeftShift))
			{
				if(playerInfo.GodPower >= 100)
				{
					GodPowerInput();
				}
				else
					Debug.Log("No Enough GodPower");
			}
		}
		/*if(Input.GetKey(KeyCode.R))
		{
			BuffSkillInput(1);
		}
		else if(Input.GetKey(KeyCode.F))
			BuffSkillInput(2);*/
		if(Input.GetKey(KeyCode.Q))
		{
			if(roomMenu.localPlayer.properties.HealthWaterNum>0)
				WaterInput(0);
		}
		else if(Input.GetKey(KeyCode.E))
		{	
			if(roomMenu.localPlayer.properties.ManaWaterNum>0)
				WaterInput(1);
		}

		if(playerAnimator.LockAnimating==false)
		{
			if(playerAnimator.lockRolling==false)
			{
				if(playerAnimator.Mode==TP_Animator.CharacterMode.None)
				{
					if(Input.GetButton("Roll")&&(playerAnimator.MoveDirection==TP_Animator.Direction.Left||playerAnimator.MoveDirection==TP_Animator.Direction.Right||playerAnimator.MoveDirection==TP_Animator.Direction.LeftForward||playerAnimator.MoveDirection==TP_Animator.Direction.RightForward))
					{
					//Jump();
						Roll();
					}
				}
			}
			if(Input.GetKey(KeyCode.Alpha1))
				PreSkillInput(1);
			if(Input.GetKey(KeyCode.Alpha2))
				PreSkillInput(2);
			if(Input.GetKey(KeyCode.Alpha3))
				PreSkillInput(3);
			if(Input.GetKey(KeyCode.Alpha4))
				PreSkillInput(4);
			if(Input.GetKey(KeyCode.Alpha5))
			{
				if(roomMenu.localPlayer.properties.ThrowingWeaponNum>0)
					PreSkillInput(5);
			}

			if(playerAnimator.Mode==TP_Animator.CharacterMode.Skilling)
			{
				if(Input.GetMouseButton(0))
					SkillInput();
				else if(Input.GetMouseButton(1))
				{
					playerAnimator.CurrentSkillRange.SetActive(false);
					playerAnimator.Mode = TP_Animator.CharacterMode.None;
				}
			}

			if(playerAnimator.State==TP_Animator.CharacterState.KnockingDown)
			{
				if(Input.anyKey)
				{
					StandUpInput();
				}
			}

			if(Input.GetKey(KeyCode.C))
			{
				if(playerInfo.GetVital((int)VitalName.Energy).CurValue==playerInfo.MaxEnergy)
				{
					PreSkillInput(5);
					//playerAnimator.LockAnimating = true;
				}
				else
					Debug.Log("Not Enough Energy");
			}
		}

		if(playerAnimator.LockAttacking==false)
		{
			if(playerAnimator.State==TP_Animator.CharacterState.Idle)
			{
				if(playerAnimator.LockAnimating==false)
				{
					/*if(Input.GetMouseButton(2))
					{
						EnergyStoreInput(0);
					}*/
					if(playerAnimator.Mode==TP_Animator.CharacterMode.None)
					{	
						if(Input.GetMouseButton(0))
						{
							if(playerInfo.Type==TP_Info.CharacterType.Melee)
								MeleeAttackInput(0);
							else
								MagicAttackInput(0);
						}	
						else if(Input.GetMouseButtonDown(1))
						{
							if(playerInfo.Type==TP_Info.CharacterType.Melee)
								MeleeAttackInput(1);
							else
								MagicAttackInput(1);
						}
					}
				}
			}
			else if(playerAnimator.State==TP_Animator.CharacterState.Running)
			{
				/*if(Input.GetMouseButton(2))
				{
					EnergyStoreInput(0);
				}*/
				/*if(Input.GetKey(KeyCode.Alpha1))
					PreSkillInput(1);
				if(Input.GetKey(KeyCode.Alpha2))
					PreSkillInput(2);
				if(Input.GetKey(KeyCode.Alpha3))
					PreSkillInput(3);
				if(Input.GetKey(KeyCode.Alpha4))
					PreSkillInput(4);
				if(Input.GetKey(KeyCode.Alpha5))
				{
					if(roomMenu.localPlayer.properties.ThrowingWeaponNum>0)
						PreSkillInput(5);
				}*/
				if(playerAnimator.Mode==TP_Animator.CharacterMode.None)
				{	
					if(playerAnimator.LockAnimating==false)
					{
						if(Input.GetMouseButton(0))
						{
							if(playerInfo.Type==TP_Info.CharacterType.Melee)
								MeleeAttackInput(0);
							else
								MagicAttackInput(0);
						}	
						else if(Input.GetMouseButtonDown(1))
						{
							if(playerInfo.Type==TP_Info.CharacterType.Magic)
								MagicAttackInput(1);
						}
					}
				}
				/*else if(playerAnimator.Mode==TP_Animator.CharacterMode.Skilling)
				{
					if(Input.GetMouseButton(0))
						SkillInput();
					else if(Input.GetMouseButton(1))
					{
						playerAnimator.CurrentSkillRange.SetActive(false);
						playerAnimator.Mode = TP_Animator.CharacterMode.None;
					}
				}*/
			}
			else if(playerAnimator.State==TP_Animator.CharacterState.MagicStoring)
			{
				if(Input.GetMouseButton(1))
				{
					//if(TP_Info.Instance.AttackWay==TP_Info.CharacterAttack.MagicAttack)
						MagicAttackInput(1);
				}
				else if(Input.GetMouseButtonUp(1))
				{
					MagicAttackInput(2);
				}
			}
			else if(playerAnimator.CanAttack())
			{
				if(playerAnimator.Mode==TP_Animator.CharacterMode.None)
				{	
					if(playerInfo.Type==TP_Info.CharacterType.Melee)
					{
						if(Input.GetMouseButton(0))
							MeleeAttackInput(0);
						else if(Input.GetMouseButton(1))
							MeleeAttackInput(1);
					}
				}
			}
		}


	}

	void Roll()
	{
		playerAnimator.Roll();
		Debug.Log("RollCCC");
	}

	/*void Jump()//can trigger all kinds of motion such as sound, particle...
	{
		playerMotor.Jump();
		playerAnimator.Jump();
	}*/

	void StandUpInput()
	{
		playerAnimator.StandUp();
	}

	void GodPowerInput()
	{
		playerInfo.IsGodPowering = true;
		playerMotor.OnCharacterAlignWithCamera();
		playerInfo.UseGodPowerInput(true);
	}

	public void EnergyStoreInput(int mouseType)
	{
		if(playerAnimator.playerCam!=null)
			playerMotor.OnCharacterAlignWithCamera();
		if(mouseType==0)
		{
			playerAnimator.EnergyStoreInput(0);
			if(playerInfo.GetVital((int)VitalName.Energy).CurValue<playerInfo.MaxEnergy)
				playerInfo.GetVital((int)VitalName.Energy).BaseValue += 1;
		}
		else
			playerAnimator.EnergyStoreInput(1);
	}

	void MeleeAttackInput(int mouseType)
	{
//		Debug.Log("INput");
		if(playerAnimator.playerCam!=null)
			playerMotor.OnCharacterAlignWithCamera();
		if(mouseType==0)
		{
			playerAnimator.MeleeAttackInput(0);
		}
		else if(mouseType==1)
			playerAnimator.MeleeAttackInput(1);
	}

	public void MagicAttackInput(int mouseType)
	{
		if(playerAnimator.playerCam!=null)
			playerMotor.OnCharacterAlignWithCamera();
		if(mouseType==0)
		{
			playerAnimator.MagicAttackInput(0);
		}
		else if(mouseType==1)
		{
			playerAnimator.MagicAttackInput(1);
			if(playerInfo.MagicAttackPower < 3)
				playerInfo.MagicAttackPower += Time.deltaTime ;
			else if(playerInfo.MagicAttackPower > 3)
				playerInfo.MagicAttackPower = 3;
		}
		else
		{
			if(playerInfo.MagicAttackPower >1f)
				playerAnimator.MagicAttackInput(2);
			else
				playerAnimator.State = TP_Animator.CharacterState.Idle;
			myPhotonView.RPC("DestroyMagicStoredEff",PhotonTargets.All);
			playerInfo.MagicAttackPower = 0;
		}

	}

	public bool CheckSkillCDTime(int skillType)
	{
		bool canSkill = false;
		float lastSkillTime = playerInfo._skillCDTimer[skillType-1];
		if(lastSkillTime==0)
			canSkill = true;
		else
		{
			if(Time.time - lastSkillTime >= playerInfo.GetMagicSkill(skillType-1).CDTime)
			{
				canSkill = true;
				//Debug.Log("TimePassed: " + (Time.time - lastSkillTime) + "CD: " + playerInfo.GetMagicSkill(skillType-1).CDTime);
			}
			else
				canSkill = false;
		}
		return canSkill;
	}

	void PreSkillInput(int lockSkillType)
	{
		if(lockSkillType==5)
		{
			skillType = 5;
			playerAnimator.Mode = TP_Animator.CharacterMode.Skilling;
			playerAnimator.PreSkillInput(lockSkillType);
//			Debug.Log("lock skill SuperSkill");
		}
		else
		{
			if(CheckMana(lockSkillType))
			{
				if(CheckSkillCDTime(lockSkillType))
				{
					if(playerAnimator.CurrentSkillRange!=null)
						playerAnimator.CurrentSkillRange.SetActive(false);
					switch(lockSkillType)
					{
					default:
						skillType = lockSkillType;
						playerAnimator.Mode = TP_Animator.CharacterMode.Skilling;
						playerAnimator.PreSkillInput(lockSkillType);
						Debug.Log("lock skill " + lockSkillType);
						break;
					case 5:
						skillType = 5;
						playerAnimator.Mode = TP_Animator.CharacterMode.Skilling;
						Debug.Log("lock skill 5-Throw Weapon");
						break;
					}
				}
			}
		}

	}

	public void SkillInput()
	{
		if(playerAnimator.playerCam)
			playerMotor.OnCharacterAlignWithCamera();
		//play skill animation and checkAttack
		playerAnimator.SkillInput(skillType);
		
		//if throwWeapon
		if(skillType==6)
		{
			roomMenu.localPlayer.properties.ThrowingWeaponNum--;
			playerInfo.CheckThrowWeaponNum();
		}
		//if superSkill
		else if(skillType==5)
			playerInfo.GetVital((int)VitalName.Energy).BaseValue = 0;

		//record time after using the skill for calculating CDtime
		if(skillType!=5&&skillType>0)
		{
			playerInfo.GetVital((int)VitalName.Mana).DamageValue += playerInfo.GetMagicSkill(skillType-1).Mana;
			playerInfo._skillCDTimer[skillType-1] = Time.time;
		}
	}

	void BuffSkillInput(int buffSkillPos)
	{
		if(playerInfo.GetBuffSkill(buffSkillPos-1).BuffName!=BuffSkillName.None)
		{
			playerAnimator.BuffSkillInput(playerInfo.GetBuffSkill(buffSkillPos-1).BuffName);
			playerInfo.BuffSkillInput(buffSkillPos-1);
		}
	}

	public void WaterInput(int waterType)
	{
		if(waterType==0)
		{
			roomMenu.localPlayer.properties.HealthWaterNum--;
			Debug.Log("use HealthWater");
		}
		else if(waterType==1)
		{
			roomMenu.localPlayer.properties.ManaWaterNum--;
			Debug.Log("use ManaWater");
		}
		playerInfo.UseWaterInput(waterType);
		playerAnimator.UseWaterInput(waterType);
	}



}
 