using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum GameUIButton
{
	ChooseLightRoleEnter,ChooseDarkRoleEnter,ChooseLightRole1,ChooseLightRole2,ChooseLightRole3,ChooseDarkRole1,ChooseDarkRole2,ChooseDarkRole3,Lobby,Esc_Yes,Esc_No
}

public enum GameUIPage
{
	ChooseLightRoleMenu,ChooseDarkMenu,State,Store
}

public enum GameUILabel
{
	Name,Level,HealthWater,ManaWater,Money,Kill,Bekiled,CurHealth,MaxHealth,CurMana,MaxMana,EnergyRate,GodPowerRate,CurExp,MaxExp
}

public enum GameUIWarningLabel
{
	NoLightSourceWarning,UseLightSourceRevivalWarning,CanUseLightSourceHint,CanStealLightSourceHint,EscWarningInfo_MC,EscWarningInfo_Normal
}

public class GameUIManager : MonoBehaviour {

	public static GameUIManager SP;
	public LightSourceManager L_LSManager;
	public LightSourceManager D_LSManager;

	public Transform myPlayer = null;
	public int myRole = 0;
	public int myTeam = 0;
	public int myPos = 0;
	public int myExp = 0;
	public TP_Info playerInfo;

	public Transform myTransform;
	public GameObject GameUIRoot;
	public List<InRoom_Menu.PlayerInfoData> playerList;
	public InRoom_Menu.PlayerInfoData localPlayer;
	public List<GameObject> gameUIButton = new List<GameObject>();
	public Transform picBase;
	public List<UILabel> LabelBase;
	public List<UILabel> WarningLabel;
	public HUDText HudWarningLabel;
	public HUDText HudRewardLabel;
	public List<GameObject> gamePlayerPic = new List<GameObject>();
	public List<GameObject> gameUIPage = new List<GameObject>();
	public List<GameObject> hotKey = new List<GameObject>();
	public List<UISprite> hotKeyCDUI = new List<UISprite>();
	public List<UISprite> vitalCDUI = new List<UISprite>();
	public UISlider expUI;
	public List<UISlider> lightSourceUI = new List<UISlider>();
	public List<Transform> States = new List<Transform>();
	public List<StateData> StateNum = new List<StateData>();
	public int[] StatePicsRecord;
	public List<StateBuy> StateItemAndBuff = new List<StateBuy>();
	public List<GameObject> ItemPics = new List<GameObject>();
	public List<GameObject> BuffPics = new List<GameObject>();
	public List<GameObject> teamMateCamera = new List<GameObject>();
	public List<GameObject> PlayerHealthBar = new List<GameObject>();
	public List<GameObject> GameUI = new List<GameObject>();
	public List<GameObject> OverViewUI = new List<GameObject>();
	public List<GameObject> VisibleUI = new List<GameObject>();
	public GameObject[] AimUI = new GameObject[2];
	public GameObject TeammateInfoPrefab;
	public GameObject TeammateInfoPos;
	public List<GameObject> TeammateInfoList = new List<GameObject>();
	public List<GameObject> LeftUI = new List<GameObject>();
	public GameObject[] moneyKill = new GameObject[2];
	public bool isShowDeadUI;
	public List<UILabel> Pings = new List<UILabel>();

	public bool PressControlling;

	[System.Serializable]
	public class StateData
	{
		public UILabel Kill;
		public UILabel Bekilled;
		public UILabel Level;
		public UILabel LS_Get;
		public UILabel LS_Use;
		public UILabel Money;
		public UILabel Name;
	}

	[System.Serializable]
	public class StateBuy
	{
		public List<GameObject> Buffs;
		public List<GameObject> Items;
	}

	void Awake()
	{
		SP = this;
		myTransform = transform;
		StatePicsRecord = new int[8];
	}

