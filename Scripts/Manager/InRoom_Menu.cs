using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using RAIN.Core;
using RAIN.Entities;

public class InRoom_Menu : Photon.MonoBehaviour {

	public enum Team
	{
		None,team1,team2
	}

	public enum Item
	{
		HealthWater,ManaWater
	}

	public enum BuffItem
	{
		Speed,Attack
	}


	public static InRoom_Menu SP;
	public bool StartGame;
	public Room_MenuUIManager roomUIManager;
	public LightSourceManager L_LSManager;
	public LightSourceManager D_LSManager;
	public Transform myPlayer;
	public Renderer[] renderer;
	public int myID;
	public int MasterClientID; //For checking Masterclient leaving room
	public int MasterClientTeam; //For checking Masterclient leaving room
	public List<Transform> LightPlayerPrefab = new List<Transform>();
	public List<Transform> DarkPlayerPrefab = new List<Transform>();
	public List<Transform> aILightPrefabs = new List<Transform>();
	public List<Transform> aIDarkPrefabs = new List<Transform>();
	public List<Transform> monsterPrefabs = new List<Transform>();
	
	public List<Effects> effectPrefabs = new List<Effects>();
	public GameObject TextPrefab;
	public HUDText HudWarningLabel;
	public Hashtable roomProperties;
	public Hashtable playerProperties;

	public bool MasterAssignTPLock;

	public List<PlayerInfoData> playerList;
	public List<Transform> AiListGo; 
	public PlayerInfoData localPlayer;
	/*public Team myTeam;
	public int myPos;
	public bool isResetPos;
	
	public List<PlayerInfoData> Team1PlayerList;
	public List<PlayerInfoData> Team2PlayerList;*/

	public bool[] team1Seat = new bool[MaxTeamNum];
	public bool[] team2Seat = new bool[MaxTeamNum];
	public bool[] team1Ready = new bool[MaxTeamNum];
	public bool[] team2Ready = new bool[MaxTeamNum];

	public int[] teamNum;
	public static int MaxTeamNum = 4;
	
	private Transform myTransform;
	
	public SpawnPoints SPScript;
	
	public bool isAimed;

	public GameObject HitAudio;
	public List<AudioClip> hitSoundPrefab;

	[System.Serializable]
	public class PlayerInfoData
	{
		public PhotonPlayer networkPlayer;
		public Transform transform;
		public int ID;
		public PropertiesInfoData properties;
		public int team;
		public int pos;
		public int roleType;
		public bool lockRole;
		public int bloodType;
		public int ping;

		public string name
		{
			get
			{
				return networkPlayer.name;
			}
		}
		
		public bool IsLocal
		{
			get
			{
				return networkPlayer.isLocal;
			}
		}
	}

	[System.Serializable]
	public class PropertiesInfoData
	{
		public int Ai;   //0->player,else->Ai flag number
		public string Name;
		public int level;
		public int Money;
		public int Exp;
		public int Kills;
		public int BeKills;
		public int[] Items;
		public int[] Buff;
		public int HealthWaterNum;
		public int ManaWaterNum;
		public int ThrowingWeaponNum;
		public int UseLightSource;
		public int GetLightSource;

		public PropertiesInfoData(string name)
		{
			Ai = 0;
			Name = name;
			level = 0;
			Money = 0;
			Exp = 0;
			Kills = 0;
			BeKills = 0;
			Items = new int[6];
			Buff = new int[2];
			HealthWaterNum = 0;
			ManaWaterNum = 0;
			UseLightSource = 0;
			GetLightSource = 0;
			ThrowingWeaponNum = 0;
		}
	}
	
