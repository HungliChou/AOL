using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum RoomUIButton
{
	LightTeam,DarkTeam,Team1Warrior,Team1Mage,Team1Giant,Team2Warrior,Team2Mage,Team2Giant,Ready,BackToLobby,Start
}

public enum RoomUILabel
{
	Timer,Light1Player,Light2Player,Light3Player,Light4Player,Dark1Player,Dark2Player,Dark3Player,Dark4Player
}

public enum RoomRoleSkillUIPage
{
	Mars,Theia,Hyperion,Darkman,Persia,Steropi
}

public enum RoomEachSkillUIpage
{
	Mars1,Mars2,Mars3,Mars4,MarsSuper,Theia1,Theia2,Theia3,Theia4,TheiaSuper,Hyperion1,Hyperion2,Hyperion3,Hyperion4,HyperionSuper,
	Darkman1,Darkman2,Darkman3,Darkman4,DarkmanSuper,Persia1,Persia2,Persia3,Persia4,PersiaSuper,Steropi1,Steropi2,Steropi3,Steropi4,SteropiSuper
}

public class Room_MenuUIManager : Photon.MonoBehaviour {

	public static Room_MenuUIManager SP;
	public GameObject PrefabAnchor;

	public List<UILabel> roomUIInfoValue = new List<UILabel>();
	public List<UISlider> roomUIInfo = new List<UISlider>();
	public List<GameObject> roomUIRoleInfo = new List<GameObject>();
	public List<GameObject> roomUIButton = new List<GameObject>();
	public List<UILabel> roomUILabel = new List<UILabel>();
	public List<GameObject> roomUIRoleBase = new List<GameObject>();
	public List<GameObject> roomUIRolePage = new List<GameObject>();
	public List<GameObject> roomRoleSkillUIPage = new List<GameObject>();
	public List<GameObject> roomEachSkillUIpage = new List<GameObject>();
	public List<GameObject> roomEachSkillUIButton = new List<GameObject>();
	public List<GameObject> RoleUIPrefab = new List<GameObject>();
	public List<GameObject> SkillUIPrefab = new List<GameObject>();
	public Hashtable roomProperties;
	public int time;
	public int nowSkill;
	public int currentTeam;
	public int currentPos;
	public int currentRole;
	public HUDText warningHud;
	//public List<InRoom_Menu.PlayerInfoData> UIplayerList;