	// Use this for initialization
	void Start () {
		myRole = InRoom_Menu.SP.localPlayer.roleType;
		myTeam = InRoom_Menu.SP.localPlayer.team;
		LabelBase[(int)GameUILabel.Name].text = InRoom_Menu.SP.localPlayer.name;
		UpdateGameSkillUI(myTeam,myRole);
		UpdatePlayerPic(myRole);
		AssignStateBase();
		AssignTempHealthBarBase();
		UIEventListener.Get(gameUIButton[(int)GameUIButton.Esc_Yes]).onClick = LeaveGameWhilePlaying;
		UIEventListener.Get(gameUIButton[(int)GameUIButton.Esc_No]).onClick = BackToGameWhilePlaying;
		InRoom_Menu.SP.OnCheckPing();
	}

	void AssignTempHealthBarBase()
	{
		for(int cnt=0;cnt<10;cnt++)
		{
			PlayerHealthBar.Add(null);
		}
	}

	void AssignStateBase()
	{
		playerList = InRoom_Menu.SP.playerList;
		localPlayer = InRoom_Menu.SP.localPlayer;

		for(int cnt=0;cnt<8;cnt++)
		{
			StateNum[cnt].Kill = States[cnt].FindChild("Kill").GetComponent<UILabel>();
			StateNum[cnt].Bekilled = States[cnt].FindChild("Bekilled").GetComponent<UILabel>();
			StateNum[cnt].Level = States[cnt].FindChild("Level").GetComponent<UILabel>();
			StateNum[cnt].LS_Get = States[cnt].FindChild("LS_Get").GetComponent<UILabel>();
			StateNum[cnt].LS_Use = States[cnt].FindChild("LS_Use").GetComponent<UILabel>();
			StateNum[cnt].Money = States[cnt].FindChild("Money").GetComponent<UILabel>();
			StateNum[cnt].Name = States[cnt].FindChild("Name").GetComponent<UILabel>();
			for(int cnt1=1;cnt1<7;cnt1++)
			{
				string findObj = "ItemBase/" + cnt1.ToString();
				StateItemAndBuff[cnt].Items[cnt1-1] = States[cnt].Find(findObj).gameObject;

			}
			for(int cnt1=0;cnt1<2;cnt1++)
			{
				if(cnt1==0)
					StateItemAndBuff[cnt].Buffs[cnt1] = States[cnt].Find("BuffBase1").gameObject;
				else
					StateItemAndBuff[cnt].Buffs[cnt1] = States[cnt].Find("BuffBase2").gameObject;
			}

			int listNum = -1;
			for(int cnt1=0;cnt1<playerList.Count;cnt1++)
			{
				if(playerList[cnt1].properties.Ai>=0)
				{
					if((playerList[cnt1].pos + (playerList[cnt1].team-1)*InRoom_Menu.MaxTeamNum)==cnt)
					{
						listNum = cnt1;
						break;
					}
				}
			}
//			Debug.Log("cnt:" + cnt + "listNum:" + listNum);
			if(listNum!=-1)
			{
				if(playerList[listNum].properties.Ai>=0)
					AssignStatePic(cnt,playerList[listNum].team,playerList[listNum].roleType);
			}
		}
	}

	void BackToLobby(GameObject button)
	{
		InRoom_Menu.SP.OnLeftRoomFromOverView();
		Destroy(InRoom_Menu.SP.gameObject);
		Destroy(GameUIRoot);
		Destroy(Game_Manager.SP.gameObject);
	}

	public void AdjustOverViewUI(bool result)
	{
		UIEventListener.Get(gameUIButton[(int)GameUIButton.Lobby]).onClick = BackToLobby;
		foreach(GameObject GO in GameUI)
		{
			Destroy(GO);
		}
		OverViewUI[0].GetComponent<UIAnchor>().relativeOffset = new Vector2(0,-0.22f);
		OverViewUI[0].GetComponent<UIAnchor>().enabled = true;
		OverViewUI[0].SetActive(true);
		OverViewUI[1].SetActive(true);
		if(result)
			OverViewUI[2].SetActive(true);
		else
			OverViewUI[3].SetActive(true);
		OverView_Menu.SP.result = result;
	}