	public void Awake()
	{
		PhotonNetwork.networkingPeer.SentCountAllowance = 10;
		DontDestroyOnLoad(transform.gameObject);
		isAimed = false;
		SP = this;
		myTransform = transform;

		playerList = new List<PlayerInfoData>();
		//Team1PlayerList = new List<PlayerInfoData>();
		//Team2PlayerList = new List<PlayerInfoData>();
		teamNum = new int[2];

		roomProperties = new Hashtable();
		playerProperties = new Hashtable();
		
		PhotonNetwork.isMessageQueueRunning = true;

		AllocatePlayer();
		/*if(PhotonNetwork.isMasterClient)
			AllocateAI();*/

		#region Error detection
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			WholeGameManager.SP.NameExisted = false;
			Application.LoadLevel("Lobby-Scene");
			return;
		}
		#endregion
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		//PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
	}

	public void StopGame(bool result)
	{
		GameUIManager.SP.AdjustOverViewUI(result);
		//PhotonNetwork.automaticallySyncScene = false;
		WholeGameManager.SP.InGame = false;
		Game_Manager.SP.MyGameState = GameState.OverView;
		//OnLeftRoom();
		Application.LoadLevel("OverView-Scene");
	}

	/*public void OnGUI()
	{
		if(WholeGameManager.SP.InGame==false)
		{
			if (GUILayout.Button("Return to Lobby"))
			{
				OnLeftRoom();
			}
			if(PhotonNetwork.isMasterClient)
			{
				if (GUI.Button(new Rect(10, 10, 50, 50), "Enter"))
				{
					ForceLoadGame();
				}
			}
		}
	}*/

	public void OnLeftRoomFromOverView()
	{
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);
		if(PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.open = true;
			PhotonNetwork.room.visible = true;
		}
		PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
	}

	public void OnLeftRoom()
	{
//		Debug.Log("OnLeftRoom (local)");
		WholeGameManager.SP.InGame = false;
	//	if(Game_Manager.SP.MyGameState!=GameState.OverView)
			RemovePlayerAndRoomProperties();
	}
	#region On Photon
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		GameObject gameScript = null;
		gameScript = GameObject.Find("GameScript");
		if(gameScript!=null)
		{
			Game_Manager.SP.StopAddAllMoney();
			Destroy(GameUIManager.SP.GameUIRoot);
			Destroy(Game_Manager.SP.gameObject);
		}
			
		/*if(PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.visible = false;
			photonView.RPC("ForceAllLeaveRoom",PhotonTargets.All);
		}*/

		RemovePlayer(PhotonNetwork.player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
	}
	
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player + " : " + player.ID);
	}
	
	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		PlayerInfoData pla = GetPlayer(player);

		Debug.Log("OnPlayerDisconneced: " + player);

		CheckForceAllLeave(player.ID);
		int PlaTeam = pla.team;
		int PlaPos = pla.pos;
		TellLeft(PlaTeam,PlaPos);

		RemovePlayer(player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
		
	}
	
	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");

		RemovePlayer(PhotonNetwork.player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
	}
	#endregion
	
	#region Manage Players
	//////////////////////////////
	// Manage players

	void AddAIProperties()
	{
		int flag = 0;
		int posN = 0;
		roomProperties = PhotonNetwork.room.customProperties;

		if(!roomProperties.ContainsKey("AICount"))
		{
			roomProperties.Add("AICount",1);
			flag = 1;
		}
		else
		{ 
			flag = (int)roomProperties["AICount"];
			flag++;
			roomProperties["AICount"] = flag;
		}

		string AINumber = "NAi" + flag.ToString();
		roomProperties.Add(AINumber,flag);
		 
		teamNum[0] = (int)roomProperties["T1"];
		teamNum[1] = (int)roomProperties["T2"];
		string AIteam = "TAi" + flag.ToString();
		string AIPos = "PAi" + flag.ToString();

		if(roomProperties.ContainsKey("P1S"))
		{
			team1Seat = (bool[])roomProperties["P1S"];
			team2Seat = (bool[])roomProperties["P2S"];
		}

		if(teamNum[0]<MaxTeamNum)
		{
			roomProperties.Add(AIteam,1);
			for(int cnt = 0; cnt<team1Seat.Length;cnt++)
			{
				if(team1Seat[cnt]==false)
				{
					team1Seat[cnt] = true;
					posN = cnt;
					break;
				}
			}

			teamNum[0]++;
			roomProperties.Add(AIPos,posN);
			roomProperties["T1"] = teamNum[0];
		}
		else
		{
			roomProperties.Add(AIteam,2);
			for(int cnt = 0; cnt<team2Seat.Length;cnt++)
			{
				if(team2Seat[cnt]==false)
				{
					team2Seat[cnt] = true;
					posN = cnt;
					break;
				}
			}
	
			teamNum[1]++;
			roomProperties.Add(AIPos,posN);
			roomProperties["T2"] = teamNum[1];
		}

		roomProperties["P1S"] = team1Seat;
		roomProperties["P2S"] = team2Seat;

		string AIMoney = "MAi" + flag.ToString();
		if(!roomProperties.ContainsKey(AIMoney))
		{
			Item[] items = new Item[6];
			BuffItem[] buffs = new BuffItem[2];

			string AIExp = "EAi" + flag.ToString();
			string AIItem = "IAi" + flag.ToString();
			string AIBuff = "BFAi" + flag.ToString();
			string AIKill = "KAi" + flag.ToString();
			string AIBeKill = "BKAi" + flag.ToString();
			string AIHealthWater = "HWAi" + flag.ToString();
			string AIManaWater = "MWAi" + flag.ToString();
			string AIThrowWeapon = "THAi" + flag.ToString();
			string AIUseL = "UseLAi" + flag.ToString();
			string AIGetL = "GetLAi" + flag.ToString();
			string AIRole = "RLAi" + flag.ToString();
			string AIUseLight = "UseLAi" + flag.ToString();
			string AIGetLight = "GetLAi" + flag.ToString();
			//int role = Random.Range(1,2);
			int role = 2;

			roomProperties.Add(AIExp,0);
			roomProperties.Add(AIMoney,0);
			roomProperties.Add(AIItem,items);
			roomProperties.Add(AIKill,0);
			roomProperties.Add(AIBeKill,0);
			roomProperties.Add(AIHealthWater,0);
			roomProperties.Add(AIManaWater,0);
			roomProperties.Add(AIThrowWeapon,0);
			roomProperties.Add(AIUseLight,0);
			roomProperties.Add(AIGetLight,0);
			roomProperties.Add(AIRole,role);
			roomProperties.Add(AIBuff,buffs);
		}

		PhotonNetwork.room.SetCustomProperties(roomProperties);
	}
	
	void CheckRoomProperties()
	{
		roomProperties = PhotonNetwork.room.customProperties;
		
		#region Check if Room Properties exist
		if(!roomProperties.ContainsKey("P1S"))
		{
			roomProperties.Add("P1S",team1Seat);
			roomProperties.Add("P2S",team2Seat);
		}
		if(!roomProperties.ContainsKey("Time"))
		{
			roomProperties.Add("Time",180);
		}
		if(!roomProperties.ContainsKey("T1"))
		{
			roomProperties.Add("T1",0);
			roomProperties.Add("T2",0);
		}
		#endregion
		#region outdated method
		/*teamNum[0] = (int)roomProperties["T1"];
		teamNum[1] = (int)roomProperties["T2"];

		if(teamNum[0]<MaxTeamNum)
		{
			playerProperties.Add("T",1);

			for(int cnt = 0; cnt<MaxTeamNum;cnt++)
			{
				if(team1Seat[cnt]==false)
				{
					team1Seat[cnt] = true;
					posN = cnt;
					break;
				}
			}
			teamNum[0]++;

			playerProperties.Add("P",posN);
			roomProperties["T1"] = teamNum[0];
		}
		else
		{
			playerProperties.Add("T",2);
			for(int cnt = 0; cnt<MaxTeamNum;cnt++)
			{
				if(team2Seat[cnt]==false)
				{
					team2Seat[cnt] = true;
					posN = cnt;
					break;
				}
			}
			teamNum[1]++;

			playerProperties.Add("P",posN);
			roomProperties["T2"] = teamNum[1];
		}

		roomProperties["P1S"] = team1Seat;
		roomProperties["P2S"] = team2Seat;*/
		#endregion
		PhotonNetwork.room.SetCustomProperties(roomProperties); 
	}

	void CheckPlayerProperties()
	{
		#region Check if Player Properties exist
		if(!playerProperties.ContainsKey("M"))
		{
			Item[] items = new Item[6];
			BuffItem[] buff = new BuffItem[2];

			playerProperties.Add("T",5);
			playerProperties.Add("P",5);
			playerProperties.Add("E",0);
			playerProperties.Add("M",0);
			playerProperties.Add("BF",buff);
			playerProperties.Add("I",items);
			playerProperties.Add("K",0);
			playerProperties.Add("BK",0);
			playerProperties.Add("HW",0);
			playerProperties.Add("MW",0);
			playerProperties.Add("TH",0);
			playerProperties.Add("UseL",0);
			playerProperties.Add("GetL",0);
			playerProperties.Add("RL",0);
		}
		#endregion

		PhotonNetwork.player.SetCustomProperties(playerProperties);
	}

	public void SetLightSourceInput(int startingLightSource)
	{
		photonView.RPC("SetLightSource",PhotonTargets.AllBuffered,startingLightSource);
	}

	public void SetNoLightSourceInput(int team)
	{
		photonView.RPC("SetNoLightSource",PhotonTargets.All,team);
	}

	[RPC]
	void SetNoLightSource(int team)
	{
		if(team==1)
		{
			L_LSManager.LightSource = 0;
		}
		else
		{
			D_LSManager.LightSource = 0;
		}
	}

	[RPC]
	void SetLightSource(int startingLightSource)
	{
		L_LSManager.LightSource = startingLightSource;
		D_LSManager.LightSource = startingLightSource;
		WholeGameManager.SP.StartingLightSource = startingLightSource;
	}

	void AddPlayerAndRoomProperties()
	{
		CheckRoomProperties();
		CheckPlayerProperties();
	}

	void RemovePlayerAndRoomProperties()
	{
		photonView.RPC("RequestRemovePlayer",PhotonTargets.MasterClient,localPlayer.ID);
	}

	[RPC]
	void RequestRemovePlayer(int ClientID)
	{
		PlayerInfoData pla = GetPlayerFromID(ClientID);
		int ClientTeam = pla.team;
		int ClientPos = pla.pos;
		if(ClientTeam==1)
		{
			team1Seat[ClientPos] = false;
			teamNum[0]--;
		}
		else
		{
			team2Seat[ClientPos] = false;
			teamNum[1]--;
		}
		pla.networkPlayer.customProperties.Clear();
		photonView.RPC("RemovePlayer",PhotonTargets.All,pla.networkPlayer);
	}

	[RPC]//Add this player into PlayerList and assign this player to be localPlayer if the player isLocal
	void AddPlayer(PhotonPlayer networkPlayer, int id1, bool player, int ai)
	{		
		PlayerInfoData pla = new PlayerInfoData();
		pla.properties = new PropertiesInfoData(networkPlayer.name);
		pla.networkPlayer = networkPlayer;
		pla.ID = id1;

		if(player)
		{
			Hashtable playerProperties = networkPlayer.customProperties;
			if(playerProperties.ContainsKey("M"))
			{
				pla.properties.Money = (int)playerProperties["M"];
				pla.properties.level = 1;
				pla.properties.Exp = (int)playerProperties["E"];
				pla.properties.Buff = (int[])playerProperties["BF"];
				pla.properties.Items = (int[])playerProperties["I"];
				pla.properties.Kills = (int)playerProperties["K"];
				pla.properties.BeKills = (int)playerProperties["BK"];
				pla.properties.HealthWaterNum = (int)playerProperties["HW"];
				pla.properties.ManaWaterNum = (int)playerProperties["MW"];
				pla.properties.ThrowingWeaponNum = (int)playerProperties["TH"];
				pla.properties.UseLightSource = (int)playerProperties["UseL"];
				pla.properties.GetLightSource = (int)playerProperties["GetL"];
			}
			if (pla.IsLocal)
			{
				if (localPlayer.ID == pla.ID) { Debug.LogError("localPlayerInfo already set?"); }
				localPlayer = pla;
				if(PhotonNetwork.isMasterClient)
				{
					photonView.RPC("AssignMasterClientID",PhotonTargets.AllBuffered,networkPlayer.ID,0);
				}
			}
//			Debug.Log("AddPlayer " + networkPlayer + " : " + + pla.ID);
		}
		else
		{
			int flag = 0;
			/*if(ai==0)
				flag = (int)roomProperties["AICount"];
			else*/
			roomProperties = PhotonNetwork.room.customProperties;
				flag = ai;
			pla.properties.Ai = flag;

			Debug.Log(flag);
			string AIMoney = "MAi" + flag.ToString();
	//string AILevel = "LAi" + flag.ToString();
			string AIExp = "EAi" + flag.ToString();
			string AIBuff = "BFAi" + flag.ToString();
			string AIItem = "IAi" + flag.ToString();
			string AIKill = "KAi" + flag.ToString();
			string AIBeKill = "BKAi" + flag.ToString();
			string AIHealthWater = "HWAi" + flag.ToString();
			string AIManaWater = "MWAi" + flag.ToString();
			string AIThrowWeapon = "THAi" + flag.ToString();
			string AIUseLight = "UseLAi" + flag.ToString();
			string AIGetLight = "GetLAi" + flag.ToString();
			string AIRole = "RLAi" + flag.ToString();
			string AIteam = "TAi" + flag.ToString();
			string AIPos = "PAi" + flag.ToString();

			pla.properties.Name = "PC" + flag.ToString();
			pla.properties.Money = (int)roomProperties[AIMoney];
			pla.properties.Exp = (int)roomProperties[AIExp];
			pla.properties.level = 1;
			pla.properties.Items = (int[])roomProperties[AIItem];
			pla.properties.Buff = (int[])roomProperties[AIBuff];
			pla.properties.Kills = (int)roomProperties[AIKill];
			pla.properties.BeKills = (int)roomProperties[AIBeKill];
			pla.properties.HealthWaterNum = (int)roomProperties[AIHealthWater];
			pla.properties.ManaWaterNum = (int)roomProperties[AIManaWater];
			pla.properties.ThrowingWeaponNum = (int)roomProperties[AIThrowWeapon];
			pla.properties.UseLightSource = (int)roomProperties[AIUseLight];
			pla.properties.GetLightSource = (int)roomProperties[AIGetLight];
			pla.roleType = (int)roomProperties[AIRole];
			pla.team = (int)roomProperties[AIteam];
			pla.pos = (int)roomProperties[AIPos];
		}
		playerList.Add(pla);
	}

	//Set player's Transform
	void SetPlayerTransform(PhotonPlayer networkPlayer, Transform pTransform)
	{
		if (!pTransform)
		{
			Debug.LogError("SetPlayersTransform has a NULL playerTransform!");
		}
		PlayerInfoData thePlayer = GetPlayer(networkPlayer);
		if (thePlayer == null)
		{
			Debug.LogError("SetPlayersPlayerTransform: No player found! " + networkPlayer + "  " + pTransform + " trans=" + playerList.Count);
		}
		thePlayer.transform = pTransform;

		if (thePlayer.IsLocal)
		{
			localPlayer = thePlayer;
		}
	}
	

	[RPC]//Destroy all the player's Network GOs, Photoview and RPCs if he is master client, also destroy local GO if it's existed, then remove this player from PlayerList
	void RemovePlayer(PhotonPlayer networkPlayer)
	{
		if(GetPlayer(networkPlayer)!=null)
		{
			PlayerInfoData thePlayer = GetPlayer(networkPlayer);

			int playerTeam = thePlayer.team;
			int pos = thePlayer.pos;

			if(!WholeGameManager.SP.InGame)
			{
				if(StartGame)
				{
					StartGame = false;
				}
				if(playerTeam==1)
				{
					team1Seat[pos] = false;
					team1Ready[pos] = false;
				}
				else
				{
					team2Seat[pos] = false;
					team2Ready[pos] = false;
				}
				roomUIManager.roomUIRoleBase[8+pos+(playerTeam-1)*MaxTeamNum].SetActive(false);
			}

			if (thePlayer.transform)
			{
				Destroy(thePlayer.transform.gameObject);
			}

			if(!networkPlayer.isLocal)
			{
				int team = (int)playerProperties["T"];
				if(thePlayer.team==team)
				{
					GameUIManager.SP.ManageTeammateInfo(thePlayer.ID,false,0,0,null);
				}
			}

			playerList.Remove(thePlayer);
			if(thePlayer==localPlayer)
			{
				localPlayer.networkPlayer = null;
				localPlayer.transform = null;
				localPlayer.ID = 0;
			}
		}
		if(networkPlayer.isLocal)
		{
			WholeGameManager.SP.EnableLobby(gameObject);
			if(PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.open = true;
				PhotonNetwork.room.visible = true;
			}
			PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
		}
	}

	public void OnCheckPing()
	{
		InvokeRepeating("CheckPing",0.01f,1f);
	}

	void CheckPing()
	{
		if(localPlayer.transform!=null)
		{
			int ping = PhotonNetwork.GetPing();
			photonView.RPC("CheckPingRPC",PhotonTargets.All,localPlayer.ID,ping);
		}
	}

	[RPC]
	void CheckPingRPC(int id, int ping)
	{
		PlayerInfoData pla = GetPlayerFromID(id);
		pla.ping = ping;
	}

	public void AssignTeamNumInput()
	{
		photonView.RPC("AssignTeamNum",PhotonTargets.AllBuffered,teamNum);
	}

	[RPC]
	void AssignTeamNum(int[] MCteamNum)
	{
		teamNum = MCteamNum;
	}
	
	void TellLeft(int team,int pos)
	{
		teamNum[team-1]--;
		if(!WholeGameManager.SP.InGame)
		{
			if(StartGame)
			{
				StartGame = false;
			}
			if(team==1)
			{
				team1Seat[pos] = false;
				team1Ready[pos] = false;
			}
			else
			{
				team2Seat[pos] = false;
				team2Ready[pos] = false;
			}
			roomUIManager.roomUIRoleBase[8+pos+(team-1)*MaxTeamNum].SetActive(false);
		}
		else
		{
			if(Game_Manager.SP.MyGameState!=GameState.OverView)
			{	
				if(teamNum[team-1]==0)
				{
					if(team==1)
					{
						L_LSManager.LightSource = 0;
						GameUIManager.SP.HudWarningLabel.Add("The game is over because the Britorn just conceded.",Color.red,5);
					}
					else
					{
						D_LSManager.LightSource = 0;
						GameUIManager.SP.HudWarningLabel.Add("The game is over because the Shadorn just conceded.",Color.red,5);
					}
				}
			}
			int who = pos + (team-1)*4;
			GameUIManager.SP.LeftUI[who].SetActive(true);
		}
	}
	
	void CheckForceAllLeave(int who)
	{
		if(WholeGameManager.SP.InGame)
		{
			if(who==MasterClientID)
			{
				if(MasterClientTeam==1)
					L_LSManager.LightSource = 0;
				else
					D_LSManager.LightSource = 0;
				GameUIManager.SP.HudWarningLabel.Add("The game is over because the room master just conceded and left room.",Color.red,5);
			}	
		}
		else
		{
			if(Game_Manager.SP)
			{
				if(Game_Manager.SP.MyGameState!=GameState.OverView)
				{
					if(who==MasterClientID)
					{
						WholeGameManager.SP.MCLeftRoomWarning = true;
						OnLeftRoomFromOverView();
					}
				}
			}
			else
			{
				if(who==MasterClientID)
				{
					WholeGameManager.SP.MCLeftRoomWarning = true;
					OnLeftRoomFromOverView();
				}
			}
		}
	}

	[RPC]
	void RemovePlayerFromID(int id)
	{
		PlayerInfoData thePlayer = GetPlayerFromID(id);
		if(thePlayer.transform)
		{	
			if(thePlayer.properties.Ai>=0)
				thePlayer.transform.GetComponent<TP_Info>().OffHDTName();
			Destroy(thePlayer.transform.gameObject);
		}
			
		if(thePlayer==localPlayer)
		{
			localPlayer.networkPlayer = null;
			localPlayer.transform = null;
			localPlayer.ID = 0;
			localPlayer.roleType = 0;
		}
		else
		{
			if(!thePlayer.networkPlayer.isLocal)
			{
				int team = (int)playerProperties["T"];
				if(thePlayer.team==team)
				{
					GameUIManager.SP.ManageTeammateInfo(id,false,0,0,null);
				}
			}
		}
		playerList.Remove(thePlayer);
	}

	[RPC]
	void RequestTeamAndPos(int ClientID)
	{
		StartCoroutine(WaitForAssignTeamPos(ClientID));
	}

	IEnumerator WaitForAssignTeamPos(int ClientID)
	{
		while(MasterAssignTPLock)
		{
			yield return new WaitForSeconds(0.005f);
		}

		AssignTeamPos(ClientID);
	}

	IEnumerator WaitForAssignChangeTeam(int ClientID, int teamToChange)
	{
		while(MasterAssignTPLock)
		{
			yield return new WaitForSeconds(0.005f);
		}
		AssignChangeTeam(ClientID, teamToChange);
	}

	IEnumerator WaitForAssignChangeRole(int ClientID, int roleToChange)
	{
		while(MasterAssignTPLock)
		{
			yield return new WaitForSeconds(0.005f);
		}
		AssignChangeRole(ClientID, roleToChange);
	}

	void AssignChangeRole(int ClientID, int roleToChange)
	{
		MasterAssignTPLock = true;

		photonView.RPC("RespondChangeRole",PhotonTargets.AllBuffered, ClientID, roleToChange);
	}

	void AssignChangeTeam(int ClientID, int teamToChange)
	{
		MasterAssignTPLock = true;
		
		PlayerInfoData pla = GetPlayerFromID(ClientID);
		int team = pla.team;
		int pos = pla.pos;
		int role = pla.roleType;
		bool NeedChangeTeam = false;
		int clientTeam = 5;
		int clientPos = 5;
		
		
		if(teamToChange==1)
		{
			if(team==2&&teamNum[0]<4)
			{
				//InRoom_Menu.SP.playerProperties["T"] = 1;
				teamNum[0]++;
				teamNum[1]--;
				
				for(int cnt=0;cnt<InRoom_Menu.MaxTeamNum;cnt++)
				{
					if(team1Seat[cnt]==false)
					{
						team2Seat[pos] = false;
						team1Seat[cnt] = true;
						clientTeam = teamToChange;
						clientPos = cnt;
						NeedChangeTeam = true;
						break;
					}
				}
			}
		}
		else if(teamToChange==2)
		{
			if(team==1&&teamNum[1]<4)
			{
				//InRoom_Menu.SP.playerProperties["T"] = 2;
				teamNum[0]--;
				teamNum[1]++;
				
				for(int cnt=0;cnt<InRoom_Menu.MaxTeamNum;cnt++)
				{
					if(team2Seat[cnt]==false)
					{
						team1Seat[pos] = false;
						team2Seat[cnt] = true;
						clientTeam = teamToChange;
						clientPos = cnt;
						NeedChangeTeam = true;
						break;
					}
				}
			}
		}
	
		if(NeedChangeTeam)
		{
			photonView.RPC("RespondChangeTeam",PhotonTargets.AllBuffered, ClientID, clientTeam, clientPos, role);
		}
	}

	void AssignTeamPos(int ClientID)
	{
		MasterAssignTPLock = true;
		
		int clientTeam = 5;
		int clientPos = 5;
		
		if(teamNum[0]<MaxTeamNum)
		{
			for(int cnt = 0; cnt<MaxTeamNum;cnt++)
			{
				if(team1Seat[cnt]==false)
				{
					team1Seat[cnt] = true;
					clientTeam = 1;
					clientPos = cnt;
					break;
				}
			}
			teamNum[0]++;
		}
		else
		{
			for(int cnt = 0; cnt<MaxTeamNum;cnt++)
			{
				if(team2Seat[cnt]==false)
				{
					team2Seat[cnt] = true;
					clientTeam = 2;
					clientPos = cnt;
					break;
				}
			}
			teamNum[1]++;
		}
		photonView.RPC("RespondTeamAndPos",PhotonTargets.AllBuffered,ClientID,clientTeam,clientPos);
	}

	[RPC]
	void RespondTeamAndPos(int ClientID,int ClientTeam, int ClientPos)
	{
		if(GetPlayerFromID(ClientID)!=null)
		{
			PlayerInfoData pla = GetPlayerFromID(ClientID);
			
			pla.team = ClientTeam;
			pla.pos = ClientPos;

			//Update player Role UI
			roomUIManager.UpdatePlayerRoleUI(ClientTeam,ClientPos,0,pla.name);

			if(PhotonNetwork.isMasterClient)
			{
				Hashtable clientProperties = pla.networkPlayer.customProperties;
				clientProperties["T"] = ClientTeam;
				clientProperties["P"] = ClientPos;
				pla.networkPlayer.SetCustomProperties(clientProperties);
				MasterAssignTPLock = false;
			}
		}
	}

	[RPC]
	void RespondChangeRole(int ClientID, int role)
	{
		#region Get playerTeam And Pos, Set player Role 
		PlayerInfoData pla = GetPlayerFromID(ClientID);

		pla.roleType = role;
		#endregion
		
		if(PhotonNetwork.isMasterClient)
		{
			Hashtable clientProperties = pla.networkPlayer.customProperties;
			clientProperties["RL"] = role;
			pla.networkPlayer.SetCustomProperties(clientProperties);
			MasterAssignTPLock = false;
		}
	}

	[RPC]
	void RespondChangeTeam(int ClientID,int ClientTeam, int ClientPos, int role)
	{
		#region Set playerTeam And Pos
		PlayerInfoData pla = GetPlayerFromID(ClientID);

		pla.team = ClientTeam;
		pla.pos = ClientPos;

		#endregion

		if(PhotonNetwork.isMasterClient)
		{
			Hashtable clientProperties = pla.networkPlayer.customProperties;
			clientProperties["T"] = ClientTeam;
			clientProperties["P"] = ClientPos;
			clientProperties["RL"] = role;
			pla.networkPlayer.SetCustomProperties(clientProperties);
			MasterAssignTPLock = false;
		}
	}

	[RPC]
	public void RequestChangeTeam(int ClientID, int teamToChange)
	{
		StartCoroutine(WaitForAssignChangeTeam(ClientID,teamToChange));
	}

	[RPC]
	public void RequestChangeRole(int ClientID, int roleToChange)
	{
		StartCoroutine(WaitForAssignChangeRole(ClientID,roleToChange));
	}

	//Send Back this networkPlayer in the PlayerList
	public PlayerInfoData GetPlayer(PhotonPlayer networkPlayer)
	{
		foreach (PlayerInfoData pla in playerList)
		{
			if (pla.networkPlayer == networkPlayer)
			{
				return pla;
			}
		}
		return null;
	}

	public PlayerInfoData GetPlayerFromID(int id)
	{
		foreach(PlayerInfoData pla in playerList)
		{
			if(pla.ID == id)
			{
				return pla;
			}
		}
		return null;
	}
	
	//When a PhotonView instantiates it has viewID=0 and is unusable.
	//We need to assign the right viewID -on all players(!)- for it to work
	void SetPhotonViewIDs(GameObject go, int id1)
	{
		PhotonView[] nViews = go.GetComponentsInChildren<PhotonView>();
		nViews[0].viewID = id1;
		//if(nViews[0].isMine)
//			Debug.Log(nViews[0].viewID + "isMine");
	}

	[RPC]
	void AssignMasterClientID(int id,int team)
	{
		if(id!=0)
			MasterClientID = id;
		if(team!=0)
		{
			if(MasterClientTeam!=0)
				MasterClientTeam = team;
		}
	}
	#endregion

	#region Allocate and Spawn Players
	void AllocatePlayer()
	{
		myID = PhotonNetwork.AllocateViewID();

		AddPlayerAndRoomProperties();

		photonView.RPC("AddPlayer", PhotonTargets.AllBuffered, PhotonNetwork.player, myID, true, 0);

		if(PhotonNetwork.isNonMasterClientInRoom)
		{
			photonView.RPC("RequestTeamAndPos",PhotonTargets.MasterClient,myID);
		}
		else
		{
			AssignTeamPos(myID);
		}

		//StartCoroutine(SpawnMyRoomPlayer(true));
	}



	public IEnumerator SpawnMyRoomPlayer(int team, int role)
	{
		while(localPlayer.team!=1&&localPlayer.team!=2)
		{
			yield return new WaitForSeconds(0.1f);
		}
		#region playerCharacterUI
		Transform chooseLocker = roomUIManager.RoleUIPrefab[6].transform;
		chooseLocker.parent = roomUIManager.roomUIButton[role+2 + (team-1)*3].transform;
		chooseLocker.localPosition = Vector3.zero;

		if(team==1)
		{
			myPlayer = Instantiate(LightPlayerPrefab[role], myTransform.position, Quaternion.identity) as Transform;
		}
		else
		{
			myPlayer = Instantiate(DarkPlayerPrefab[role], myTransform.position, Quaternion.identity) as Transform;
		}
		if(team==2&&role==0)
			myPlayer.localScale = new Vector3(0.03f,0.03f,0.03f);
		else
			myPlayer.localScale = new Vector3(0.7f,0.7f,0.7f);
		WholeGameManager.SP.ChangeLayersRecursively(myPlayer,"UI");
		myPlayer.parent = roomUIManager.PrefabAnchor.transform;
		myPlayer.localPosition = Vector3.zero;
		if(team==2&&role==1)
			myPlayer.localPosition += new Vector3(-40,0,0);
		myPlayer.Rotate(new Vector3(0,180,0));
		#endregion
	}

	/*[RPC]
	public void SpawnRoomPlayerUI(PhotonPlayer np, int team, int pos, int role)
	{
		//Set chosen data into the player's playerInfoData
		PlayerInfoData thePlayer = GetPlayer(np);

		//Update player Role UI
		roomUIManager.UpdatePlayerRoleUI(team,pos,role,thePlayer.name);
	}*/

	public void AllocateAI()
	{
		/*for(int cnt = 1;cnt<=8;cnt++)
		{
			if(cnt<=4)
			{
				if(!team1Seat[cnt-1])
				{
					DoAllocateAI(cnt-1);
				}
			}
			else
			{
				if(!team2Seat[cnt-5])
				{
					DoAllocateAI(cnt-1);
				}
			}
		}*/
	}

	void DoAllocateAI(int flag)
	{
		int aiID = PhotonNetwork.AllocateViewID();
		
		AddAIProperties();
		
		photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.player, aiID, false, flag);
		
		photonView.RPC("SpawnRoomAIUI", PhotonTargets.All, aiID);
	}

	[RPC]
	public void SpawnRoomAIUI(int aiID)
	{
		//Transform newAI = Instantiate(aIPrefabs[0],pos,rot) as Transform;
		//AiListGo.Add(newAI);
	
		PlayerInfoData pla = GetPlayerFromID(aiID);

		int team = pla.team;
		int pos = pla.pos;
		int role = pla.roleType;
		string aiName = "PC " + pla.properties.Ai.ToString();
		//Update player Role UI
		roomUIManager.UpdatePlayerRoleUI(team,pos,role,aiName);
		//pla.transform = newAI;

		//Set photonviewID everywhere!
		//SetPhotonViewIDs(newAI.gameObject, aiID);
	}

	public void SpawnPlayer()
	{
		Transform[] spawnPoints;
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		SPScript = GameObject.FindGameObjectWithTag("SpawnPoints").GetComponent<SpawnPoints>();
		playerProperties = PhotonNetwork.player.customProperties;
		if(playerProperties.ContainsKey("T")&&playerProperties.ContainsKey("P"))
		{
			int myTeam = (int)playerProperties["T"];
			int myPos = (int)playerProperties["P"];
			int myRole = (int)playerProperties["RL"];

			if(myTeam==1)
			{
				spawnPoints = SPScript.team1SpawnPoints;
			}
			else
			{
				spawnPoints = SPScript.team2SpawnPoints;
			}
			Transform theGO = spawnPoints[myPos];
			pos = theGO.position;
			rot = theGO.rotation;
			photonView.RPC("SpawnGamePlayer",PhotonTargets.AllBuffered, PhotonNetwork.player, 0, pos, rot, myTeam, myRole, myPos);
			if(PhotonNetwork.isMasterClient)
				photonView.RPC("AssignMasterClientID",PhotonTargets.AllBuffered,0,myTeam);
		}
		else
		{
			Debug.Log("You Dont Have Team and Position");
		}
	}

	public void SetPlayerInfoID()
	{
		int noTrans = 0;
		foreach(PlayerInfoData pla in playerList)
		{
			if(pla.transform!=null)
			{
				TP_Info plaInfo = pla.transform.GetComponent<TP_Info>();
				plaInfo.ID = pla.ID;
			}
			else
			{
				noTrans++;
			}
		}
		if(noTrans==0)
		{
			Game_Manager.SP.SetAllPlayerInfoID = true;
		}
	}

	public void AllocateAndSpawnMonster()
	{
		for(int cnt = 1;cnt<=6;cnt++)
		{
			if(cnt>4)
			{

				//for(int cnt1 = 1; cnt1<=3;cnt1++)
				//{
					int aiID = PhotonNetwork.AllocateViewID();
					int type = 1;
			
					/*if(cnt1>1)
						type=-1;*/
					photonView.RPC("AddMonster", PhotonTargets.AllBuffered, PhotonNetwork.player, aiID , type, cnt,-(1+(cnt-1)*3));
				//}
			}
			Game_Manager.SP.WildExist[cnt-1] = true;
		}
	}

	public void RespawnMonster(int pos)
	{
		//for(int cnt = 1;cnt<=3;cnt++)
		//{
			int aiID = PhotonNetwork.AllocateViewID();
			int type = 1;
			
			//if(cnt>1)
			//	 type=-1;

			bool monsterExist = false;
			foreach(PlayerInfoData pla in playerList)
			{
				if(-pla.properties.Ai==(1+pos*3))
				{
					monsterExist = true;
				}
			}
			if(!monsterExist)
			{
				photonView.RPC("AddMonster", PhotonTargets.All, PhotonNetwork.player, aiID , type, pos+1,-(1+pos*3));
			}
		//}
		Game_Manager.SP.WildExist[pos] = true;
	}

	public void SpawnAI()
	{
		Transform[] spawnPoints;
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		SPScript = GameObject.FindGameObjectWithTag("SpawnPoints").GetComponent<SpawnPoints>();
		roomProperties = PhotonNetwork.room.customProperties;
		int aiCount = (int)roomProperties["AICount"];
		for(int cnt = 1;cnt<=aiCount-1;cnt++)
		{
			string AITeam = "TAi" + cnt.ToString();
			string AIPos = "PAi" + cnt.ToString();
			string AIRole = "RLAi" + cnt.ToString();

			if(roomProperties.ContainsKey(AITeam) && roomProperties.ContainsKey(AIPos))
			{
				int myTeam = (int)roomProperties[AITeam];
				int myPos = (int)roomProperties[AIPos];
				int myRole = (int)roomProperties[AIRole];

				if(myTeam==1)
				{
					spawnPoints = SPScript.team1SpawnPoints;
				}
				else
				{
					spawnPoints = SPScript.team2SpawnPoints;
				}
				Transform theGO = spawnPoints[myPos];
				pos = theGO.position;
				rot = theGO.rotation;
				photonView.RPC("SpawnGamePlayer",PhotonTargets.AllBuffered, PhotonNetwork.player, cnt , pos, rot, myTeam, myRole, myPos);
			}
		}
	}

	[RPC]//Add this player into PlayerList and assign this player to be localPlayer if the player isLocal
	void AddMonster(PhotonPlayer networkPlayer, int id1, int type, int wildPos, int flag)
	{	
		PlayerInfoData pla = new PlayerInfoData();
		pla.properties = new PropertiesInfoData(networkPlayer.name);
		pla.networkPlayer = networkPlayer;
		pla.ID = id1;
		
		pla.properties.Name = "Monster";
	
		pla.properties.Ai = flag;
		pla.roleType = type;
		pla.pos = wildPos;
		
		playerList.Add(pla);

		//Instantiate the character that player chose
		Transform clone = null;
		if(type==1)
		{
			clone = monsterPrefabs[0];
		}
		else if(type==-1)
		{
			clone = monsterPrefabs[1];
		}

		Transform monster = Instantiate(clone, Vector3.zero, Quaternion.identity) as Transform;
		Transform[] spawnPoints = Game_Manager.SP.WildMonsterAnchor;
		Transform theGO = spawnPoints[wildPos-1];
		monster.parent = theGO;
		monster.localPosition = Vector3.zero;
		monster.localRotation = Quaternion.identity;
		if(monster.parent.parent.name=="Wild_M2")
			monster.Rotate(new Vector3(0,-180,0));

		int monsterStatus = (-flag)%3;//Boss or little Monster
		if(monsterStatus==2)
		{
			monster.localPosition += new Vector3(3,0,2);
			monster.Rotate(new Vector3(0,-45,0));
			monster.localScale = Vector3.one;
		}
		else if(monsterStatus==0)
		{
			monster.localPosition += new Vector3(-3,0,2);
			monster.Rotate(new Vector3(0,45,0));
			monster.localScale = Vector3.one;
		}
		else
			monster.localScale = Vector3.one*2;
		
		//Get the player's playerInfoData
		PlayerInfoData theMonster = null;
	
		pla.transform = monster.transform;
		pla.properties.level = monster.GetComponent<TP_Info>().Level;
		pla.bloodType = DecideBloodType();
		//Set photonviewID everywhere!
		SetPhotonViewIDs(monster.gameObject, id1);
		
		//Open player's necessary scripts
		OpenPlayerScripts(pla,0,0);
	}

	[RPC]
	public void SpawnGamePlayer(PhotonPlayer np,int ai, Vector3 pos, Quaternion rot, int myTeam, int role, int myPos)
	{
		//Instantiate the character that player chose
		Transform clone = null;
		if(ai==0)
		{
			if(myTeam==1)
				clone = LightPlayerPrefab[role];
			else
				clone = DarkPlayerPrefab[role];
		}
		else
		{
			if(myTeam==1)
				clone = aILightPrefabs[role];
			else
				clone = aIDarkPrefabs[role];
		}
		myPlayer = Instantiate(clone, pos, rot) as Transform;
	

		//Get the player's playerInfoData
		PlayerInfoData thePlayer = null;

		if(ai==0)
		{
			thePlayer = GetPlayer(np);
			//Set transform
			SetPlayerTransform(np,myPlayer);
		}
		else
		{
			foreach(PlayerInfoData pla in playerList)
			{
				if(pla.properties.Ai==ai)
				{
					thePlayer = pla;
					break;
				}
			}

			thePlayer.transform = myPlayer.transform;
		}

		thePlayer.team = myTeam;
		thePlayer.roleType = role;
		thePlayer.pos = myPos;


		thePlayer.bloodType = DecideBloodType();
		//Set photonviewID everywhere!
		SetPhotonViewIDs(myPlayer.gameObject, thePlayer.ID);

		//Open player's necessary scripts
		OpenPlayerScripts(thePlayer,myTeam,0);
		if(thePlayer!=localPlayer&&!thePlayer.networkPlayer.isLocal&&thePlayer.properties.Ai>=0)
		{
			int team = (int)playerProperties["T"];
			if(thePlayer.team==team)
			{
				GameUIManager.SP.ManageTeammateInfo(thePlayer.ID,true,myTeam,role,thePlayer.name);
			}
		}
	}

	int DecideBloodType()
	{
		int bloodNum = 0;
		for(int num = 0; num < playerList.Count; num++)
		{
			bool noOwner = true;
			foreach(PlayerInfoData pla in playerList)
			{
				if(pla.bloodType==num)
				{
					noOwner = false;
				}
				if(!noOwner)
					break;
			}
			if(noOwner)
			{
				bloodNum = num;
				break;
			}
		}

		return bloodNum;
	}

	public void UseLightSourceToReviveInput(int myTeam,int id)
	{
		photonView.RPC("UseLightSourceToRevive",PhotonTargets.All,myTeam,-30,id);
	}

	void HeadHudSwitch(bool hud,int localTeam, Camera cam)
	{
		foreach(PlayerInfoData plas in playerList)
		{
			if(plas.team == localTeam)
			{
				if(plas.properties.Ai>=0)
				{
					if(plas.transform)
					{
						if(hud)
						{
							plas.transform.GetComponent<TP_Info>().HeadNameHUD.gameObject.SetActive(true);
							plas.transform.GetComponent<TP_Info>().HeadNameHUD.transform.GetComponent<UIFollowTarget>().gameCamera = cam;
						}
						else
							plas.transform.GetComponent<TP_Info>().HeadNameHUD.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void DoDeadPlayer()
	{
		int id = PhotonNetwork.AllocateViewID();
		int localTeam = (int)playerProperties["T"];
		HeadHudSwitch(false,localTeam,null);
		photonView.RPC("RemovePlayerFromID",PhotonTargets.All,localPlayer.ID);
		photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.player, id, true, 0);
		
		//photonView.RPC("GiveTeamAndPos",PhotonTargets.AllBuffered, id, GameUIManager.SP.myTeam, GameUIManager.SP.myPos);
	}

	public void RespawnPlayer(int myTeam, int myPos, int oldID, int ai, int myRole, int myExp)
	{
		Transform[] spawnPoints;
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		int id = 0;
		
		if(myTeam==1)
		{
			spawnPoints = SPScript.team1SpawnPoints;
		}
		else
		{
			spawnPoints = SPScript.team2SpawnPoints;
		}
		Transform theGO = spawnPoints[myPos];
		pos = theGO.position;
		rot = theGO.rotation;

		GameUIManager.SP.UpdateGameSkillUI(myTeam,myRole);

		if(ai==0)
		{
			playerProperties = PhotonNetwork.player.customProperties;
			playerProperties["RL"] = myRole;
			PhotonNetwork.player.SetCustomProperties(playerProperties);
			id = localPlayer.ID;
			photonView.RPC("RespawnGamePlayer", PhotonTargets.All, pos, rot, id, PhotonNetwork.player, 0, myTeam, myPos, myRole, myExp);
		}
		else
		{
			id = PhotonNetwork.AllocateViewID();
			photonView.RPC("RemovePlayerFromID",PhotonTargets.All,oldID);
			photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.player, id, false, ai);
			photonView.RPC("RespawnGamePlayer", PhotonTargets.All, pos, rot, id, PhotonNetwork.player, ai, myTeam, myPos, myRole,myExp);
		}
	}

	public void DoDeadMonster(int ID)
	{
		photonView.RPC("RemovePlayerFromID",PhotonTargets.All,ID);
	}

	[RPC]
	void GiveTeamAndPos(int id, int myTeam, int myPos)
	{
		PlayerInfoData thePlayer = GetPlayerFromID(id);
		
		thePlayer.team = myTeam;
		thePlayer.pos = myPos;
	}

	[RPC]
	void RespawnGamePlayer(Vector3 pos, Quaternion rot, int id, PhotonPlayer np, int ai, int myTeam, int myPos, int myRole, int myExp)
	{
		Transform clone = null;

		PlayerInfoData thePlayer = GetPlayerFromID(id);

		if(ai==0)
		{
			if(myTeam==1)
			{
				clone = LightPlayerPrefab[myRole];
			}
			else
			{
				clone = DarkPlayerPrefab[myRole];
			}

			Transform newPlayer = Instantiate(clone, pos, rot) as Transform;
			thePlayer.transform = newPlayer.transform;


			if (thePlayer.IsLocal)
			{
				localPlayer = thePlayer;
			}

		
			//Set photonviewID everywhere!
			SetPhotonViewIDs(newPlayer.gameObject, id);
		}
		else
		{
			Debug.Log("RespawnRole:" + myRole);
			Transform newAI = Instantiate(aILightPrefabs[1],pos,rot) as Transform;
		//	GameObject newAI = PhotonNetwork.InstantiateSceneObject("Theia AI",pos,rot,0,null) as GameObject;
			thePlayer.transform = newAI.transform;

			//Set photonviewID everywhere!
			SetPhotonViewIDs(newAI.gameObject, id);
		}

		thePlayer.team = myTeam;
		thePlayer.roleType = myRole;
		thePlayer.pos = myPos;
		thePlayer.properties.Exp = myExp;
		thePlayer.bloodType = DecideBloodType();
		OpenPlayerScripts(thePlayer,myTeam,myExp);

		if(thePlayer!=localPlayer&&!thePlayer.networkPlayer.isLocal&&thePlayer.properties.Ai>=0)
		{
			int team = (int)playerProperties["T"];
			if(thePlayer.team==team)
			{
				GameUIManager.SP.ManageTeammateInfo(id,true,myTeam,myRole,thePlayer.name);
			}
		}
	}
	
	#endregion

	#region Dealing with properties 

	public void AddAllPlayerMoney(int money)
	{
		playerProperties = PhotonNetwork.player.customProperties;
		int myMoney = (int)playerProperties["M"];
		myMoney += money;
		playerProperties["M"] = myMoney;
		PhotonNetwork.player.SetCustomProperties(playerProperties);

		//AddAIMONEY

//		Debug.Log("Money" + (int)playerProperties["M"]);
		photonView.RPC("AddMoney", PhotonTargets.AllBuffered,-1,money);
	}

	public void AddPlayerMoney(int ID,int money)
	{
		playerProperties = PhotonNetwork.player.customProperties;
		int myMoney = (int)playerProperties["M"];
		myMoney += money;
		playerProperties["M"] = myMoney;
		PhotonNetwork.player.SetCustomProperties(playerProperties);

		photonView.RPC("AddMoney", PhotonTargets.All,ID,money);
	}

	public void AddAIMoney(int ID,int money)
	{
		/*playerProperties = PhotonNetwork.player.customProperties;
		int myMoney = (int)playerProperties["M"];
		myMoney += money;
		playerProperties["M"] = myMoney;
		PhotonNetwork.player.SetCustomProperties(playerProperties);*/
		
		photonView.RPC("AddMoney", PhotonTargets.All,ID,money);
	}

	[RPC]
	void AddMoney(int id, int money)
	{
		if(id==-1)
		{
			foreach(PlayerInfoData pla in playerList)
			{
				pla.properties.Money += money;
			}
		}
		else
		{
			foreach(PlayerInfoData pla in playerList)
			{
				if(pla.ID==id)
				{
					pla.properties.Money += money;
					if(pla.IsLocal)
					{
						TP_Info myInfo = pla.transform.GetComponent<TP_Info>();
						string str = "Money +" + money.ToString();

						if(!myInfo.isAI)
							GameUIManager.SP.HudRewardLabel.Add(str,Color.yellow,1f);
					}
				}
			}
		}
	}

	/*[RPC] void BuyItem(int id, )
	{

	}*/

	#endregion

	void OpenPlayerScripts(PlayerInfoData pla, int myTeam, int myExp)
	{
		Transform myPlayer = pla.transform;
		NetworkController playerNetworkController = myPlayer.GetComponent<NetworkController>();
		PhotonView playerPhotonView = myPlayer.GetComponent<PhotonView>();
		TP_Controller playerController = myPlayer.GetComponent<TP_Controller>();
		TP_Animator playerAnimator = myPlayer.GetComponent<TP_Animator>();
		TP_Motor playerMotor = myPlayer.GetComponent<TP_Motor>();
		TP_Info playerInfo = myPlayer.GetComponent<TP_Info>();
		NJGMapItem mapItem = myPlayer.GetComponent<NJGMapItem>();

		if(pla.properties.Ai>0)
		{
			if(PhotonNetwork.isMasterClient)
			{
				AIScript playerAI = myPlayer.GetComponent<AIScript>();
				playerAI.enabled = true;
			
				playerAI.aiRig = myPlayer.GetComponentInChildren<AIRig>();
				playerAI.aiRig.enabled = true;
				playerAI.ettRig = myPlayer.FindChild("PlayerEntity").GetComponent<EntityRig>();
				playerAI.AssignTeam(myTeam);
			}
		}
		else if(pla.properties.Ai<0)
		{
			if(PhotonNetwork.isMasterClient)
			{
				MonsterScript monsterAI = myPlayer.GetComponent<MonsterScript>();
				monsterAI.enabled = true;
				
				monsterAI.aiRig = myPlayer.GetComponentInChildren<AIRig>();
				monsterAI.aiRig.enabled = true;
			}
		}

		playerPhotonView.observed = playerNetworkController;
		playerNetworkController.enabled = true;
		playerController.enabled = true;
		playerAnimator.enabled = true;
		playerMotor.enabled = true;
		playerInfo.enabled = true;
		GameObject playerAttack = myPlayer.FindChild("AttackRangeCol").gameObject;
		playerAttack.SetActive(true);
		playerController.OnPlayerAttack();
		playerAnimator.OnPlayerAttack();

		if(myTeam==1)
		{
			myPlayer.tag = "team1";
			myPlayer.gameObject.layer = 11;
		}
		else if(myTeam==2)
		{
			myPlayer.tag = "team2";
			myPlayer.gameObject.layer = 12;
		}
		else if(myTeam==0)
		{
			myPlayer.tag = "monster";
		}

		playerInfo.Team = myTeam;

		playerInfo.AddExp(myExp);
		playerInfo.playerListNum = pla.bloodType;
		int localTeam = (int)playerProperties["T"];
		if(myTeam==localTeam)
		{
			if(pla.properties.Ai==0)
			{
				playerInfo.Name = pla.name;
				playerInfo.OnHDTName();
			}
			else if(pla.properties.Ai>0)
			{
				playerInfo.Name = playerInfo.Role.ToString();
				playerInfo.OnHDTName();
			}
		}

		if(pla.ID==localPlayer.ID)
		{
			GameObject playerCamera = myPlayer.FindChild("Camera").gameObject;// transform.FindChild("Camera").gameObject;

			playerCamera.SetActive(true);

			playerCamera.transform.parent = myPlayer.FindChild("_TargetLookAtAnchor/targetLookAt").transform;

			TP_Camera playerCam = playerCamera.GetComponent<TP_Camera>();

			playerCam.playerAnimator = playerAnimator;

			playerCam.playerMotor = playerMotor;

			playerAnimator.playerCam = playerCam;

			playerController.SetIsLocalPlayer(true);

			StartCoroutine(playerInfo.ShowAim(myTeam));

			mapItem.type = 1;

			HeadHudSwitch(true,localTeam,playerCamera.GetComponent<Camera>());

		}
		else if(pla.team==localPlayer.team)
		{
			mapItem.type = 7;
		}
	}

	//True if it is AI but is not masterclient's AI
	public bool isNotMasterClinetAI(int myID)
	{
		PlayerInfoData attacker = GetPlayerFromID(myID);
		if(attacker.properties.Ai!=0&&!PhotonNetwork.isMasterClient)
			return true;
		else
			return false;
	}

	[RPC]
	void HitSoundEffect(int hitSound, Vector3 pos)
	{
		if(HitAudio!=null)
		{
			GameObject sound = GameObject.Instantiate(HitAudio,pos,Quaternion.identity)as GameObject;
			AudioSource soundSource = sound.GetComponent<AudioSource>();
			soundSource.clip = hitSoundPrefab[hitSound-1];
			soundSource.Play();
			Destroy(sound,3);
		}
	}
	
	#region Method for calling RPC
	public void Hit(int myID,int enemyID,int force, TP_Animator.HitWays hitWay, HitSound sound)
	{
		if(isNotMasterClinetAI(myID))
			return;
		PlayerInfoData enemy = GetPlayerFromID(enemyID);
		Transform enemyTrans = enemy.transform;
		TP_Info myInfo = GetPlayerFromID(myID).transform.GetComponent<TP_Info>();

		if(enemyTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead&&!enemyTrans.GetComponent<TP_Info>().CanShopping&&!myInfo.CanShopping)
		{
			PhotonView enemyView = enemyTrans.GetComponent<PhotonView>();

			float flunctuation;
			Debug.Log("enemy:" + enemy.transform.name + "F: " + force);
			flunctuation = UnityEngine.Random.Range(-10,10)/100.0f;
			enemyView.RPC("BeHit", PhotonTargets.All, myID, enemyID, force, flunctuation, (int)hitWay);

			Vector3 pos = myInfo.myTransform.position;
			if(sound!=HitSound.None)
			{
				photonView.RPC("HitSoundEffect",PhotonTargets.All,(int)sound,pos);
			}
		}
	}

	public void Freeze(int myID,int enemyID,int force)
	{
		PlayerInfoData enemy = GetPlayerFromID(enemyID);
		Transform enemyTrans = enemy.transform;
		TP_Info myInfo = GetPlayerFromID(myID).transform.GetComponent<TP_Info>();
		if(enemyTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead&&!enemyTrans.GetComponent<TP_Info>().CanShopping&&!myInfo.CanShopping)
		{
			PhotonView enemyView = enemyTrans.GetComponent<PhotonView>();
			enemyView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.DarkMan,2);
			enemyView.RPC("DarkmanFreeze", PhotonTargets.All, myID, enemyID, force);
		}
	}

	public void DizzyFire(int myID,int enemyID,int force)
	{
		PlayerInfoData enemy = GetPlayerFromID(enemyID);
		Transform enemyTrans = enemy.transform;
		TP_Info myInfo = GetPlayerFromID(myID).transform.GetComponent<TP_Info>();
		if(enemyTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead&&!enemyTrans.GetComponent<TP_Info>().CanShopping&&!myInfo.CanShopping)
		{
			PhotonView enemyView = enemyTrans.GetComponent<PhotonView>();
			Debug.Log(enemy.name);
			enemyView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Mars,2);
			enemyView.RPC("MarsDizzyFire", PhotonTargets.All, myID, enemyID, force);
		}
	}


	public void Addhealth(int myID,int targetPlayerID,int force)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		Transform targetTrans = target.transform;
		if(targetTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead)
		{
			PhotonView targetView = targetTrans.GetComponent<PhotonView>();
			targetView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Theia,3);
			targetView.RPC("AddHealth", PhotonTargets.All, myID, targetPlayerID, force);
		}
	}

	public void TheiaAddDefense(int myID,int targetPlayerID,int force)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		Transform targetTrans = target.transform;
		if(targetTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead)
		{
			PhotonView targetView = targetTrans.GetComponent<PhotonView>();
			targetView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Theia,4);
			targetView.RPC("TheiaAddDefense", PhotonTargets.All, myID, targetPlayerID, force);
		}
	}

	public void RoleAddDefense(int myID, int targetPlayerID,int force,TP_Info.CharacterRole role)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		PhotonView targetView = target.transform.GetComponent<PhotonView>();

		targetView.RPC("RoleAddDefense", PhotonTargets.All, myID, force,(int)role);
	}

	public void RoleAddAttack(int myID, int targetPlayerID,int force,TP_Info.CharacterRole role)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		PhotonView targetView = target.transform.GetComponent<PhotonView>();
		
		targetView.RPC("RoleAddAttack", PhotonTargets.All, myID, force,(int)role);
	}

	public void PersiaDecreaseDefense(int myID,int targetPlayerID,int force)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		Transform targetTrans = target.transform;
		TP_Info myInfo = GetPlayerFromID(myID).transform.GetComponent<TP_Info>();
		if(targetTrans.GetComponent<TP_Animator>().State!=TP_Animator.CharacterState.Dead&&!targetTrans.GetComponent<TP_Info>().CanShopping&&!myInfo.CanShopping)
		{
			PhotonView targetView = targetTrans.GetComponent<PhotonView>();
			targetView.RPC("SkillEffect",PhotonTargets.All,(int)CharacterRoleEff.Persia,4);
			targetView.RPC("PersiaDecreaseDefense", PhotonTargets.All, myID, targetPlayerID, force);
		}
	}

	public void HyperionUnbeatable(int myID, int targetPlayerID,int force)
	{
		PlayerInfoData target = GetPlayerFromID(targetPlayerID);
		PhotonView targetView = target.transform.GetComponent<PhotonView>();
		
		targetView.RPC("HyperionUnbeatable", PhotonTargets.All, myID, force);
	}
	#endregion

	public void LockRoleInput()
	{
		photonView.RPC("LockRole",PhotonTargets.AllBuffered,localPlayer.ID);
	}

	[RPC]
	public void UseLightSourceToRevive(int team, int amount,int id)
	{
		InRoom_Menu.PlayerInfoData pla = InRoom_Menu.SP.GetPlayerFromID(id);
		PhotonView playerView = pla.transform.GetComponent<PhotonView>();

		if(team==1)
		{
			L_LSManager.LightSource += amount;
		}
		else
		{
			D_LSManager.LightSource += amount;
		}

		HUDText HDT = pla.transform.GetComponent<TP_Info>().CreateHDT();
		if(playerView.isMine)
		{
			if(pla.properties.Ai==0)
			{
				string str = "You lost " + amount +" Resource!";
				HDT.Add(str,Color.cyan,1f);
			}
		}
		pla.properties.UseLightSource += amount;
	}

	[RPC]
	void LockRole(int ID)
	{
		PlayerInfoData pla = GetPlayerFromID(ID);
		pla.lockRole = true;
		int team = pla.team;
		int pos = pla.pos;
		if(team==1)
		{
			team1Ready[pos] = true;
		}
		else
		{
			team2Ready[pos] = true;
		}
		roomUIManager.roomUIRoleBase[8+pos+(team-1)*MaxTeamNum].SetActive(true);
	}
}