	void AssignRoomButtonListener()
	{
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.LightTeam]).onClick = ChangeTeam;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.DarkTeam]).onClick = ChangeTeam;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Warrior]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Mage]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Giant]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Warrior]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Mage]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Giant]).onClick = ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Ready]).onClick = LockUI;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.BackToLobby]).onClick = BackToLobby;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Start]).onClick = StartGame;
		foreach(GameObject button in roomEachSkillUIButton)
		{
			UIEventListener.Get(button).onClick = Changeskill;
		}
	}

	void LockRoomButtonListener()
	{
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.LightTeam]).onClick -= ChangeTeam;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.DarkTeam]).onClick -= ChangeTeam;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Warrior]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Mage]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team1Giant]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Warrior]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Mage]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Team2Giant]).onClick -= ChangeRole;
		UIEventListener.Get(roomUIButton[(int)RoomUIButton.Ready]).onClick -= LockUI;
		//UIEventListener.Get(roomUIButton[(int)RoomUIButton.BackToLobby]).onClick -= BackToLobby;
		//UIEventListener.Get(roomUIButton[(int)RoomUIButton.Start]).onClick -= StartGame;
	}

	void BackToLobby(GameObject button)
	{
		WaitForBackingToLobby();
		//StartCoroutine(WaitForBackingToLobby());
	}

	void WaitForBackingToLobby()
	{
		/*if(PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.visible = false;
			photonView.RPC("ForceAllLeaveRoom",PhotonTargets.All);
			/*if(InRoom_Menu.SP.playerList.Count>1)
			{
				while(InRoom_Menu.SP.MasterAssignTPLock)
					yield return new WaitForSeconds(0.05f);
				int id = SelectMasterClient();
				InRoom_Menu.SP.SendMasterClientInfoToNextInput(id);
				yield return new WaitForSeconds(0.2f);
				PhotonNetwork.SetMasterClient(InRoom_Menu.SP.GetPlayerFromID(id).networkPlayer);
				Debug.Log("ID:" + id + " Name:" + InRoom_Menu.SP.GetPlayerFromID(id).networkPlayer.name);
			}*/

		//}
		if(WholeGameManager.SP.InGame==false)
		{
			if(!InRoom_Menu.SP.StartGame)
				InRoom_Menu.SP.OnLeftRoom();
		}
	}

	[RPC]
	void ForceAllLeaveRoom()
	{
		if(PhotonNetwork.isNonMasterClientInRoom)
			InRoom_Menu.SP.OnLeftRoomFromOverView();
	}

	int SelectMasterClient()
	{
		int localID = InRoom_Menu.SP.localPlayer.ID;
		int playerNum = (int)(Random.Range(0,InRoom_Menu.SP.playerList.Count-0.01f));
		int id = InRoom_Menu.SP.playerList[playerNum].ID;
		if(id==localID)
		{
			return SelectMasterClient();
		}
		else
		{
			return id;
			Debug.Log("SelectID:" + id);
		}
	}

	void StartGame(GameObject button)
	{
		if(WholeGameManager.SP.InGame==false)
		{
			if(PhotonNetwork.isMasterClient)
			{
				if((InRoom_Menu.SP.teamNum[0]>0&&InRoom_Menu.SP.teamNum[1]>0)||WholeGameManager.SP.isTesting)
				{
					PhotonNetwork.room.open = false;
					PhotonNetwork.room.visible = false;
					roomProperties = PhotonNetwork.room.customProperties;
					roomProperties["Time"] = 5;
					photonView.RPC("ReadyToEnterGame",PhotonTargets.All);
					UIEventListener.Get(roomUIButton[(int)RoomUIButton.Start]).onClick -= StartGame;
				}
				else
				{
					warningHud.Add("No enough player to start the game!",Color.yellow,2);
				}
			}
		}
	}

	public void UpdateRoleInfo(TP_Info info)
	{
		int curValue = 0;
		float maxValue = 0;
		for(int cnt = 0; cnt<roomUIInfo.Count;cnt++)
		{
			curValue = info.GetAbility(cnt).CurValue;
			roomUIInfoValue[cnt].text = curValue.ToString();

			if(cnt==0)
				maxValue = 200;
			else if(cnt==1)
			{
				maxValue = 20;
				curValue -= 75;
			}
			else if(cnt==2)
				maxValue = 15;
			else if(cnt==3)
			{
				maxValue = 30;
				curValue -= 100;
			}
			else if(cnt==4)
				maxValue = 100;
			else if(cnt==5)
			{
				maxValue = 100;
				curValue -= 100;
			}
			else if(cnt==6)
				maxValue = 100;

			roomUIInfo[cnt].value = (curValue/maxValue);
		}
	}

	public void UpdateUIRoleInfo()
	{
		foreach(GameObject GO in roomUIRoleInfo)
		{
			GO.SetActive(false);
		}
		roomUIRoleInfo[InRoom_Menu.SP.localPlayer.roleType + (InRoom_Menu.SP.localPlayer.team-1)*3].SetActive(true);
	}

	public void UpdateRoleEachSkillValue(TP_Info info, int role, int team)
	{
		int curValue = 0;

		for(int cnt = 0; cnt<4;cnt++)
		{
			curValue = (int)info.GetMagicSkill(cnt).CurValue;
			roomEachSkillUIpage[cnt + ((role) + (team-1)*3)*5].transform.Find("PowerTag/Value").GetComponent<UILabel>().text = curValue.ToString();
		}	
		curValue = (int)info.GetSuperSkill().CurValue;
		roomEachSkillUIpage[4 + ((role) + (team-1)*3)*5].transform.Find("PowerTag/Value").GetComponent<UILabel>().text = curValue.ToString();
	}

	public void ResetEachAllSkillPage(int team,int role)
	{
		int skillnum = ((role) + (team-1)*3)*5;
		for(int cnt = skillnum;cnt<=skillnum+4;cnt++)
		{
			if(cnt==skillnum)
				roomEachSkillUIpage[cnt].SetActive(true);
			else
				roomEachSkillUIpage[cnt].SetActive(false);
		}
		nowSkill = skillnum;
	}

	void Changeskill(GameObject button)
	{
		int whichSkill = -1;
		for(int cnt = 0;cnt<roomEachSkillUIButton.Count;cnt++)
		{
			if(button==roomEachSkillUIButton[cnt])
			{
				whichSkill = cnt;
				break;
			}
		}
		roomEachSkillUIpage[nowSkill].SetActive(false);
		roomEachSkillUIpage[nowSkill].transform.localPosition += Vector3.up*500;
		roomEachSkillUIpage[nowSkill].GetComponent<SpringPosition>().enabled = true;

		nowSkill = whichSkill;
		roomEachSkillUIpage[nowSkill].SetActive(true);
	}

	void ChangeTeam(GameObject button)
	{
		int teamToChange = 0;
		if(button==roomUIButton[(int)RoomUIButton.LightTeam])
		{
			if(currentTeam!=1)
			{
				teamToChange = 1;
			}
		}
		else if(button==roomUIButton[(int)RoomUIButton.DarkTeam])
		{
			if(currentTeam!=2)
			{
				teamToChange = 2;
			}
		}
		if(teamToChange>0)
			photonView.RPC("RequestChangeTeam",PhotonTargets.MasterClient,InRoom_Menu.SP.localPlayer.ID,teamToChange);
	}

	void ChangeRole(GameObject button)
	{
		int team = InRoom_Menu.SP.localPlayer.team;
		int role = InRoom_Menu.SP.localPlayer.roleType;
		int newRole = 0;

		if(team==1)
		{
			if(button==roomUIButton[(int)RoomUIButton.Team1Warrior])
			{
				if(role!=0)
				{
					newRole = 1;
				}
			}
			else if(button==roomUIButton[(int)RoomUIButton.Team1Mage])
			{
				if(role!=1)
				{
					newRole = 2;
				}
			}
			else if(button==roomUIButton[(int)RoomUIButton.Team1Giant])
			{
				if(role!=2)
				{
					newRole = 3;
				}
			}
		}
		else if(team==2)
		{
			if(button==roomUIButton[(int)RoomUIButton.Team2Warrior])
			{
				if(role!=0)
				{
					newRole = 1;
				}
			}
			else if(button==roomUIButton[(int)RoomUIButton.Team2Mage])
			{
				if(role!=1)
				{
					newRole = 2;
				}
			}
			else if(button==roomUIButton[(int)RoomUIButton.Team2Giant])
			{
				if(role!=2)
				{
					newRole = 3;
				}
			}
		}

		if(newRole>0)
		{

			photonView.RPC("RequestChangeRole",PhotonTargets.MasterClient,InRoom_Menu.SP.localPlayer.ID,newRole-1);
		
			/*foreach(GameObject page in roomRoleSkillUIPage)
			{
				page.SetActive(false);
			}*/
			//roomRoleSkillUIPage[(newRole-1)+(team-1)*3].SetActive(true);
			//ResetEachAllSkillPage(team,role);
			//Destroy(InRoom_Menu.SP.myPlayer.gameObject);
			//InRoom_Menu.SP.SpawnMyRoomPlayer();
		}
	}

	void LockUI(GameObject button)
	{
		LockRole();
	}

	void Awake()
	{
		SP = this;
		currentTeam = 0;
		currentPos = 0;
		currentRole = 0;
	}

	// Use this for initialization
	void Start () {
		AssignRoomButtonListener();
		InvokeRepeating("TimeManager",0.01f,1);
		//UIplayerList = InRoom_Menu.SP.playerList;
	}

	void TimeManager()
	{
		roomProperties = PhotonNetwork.room.customProperties;
		time = (int)roomProperties["Time"];

		if(time<=0)
		{
			time = 0;
		}

		string sec = ((int)(time%60)).ToString();
		if(sec=="0")
			sec = "00";
		string timeStr = "0" + (int)(time/60) + ":" + sec;

		roomUILabel[(int)RoomUILabel.Timer].text = timeStr;
		if(roomUILabel[(int)RoomUILabel.Timer].color != Color.white)
			roomUILabel[(int)RoomUILabel.Timer].color = Color.white;


		if(time==0)
		{
			if(InRoom_Menu.SP.teamNum[0]>0&&InRoom_Menu.SP.teamNum[1]>0)
			{
				LockRole();
				if(PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.room.open = false;
					PhotonNetwork.room.visible = false;
				 	roomUIButton[(int)RoomUIButton.Start].SetActive(true);
				}
				CancelInvoke("TimeManager");
			}
			else
			{
				if(PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.room.open = true;
					PhotonNetwork.room.visible = true;
					roomProperties["Time"] = 60;
					PhotonNetwork.room.SetCustomProperties(roomProperties);
					photonView.RPC("RestartCountDown",PhotonTargets.All,0);
				}
			}
		}
		else
		{
			if(PhotonNetwork.isMasterClient)
			{
				time -= 1;
				roomProperties["Time"] = time;
				PhotonNetwork.room.SetCustomProperties(roomProperties);
			}
		}
	}


	void ReadyTimeManager()
	{
		roomProperties = PhotonNetwork.room.customProperties;
		time = (int)roomProperties["Time"];

		if(time<=0)
		{
			time = 0;
		}
		
		string sec = ((int)(time%60)).ToString();
		if(sec=="0")
			sec = "00";
		string timeStr = "0" + (int)(time/60) + ":" + sec;
		
		roomUILabel[(int)RoomUILabel.Timer].text = timeStr;
		if(roomUILabel[(int)RoomUILabel.Timer].color != Color.red)
			roomUILabel[(int)RoomUILabel.Timer].color = Color.red;

		if(time==0)
		{
			if((InRoom_Menu.SP.teamNum[0]>0&&InRoom_Menu.SP.teamNum[1]>0)||WholeGameManager.SP.isTesting)
			{
				if(PhotonNetwork.isMasterClient)
				{
					if(InRoom_Menu.SP.StartGame)
					{
						PhotonNetwork.room.open = false;
						PhotonNetwork.room.visible = false;
						ForceLoadGame();
					}
					else
					{
						PhotonNetwork.room.open = true;
						PhotonNetwork.room.visible = true;
						roomProperties["Time"] = 60;
						PhotonNetwork.room.SetCustomProperties(roomProperties);
						photonView.RPC("RestartCountDown",PhotonTargets.All,1);
					}
				}

			}
			else
			{
				if(InRoom_Menu.SP.StartGame)
					InRoom_Menu.SP.StartGame = false;
				if(PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.room.open = true;
					PhotonNetwork.room.visible = true;
					roomProperties["Time"] = 60;
					PhotonNetwork.room.SetCustomProperties(roomProperties);
					photonView.RPC("RestartCountDown",PhotonTargets.All,1);
				}
			}
			CancelInvoke("ReadyTimeManager");
		}
		else
		{
			if(PhotonNetwork.isMasterClient)
			{
				time -= 1;
				roomProperties["Time"] = time;
				PhotonNetwork.room.SetCustomProperties(roomProperties);
			}
		}
	}
	
	[RPC]
	void RestartCountDown(int type)
	{
		if(type==1)
			warningHud.Add("Reallocate the game.( Someone left the room. )",Color.red,2);
		else if(type==0)
		{
			CancelInvoke("TimeManager");
			warningHud.Add("Reallocate the game.( Not enough players )",Color.red,2);
		}
		InvokeRepeating("TimeManager",0.01f,1);
		AssignRoomButtonListener();
	}

	void LockRole()
	{
		InRoom_Menu.SP.LockRoleInput();
		LockRoomButtonListener();
	}

	public void ReadyTimeManagerInput()
	{
		InRoom_Menu.SP.StartGame = true;
		CancelInvoke("TimeManager");
		if(PhotonNetwork.isMasterClient)
		{
//			Debug.Log("AllocateAI");
			InRoom_Menu.SP.AllocateAI();
		}
		InvokeRepeating("ReadyTimeManager",0.01f,1);
	}

	public void UpdatePlayerRoleUI( int team, int pos, int role, string name)
	{
		//roomUILabel[(pos+1)+(team-1)*InRoom_Menu.MaxTeamNum].text = name;

		Transform UIRoleBase = roomUIRoleBase[pos + (team-1)*InRoom_Menu.MaxTeamNum].transform;

		Transform roleUI = UIRoleBase.FindChild("RoleUI");
		if(roleUI!=null)
		{
			Destroy(roleUI.gameObject);
		}

		GameObject newRoleUI = Instantiate(RoleUIPrefab[(role)*2 + (team-1)], Vector3.zero,Quaternion.identity) as GameObject;
		Transform newRoleUITrans = newRoleUI.transform;
		newRoleUITrans.parent = UIRoleBase;
		newRoleUITrans.name = "RoleUI";
		newRoleUITrans.localRotation = Quaternion.identity;
		newRoleUITrans.localScale = Vector3.one;
		newRoleUITrans.localPosition = Vector3.zero;
	}

	#region Load Game-Scene
	public void ForceLoadGame()
	{
		PhotonNetwork.automaticallySyncScene = true;
		
		photonView.RPC("DoLoadGame", PhotonTargets.AllBuffered);
	}
	
	[RPC]
	void ReadyToEnterGame()
	{
		ReadyTimeManagerInput();
	}
	
	[RPC]
	void DoLoadGame()
	{
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel("Game-Scene");
		this.enabled = false;
	}

	#endregion

	void UpdateRoomScreenUI()
	{
		int maxTeamNum = InRoom_Menu.MaxTeamNum;
		int team = 0;
		int pos = 0;
		int role = 0;
		Transform UIRoleBase = null;

		foreach(InRoom_Menu.PlayerInfoData pla in InRoom_Menu.SP.playerList)
		{
				team = pla.team;
				pos = pla.pos;
				role = pla.roleType;
			if(team!=0)
			{
				UIRoleBase = roomUIRoleBase[pos + (team-1)* maxTeamNum].transform;

				Transform roleUI = UIRoleBase.FindChild("RoleUI");
				if(roleUI!=null)
				{
					Destroy(roleUI.gameObject);
				}

				GameObject newRoleUI = Instantiate(RoleUIPrefab[(role)*2 + (team-1)], Vector3.zero,Quaternion.identity) as GameObject;
				Transform newRoleUITrans = newRoleUI.transform;
				newRoleUITrans.parent = UIRoleBase;
				newRoleUITrans.name = "RoleUI";
				newRoleUITrans.localRotation = Quaternion.identity;
				newRoleUITrans.localScale = Vector3.one;
				newRoleUITrans.localPosition = Vector3.zero;

				roomUILabel[(pos+1)+(team-1)* maxTeamNum].text = pla.name;
			}
		}

		for(int checkTeam = 1; checkTeam<=2; checkTeam++)
		{
			for(int checkPos = 0; checkPos<maxTeamNum; checkPos++)
			{
				bool hasPlayer = false;
				foreach(InRoom_Menu.PlayerInfoData pla in InRoom_Menu.SP.playerList)
				{
					if(pla.team==checkTeam&&pla.pos==checkPos)
					{
						hasPlayer = true;
						break;
					}
				}
				if(!hasPlayer)
				{
					UIRoleBase = roomUIRoleBase[checkPos + (checkTeam-1)* maxTeamNum].transform;
					
					Transform roleUI = UIRoleBase.FindChild("RoleUI");
					if(roleUI!=null)
					{
						Destroy(roleUI.gameObject);
					}
					roomUILabel[(checkPos+1)+(checkTeam-1)* maxTeamNum].text = "No Player";
				}
			}
		}

		if(currentTeam==0)
		{
			currentTeam = InRoom_Menu.SP.localPlayer.team;
			currentPos = InRoom_Menu.SP.localPlayer.pos;
			currentRole = InRoom_Menu.SP.localPlayer.roleType;
			if(currentTeam!=0)
			{
				roomRoleSkillUIPage[(currentRole)+(currentTeam-1)*3].SetActive(true);
				ResetEachAllSkillPage(currentTeam,currentRole);
				StartCoroutine(InRoom_Menu.SP.SpawnMyRoomPlayer(currentTeam,currentRole));
			}
		}
		else
		{
			bool needChangeLocalPlayerUI = false;
			int localTeam = InRoom_Menu.SP.localPlayer.team;
			int localRole = InRoom_Menu.SP.localPlayer.roleType;

			if(currentTeam!=localTeam)
			{
				currentTeam = localTeam;
				needChangeLocalPlayerUI = true;
			}
			if(currentRole!=localRole)
			{
				currentRole = localRole;
				needChangeLocalPlayerUI = true;
			}
			if(needChangeLocalPlayerUI)
			{
				if(InRoom_Menu.SP.myPlayer!=null)
				{
					if(InRoom_Menu.SP.myPlayer.gameObject!=null)
						Destroy(InRoom_Menu.SP.myPlayer.gameObject);
				}
				foreach(GameObject page in roomRoleSkillUIPage)
				{
					page.SetActive(false);
				}
				roomRoleSkillUIPage[(currentRole)+(currentTeam-1)*3].SetActive(true);
				ResetEachAllSkillPage(currentTeam,currentRole);
				StartCoroutine(InRoom_Menu.SP.SpawnMyRoomPlayer(localTeam,localRole));
			}

			for(int cnt = 1; cnt<=roomUIRolePage.Count;cnt++)
			{
				if(cnt==localTeam)
					roomUIRolePage[cnt-1].SetActive(true);
				else
					roomUIRolePage[cnt-1].SetActive(false);
			}
		}
	}

	void Update()
	{
		UpdateRoomScreenUI();
		if(PhotonNetwork.isMasterClient)
			CheckReady();
	}

	void CheckReady()
	{
		int notReady = 0;
		int team1 = 0;
		int team2 = 0;
		for(int cnt = 0;cnt<InRoom_Menu.MaxTeamNum;cnt++)
		{
			if(InRoom_Menu.SP.team1Seat[cnt])
			{
				team1++;
				if(!InRoom_Menu.SP.team1Ready[cnt])
					notReady++;
			}
			if(InRoom_Menu.SP.team2Seat[cnt])
			{
				team2++;
				if(!InRoom_Menu.SP.team2Ready[cnt])
					notReady++;
			}
		}

		if(notReady==0)
		{
			roomUIButton[(int)RoomUIButton.Start].SetActive(true);
		}
		else
			roomUIButton[(int)RoomUIButton.Start].SetActive(false);
	}
}