	void LeaveGameWhilePlaying(GameObject button)
	{
		GameUIManager.SP.gameUIPage[3].SetActive(false);
		if(PhotonNetwork.isNonMasterClientInRoom)
		{
			InRoom_Menu.SP.OnLeftRoomFromOverView();
			Destroy(InRoom_Menu.SP.gameObject);
			Destroy(GameUIRoot);
			Destroy(Game_Manager.SP.gameObject);
		}
		else
		{
			InRoom_Menu.SP.SetNoLightSourceInput(InRoom_Menu.SP.localPlayer.team);
		}
	}


	void BackToGameWhilePlaying(GameObject button)
	{
		Game_Manager.SP.MyGameState = GameState.Playing;
		GameUIManager.SP.gameUIPage[3].SetActive(false);
	}

	void OpenESCPage()
	{
		bool active = GameUIManager.SP.gameUIPage[3].activeSelf;
		GameUIManager.SP.gameUIPage[3].SetActive(!active);
		if(!active)
		{
			Game_Manager.SP.MyGameState = GameState.Esc;
			if(PhotonNetwork.isMasterClient)
			{
				WarningLabel[(int)GameUIWarningLabel.EscWarningInfo_Normal].gameObject.SetActive(false);
				WarningLabel[(int)GameUIWarningLabel.EscWarningInfo_MC].gameObject.SetActive(true);
			}
			else
			{
				WarningLabel[(int)GameUIWarningLabel.EscWarningInfo_MC].gameObject.SetActive(false);
				WarningLabel[(int)GameUIWarningLabel.EscWarningInfo_Normal].gameObject.SetActive(true);
			}
		}
		else
		{
			Game_Manager.SP.MyGameState = GameState.Playing;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(InRoom_Menu.SP.L_LSManager.LightSource>0&&InRoom_Menu.SP.D_LSManager.LightSource>0)
				OpenESCPage();
		}

		if(myPlayer==null&&InRoom_Menu.SP.localPlayer.transform!=null)
		{
			myPlayer = InRoom_Menu.SP.localPlayer.transform;
			playerInfo = myPlayer.GetComponent<TP_Info>();
		}

		CheckCursor();

		if(Game_Manager.SP.MyGameState==GameState.OverView)
			return;

		if(Input.GetKeyDown(KeyCode.F12))
		{
			bool active = VisibleUI[0].activeSelf;
			foreach(GameObject ui in VisibleUI)
			{
				ui.SetActive(!active);
			}
		}

		UpdateState();
		UpdateLocalState();
		if(playerInfo!=null)
		{
			UpdateSkillCD();
			UpdateLightSource();
			//debug
			LabelBase[15].text = playerInfo.playerAnimator.State.ToString();
		}

		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if(!gameUIPage[(int)GameUIPage.State].activeSelf)
				gameUIPage[(int)GameUIPage.State].SetActive(true);
		}
		else if(Input.GetKeyUp(KeyCode.Tab)||Input.GetKeyDown(KeyCode.F12))
		{
			if(gameUIPage[(int)GameUIPage.State].activeSelf)
				gameUIPage[(int)GameUIPage.State].SetActive(false);
		}

