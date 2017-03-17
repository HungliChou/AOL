using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TP_Info : MonoBehaviour 
{
	#region Enum
	public enum CharacterRole
	{
		DarkMan,Mars,Persia,Theia,Steropi,Hyperion,MonsterBoss,Monster
	}

	public enum CharacterType
	{
		None,Melee,Magic
	}

	public enum ThrowingWeapons
	{
		None,Firework,BigRock
	}
	#endregion

	public bool isAI;
	public bool isMonster;
	#region other scripts reference
	public NetworkController playerNetController;
	public PhotonView playerView;
	public TP_Controller playerController;
	public TP_Animator playerAnimator;
	public NJGMapItem it;
	public InRoom_Menu roomMenu;
	public LightSourceManager L_LSManager;
	public LightSourceManager D_LSManager;
	public List<Rigidbody> RigidEffectPref = new List<Rigidbody>();
	public List<GameObject> GOEffectPref = new List<GameObject>();
	public Transform myTransform;
	public Transform LeftHandTrans;
	//public List<HUDText> HDT = new List<HUDText>();
	//public UIFollowTarget HDTfollow;
	public Transform HUDTextTarget;
	public GameObject MeleeAttack;
	public HUDText HeadNameHUD;

	#endregion
	/*public bool _isDead;
	public bool IsDead{get{return _isDead;}set{_isDead = value;}}*/
	public CharacterRole _role;
	public CharacterRole Role {get{return _role;} set{_role = value;}}
	public CharacterType _type;
	public CharacterType Type {get{return _type;} set{_type = value;}}

	#region for assigning values to info
	private const int _maxlevel = 15;
	public int STARTING_ATTIBUTE_VALUE_STRENGTH = 50;
	public int STARTING_ATTIBUTE_VALUE_NIMBLENESS = 20;
	public int STARTING_ATTIBUTE_VALUE_CONSTITUTION = 20;
	public int STARTING_ATTIBUTE_VALUE_CONCENTRATION = 5;
	public int STARTING_ATTIBUTE_VALUE_WILLPOWER = 10;
	public int STARTING_VITAL_VALUE_HEALTH = 190;
	public const int MAX_VITAL_VALUE_ENERGY = 2000;
	public int STARTING_VITAL_VALUE_ENERGY = 0;
	public int STARTING_VITAL_VALUE_MANA = 300;
	public int STARTING_ABILITY_VALUE = 0;
	public int STARTING_ABILITY_VALUE_MeleeAttack = 40;
	public int STARTING_ABILITY_VALUE_SkillAttack = 8;
	public int STARTING_ABILITY_VALUE_Defense = 40;
	public int STARTING_ABILITY_VALUE_ATTACKSPEED = 40;
	public int STARTING_ABILITY_VALUE_MOVESPEED = 8;
	private const int STARTING_BUFFSKILL_VALUE_Buff_Speed = 10;
	private const int STARTING_BUFFSKILL_VALUE_Buff_Power = 20;
	#endregion

	#region character info decleration
	public  string _name;
	public int playerListNum;//what cnt in playerList List
	public int _id;
	public int _team;
	public int _level;
	public int _expTpLevelUp;
	public int _freeExp;
	public int MaxHealth;
	public float _godPower;												//amount of god power now
	public float _magicAttackPower;
	public float _meleeAttackDistance = 6;
	public float _meleeAttackAngle = 0.5f;
	public bool _isGodPowering;
	public bool _isArtifacting;
	public bool _canShopping;

	public Attribute[] _primaryAttribute;
	public Vital[] _vital;
	public Ability[] _ability;
	public MagicSkill[] _magicskill;
	public BuffSkill[] _allBuffSkill;
	public BuffSkill[] _buffskill;
	public SuperSkill _superskill;
	public ThrowingWeapons _throwingWeapon;

	//public int _healthWaterNum;
	public int _manaWaterNum;

	public int[] _godPowerBuffTemp;
	public int[] _artifactBuffTemp;
	#endregion

	public int _attackBywhom;
	public int AttackByWhom{get{return _attackBywhom;}set{_attackBywhom = value;}}
	public bool isDead;
	public bool isAbilityModified;

	#region calculation tools
	public float[] _skillCDTimer;
	public List<BuffCondition> _buffcondition;
	public List<BuffCondition> tempBuff = new List<BuffCondition>();
	#endregion

	#region getter and setter
	public string Name{get{return _name;}set{ _name = value;}}
	public int ID{get{return _id;}set{_id = value;}}
	public int Team{get{return _team;}set{_team = value;}}
	public int Level{get{return _level;}set{_level = value;}}
	public int ExpTpLevelUp{get{return _expTpLevelUp;}set{ _expTpLevelUp = value;}}
	public int FreeExp{get{return _freeExp;}set{ _freeExp = value;}}
	public float GodPower{get{return _godPower;}set{_godPower = value;}}
	public int MaxEnergy{get{return MAX_VITAL_VALUE_ENERGY;}}
	public float MagicAttackPower{get{return _magicAttackPower;}set{_magicAttackPower = value;}}
	public float MeleeAttackDistance{get{return _meleeAttackDistance;}set{_meleeAttackDistance = value;}}
	public float MeleeAttackAngle{get{return _meleeAttackAngle;}set{_meleeAttackAngle = value;}}
	public bool IsGodPowering{get{return _isGodPowering;}set{_isGodPowering = value;}}
	public bool IsArtifacting{get{return _isArtifacting;}set{_isArtifacting = value;}}
	public bool CanShopping{get{return _canShopping;}set{_canShopping = value;}}
	public ThrowingWeapons ThrowingWeapon{get{return _throwingWeapon;}set{_throwingWeapon = value;}}
	#endregion

	public void Awake() 
	{
		//playersInfo = GameObject.Find("_SCRIPTS").GetComponent<PlayersInfo>();
		myTransform = transform;
		playerNetController = GetComponent<NetworkController>();
		playerView = GetComponent<PhotonView>();
		playerController = GetComponent<TP_Controller>();
		playerAnimator = GetComponent<TP_Animator>();
		it = transform.GetComponent<NJGMapItem>();
		roomMenu = GameObject.FindGameObjectWithTag("RoomMenu").GetComponent<InRoom_Menu>(); 
		#region create instance for info
		_name = string.Empty;
		_expTpLevelUp = 100;
		_level = 1;
		_freeExp = 0;
		_isGodPowering = false;
		_isArtifacting = false;
		isAbilityModified = false;
		//_isDead = false;

		_primaryAttribute = new Attribute[Enum.GetValues(typeof(AttributeName)).Length];
		_vital = new Vital[Enum.GetValues(typeof(VitalName)).Length];
		_ability = new Ability[Enum.GetValues(typeof(AbilityName)).Length];
		//_meleeskill = new Skill[Enum.GetValues(typeof(MeleeSkillName)).Length];
		_magicskill = new MagicSkill[4];
		_allBuffSkill = new BuffSkill[Enum.GetValues(typeof(BuffSkillName)).Length];
		_buffskill = new BuffSkill[2];
		_godPowerBuffTemp = new int[7];
		_artifactBuffTemp = new int[7]; 
		_superskill = new SuperSkill();
		_throwingWeapon = ThrowingWeapons.None;
		#endregion

		#region create instance for calculation
		_skillCDTimer = new float[4];
		#endregion

		SetupPrimaryAttributes();
		SetupVitals();
		SetupAbilities();
		//SetupMeleeSkills();
		SetupMagicSkills();
		SetupAllBuffSkills();
		SetupBuffSkills();
		SetupSuperSkill();


		//GetVital((int)VitalName.Energy).BaseValue += 2000;
		/*GetVital((int)VitalName.Health).DamageValue += 500;
		GetVital((int)VitalName.Mana).DamageValue += 100;*/

	}

	public void OnHDTName()
	{
		HUDText HDT = CreateHDT();
		HDT.gameObject.GetComponent<HudScript>().enabled = false;

		UILabel headName = HDT.transform.FindChild("Name").GetComponent<UILabel>();
		headName.gameObject.SetActive(true);
		if(_name!=string.Empty)
		{
			headName.text = _name;
		}
		HeadNameHUD = HDT;
	}

	public void OffHDTName()
	{
		if(HeadNameHUD)
		{
			if(HeadNameHUD.gameObject)
				Destroy(HeadNameHUD.gameObject);
		}
	}

	public IEnumerator ShowAim(int team)
	{
		while(GameUIManager.SP==null)
		{
			yield return new WaitForSeconds(0.1f);
		}

		foreach(GameObject aim in GameUIManager.SP.AimUI)
		{
			aim.SetActive(false);
		}
		if(_type==CharacterType.Magic)
		{
			GameUIManager.SP.AimUI[team-1].SetActive(true);
		}
	}
	

	#region LevelUp and GodPower Calculation
	public void AddExp(int exp)
	{
		if(_level<_maxlevel)
		{
			_freeExp += exp;
		}
	}
	
	//take avg of all of the players skills and assign that as the player level
	public void CalculateLevel()
	{ 
		if(_freeExp>=_expTpLevelUp)
		{
			LevelUpInput();
			_freeExp -= _expTpLevelUp;
			_expTpLevelUp = (int)(_expTpLevelUp * 1.25f);
		}
	}

	private void LevelUpInput()
	{
		if(IsAIButNotMasterClients())
			return;
		if(playerView.isMine)
		{
			playerView.RPC("LevelUp",PhotonTargets.All);
			playerView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Other,5);
		}
	}

	private void AttributeLevelUp()
	{
		for(int cnt = 0; cnt < Enum.GetValues(typeof(AttributeName)).Length;cnt++)
		{
			if(cnt==0||cnt==2||cnt==3||cnt==4)//Strength,Constitution,Willpower,Concentration
				GetPrimaryAttribute(cnt).BaseValue += (int)(GetPrimaryAttribute(cnt).BaseValue * 0.07f);
			else if(cnt==1)//Nimbleness
				GetPrimaryAttribute(cnt).BaseValue += (int)(GetPrimaryAttribute(cnt).BaseValue * 0.08f);
		}
		/*float levelUpModifierValue = _superskill.CurValue / 1.09f;

		_superskill.BaseValue -= (int)(_superskill.CurValue - levelUpModifierValue);*/
	}

	[RPC]
	public void LevelUp()
	{
		_level++;

		//Adjust Attrubute
		AttributeLevelUp();

		InRoom_Menu.PlayerInfoData pla = InRoom_Menu.SP.GetPlayerFromID(playerView.viewID);
			
		pla.properties.level++;
	}

	/*private void AddGodPower(int power)
	{
		_godPower += power;
	}*/
	#endregion

	#region SetupEachInfo
	private void SetupPrimaryAttributes()
	{
		for(int cnt =0; cnt< _primaryAttribute.Length; cnt++){
			_primaryAttribute[cnt] = new Attribute();
			if(cnt==0)
				GetPrimaryAttribute(cnt).Name = AttributeName.Strength;
			else if(cnt==1)
				GetPrimaryAttribute(cnt).Name = AttributeName.Nimbleness;
			else if(cnt==2)
				GetPrimaryAttribute(cnt).Name = AttributeName.Constitution;
			else if(cnt==3)
				GetPrimaryAttribute(cnt).Name = AttributeName.Concentration;
			else if(cnt==4)
				GetPrimaryAttribute(cnt).Name = AttributeName.Willpower;
		}
		SetupAttributesPoints();
	}
	
	private void SetupVitals()
	{
		for(int cnt =0; cnt< _vital.Length; cnt++){
			_vital[cnt] = new Vital();
			if(cnt==0)
				GetVital(cnt).Name = VitalName.Health;
			else if(cnt==1)
				GetVital(cnt).Name = VitalName.Energy;
			else if(cnt==2)
				GetVital(cnt).Name = VitalName.Mana;
		}
		SetupVitalModifiers();
		SetupVitalsPoints();
	}
	
	private void SetupAbilities()
	{
		for(int cnt =0; cnt< _ability.Length; cnt++){
			_ability[cnt] = new Ability();
			if(cnt==0)
				GetAbility(cnt).Name = AbilityName.MeleeAttack;
			else if(cnt==1)
				GetAbility(cnt).Name = AbilityName.AttackSpeed;
			else if(cnt==2)
				GetAbility(cnt).Name = AbilityName.MoveSpeed;
			else if(cnt==3)
				GetAbility(cnt).Name = AbilityName.Defense;
			else if(cnt==4)
				GetAbility(cnt).Name = AbilityName.Crits;
			else if(cnt==5)
				GetAbility(cnt).Name = AbilityName.SkillAttack;
			else if(cnt==6)
				GetAbility(cnt).Name = AbilityName.BuffPower;
		}
		SetupAbilityModifiers();
		SetupAbilityPoints();
	
		isAbilityModified = true;
	}
	
	/*private void SetupMeleeSkills(){
		for(int cnt =0; cnt< _meleeskill.Length; cnt++){
			_meleeskill[cnt] = new Skill();
		}
		SetupMeleeSkillModifiers();
	}*/
	
	private void SetupMagicSkills()
	{
		for(int cnt =0; cnt< _magicskill.Length; cnt++){
			_magicskill[cnt] = new MagicSkill();
		}
		SetupMagicSkillBase();
		SetupMagicSkillModifiers();
	}

	private void SetupAllBuffSkills()
	{
		for(int cnt =0; cnt< _allBuffSkill.Length; cnt++){
			_allBuffSkill[cnt] = new BuffSkill();
		}
		SetupAllBuffSkillBase();
		//SetupAllBuffSkillModifiers();
		SetupAllBuffSkillPoints();
	}

	private void SetupBuffSkills()
	{
		for(int cnt =0; cnt< _buffskill.Length; cnt++){
			_buffskill[cnt] = new BuffSkill();
		}
		InitiateBuffSkill();
	}

	private void SetupSuperSkill()
	{
		SetupSuperSkillBase();
		SetupSuperSkillModifiers();
	}
	#endregion

	#region SetupEachInfoPoints
	private void SetupAttributesPoints()
	{
		for(int cnt = 0; cnt < Enum.GetValues(typeof(AttributeName)).Length;cnt++)
		{
			if(cnt==0)
				GetPrimaryAttribute(cnt).BaseValue = STARTING_ATTIBUTE_VALUE_STRENGTH;
			else if(cnt==1)
				GetPrimaryAttribute(cnt).BaseValue = STARTING_ATTIBUTE_VALUE_NIMBLENESS;
			else if(cnt==2)
				GetPrimaryAttribute(cnt).BaseValue = STARTING_ATTIBUTE_VALUE_CONSTITUTION;
			else if(cnt==3)
				GetPrimaryAttribute(cnt).BaseValue = STARTING_ATTIBUTE_VALUE_CONCENTRATION;
			else
				GetPrimaryAttribute(cnt).BaseValue = STARTING_ATTIBUTE_VALUE_WILLPOWER;
		}
	}
	private void SetupVitalsPoints()
	{
		for(int cnt = 0; cnt < Enum.GetValues(typeof(VitalName)).Length;cnt++)
		{
			if(cnt==0)
				GetVital(cnt).BaseValue += STARTING_VITAL_VALUE_HEALTH;
			else if(cnt==1)
				GetVital(cnt).BaseValue += STARTING_VITAL_VALUE_ENERGY;
			else if(cnt==2)
				GetVital(cnt).BaseValue += STARTING_VITAL_VALUE_MANA;
		}
	}
	private void SetupAbilityPoints()
	{
		for(int cnt = 0; cnt < Enum.GetValues(typeof(AbilityName)).Length;cnt++)
		{
			if(cnt==0)
				GetAbility(cnt).BaseValue += STARTING_ABILITY_VALUE_MeleeAttack;
			else if(cnt==1)
				GetAbility(cnt).BaseValue += STARTING_ABILITY_VALUE_ATTACKSPEED;
			else if(cnt==2)
				GetAbility(cnt).BaseValue += STARTING_ABILITY_VALUE_MOVESPEED;
			else if(cnt==3)
				GetAbility(cnt).BaseValue += STARTING_ABILITY_VALUE_Defense;
			else if(cnt==5)
				GetAbility(cnt).BaseValue += STARTING_ABILITY_VALUE_SkillAttack;
		}
	}
	/*private void SetupMeleeSkill()
	{
		for(int cnt = 0; cnt < Enum.GetValues(typeof(MeleeSkillName)).Length;cnt++)
		{
			if(cnt==0)
				_toon.GetMeleeSkill(cnt).BaseValue += STARTING_MELEESKILL_VALUE_OFFENSE001;
		}
	}*/
	/*private void SetupMagicSkill()
	{
		for(int cnt = 0; cnt < _magicskill.Length;cnt++)
		{
			if(cnt==0)
				//_toon.GetMagicSkill(cnt).BaseValue += STARTING_MAGICSKILL_VALUE_OFFENSE001;
		}
	}*/
	private void SetupAllBuffSkillPoints()
	{
		for(int cnt = 0; cnt < _allBuffSkill.Length;cnt++)
		{
			if(cnt==1)
				GetAllBuffSkill(cnt).BaseValue += STARTING_BUFFSKILL_VALUE_Buff_Speed;
			else if(cnt==2)
				GetAllBuffSkill(cnt).BaseValue += STARTING_BUFFSKILL_VALUE_Buff_Power;
		}
	}
	#endregion

	#region tools for Get Each Info
	public Attribute GetPrimaryAttribute(int index){
		return _primaryAttribute[index];
	}
	
	public Vital GetVital(int index){
		return _vital[index];
	}
	
	public Ability GetAbility(int index){
		return _ability[index];
	}
	
	/*public Skill GetMeleeSkill(int index){
		return _meleeskill[index];
	}*/
	
	public MagicSkill GetMagicSkill(int index){
		return _magicskill[index];
	}

	public SuperSkill GetSuperSkill(){
		return _superskill;
	}

	public BuffSkill GetAllBuffSkill(int index){
		return _allBuffSkill[index];
	}

	public BuffSkill GetBuffSkill(int index){
		return _buffskill[index];
	}
	#endregion

	#region Setup Each info Base and Modifiers
	private void SetupVitalModifiers(){
		//health
		ModifyingAttribute healthModifier = new ModifyingAttribute();
		healthModifier.attribute = GetPrimaryAttribute((int)AttributeName.Constitution);
		healthModifier.ratio = 50;
		
		GetVital((int)VitalName.Health).AddModifier(healthModifier);
		//GetVital((int)VitalName.Health).AddModifier(new ModifyingAttribute{attibute = GetPrimaryAttribute((int)AttributeName.Constitution),ratio = .5f)}
		
		//mana
		ModifyingAttribute manaModifier = new ModifyingAttribute();
		manaModifier.attribute = GetPrimaryAttribute((int)AttributeName.Willpower);
		manaModifier.ratio = 10;
		
		GetVital((int)VitalName.Mana).AddModifier(manaModifier);
	}
	
	private void SetupAbilityModifiers(){
		//MeleeAttack
		GetAbility((int)AbilityName.MeleeAttack).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 1));
		//AttackSpeed:need to be divided by 5 when calculating
		GetAbility((int)AbilityName.AttackSpeed).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Nimbleness), 0.3f));
		//MoveSpeed:need to be divided by 10 when calculating
		GetAbility((int)AbilityName.MoveSpeed).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Nimbleness), 0.2f));
		//Defense
		GetAbility((int)AbilityName.Defense).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Constitution), 1f));
		//Crits
		GetAbility((int)AbilityName.Crits).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 0.2f));
		//MagicAttack
		GetAbility((int)AbilityName.SkillAttack).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Willpower), 1f));
		//BuffPower
		GetAbility((int)AbilityName.BuffPower).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 1f));
	}
	
	/*private void SetupMeleeSkillModifiers(){
		//melee offence
		GetMeleeSkill((int)MeleeSkillName.Melee_Offense001).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 1f));
		
	}*/

	private void SetupSuperSkillBase(){

		_superskill.SuperName = (SuperSkillName)((int)_role);
		_superskill.BaseValue += 200;
		/*switch(_role)
		{
		case CharacterRole.DarkMan:
			_superskill.SuperName = SuperSkillName.DarkMan;
			_superskill.BaseValue += 200;
			break;
		case CharacterRole.Theia:
			_superskill.SuperName = SuperSkillName.Theia;
			_superskill.BaseValue += 200;
			break;
		case CharacterRole.Persia:
			_superskill.SuperName = SuperSkillName.Persia;
			_superskill.BaseValue += 200;
		}*/
	}

	private void SetupSuperSkillModifiers(){
		switch(_role)
		{
		case CharacterRole.Mars:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		case CharacterRole.DarkMan:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		case CharacterRole.Theia:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		case CharacterRole.Persia:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		case CharacterRole.Steropi:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		case CharacterRole.Hyperion:
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MeleeAttack), 0.5f));
			_superskill.AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 0.5f));
			break;
		}
	}

	private void SetupMagicSkillBase(){
		switch(_role)
		{
		case CharacterRole.Mars:
			GetMagicSkill(0).MagicName = MagicSkillName.Mars_Assault;
			GetMagicSkill(0).BaseValue += 0;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Mars_Stun;
			GetMagicSkill(1).BaseValue += 0;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 20;
			GetMagicSkill(2).MagicName = MagicSkillName.Mars_FightLight;
			GetMagicSkill(2).BaseValue += 30;
			GetMagicSkill(2).CDTime = 20f;
			GetMagicSkill(2).Mana = 30;
			GetMagicSkill(3).MagicName = MagicSkillName.Mars_AresBlessing;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 20f;
			GetMagicSkill(3).Mana = 30;
			break;
		case CharacterRole.DarkMan:
			GetMagicSkill(0).MagicName = MagicSkillName.Darkman_01;
			GetMagicSkill(0).BaseValue += 0;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Darkman_02;
			GetMagicSkill(1).BaseValue += 0;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 20;
			GetMagicSkill(2).MagicName = MagicSkillName.Darkman_03;
			GetMagicSkill(2).BaseValue += 20;
			GetMagicSkill(2).CDTime = 20f;
			GetMagicSkill(2).Mana = 30;
			GetMagicSkill(3).MagicName = MagicSkillName.Darkman_04;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 20f;
			GetMagicSkill(3).Mana = 30;
			break;
		case CharacterRole.Theia:
			GetMagicSkill(0).MagicName = MagicSkillName.Theia_CrossFire;
			GetMagicSkill(0).BaseValue += 0;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Theia_Meteorite;
			GetMagicSkill(1).BaseValue += 20;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 30;
			GetMagicSkill(2).MagicName = MagicSkillName.Theia_Pray;
			GetMagicSkill(2).BaseValue += 750;
			GetMagicSkill(2).CDTime = 15f;
			GetMagicSkill(2).Mana = 30;
			GetMagicSkill(3).MagicName = MagicSkillName.Theia_Crusade;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 25f;
			GetMagicSkill(3).Mana = 30;
			break;
		case CharacterRole.Persia:
			GetMagicSkill(0).MagicName = MagicSkillName.Persia_ThunderousWave;
			GetMagicSkill(0).BaseValue += 10;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Persia_Hail;
			GetMagicSkill(1).BaseValue += 15;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 30;
			GetMagicSkill(2).MagicName = MagicSkillName.Persia_GroundedLightning;
			GetMagicSkill(2).BaseValue += 25;
			GetMagicSkill(2).CDTime = 25f;
			GetMagicSkill(2).Mana = 50;
			GetMagicSkill(3).MagicName = MagicSkillName.Persia_Curse;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 25f;
			GetMagicSkill(3).Mana = 30;
			break;
		case CharacterRole.Steropi:
			GetMagicSkill(0).MagicName = MagicSkillName.Steropi_SnowBall;
			GetMagicSkill(0).BaseValue += 10;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Steropi_RollingStrike;
			GetMagicSkill(1).BaseValue += 20;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 20;
			GetMagicSkill(2).MagicName = MagicSkillName.Steropi_SlamCrush;
			GetMagicSkill(2).BaseValue += 30;
			GetMagicSkill(2).CDTime = 20f;
			GetMagicSkill(2).Mana = 50;
			GetMagicSkill(3).MagicName = MagicSkillName.Steropi_Roar;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 20f;
			GetMagicSkill(3).Mana = 30;
			break;
		case CharacterRole.Hyperion:
			GetMagicSkill(0).MagicName = MagicSkillName.Hyperion_HotRock;
			GetMagicSkill(0).BaseValue += 10;
			GetMagicSkill(0).CDTime = 10f;
			GetMagicSkill(0).Mana = 20;
			GetMagicSkill(1).MagicName = MagicSkillName.Hyperion_Unbeatable;
			GetMagicSkill(1).BaseValue += 2;
			GetMagicSkill(1).CDTime = 15f;
			GetMagicSkill(1).Mana = 20;
			GetMagicSkill(2).MagicName = MagicSkillName.Hyperion_BombSpreading;
			GetMagicSkill(2).BaseValue += 20;
			GetMagicSkill(2).CDTime = 20f;
			GetMagicSkill(2).Mana = 50;
			GetMagicSkill(3).MagicName = MagicSkillName.Hyperion_Intimadate;
			GetMagicSkill(3).BaseValue += 0;
			GetMagicSkill(3).CDTime = 20f;
			GetMagicSkill(3).Mana = 30;
			break;
		}
	}
	private void SetupMagicSkillModifiers(){
		//magic offence
		switch(_role)
		{
		case CharacterRole.Mars:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		case CharacterRole.DarkMan:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		case CharacterRole.Theia:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		case CharacterRole.Persia:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.05f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		case CharacterRole.Steropi:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.05f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		case CharacterRole.Hyperion:
			GetMagicSkill(0).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(1).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1f));
			GetMagicSkill(2).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 1.1f));
			GetMagicSkill(3).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.SkillAttack), 0.05f));
			break;
		}
		//GetMagicSkill((int)MagicSkillName.Magic_Offense001).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.MagicAttack), 1f));
	}

	private void SetupAllBuffSkillBase(){
			_allBuffSkill[0].BuffName = BuffSkillName.None;
			_allBuffSkill[0].CDTime = 0;
			_allBuffSkill[1].BuffName = BuffSkillName.Buff_Speed;
			_allBuffSkill[1].CDTime = 30f;
			_allBuffSkill[2].BuffName = BuffSkillName.Buff_Power;
			_allBuffSkill[2].CDTime = 30f;
	}

	private void SetupAllBuffSkillModifiers(){
		//buff
		//GetBuffSkill((int)BuffSkillName.Buff_Speed).AddModifier_Abi(new ModifyingAbility(GetAbility((int)AbilityName.BuffPower), 1f));
		//GetBuffSkill((int)BuffSkillName.Buff_Power).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .5f));
	}
	#endregion

	//No buff skill when game starts
	private void InitiateBuffSkill()
	{
		_buffskill[0] = _allBuffSkill[2];
		_buffskill[1] = _allBuffSkill[1];
	}

	//Player got new buff skill
	private void GotBuffSkill(int buffSkillType)
	{
		if(_buffskill[0].BuffName!=BuffSkillName.None)
			_buffskill[0] = _allBuffSkill[buffSkillType];
		else
			_buffskill[1] = _allBuffSkill[buffSkillType];
	}

	public void StatUpdate(){
		for(int cnt = 0; cnt < _vital.Length; cnt++)
		{
			_vital[cnt].Update();
			GetVital(cnt).CurValue = GetVital(cnt).AdjustedBaseValue;
			GetVital(cnt).MaxValue = GetVital(cnt).AdjustedMaxValue;
		}
		for(int cnt = 0; cnt < _ability.Length; cnt++)
		{
			_ability[cnt].Update();
			GetAbility(cnt).CurValue = GetAbility(cnt).AdjustedBaseValue;
		}
		/*for(int cnt = 0; cnt < _meleeskill.Length; cnt++)
			_meleeskill[cnt].Update();*/
		for(int cnt = 0; cnt < _magicskill.Length; cnt++)
		{
			_magicskill[cnt].Update();
			GetMagicSkill(cnt).CurValue = GetMagicSkill(cnt).AdjustedBaseValue;
		}
		for(int cnt = 0; cnt < _allBuffSkill.Length; cnt++)
		{
			_allBuffSkill[cnt].Update();
			GetAllBuffSkill(cnt).CurValue = GetAllBuffSkill(cnt).AdjustedBaseValue;
		}
			_superskill.Update();
			_superskill.CurValue = _superskill.AdjustedBaseValue;
		if(GetVital((int)VitalName.Energy).CurValue>2000)
			GetVital((int)VitalName.Energy).CurValue = 2000;
	}
	
	public void CheckThrowWeaponNum()
	{
		if(roomMenu.localPlayer.properties.ThrowingWeaponNum==0)
			_throwingWeapon = ThrowingWeapons.None;
	}

	bool IsAIButNotMasterClients()
	{
		if(isAI&&!PhotonNetwork.isMasterClient)
			return true;
		else
			return false;
	}

	public void UseArtifactInput(bool mode)
	{
		if(IsAIButNotMasterClients())
			return;
		playerView.RPC("UseArtifact",PhotonTargets.All,mode);
	}

	[RPC]
	public void UseArtifact(bool mode)
	{
		playerAnimator.ArtifactInput(mode);
		for(int cnt = 0; cnt < (_ability.Length-1); cnt++)
		{
			if(mode)
			{
				_artifactBuffTemp[cnt] = GetAbility(cnt).CurValue;
				if(cnt==(int)AbilityName.AttackSpeed||cnt==(int)AbilityName.MoveSpeed)
					_artifactBuffTemp[cnt] = (int)(_artifactBuffTemp[cnt] * 0.05f);
				else
					_artifactBuffTemp[cnt] = (int)(_artifactBuffTemp[cnt] * 0.4f);
				GetAbility(cnt).BuffValue += _artifactBuffTemp[cnt];
			}
			else
			{
				GetAbility(cnt).BuffValue -= _artifactBuffTemp[cnt];
				_artifactBuffTemp[cnt] = 0;
			}
		}
	}

	public void UseGodPowerInput(bool mode)
	{
		if(IsAIButNotMasterClients())
			return;
		playerView.RPC("UseGodPower",PhotonTargets.All,mode);
	}

	[RPC]
	public void UseGodPower(bool mode)
	{
		playerAnimator.GodPowerInput(mode);
	
		for(int cnt = 0; cnt < (_ability.Length-1); cnt++)
		{
			if(mode)
			{
				_godPowerBuffTemp[cnt] = GetAbility(cnt).CurValue;
				if(cnt==(int)AbilityName.AttackSpeed)
					_godPowerBuffTemp[cnt] = (int)(_godPowerBuffTemp[cnt] * 0.1f);
				else if(cnt==(int)AbilityName.MoveSpeed)
					_godPowerBuffTemp[cnt] = (int)(_godPowerBuffTemp[cnt] * 0.5f);
				else
					_godPowerBuffTemp[cnt] = (int)(_godPowerBuffTemp[cnt] * 0.6f);
				GetAbility(cnt).BuffValue += _godPowerBuffTemp[cnt];
			}
			else
			{
				GetAbility(cnt).BuffValue -= _godPowerBuffTemp[cnt];
				_godPowerBuffTemp[cnt] = 0;
			}
		}
	}

	void BuffConditionCountDown()
	{
		foreach(BuffCondition buff in _buffcondition)
		{
			if(buff.Timer>0)
			{
				if(buff.BuffName==BuffConditionName.Darkman_Freeze||buff.BuffName==BuffConditionName.Mars_DizzyFire)
				{
					if(buff.Timer<(buff.Duration/2))
					{
						switch(buff.BuffName)
						{
						case BuffConditionName.Darkman_Freeze:
							if(playerAnimator.State==TP_Animator.CharacterState.Freeze)
								buff.Freeze(playerAnimator,false);
							break;
						case BuffConditionName.Mars_DizzyFire:
							if(playerAnimator.State==TP_Animator.CharacterState.Dizzing)
							{
								CancelInvoke("Burned");
								buff.Dizzy(playerAnimator,false);
							}
							break;
						}
					}
				}

				buff.Timer-=Time.deltaTime;
			}
			else
			{
				if(buff.BuffName==BuffConditionName.Darkman_Freeze||buff.BuffName==BuffConditionName.Mars_DizzyFire||buff.BuffName==BuffConditionName.HealthWater||buff.BuffName==BuffConditionName.ManaWater)
				{
					switch(buff.BuffName)
					{
					case BuffConditionName.Darkman_Freeze:

						break;
					case BuffConditionName.Mars_DizzyFire:
						CancelInvoke("Burned");
						break;
					case BuffConditionName.HealthWater:
						if(playerView.isMine)
						{
							if(!IsAIButNotMasterClients())
							{
								CancelInvoke("ApplyHealthWater");
							}
						}
						break;
					case BuffConditionName.ManaWater:
						if(playerView.isMine)
						{
							if(!IsAIButNotMasterClients())
							{
								CancelInvoke("ApplyManaWater");
							}
						}
						break;
					}
				}
				tempBuff.Clear();
				if(buff.AffectedBuff.Count>0)
				{
					foreach(AffectedAbility affabi in buff.AffectedBuff)
					{
						GetAbility((int)affabi.AbiName).BuffValue -= affabi.Force;
					}
				}
				tempBuff.Add(buff);
			}
		}

		if(tempBuff.Count>0)
		{
			foreach(BuffCondition buff1 in tempBuff)
				_buffcondition.Remove(buff1);
		}
	}

	public HUDText CreateHDT()
	{
		GameObject Root = GameObject.Find("UI Root");
		GameObject HUDT = NGUITools.AddChild(Root,roomMenu.TextPrefab);
		HUDText HDT = HUDT.GetComponent<HUDText>();
		HDT.transform.localScale = new Vector3(30,30,30);
		UIFollowTarget HDTfollow = HUDT.GetComponent<UIFollowTarget>();
		HDTfollow.target = HUDTextTarget;
		HUDT.GetComponentInChildren<UISlider>().gameObject.SetActive(false);
		return HDT;
	}

	void Burned()
	{
		float FireDamage = 0;
		foreach(BuffCondition buff in _buffcondition)
		{
			if(buff.BuffName==BuffConditionName.Mars_DizzyFire)
			{
				FireDamage = buff.Force;
			}
		}
		playerView.RPC("ApplyDamage",PhotonTargets.All,playerView.viewID,playerView.viewID,(int)FireDamage);
		//GetVital((int)VitalName.Health).DamageValue += force;
	}

	void Update()
	{
		if(WholeGameManager.SP.InGame)
		{
			if(playerView.isMine&&!isAI)
			{
				if(playerAnimator.Mode==TP_Animator.CharacterMode.None)
				{
					if(_type==CharacterType.Melee)
					{
						if(!MeleeAttack.activeSelf)
						{
							MeleeAttack.SetActive(true);
						}
					}
				}
				else
				{
					if(_type==CharacterType.Melee)
					{
						if(MeleeAttack.activeSelf)
						{
							MeleeAttack.SetActive(false);
						}
					}
				}
			}
		}
		if(_buffcondition.Count>0)
		{
			BuffConditionCountDown();
		}
		//NJGMapItem it = transform.GetComponent<NJGMapItem>();

		if(Input.GetKeyDown(KeyCode.Keypad0))
		{
			it.type = 0;
		}
		else if(Input.GetKeyDown(KeyCode.Keypad1))
		{
			it.type = 1;
		}
		else if(Input.GetKeyDown(KeyCode.Keypad3))
		{
			it.type = 3;
		}
		else if(Input.GetKeyDown(KeyCode.Keypad5))
		{
			it.type = 5;
		}
//		Debug.Log("Type:" + it.type);

		if(isAbilityModified)
		{
			if(!WholeGameManager.SP.InGame)
			{
				if(Room_MenuUIManager.SP.roomUIInfo[0]!=null)
				{
					if(GetAbility((int)AbilityName.BuffPower).CurValue>0)
					{
						Room_MenuUIManager.SP.UpdateRoleInfo(this);
						Room_MenuUIManager.SP.UpdateRoleEachSkillValue(this,InRoom_Menu.SP.localPlayer.roleType,InRoom_Menu.SP.localPlayer.team);
						Room_MenuUIManager.SP.UpdateUIRoleInfo();
						isAbilityModified = false;
					}
				}
			}
		}


		StatUpdate();

		CalculateLevel();
		//if(HDT)
		//{
			if(WholeGameManager.SP.InGame)
			{
				/*GameObject Root = GameObject.Find("UI Root");
				GameObject HUDT_Health = NGUITools.AddChild(Root,roomMenu.TextPrefab);
				//GameObject HUDT = GameObject.FindGameObjectWithTag("HUDText");
				HDT = HUDT_Health.GetComponent<HUDText>();
				//if(playerView.isMine)
				//	HDT.transform.localScale = new Vector3(30,30,30);
				//else
					HDT.transform.localScale = new Vector3(30,30,30);
				HDTfollow = HUDT.GetComponent<UIFollowTarget>();
				HDTfollow.target = HUDTextTarget;*/

				if(L_LSManager==null||D_LSManager==null)
				{
					L_LSManager = GameObject.FindGameObjectWithTag("team1Light").GetComponent<LightSourceManager>();
					D_LSManager = GameObject.FindGameObjectWithTag("team2Light").GetComponent<LightSourceManager>();
				}
			}
		//}

		/*if(!playerNetController.enabled)
		{
			if(WholeGameManager.SP.InGame)
				playerNetController.enabled = true;
		}*/
			
		/*if(_isDead)
		{
			Dead();
		}*/

		if(_isGodPowering)
		{
			if(_godPower<=0)
			{
				_godPower = 0;
				UseGodPowerInput(false);
				_isGodPowering = false;
			}
			else
				DecreaseGodPower();
		}

		if(GetVital((int)VitalName.Health).CurValue<=0)
		{
			if(playerView.isMine)
			{
				if(IsAIButNotMasterClients())
				{
					return;
				}
				else
				{
					if(playerAnimator.State!=TP_Animator.CharacterState.Dead)
					{
						//if(!isMonster)
						//{
							if(!isDead)
							{
								if(_isArtifacting)
								{
									playerView.RPC("DropGodWeapon",PhotonTargets.All,myTransform.position);
								}
								playerView.RPC("DetermineKillOrBekilled",PhotonTargets.All,_attackBywhom,playerView.viewID,_level);
								playerAnimator.Die();
							}
						//}
						playerAnimator.State = TP_Animator.CharacterState.Dead;
						playerAnimator.Die();
					}
				}
			}
		}
	}

	/*void Dead()
	{
		PhotonPlayer myPlayer = playerView.owner;
		Hashtable plaProperties = new Hashtable();
		plaProperties = myPlayer.customProperties;
		if(plaProperties.ContainsKey("T")&&plaProperties.ContainsKey("P"))
		{
			int myTeam = (int)plaProperties["T"];
			int myPosition = (int)plaProperties["P"];
			Debug.Log("has: T: " + myTeam + " P " + myPosition);
			roomMenu.RespawnPlayer(myPlayer,myTeam,myPosition);
		}
		else
		{
			Debug.Log("You Dont Have Team and Position");
		}
	}*/

	public void LightSourceInput()
	{
		if(IsAIButNotMasterClients())
			return;
		if(L_LSManager!=null&&D_LSManager!=null)
		{
			if(_team==1)
			{
				if(L_LSManager.teamList.Contains(myTransform))
				{
					//resume vital
					int myHealthDamage = (int)GetVital((int)VitalName.Health).DamageValue;
					if(myHealthDamage>0)
					{
						if(L_LSManager.LightSource>10)
						{
							int force = (int)GetVital((int)VitalName.Health).MaxValue;
							force = (int)(force*0.1f);
							playerView.RPC("AddHealth",PhotonTargets.All,0,0,force);
							playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,1,-1,false);
						}
						else
						{
							GameUIManager.SP.HudWarningLabel.Add("No Enough LightSource!",Color.gray,1);
						}
						//playerView.RPC("BuyItems",PhotonTargets.AllBuffered,playerView.viewID,5);
					}
					int myManaDamage = (int)GetVital((int)VitalName.Mana).DamageValue;
					if(myManaDamage>0)
					{
						if(L_LSManager.LightSource>10)
						{
							int force = (int)GetVital((int)VitalName.Mana).MaxValue;
							force = (int)(force*0.1f);
							playerView.RPC("AddMana",PhotonTargets.All,0,0,force);
							playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,1,-1,false);
						}
						else
						{
							GameUIManager.SP.HudWarningLabel.Add("No Enough LightSource!",Color.gray,1);
						}
					}
				}
				else if(D_LSManager.enemyList.Contains(myTransform))
				{
					//suck light source
					playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,2,-1,true);
					playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,1,1,false);
				}
			}
			else if(_team==2)
			{
				if(D_LSManager.teamList.Contains(myTransform))
				{
					//resume vital
					int myHealthDamage = (int)GetVital((int)VitalName.Health).DamageValue;
					if(myHealthDamage>0)
					{
						if(D_LSManager.LightSource>10)
						{
							int force = (int)GetVital((int)VitalName.Health).MaxValue;
							force = (int)(force*0.1f);
							playerView.RPC("AddHealth",PhotonTargets.All,0,0,force);
							playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,2,-1,false);
						}
						else
						{
							GameUIManager.SP.HudWarningLabel.Add("No Enough LightSource!",Color.gray,1);
						}
					}
					int myManaDamage = (int)GetVital((int)VitalName.Mana).DamageValue;
					if(myManaDamage>0)
					{
						if(D_LSManager.LightSource>10)
						{
							int force = (int)GetVital((int)VitalName.Mana).MaxValue;
							force = (int)(force*0.1f);
							playerView.RPC("AddMana",PhotonTargets.All,0,0,force);
							playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,2,-1,false);
						}
						else
						{
							GameUIManager.SP.HudWarningLabel.Add("No Enough LightSource!",Color.gray,1);
						}
					}
				}
				else if(L_LSManager.enemyList.Contains(myTransform))
				{
					//suck lightsource
					playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,1,-1,true);
					playerView.RPC("DecreaseOrAddLightSource",PhotonTargets.All,2,1,false);
				}
			}
		}
	}

	private void DecreaseGodPower()
	{
		_godPower -= Time.deltaTime * 5;
	}

	[RPC]
	public void MagicStore()
	{
		GameObject clone;
		clone = (GameObject)Instantiate(GOEffectPref[0],LeftHandTrans.position,LeftHandTrans.rotation);
		clone.transform.parent = LeftHandTrans;
	}


	public void StoreMagicInput()
		{
			playerView.RPC("MagicStore",PhotonTargets.All);
	}

	[RPC]
	public void EnergyStore(bool active)
	{
		if(active)
		{
			if(!playerAnimator.StoreEnergyEff.activeSelf)
				playerAnimator.StoreEnergyEff.SetActive(true);
		}
		else
		{
			if(playerAnimator.StoreEnergyEff.activeSelf)
				playerAnimator.StoreEnergyEff.SetActive(false);
		}
	}

	public void EnergyStoreInput(bool active)
	{
		playerView.RPC("EnergyStore",PhotonTargets.All,active);
	}

	public void StoredMagicAttackInput(int force)
	{
		playerView.RPC("MagicAttack",PhotonTargets.All,force);
	}

	public void MagicAttackInput(int force)
	{
		playerView.RPC("MagicAttack",PhotonTargets.All,force);
	}

	public void PersiaThunderousWaveInput(int force)
	{
		playerView.RPC("PersiaThunderousWave",PhotonTargets.All,force);
	}

	public void SteropiSnowBallInput(int force)
	{
		playerView.RPC("SteropiSnowBall",PhotonTargets.All,force);
	}

	public void MarsFightLightInput(int force)
	{
		playerView.RPC("MarsFightLight",PhotonTargets.All,force);
	}

	public void HyperionHotRockInput(int force)
	{
		playerView.RPC("HyperionHotRock",PhotonTargets.All,force);
	}




	[RPC]
	public void DestroyMagicStoredEff()
	{
		GameObject storedClone = LeftHandTrans.GetChild(0).gameObject;
		Destroy(storedClone);
	}

	[RPC]
	public void MagicAttack(int force)
	{

		Rigidbody clone;
		if(playerAnimator.State==TP_Animator.CharacterState.MagicStoredAttacking)
		{
			clone = (Rigidbody)Instantiate(RigidEffectPref[0],LeftHandTrans.position,LeftHandTrans.rotation);
			clone.velocity = LeftHandTrans.forward * 70;
		}
		else
		{
			clone = (Rigidbody)Instantiate(RigidEffectPref[0],LeftHandTrans.position,LeftHandTrans.rotation);
			clone.velocity = LeftHandTrans.forward * 50;
		}
		ColliderAttack cloneAttack = clone.GetComponent<ColliderAttack>();
		string myTeam = transform.tag;
		if(myTeam=="team1")
			cloneAttack.EnemyTeam = "team2";
		else
			cloneAttack.EnemyTeam = "team1";
		cloneAttack.parentTrans = transform;
		cloneAttack.playerView = playerView;
		cloneAttack.Force = force;
	}

	[RPC]
	public void DecreaseOrAddLightSource(int team, int amount, bool beStolen)
	{
		InRoom_Menu.PlayerInfoData pla = InRoom_Menu.SP.GetPlayerFromID(playerView.viewID);

		if(team==1)
		{
			L_LSManager.LightSource += amount;
		}
		else
		{
			D_LSManager.LightSource += amount;
		}

		if(amount>0)
		{
			HUDText HDT = CreateHDT();
			if(playerView.isMine)
			{
				if(!isAI)
				{
					string str = "Steal Resource +" + amount + "!";
					HDT.Add(str,Color.cyan,1f);
				}
			}
			pla.properties.GetLightSource += amount;
		}
		else if(amount<0&&!beStolen)
		{
			pla.properties.UseLightSource -= amount;
		}
	}

	[RPC]
	public void BuyItems(int ID, int itemCode)
	{
		foreach(InRoom_Menu.PlayerInfoData pla in roomMenu.playerList)
		{
			if(pla.ID==ID)
			{
				for(int cnt=0;cnt<6;cnt++)
				{
					if(pla.properties.Items[cnt]==0)
					{
						pla.properties.Items[cnt] = itemCode;
						break;
					}
				}
			}
		}
	}

	[RPC]
	public void BuyBuff(int ID, int itemCode)
	{
		foreach(InRoom_Menu.PlayerInfoData pla in roomMenu.playerList)
		{
			if(pla.ID==ID)
			{
				for(int cnt=0;cnt<2;cnt++)
				{
					if(pla.properties.Items[cnt]==0)
					{
						pla.properties.Items[cnt] = itemCode;
						break;
					}
				}
			}
		}
	}

	[RPC]
	public void AddHealth(int attackID ,int ID, int force)
	{
		int myHealth = (int)GetVital((int)VitalName.Health).CurValue;
		int myDamage = (int)GetVital((int)VitalName.Health).DamageValue;
		int finalForce = force;
		if(force>myDamage)
			finalForce = myDamage;

		GetVital((int)VitalName.Health).DamageValue -= finalForce;

		Debug.Log(transform.name + " got health, adding " + finalForce + " health");
		HUDText HDT = CreateHDT();
		if(attackID!=0)
		{
			if(roomMenu.localPlayer.ID==attackID)
			{
				if(!isAI)
				{
					if(finalForce==0)
						HDT.Add("+0",Color.white,1f);
					else
						HDT.Add(finalForce,Color.white,1f);
				}
				else
				{
					if(finalForce==0)
						HDT.Add("+0",Color.yellow,1f);
					else
						HDT.Add(finalForce,Color.yellow,1f);
				}

			}
			else if(playerView.isMine)
			{
				if(roomMenu.GetPlayerFromID(playerView.viewID).properties.Ai==0)
				{
					if(finalForce==0)
						HDT.Add("+0",Color.yellow,1f);
					else
						HDT.Add(finalForce,Color.yellow,1f);
				}
			}
		}
		else if(attackID==0)//add health by myself
		{
			if(playerView.isMine)
			{
				if(!isAI)
					HDT.Add(finalForce,Color.white,1f);
			}
		}
	}

	[RPC]
	public void AddMana(int attackID ,int ID, int force)
	{
		int myMana = (int)GetVital((int)VitalName.Mana).CurValue;
		int myDamage = (int)GetVital((int)VitalName.Mana).DamageValue;
		int finalForce = force;
		if(force>myDamage)
			finalForce = myDamage;
		
		GetVital((int)VitalName.Mana).DamageValue -= finalForce;
		
		Debug.Log(transform.name + " got mana, adding " + finalForce + " mana");

		HUDText HDT = CreateHDT();

		AddHUDTextBelow(HDT,10);

		if(attackID!=0)
		{
			if(playerView.isMine)
			{
				if(finalForce==0)
					HDT.Add("+0",Color.magenta,1f);
				else
					HDT.Add(finalForce,Color.magenta,1f);
			}
			else if(roomMenu.localPlayer.ID==attackID)
			{
				if(finalForce==0)
					HDT.Add("+0",Color.blue,1f);
				else
					HDT.Add(finalForce,Color.blue,1f);
			}
		}
		else if(attackID==0)//add mana by myself
		{
			if(!isAI)
				HDT.Add(finalForce,Color.blue,1f);
		}
	}

	[RPC]
	public void TheiaAddDefense(int attackID ,int ID, int force)
	{
		BuffCondition buff = new BuffCondition(10);
		buff.BuffName = BuffConditionName.Theia_AddDefense;

		bool hasBuff = false;
		string str = null;
		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = 10;
				hasBuff = true;
				str = "Regain Buff Defense +" + force.ToString();
			}
		}

		if(!hasBuff)
		{
			AffectedAbility defense = new AffectedAbility();
			defense.AbiName = AbilityName.Defense;
			defense.Force = force;
			buff.AddAffectAbility(defense);
			_buffcondition.Add(buff);

			GetAbility((int)AbilityName.Defense).BuffValue += force;

			str = "Defense +" + force.ToString();
		}

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
		else if(roomMenu.localPlayer.ID==attackID)
		{
			HDT.Add(str,Color.white,1f);
		}
	}

	[RPC]
	public void PersiaDecreaseDefense(int attackID ,int ID, int force)
	{
		BuffCondition buff = new BuffCondition(10);
		buff.BuffName = BuffConditionName.Persia_DecreaseDefense;

		bool hasBuff = false;
		string str = null;
		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = 10;
				hasBuff = true;
				str = "Regain Buff Defense -" + force.ToString();
			}
		}

		if(!hasBuff)
		{
			AffectedAbility defense = new AffectedAbility();
			defense.AbiName = AbilityName.Defense;
			defense.Force = -force;
			buff.AddAffectAbility(defense);
			_buffcondition.Add(buff);
			
			GetAbility((int)AbilityName.Defense).BuffValue += defense.Force;
			
			str = "Defense -" + force.ToString();
		}
		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
		else if(roomMenu.localPlayer.ID==attackID)
		{
			HDT.Add(str,Color.white,1f);
		}
	}

	[RPC]
	public void RoleAddDefense(int attackID, int force, int role)
	{
		BuffCondition buff = new BuffCondition(10);
		if((TP_Info.CharacterRole)role == TP_Info.CharacterRole.DarkMan)
			buff.BuffName = BuffConditionName.Darkman_AddDefense;
		else if((TP_Info.CharacterRole)role == TP_Info.CharacterRole.Hyperion)
			buff.BuffName = BuffConditionName.Hyperion_Intimidate;

		bool hasBuff = false;
		string str = null;

		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = 10;
				hasBuff = true;
				str = "Regain Buff Defense +" + force.ToString();
			}
		}

		if(!hasBuff)
		{
			AffectedAbility defense = new AffectedAbility();
			defense.AbiName = AbilityName.Defense;
			defense.Force = force;
			buff.AddAffectAbility(defense);
			_buffcondition.Add(buff);
			
			GetAbility((int)AbilityName.Defense).BuffValue += force;

			str = "Defense +" + force.ToString();
		}

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
	}

	[RPC]
	public void HyperionUnbeatable(int attackID, int forceTime)
	{
		BuffCondition buff = new BuffCondition(5);
		buff.BuffName = BuffConditionName.Hyperion_Unbeatable;

		bool hasBuff = false;
		string str = null;
		
		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = forceTime;
				hasBuff = true;
				str = "Regain to be Unbeatable!!! Defense +9999";
			}
		}

		if(!hasBuff)
		{
			AffectedAbility defense = new AffectedAbility();
			defense.AbiName = AbilityName.Defense;
			defense.Force = 9999;
			buff.AddAffectAbility(defense);
			_buffcondition.Add(buff);
			
			GetAbility((int)AbilityName.Defense).BuffValue += 9999;
			
			str = "I am Unbeatable!!! Defense +9999";
		}

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
	}

	public void UseWaterInput(int waterType)
	{
		playerView.RPC("UseWater",PhotonTargets.All,waterType);
	}

	void ApplyHealthWater()
	{
		int amount = (int)(GetVital((int)VitalName.Health).MaxValue*3/100);
		GetVital((int)VitalName.Health).DamageValue -= amount;
	}
	void ApplyManaWater()
	{
		int amount = (int)(GetVital((int)VitalName.Mana).MaxValue*3/100);
		GetVital((int)VitalName.Mana).DamageValue -= amount;
	}

	[RPC]
	public void UseWater(int waterType)
	{
		BuffCondition buff = new BuffCondition(15);

		bool hasBuff = false;
		string str = null;

		if(waterType==0)
		{
			buff.BuffName = BuffConditionName.HealthWater;
			str = "Use HealthWater!";
		}
		else if(waterType==1)
		{
			buff.BuffName = BuffConditionName.ManaWater;
			str = "Use ManaWater!";
		}

		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer += 15;
				hasBuff = true;
			}
		}
		
		if(!hasBuff)
		{
			_buffcondition.Add(buff);
			if(playerView.isMine)
			{
				if(!IsAIButNotMasterClients())
				{
					if(waterType==0)
					{
						InvokeRepeating("ApplyHealthWater",0.01f,1);
					}
					else if(waterType==1)
					{
						InvokeRepeating("ApplyManaWater",0.01f,1);
					}
				}
			}
		}


		
		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
	}

	[RPC]
	public void RoleAddAttack(int attackID, int force, int role)
	{
		BuffCondition buff = new BuffCondition(10);
		if((TP_Info.CharacterRole)role==TP_Info.CharacterRole.Mars)
			buff.BuffName = BuffConditionName.Mars_AddAttack;
		else if((TP_Info.CharacterRole)role==TP_Info.CharacterRole.Steropi)
			buff.BuffName = BuffConditionName.Steropi_Roar;

		bool hasBuff = false;
		string str = null;
		
		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = 10;
				hasBuff = true;
				str = "Regain Attack +" + force.ToString();
			}
		}

		if(!hasBuff)
		{
			AffectedAbility attack = new AffectedAbility();
			attack.AbiName = AbilityName.MeleeAttack;
			attack.Force = force;
			buff.AddAffectAbility(attack);
			_buffcondition.Add(buff);
			
			GetAbility((int)AbilityName.MeleeAttack).BuffValue += force;
			
			str = "Attack +" + force.ToString();
		}

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
	}

	public void BuffSkillInput(int BuffSkillPos)
	{
		int skillType = (int)(GetBuffSkill(BuffSkillPos).BuffName);
		int force = (int)(GetBuffSkill(BuffSkillPos).CurValue);
		playerView.RPC("UseBuffSkill",PhotonTargets.All,skillType,force);
	}

	[RPC]
	public void UseBuffSkill(int skillType, int force)
	{
		BuffCondition buff = new BuffCondition(10);
		if(skillType==1)
		{
			buff.BuffName = BuffConditionName.Buff_SpeedUp;
		}
		else if(skillType==2)
		{
			buff.BuffName = BuffConditionName.Buff_AttackUp;
		}

		bool hasBuff = false;
		string str = null;

		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				thisBuff.Timer = 10;
				hasBuff = true;
				if(skillType==1)
					str = "Regain Buff Speed +" + force.ToString();
				else if(skillType==2)
					str = "Regain Buff Attack +" + force.ToString();
			}
		}
		
		if(!hasBuff)
		{
			if(skillType==1)
			{
				AffectedAbility speed = new AffectedAbility();
				speed.AbiName = AbilityName.MoveSpeed;
				speed.Force = force;
				buff.AddAffectAbility(speed);
				_buffcondition.Add(buff);
				
				GetAbility((int)AbilityName.MoveSpeed).BuffValue += force;
				str = "Buff Speed +" + force.ToString();
			}
			else if(skillType==2)
			{
				AffectedAbility attack = new AffectedAbility();
				attack.AbiName = AbilityName.MeleeAttack;
				attack.Force = force;
				buff.AddAffectAbility(attack);
				_buffcondition.Add(buff);
				
				GetAbility((int)AbilityName.MeleeAttack).BuffValue += force;
				str = "Buff Attack +" + force.ToString();
			}				
		}
		
		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
	}

	[RPC]
	public void DarkmanFreeze(int attackID ,int ID, int force)
	{
		BuffCondition buff = new BuffCondition(5);
		buff.BuffName = BuffConditionName.Darkman_Freeze;

		int buffMoveSpeed = (int)(GetAbility((int)AbilityName.MoveSpeed).CurValue/5f);

		AffectedAbility moveSpeed = new AffectedAbility();
		moveSpeed.AbiName = AbilityName.MoveSpeed;
		moveSpeed.Force = -buffMoveSpeed;
		buff.AddAffectAbility(moveSpeed);

		buff.Freeze(playerAnimator,true);
		
		_buffcondition.Add(buff);

		GetAbility((int)AbilityName.MoveSpeed).BuffValue -= buffMoveSpeed;

		string str = "Freezed";

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
		else if(roomMenu.localPlayer.ID==attackID)
		{
			HDT.Add(str,Color.white,1f);
		}
	}

	[RPC]
	public void PersiaThunderousWave(int force)
	{
		Rigidbody clone;
		clone = (Rigidbody)Instantiate(RigidEffectPref[1],LeftHandTrans.position,LeftHandTrans.rotation);
		clone.velocity = LeftHandTrans.forward * 50;

		ColliderAttack cloneAttack = clone.GetComponent<ColliderAttack>();
		string myTeam = transform.tag;
		if(myTeam=="team1")
			cloneAttack.EnemyTeam = "team2";
		else
			cloneAttack.EnemyTeam = "team1";
		cloneAttack.parentTrans = transform;
		cloneAttack.playerView = playerView;
		cloneAttack.Force = force;
	}

	[RPC]
	public void MarsFightLight(int force)
	{
		Rigidbody clone;
		clone = (Rigidbody)Instantiate(RigidEffectPref[0],myTransform.position+new Vector3(0,1,0),myTransform.rotation);
		clone.velocity = myTransform.forward * 30;
		
		ColliderAttack cloneAttack = clone.GetComponent<ColliderAttack>();
		string myTeam = transform.tag;
		if(myTeam=="team1")
			cloneAttack.EnemyTeam = "team2";
		else
			cloneAttack.EnemyTeam = "team1";
		cloneAttack.parentTrans = transform;
		cloneAttack.playerView = playerView;
		cloneAttack.Force = force;
	}


	[RPC]
	public void SteropiSnowBall(int force)
	{
		Rigidbody clone;
		clone = (Rigidbody)Instantiate(RigidEffectPref[0],myTransform.position+new Vector3(0,1,0),myTransform.rotation);
		clone.velocity = myTransform.forward * 30;
		
		ColliderAttack cloneAttack = clone.GetComponent<ColliderAttack>();
		string myTeam = transform.tag;
		if(myTeam=="team1")
			cloneAttack.EnemyTeam = "team2";
		else
			cloneAttack.EnemyTeam = "team1";
		cloneAttack.parentTrans = transform;
		cloneAttack.playerView = playerView;
		cloneAttack.Force = force;
	}

	[RPC]
	public void HyperionHotRock(int force)
	{
		Rigidbody clone;
		clone = (Rigidbody)Instantiate(RigidEffectPref[0],myTransform.position+new Vector3(0,1,0),myTransform.rotation);
		clone.velocity = myTransform.forward * 30;
		
		ColliderAttack cloneAttack = clone.GetComponent<ColliderAttack>();
		string myTeam = transform.tag;
		if(myTeam=="team1")
			cloneAttack.EnemyTeam = "team2";
		else
			cloneAttack.EnemyTeam = "team1";
		cloneAttack.parentTrans = transform;
		cloneAttack.playerView = playerView;
		cloneAttack.Force = force;
	}


	[RPC]
	public void MarsDizzyFire(int attackID ,int ID, int force)
	{
		BuffCondition buff = new BuffCondition(5);
		buff.BuffName = BuffConditionName.Mars_DizzyFire;
		buff.Force = force;

		bool hasBuff = false;
		string str = null;

		foreach(BuffCondition thisBuff in _buffcondition)
		{
			if(thisBuff.BuffName==buff.BuffName)
			{
				CancelInvoke("Burned");
				thisBuff.Timer = (int)(force/10);
				thisBuff.Force = force;
				hasBuff = true;
				str = "Regain Got Burned and Dizzy";
			}
		}
		
		if(!hasBuff)
		{
			_buffcondition.Add(buff);

			str = "Got Burned and Dizzy";
		}

		InvokeRepeating("Burned",1f,1f);
		buff.Dizzy(playerAnimator,true);

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(str,Color.yellow,1f);
		}
		else if(roomMenu.localPlayer.ID==attackID)
		{
			HDT.Add(str,Color.white,1f);
		}
	}

	[RPC]
	public void ApplyDamage(int attackID, int ID, int damage)
	{
		int myHealth = (int)GetVital((int)VitalName.Health).CurValue;
		if(myHealth>0)
		{
				GetVital((int)VitalName.Health).DamageValue += damage;
//				Debug.Log(transform.name + " got hit, lossing " + damage + " health");
		}

		HUDText HDT = CreateHDT();
		if(playerView.isMine)
		{
			if(!isAI)
				HDT.Add(-damage,Color.red,1f);
		}
		if(roomMenu.localPlayer.ID==attackID)
		{
			if(roomMenu.localPlayer.properties.Ai==0)
			{
				HDT.Add(-damage,Color.white,1f);

				if(GameUIManager.SP.PlayerHealthBar[playerListNum]!=null)
					Destroy(GameUIManager.SP.PlayerHealthBar[playerListNum]);
				GameObject healthBar = HDT.transform.FindChild("Progress Bar").gameObject;
				UISlider slider = healthBar.GetComponent<UISlider>();
				GameUIManager.SP.PlayerHealthBar[playerListNum] = healthBar;
				slider.transform.FindChild("Background").gameObject.SetActive(true);
				slider.alpha = 1;
				slider.value = (float)(myHealth-damage)/GetVital((int)VitalName.Health).MaxValue;
			}
		}
	}
	
	[RPC]
	public void BeHit(int attackID ,int ID, int force, float flunctuation, int hitWay)
	{
		PhotonView myPV = null;
		foreach(InRoom_Menu.PlayerInfoData pla in roomMenu.playerList)
		{
			if(pla.ID==ID)
			{
				Debug.Log(gameObject.name + "got hit!!!!");
				TP_Controller myController = pla.transform.GetComponent<TP_Controller>();
				TP_Animator myAnimator = pla.transform.GetComponent<TP_Animator>();
				PhotonView myView = pla.transform.GetComponent<PhotonView>();
				myController.UselightSourceTimer = 0;

				if(hitWay==(int)TP_Animator.HitWays.BeHit)
				{
					if(myAnimator.State!=TP_Animator.CharacterState.KnockingDown && myAnimator.State!=TP_Animator.CharacterState.Skilling01 && myAnimator.State!=TP_Animator.CharacterState.Skilling02 && 
					   myAnimator.State!=TP_Animator.CharacterState.Skilling03 && myAnimator.State!=TP_Animator.CharacterState.Skilling04 && myAnimator.State!=TP_Animator.CharacterState.SuperSkilling &&
					   myAnimator.State!=TP_Animator.CharacterState.MagicStoring&& myAnimator.State!=TP_Animator.CharacterState.StandingUp&&myAnimator.State!=TP_Animator.CharacterState.Freeze&&myAnimator.State!=TP_Animator.CharacterState.Dizzing)
						myAnimator.Behit();
				}
				else if(hitWay==(int)TP_Animator.HitWays.KnockDown && myAnimator.State!=TP_Animator.CharacterState.Skilling01 && myAnimator.State!=TP_Animator.CharacterState.Skilling02 && 
				        myAnimator.State!=TP_Animator.CharacterState.Skilling03 && myAnimator.State!=TP_Animator.CharacterState.Skilling04 && myAnimator.State!=TP_Animator.CharacterState.SuperSkilling &&
				        myAnimator.State!=TP_Animator.CharacterState.Freeze&&myAnimator.State!=TP_Animator.CharacterState.Dizzing)
					myAnimator.KnockDown();


				myPV = pla.transform.GetComponent<PhotonView>();

				if(myPV.isMine)
				{
					//only MasterClient can calculate AI
					if(pla.properties.Ai!=0&&!PhotonNetwork.isMasterClient)
						return;

					int damage;
					int defence;
					int myHealth;
					if(pla.properties.Ai==0)
					{
						_attackBywhom = attackID;
						defence = GetAbility((int)AbilityName.Defense).CurValue;
						myHealth = (int)GetVital((int)VitalName.Health).CurValue;
					}
					else
					{
						TP_Info myInfo = pla.transform.GetComponent<TP_Info>();
						myInfo.AttackByWhom = attackID;
						defence = myInfo.GetAbility((int)AbilityName.Defense).CurValue;
						myHealth = (int)myInfo.GetVital((int)VitalName.Health).CurValue;
					}
					if(force <= defence)
						damage = 10;
					else 
						damage = (force - defence)*10;
					
					damage = (int)(damage * (1+flunctuation));

					myView.RPC("ApplyDamage", PhotonTargets.All, attackID, ID, damage);
				}
			}
		}
	}

	[RPC]
	public void DropGodWeapon(Vector3 pos)
	{
		GameObject godWeapon = GameObject.Instantiate(InRoom_Menu.SP.effectPrefabs[(int)CharacterRoleEff.Other-1].effectPrefs[5].effectPref,pos+Vector3.one*10,Quaternion.identity) as GameObject;
	}

	[RPC]
	public void DetermineKillOrBekilled(int attackID, int bekilledID, int bekilledLevel)
	{
		int myID = 0;
		InRoom_Menu.PlayerInfoData myPlayer = null;
		PhotonView myPlayerView = null;

		//myPlayer-> must be a real client player
		if(InRoom_Menu.SP.localPlayer.transform!=null)
		{
			myPlayer = roomMenu.GetPlayerFromID(InRoom_Menu.SP.localPlayer.ID);
			myPlayerView = myPlayer.transform.GetComponent<PhotonView>();
			myID = myPlayer.ID;
		}

		//attPlayer-> could be any character: client,ai,or monster
		//BKPlayer -> could be any character: client,ai,or monster
		InRoom_Menu.PlayerInfoData attPlayer = roomMenu.GetPlayerFromID(attackID);
		PhotonView attPlayerView = attPlayer.transform.GetComponent<PhotonView>();
		InRoom_Menu.PlayerInfoData BKPlayer = roomMenu.GetPlayerFromID(bekilledID);
		PhotonView BKPlayerView = BKPlayer.transform.GetComponent<PhotonView>();

		if(!BKPlayer.transform.GetComponent<TP_Info>().isDead)
		{
			BKPlayer.transform.GetComponent<TP_Info>().isDead = true;

			int levelDifference = BKPlayer.properties.level - attPlayer.properties.level;

			float godPower = 20;
			int money = 30;
			if(levelDifference>0)
			{
				//Calculate Additional GodPower Reward
				for(int cnt=0;cnt<levelDifference;cnt++)
				{
					godPower += godPower*0.25f;
				}
				//Calculate Additional Money Reward
				for(int cnt=0;cnt<levelDifference;cnt++)
				{
					money += (int)(money*0.25f);
				}
			}

			Debug.Log("myid"+myID + "attackID " + attackID + "bekilledID" + bekilledID);

			if(myID!=0)
			{
				if(myID==attackID)//Player kills other
				{
					//player kills other player or ai
					if(!BKPlayer.transform.GetComponent<TP_Info>().isMonster&&!myPlayer.transform.GetComponent<TP_Info>().isMonster)
					{
						myPlayerView.RPC("KillAndExp",PhotonTargets.All, myID, bekilledLevel);
						myPlayerView.RPC("AddGodPower", PhotonTargets.All, myID,(int)godPower);
						InRoom_Menu.SP.AddPlayerMoney(myID,money);
						GameObject moneyKillAudio = GameObject.Instantiate(GameUIManager.SP.moneyKill[0]) as GameObject;
						Destroy(moneyKillAudio,3);
					}
					//player kills monster
					else if(BKPlayer.transform.GetComponent<TP_Info>().isMonster&&!myPlayer.transform.GetComponent<TP_Info>().isMonster)
					{
						//AddEXP
						myPlayerView.RPC("WildExp",PhotonTargets.All, myID, bekilledLevel);
						InRoom_Menu.SP.AddPlayerMoney(myID,money);
						GameObject moneyAudio = GameObject.Instantiate(GameUIManager.SP.moneyKill[1]) as GameObject;
						Destroy(moneyAudio,3);
					}
				}
				else if(myID==bekilledID)//Player got killed by other player or ai
				{
					if(!attPlayer.transform.GetComponent<TP_Info>().isMonster)
						myPlayerView.RPC("BeKilled",PhotonTargets.All, myID);
				}
			}

			if(attPlayer.properties.Ai>0)//ai kills other
			{
				if(!attPlayerView.owner.isMasterClient)
					return;
				//ai kills other player or ai
				if(!BKPlayer.transform.GetComponent<TP_Info>().isMonster)
				{
					attPlayerView.RPC("KillAndExp",PhotonTargets.All, attackID, bekilledLevel);
					InRoom_Menu.SP.AddPlayerMoney(attPlayerView.viewID,money);
				}
				//ai kills monster
				else if(BKPlayer.transform.GetComponent<TP_Info>().isMonster)
				{
					attPlayerView.RPC("WildExp",PhotonTargets.All, attackID, bekilledLevel);
					InRoom_Menu.SP.AddPlayerMoney(attPlayerView.viewID,money);
				}
			}

			//ai got killed by other player or ai
			if(BKPlayer.properties.Ai>0)
			{
				if(!BKPlayerView.owner.isMasterClient)
					return;
				if(!attPlayer.transform.GetComponent<TP_Info>().isMonster)
					BKPlayerView.RPC("BeKilled",PhotonTargets.All, bekilledID);
			}
		}
	}
	
	[RPC]
	public void KillAndExp(int attackID, int bekilledLevel)
	{
		InRoom_Menu.PlayerInfoData myPlayer = roomMenu.GetPlayerFromID(attackID);
		myPlayer.properties.Kills++;
		string myTag = myPlayer.transform.tag;

		foreach(InRoom_Menu.PlayerInfoData pla in roomMenu.playerList)
		{
			if(pla.transform.tag==myTag)
			{
				if(pla.properties.Ai==0||(pla.properties.Ai>0&&PhotonNetwork.isMasterClient))
				{
					float exp = 30;
					for(int cnt=1;cnt<bekilledLevel;cnt++)
					{
						exp += exp*0.25f;
					}
					
					if(pla.ID==attackID)
					{
						exp = exp * 1.25f;
					}

					pla.transform.GetComponent<TP_Info>().AddExp((int)exp);
					pla.properties.Exp += (int)exp;
					if(pla.IsLocal)
					{
						if(pla.properties.Ai==0)
						{
							string str = "Exp +" + ((int)exp).ToString();
							GameUIManager.SP.HudRewardLabel.Add(str,Color.yellow,1f);
						}
					}
				}
			}
		}

		if(myPlayer==roomMenu.localPlayer)
		{
			playerView.RPC("TellTeammateKilled",PhotonTargets.All,_team,1);
		}			
	}

	[RPC]
	public void WildExp(int attackID, int bekilledLevel)
	{
		InRoom_Menu.PlayerInfoData myPlayer = roomMenu.GetPlayerFromID(attackID);
		string myTag = myPlayer.transform.tag;

		foreach(InRoom_Menu.PlayerInfoData pla in roomMenu.playerList)
		{
			if(pla.transform.tag==myTag)
			{
				if(pla.properties.Ai==0||(pla.properties.Ai>0&&PhotonNetwork.isMasterClient))
				{
					float exp = 50;
					for(int cnt=1;cnt<bekilledLevel;cnt++)
					{
						exp += exp*0.25f;
					}
					
					if(pla.ID==attackID)
						exp = exp * 1.25f;
					
					pla.transform.GetComponent<TP_Info>().AddExp((int)exp);
					pla.properties.Exp += (int)exp;
					if(pla.IsLocal)
					{
						if(pla.properties.Ai==0)
						{
							string str = "Exp +" + ((int)exp).ToString();
							GameUIManager.SP.HudRewardLabel.Add(str,Color.yellow,1f);
						}
					}
				}
			}
		}
	}

	[RPC]
	public void AddGodPower(int myID, int amount)
	{
		InRoom_Menu.PlayerInfoData myPlayer = roomMenu.GetPlayerFromID(myID);
		myPlayer.transform.GetComponent<TP_Info>().GodPower += amount;

		string str = "GodPower +" + amount.ToString();
		if(playerView.isMine)
		{
			if(!isAI)
				GameUIManager.SP.HudRewardLabel.Add(str,Color.magenta,1f);
		}
	}

	[RPC]
	public void BeKilled(int bekilledID)
	{
		InRoom_Menu.PlayerInfoData myPlayer = roomMenu.GetPlayerFromID(bekilledID);
		myPlayer.properties.BeKills++;
	}

	[RPC]
	public void TellTeammateKilled(int myteam, int type)
	{
		if(InRoom_Menu.SP.localPlayer.team==myteam)
		{
			if(type==-1)//tell teammate that I'm dead
			{	
				if(!playerView.isMine||(playerView.isMine&&isAI))
				{
//					Debug.Log("[TellTeammateKilled]An ally has been slain.");
					GameUIManager.SP.HudWarningLabel.Add("An ally has been slain.",Color.red,2);
					GameObject beenSlainAudio = GameObject.Instantiate(GameUIManager.SP.moneyKill[3]) as GameObject;
					Destroy(beenSlainAudio,3);
				}
			}
			else if(type==1)
			{
				//if(playerView.isMine||(playerView.isMine&&isAI))
				//{
//					Debug.Log("[TellTeammateKilled]An enemy has been slain.");
				GameUIManager.SP.HudWarningLabel.Add("An enemy has been slain.",Color.red,2);
				GameObject beenSlainAudio = GameObject.Instantiate(GameUIManager.SP.moneyKill[4]) as GameObject;
				Destroy(beenSlainAudio,3);
				//}
			}
		}	
	}

	void AddHUDTextBelow(HUDText HDT,int below)
	{
		Transform tempTextTargetTrans = new GameObject("tempTextTarget").transform;
		tempTextTargetTrans.parent = HUDTextTarget;
		tempTextTargetTrans.localPosition = Vector3.zero;
		tempTextTargetTrans.localPosition -= new Vector3(0,below,0);
		HudScript HDTScript = HDT.gameObject.GetComponent<HudScript>();
		HDTScript.TextTarget = tempTextTargetTrans;
		UIFollowTarget HDTfollow = HDT.gameObject.GetComponent<UIFollowTarget>();
		HDTfollow.target = tempTextTargetTrans;
		
		StartCoroutine(HDTScript.destoryTextTarget());
	}
	
	public void DoDead()
	{
		if(!isAI)
		{
			if(playerView.isMine)
			{
				if(Game_Manager.SP.MyGameState == GameState.Playing)
				{
					Game_Manager.SP.MyGameState = GameState.None;
					PhotonPlayer myPlayer = playerView.owner;
					Hashtable plaProperties = new Hashtable();
					plaProperties = myPlayer.customProperties;
					if(plaProperties.ContainsKey("T")&&plaProperties.ContainsKey("P"))
					{
						int myTeam = (int)plaProperties["T"];
						int myPosition = (int)plaProperties["P"];
						//bool canRivive = false;
						InRoom_Menu.PropertiesInfoData localProperties = roomMenu.localPlayer.properties;
						plaProperties["M"] = localProperties.Money;
						//plaProperties["L"] = localProperties.level;
						plaProperties["E"] = localProperties.Exp;
						plaProperties["BF"] = localProperties.Buff;
						plaProperties["I"] = localProperties.Items;
						plaProperties["K"] = localProperties.Kills;
						plaProperties["BK"] = localProperties.BeKills;
						plaProperties["HW"] = localProperties.HealthWaterNum;
						plaProperties["MW"] = localProperties.ManaWaterNum;
						plaProperties["TH"] = localProperties.ThrowingWeaponNum;
						plaProperties["UseL"] = localProperties.UseLightSource;
						plaProperties["GetL"] = localProperties.GetLightSource;
						myPlayer.SetCustomProperties(plaProperties);

						GameUIManager.SP.OpenChooseRolePage(myTeam,myPosition,localProperties.Exp);
						roomMenu.DoDeadPlayer();
						Debug.Log("Times");
						if(!GameUIManager.SP.isShowDeadUI)
						{
							GameUIManager.SP.SwitchTeamMataCamera();
							GameUIManager.SP.HudWarningLabel.Add("You have been slain.!",Color.red,2f);
							GameUIManager.SP.isShowDeadUI = true;
						}
						GameObject beenSlainAudio = GameObject.Instantiate(GameUIManager.SP.moneyKill[2]) as GameObject;
						Destroy(beenSlainAudio,3);
					}
					if(GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.activeSelf)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.SetActive(false);
					}
					foreach(GameObject aim in GameUIManager.SP.AimUI)
					{
						aim.SetActive(false);
					}
					if(GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.activeSelf)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(false);
					}

					Destroy(myTransform.gameObject);
				}
					//roomMenu.RespawnPlayer(myTeam,myPosition,0,0);
			}
		}
		else
		{
			if(PhotonNetwork.isMasterClient)
			{
				if(!isMonster)
				{
					Hashtable roomProperties = new Hashtable();
					roomProperties = PhotonNetwork.room.customProperties;
					InRoom_Menu.PropertiesInfoData localProperties = roomMenu.GetPlayerFromID(playerView.viewID).properties;
					int ai = localProperties.Ai;
					Debug.Log("ai=" + ai);
					string AIteam = "TAi" + ai.ToString();
					string AIPos = "PAi" + ai.ToString();
					int AIRole = 0;
					int myTeam = (int)roomProperties[AIteam];
					int myPosition = (int)roomProperties[AIPos];
					string AIMoney = "MAi" + ai.ToString();
					string AIExp = "EAi" + ai.ToString();
					string AIBuff = "BFAi" + ai.ToString();
					string AIItem = "IAi" + ai.ToString();
					string AIKill = "KAi" + ai.ToString();
					string AIBeKill = "BKAi" + ai.ToString();
					string AIHealthWater = "HWAi" + ai.ToString();
					string AIManaWater = "MWAi" + ai.ToString();
					string AIThrowWeapon = "THAi" + ai.ToString();
					string AIUseL = "UseLAi" + ai.ToString();
					string AIGetL = "GetLAi" + ai.ToString();

					roomProperties[AIMoney] = localProperties.Money;
					roomProperties[AIExp] = localProperties.Exp;
					roomProperties[AIBuff] = localProperties.Buff;
					roomProperties[AIItem] = localProperties.Items;
					roomProperties[AIKill] = localProperties.Kills;
					roomProperties[AIBeKill] = localProperties.BeKills;
					roomProperties[AIHealthWater] = localProperties.HealthWaterNum;
					roomProperties[AIManaWater] = localProperties.ManaWaterNum;
					roomProperties[AIThrowWeapon] = localProperties.ThrowingWeaponNum;
					roomProperties[AIUseL] = localProperties.UseLightSource;
					roomProperties[AIGetL] = localProperties.GetLightSource;

					PhotonNetwork.room.SetCustomProperties(roomProperties);
					AIRole = UnityEngine.Random.Range(1,2);
					if(myTeam==1)
					{
						if(L_LSManager.LightSource>=35)
							roomMenu.RespawnPlayer(myTeam,myPosition,playerView.viewID,ai,AIRole,localProperties.Exp);
					}
					else
					{
						if(D_LSManager.LightSource>=35)
							roomMenu.RespawnPlayer(myTeam,myPosition,playerView.viewID,ai,AIRole,localProperties.Exp);
					}
				}
				else//is monster
				{
					if(_role==CharacterRole.MonsterBoss)
					{
						int pos = (int)((-(InRoom_Menu.SP.GetPlayerFromID(playerView.viewID).properties.Ai))/3);
						Game_Manager.SP.WildExist[pos] = false;
						Game_Manager.SP.WildTimer[pos] = 30;
					}
					roomMenu.DoDeadMonster(playerView.viewID);
				}
			}
		}
	}

	public void WarningForEnemyInput(bool active)
	{
		if((playerView.isMine&&!isAI)||IsAIButNotMasterClients())
			playerView.RPC("WarningForEnemy",PhotonTargets.All,active);
	}

	[RPC]
	public void WarningForEnemy(bool active)
	{
		if(roomMenu.localPlayer.team!=Team)
		{
			if(active)
			{
				string name = null;
				if(isAI)
				{
					name = roomMenu.GetPlayerFromID(playerView.viewID).properties.Name;
				}
				else
					name = roomMenu.GetPlayerFromID(playerView.viewID).name;
				string str = "Warning!!!   Found the invader: " + name;
				GameUIManager.SP.HudWarningLabel.Add(str,Color.red,1);
				it.type = 2;
				//WarningSound
			}
			else
			{
				it.type = 0;
			}
		}
	}

}