		UpdateTeammateInfo();
	}

	void UpdateTeammateInfo()
	{
		foreach(GameObject info in TeammateInfoList)
		{
			if(info!=null)
			{
				string IDstr = info.name;
				int id = Convert.ToInt32(IDstr);
				TP_Info plaInfo = InRoom_Menu.SP.GetPlayerFromID(id).transform.GetComponent<TP_Info>();
				Vital plaHealth = plaInfo.GetVital((int)VitalName.Health);
				int maxHealth = plaHealth.MaxValue;
				float curHealth = plaHealth.CurValue;
				info.GetComponentInChildren<UISlider>().value = curHealth/maxHealth;
				info.transform.FindChild("MaxHealth").GetComponent<UILabel>().text = maxHealth.ToString();
				info.transform.FindChild("CurHealth").GetComponent<UILabel>().text = ((int)curHealth).ToString();
			}
		}
	}

	public void ManageTeammateInfo(int id ,bool active, int team, int role, string name)
	{
		if(active)
		{
			GameObject info = GameObject.Instantiate(TeammateInfoPrefab) as GameObject;
			Transform infoTrans = info.transform;
			infoTrans.parent = TeammateInfoPos.transform;
			infoTrans.localPosition = Vector3.zero;
			infoTrans.localRotation = Quaternion.identity;
			infoTrans.localScale = Vector3.one;
			infoTrans.name = id.ToString();

			GameObject infoPic = GameObject.Instantiate(gamePlayerPic[role + (team-1)*3]) as GameObject;
		
			Transform infoPicTrans = infoPic.transform;
			infoPicTrans.parent = infoTrans;
			infoPicTrans.localPosition = new Vector3(-20,19,0);
			infoPicTrans.localRotation = Quaternion.identity;
			infoPicTrans.localScale = Vector3.one;
			infoPic.GetComponent<UISprite>().SetDimensions(40,40);
			infoTrans.FindChild("Name").GetComponent<UILabel>().text = name;

			TeammateInfoList.Add(info);
		}
		else
		{
			GameObject info = null;
			info = TeammateInfoPos.transform.FindChild(id.ToString()).gameObject;
			if(info!=null)
			{
				Destroy(info);
				TeammateInfoList.Remove(info);
			}
		}
		TeammateInfoPos.GetComponent<UIGrid>().Reposition();
	}

	void CheckCursor()
	{
		if(Game_Manager.SP.MyGameState==GameState.Playing)
		{
			if(Input.GetKey(KeyCode.LeftControl))
			{
				Screen.lockCursor = false;
				Cursor.visible = true;
				PressControlling = true;
			}
			else
			{
				if(Cursor.visible)
				{
					if(PressControlling)
						PressControlling = false;
					Screen.lockCursor = true;
					Cursor.visible = false;
				}
			}
		}
		else if(Game_Manager.SP.MyGameState==GameState.DeadLock||Game_Manager.SP.MyGameState==GameState.Shopping||
		        Game_Manager.SP.MyGameState==GameState.CanChoosing||Game_Manager.SP.MyGameState==GameState.OverView||Game_Manager.SP.MyGameState==GameState.Esc)
		{
			if(!Cursor.visible)
			{
				Screen.lockCursor = false;
				Cursor.visible = true;
			}
		}
	}

	public void UpdateLocalState()
	{
		int kills = InRoom_Menu.SP.localPlayer.properties.Kills;
		int bekilleds = InRoom_Menu.SP.localPlayer.properties.BeKills;
		int level = InRoom_Menu.SP.localPlayer.properties.level;
		/*int getLS = InRoom_Menu.SP.localPlayer.properties.GetLightSource;
		int useLS = InRoom_Menu.SP.localPlayer.properties.UseLightSource;*/
		int money = InRoom_Menu.SP.localPlayer.properties.Money;
		int HealthWaterNum = InRoom_Menu.SP.localPlayer.properties.HealthWaterNum;
		int ManaHealthWaterNum = InRoom_Menu.SP.localPlayer.properties.ManaWaterNum;

		if(kills!=0)
			LabelBase[(int)GameUILabel.Kill].text = kills.ToString();
		if(level!=1)
			LabelBase[(int)GameUILabel.Level].text = level.ToString();
		if(bekilleds!=0)
			LabelBase[(int)GameUILabel.Bekiled].text = bekilleds.ToString();
		LabelBase[(int)GameUILabel.Money].text = money.ToString();
		LabelBase[(int)GameUILabel.HealthWater].text = HealthWaterNum.ToString();
		LabelBase[(int)GameUILabel.ManaWater].text = ManaHealthWaterNum.ToString();

		if(playerInfo!=null)
		{
			int curHealth = (int)playerInfo.GetVital((int)VitalName.Health).CurValue;
			int maxHealth = (int)playerInfo.GetVital((int)VitalName.Health).MaxValue;
			int curMana = (int)playerInfo.GetVital((int)VitalName.Mana).CurValue;
			int maxMana = (int)playerInfo.GetVital((int)VitalName.Mana).MaxValue;
			int EnergyRate = (int)((playerInfo.GetVital((int)VitalName.Energy).CurValue/2000f)*100);
			int GodPowerRate = (int)((playerInfo.GodPower/100f)*100);
			int curExp= playerInfo.FreeExp;
			int maxExp = playerInfo.ExpTpLevelUp;


			LabelBase[(int)GameUILabel.CurHealth].text = curHealth.ToString();
			LabelBase[(int)GameUILabel.MaxHealth].text = maxHealth.ToString();
			LabelBase[(int)GameUILabel.CurMana].text = curMana.ToString();
			LabelBase[(int)GameUILabel.MaxMana].text = maxMana.ToString();
			LabelBase[(int)GameUILabel.EnergyRate].text = EnergyRate.ToString();
			LabelBase[(int)GameUILabel.GodPowerRate].text = GodPowerRate.ToString();
			LabelBase[(int)GameUILabel.CurExp].text = curExp.ToString();
			LabelBase[(int)GameUILabel.MaxExp].text = maxExp.ToString();

			vitalCDUI[0].fillAmount = (float)curHealth/maxHealth;
			vitalCDUI[1].fillAmount = (float)curMana/maxMana;
			vitalCDUI[2].fillAmount = (playerInfo.GetVital((int)VitalName.Energy).CurValue/2000);
			vitalCDUI[3].fillAmount = (playerInfo.GodPower/100);
			expUI.value = curExp/(float)maxExp;
		}
	}


	public void UpdateState()
	{
		foreach(InRoom_Menu.PlayerInfoData pla in InRoom_Menu.SP.playerList)
		{
			if(pla.properties.Ai>=0)
			{
				int team = pla.team;
				int role = pla.roleType;
				int pos = pla.pos;
				int StateAnchorNum = pos + (team-1)*InRoom_Menu.MaxTeamNum;
				int listNum = -1;
				for(int cnt=0;cnt<playerList.Count;cnt++)
				{
					if(playerList[cnt].team==team&&playerList[cnt].pos==pos)
					{
						listNum = cnt;
						break;
					}
				}
				if(team!=0)
				{
					int ping = pla.ping;
					int kills = pla.properties.Kills;
					int bekilleds = pla.properties.BeKills;
					int level = pla.properties.level;
					int getLS = pla.properties.GetLightSource;
					int useLS = pla.properties.UseLightSource;
					int money = pla.properties.Money;

					if(ping!=0)
						Pings[StateAnchorNum].text = ping.ToString();
					if(kills!=0)
						StateNum[StateAnchorNum].Kill.text = kills.ToString();
					if(level!=1)
						StateNum[StateAnchorNum].Level.text = level.ToString();
					if(bekilleds!=0)
						StateNum[StateAnchorNum].Bekilled.text = bekilleds.ToString();
					if(getLS!=0)
						StateNum[StateAnchorNum].LS_Get.text = getLS.ToString();
					if(useLS!=0)
						StateNum[StateAnchorNum].LS_Use.text = useLS.ToString();
					if(money!=0)
						StateNum[StateAnchorNum].Money.text = money.ToString();
					if(StateNum[StateAnchorNum].Name.text=="PC")
						StateNum[StateAnchorNum].Name.text = pla.properties.Name;
						
					for(int cnt1 = 0;cnt1<6;cnt1++)
					{
						int item = pla.properties.Items[cnt1];

						if(playerList[listNum].properties.Items[cnt1]!=item)
						{ 
							Transform ThisItemBase = StateItemAndBuff[StateAnchorNum].Items[cnt1].transform;
							GameObject itemPic = ThisItemBase.GetComponentInChildren<UISprite>().gameObject;
							if(itemPic!=null)
								Destroy(itemPic);
							GameObject newItemPic = Instantiate(ItemPics[item]) as GameObject;
							newItemPic.transform.parent = ThisItemBase;
							newItemPic.transform.localPosition = Vector3.zero;
							playerList[listNum].properties.Items[cnt1] = item;
						}
					}
					for(int cnt2 = 0;cnt2<2;cnt2++)
					{
						int buff = pla.properties.Buff[cnt2];
						if(playerList[listNum].properties.Buff[cnt2]!=buff)
						{
							Transform ThisBuffBase = StateItemAndBuff[StateAnchorNum].Buffs[cnt2].transform;
							GameObject buffPic = ThisBuffBase.GetComponentInChildren<UISprite>().gameObject;
							if(buffPic!=null)
								Destroy(buffPic);
							GameObject newBuffPic = Instantiate(BuffPics[buff]) as GameObject;
							newBuffPic.transform.parent = ThisBuffBase;
							newBuffPic.transform.localPosition = Vector3.zero;
							playerList[listNum].properties.Buff[cnt2] = buff;
						}
					}
					if(StatePicsRecord[StateAnchorNum]!=role)
					{
						Debug.Log("StateAnchorNum:" + StateAnchorNum);
						AssignStatePic(StateAnchorNum,team,role);
					}

				}
			}
		}
	}



	void AssignStatePic(int cnt, int team, int role)
	{
		StatePicsRecord[cnt] = role;
		Debug.Log("cnt:" + cnt);
		Transform ThisStatePic = States[cnt];

		GameObject statePic = null;
		Transform statePicTrans = ThisStatePic.FindChild("StatePic");
		if(statePicTrans!=null)
		{
			statePic = statePicTrans.gameObject;
			Destroy(statePic);
		}
		GameObject newStatePic = Instantiate(gamePlayerPic[(role) + (team-1)*3]) as GameObject;
		Transform newStatePicTrans = newStatePic.transform;
		newStatePicTrans.parent = ThisStatePic;
		newStatePic.GetComponent<UISprite>().pivot = UIWidget.Pivot.Center;
		newStatePicTrans.localPosition = Vector3.zero;
		newStatePicTrans.localScale = Vector3.one;
		newStatePic.name = "StatePic";
		newStatePic.GetComponent<UISprite>().SetDimensions(50,50);

	}

	void UpdateLightSource()
	{
		lightSourceUI[0].value = L_LSManager.LightSource/(float)WholeGameManager.SP.StartingLightSource;
		lightSourceUI[1].value = D_LSManager.LightSource/(float)WholeGameManager.SP.StartingLightSource;
	}

	void UpdateSkillCD()
	{
		for(int cnt=0;cnt<4;cnt++)
		{
			float lastSkillTime = playerInfo._skillCDTimer[cnt];
			if(lastSkillTime!=0)
			{
				if(Time.time - lastSkillTime >= playerInfo.GetMagicSkill(cnt).CDTime)
				{
					if(hotKeyCDUI[cnt].fillAmount>0)
					{
						hotKeyCDUI[cnt].fillAmount = 0;
					}
				}
				else
				{
					hotKeyCDUI[cnt].fillAmount = 1 - (float)((Time.time - lastSkillTime)/playerInfo.GetMagicSkill(cnt).CDTime);
				}
			}
			else
			{
				hotKeyCDUI[cnt].fillAmount = 0;
			}
		}
	}

	public void OpenChooseRolePage(int Team, int pos, int exp)
	{
		Screen.lockCursor = false;

		gameUIPage[Team-1].SetActive(true);
		myTeam = Team;
		if(exp!=-1)
		{
			myRole = 0;
			myPos = pos;
			myExp = exp;
		}
	
		UpdateChosenRoleObject(myTeam);

		if(Team==1)
		{
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRoleEnter]).onClick = RespawnChosenRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole1]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole2]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole3]).onClick = ChooseRole;
		}
		else
		{
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRoleEnter]).onClick = RespawnChosenRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole1]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole2]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole3]).onClick = ChooseRole;
		}
		Game_Manager.SP.MyGameState = GameState.CanChoosing;
	}

	public void DeadLockChooseRolePage()
	{
		Screen.lockCursor = true;
		gameUIPage[myTeam-1].SetActive(false);
		myRole = 0;
		Destroy(myPlayer.gameObject);
		myPlayer = null;
	}

	public void CloseChooseRolePage()
	{
		Screen.lockCursor = true;
		gameUIPage[myTeam-1].SetActive(false);
		myTeam = 0;
		myRole = 0;
		myPos = 0;
		myExp = 0;
		Destroy(myPlayer.gameObject);
		myPlayer = null;
		
		/*if(myTeam==1)
		{
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRoleEnter]).onClick = RespawnChosenRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole1]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole2]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseLightRole3]).onClick = ChooseRole;
		}
		else
		{
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRoleEnter]).onClick = RespawnChosenRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole1]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole2]).onClick = ChooseRole;
			UIEventListener.Get(gameUIButton[(int)GameUIButton.ChooseDarkRole3]).onClick = ChooseRole;
		}*/
	}

	void UpdateChosenRoleObject(int team)
	{
		if(myPlayer!=null)
		{
			Destroy(myPlayer.gameObject);
			myPlayer = null;
		}

		if(team==1)
			myPlayer = Instantiate(InRoom_Menu.SP.LightPlayerPrefab[myRole], myTransform.position, Quaternion.identity) as Transform;
		else
			myPlayer = Instantiate(InRoom_Menu.SP.DarkPlayerPrefab[myRole], myTransform.position, Quaternion.identity) as Transform;

		ParticleSystem[] PSs = myPlayer.GetComponentsInChildren<ParticleSystem>();
		if(PSs.Length>0)
		{
			foreach(ParticleSystem PS in PSs)
			{
				PS.enableEmission = false;
			}
		}

		//myPlayer
		WholeGameManager.SP.ChangeLayersRecursively(myPlayer,"UI");


		myPlayer.parent = gameUIPage[team-1].transform;
		myPlayer.Rotate(new Vector3(0,180,0));
		myPlayer.localPosition = new Vector3(0,-70,-250);
		myPlayer.localScale = Vector3.one*5;
		if(team==1)
		{
			if(myRole==2)
				myPlayer.localScale = Vector3.one*120;
			else if(myRole==1)
				myPlayer.localScale = Vector3.one*140;
			else
				myPlayer.localScale = Vector3.one*130;
		}
		else
		{
			if(myRole!=0)
			{
				myPlayer.localScale = Vector3.one*120;
			}
			else
			{
				myPlayer.localScale = Vector3.one*4;
			}
		}
	}

	void UpdatePlayerPic(int myRole)
	{
		GameObject picture = Instantiate(gamePlayerPic[(myRole) + (myTeam-1)*3]) as GameObject;
		Transform pictureTrans = picture.transform;
		pictureTrans.parent = picBase;
		pictureTrans.localPosition = new Vector3(10,-11,0);
		pictureTrans.localScale = Vector3.one;

	}

	void RespawnChosenRole(GameObject button)
	{
		//bool canRivive = false;
		/*if(Game_Manager.SP.currentTeam==1)
		{
			if(L_LSManager.LightSource>35)
			{
				canRivive = true;
			}
		}
		else if(Game_Manager.SP.currentTeam==2)
		{
			if(D_LSManager.LightSource>35)
			{
				canRivive = true;
			}
		}*/
		//if(canRivive)
		//{
			OffTeamMateCamera();
			Game_Manager.SP.MyGameState = GameState.Playing;
			GameUIManager.SP.isShowDeadUI = false;
			InRoom_Menu.SP.RespawnPlayer(myTeam,myPos,1,0,myRole,myExp);
			UpdatePlayerPic(myRole);
			CloseChooseRolePage();
		//}
	}

	void ChooseRole(GameObject button)
	{
		if(button==gameUIButton[(int)GameUIButton.ChooseLightRole1])
		{
			if(myPlayer!=InRoom_Menu.SP.LightPlayerPrefab[0])
				myRole = 0;
		}
		else if(button==gameUIButton[(int)GameUIButton.ChooseLightRole2])
		{
			if(myPlayer!=InRoom_Menu.SP.LightPlayerPrefab[1])
				myRole = 1;
		}
		else if(button==gameUIButton[(int)GameUIButton.ChooseLightRole3])
		{
			if(myPlayer!=InRoom_Menu.SP.LightPlayerPrefab[2])
				myRole = 2;
		}
		else if(button==gameUIButton[(int)GameUIButton.ChooseDarkRole1])
		{
			if(myPlayer!=InRoom_Menu.SP.DarkPlayerPrefab[0])
				myRole = 0;
		}
		else if(button==gameUIButton[(int)GameUIButton.ChooseDarkRole2])
		{
			if(myPlayer!=InRoom_Menu.SP.DarkPlayerPrefab[1])
				myRole = 1;
		}
		else if(button==gameUIButton[(int)GameUIButton.ChooseDarkRole3])
		{
			if(myPlayer!=InRoom_Menu.SP.DarkPlayerPrefab[2])
				myRole = 2;
		}
		UpdateChosenRoleObject(myTeam);
	}

	public void UpdateGameSkillUI(int team, int role)
	{
		for(int cnt = 0; cnt <5; cnt++)
		{
			GameObject skill = Instantiate(Room_MenuUIManager.SP.SkillUIPrefab[(role)*5 + cnt + (team-1)*15]) as GameObject;
			Transform skillTrans = skill.transform;
			skillTrans.parent = hotKey[cnt].transform;
			skillTrans.localPosition = Vector3.zero;
			skillTrans.localScale = Vector3.one;
		}
	}	

	public void OffTeamMateCamera()
	{
		foreach(GameObject cam in teamMateCamera)
		{
			if(cam!=null&&cam.activeSelf==true)
			{
				cam.SetActive(false);
			}
		}
	}

	public void SwitchTeamMataCamera()
	{
		teamMateCamera.Clear();
		
		foreach(InRoom_Menu.PlayerInfoData pla in InRoom_Menu.SP.playerList)
		{
			if(myTeam==0)
			{
				myTeam = InRoom_Menu.SP.localPlayer.team;
			}
			if(pla.team==myTeam)
			{
				if(!pla.IsLocal)
				{
					teamMateCamera.Add(pla.transform.FindChild("Camera").gameObject);
				}
			}
		}
		if(teamMateCamera.Count>0)
		{
			int num = (int)(UnityEngine.Random.Range(0,teamMateCamera.Count-0.01f));
			teamMateCamera[num].SetActive(true);
		}
		else
		{
			GameUIManager.SP.HudWarningLabel.Add("No camera on alley can be opened...",Color.red,3);
		}
	}

	void ShowLightSourceWarning()
	{
		bool show = WarningLabel[(int)GameUIWarningLabel.NoLightSourceWarning].enabled;
		WarningLabel[(int)GameUIWarningLabel.NoLightSourceWarning].enabled = !show;
	}
	
}
