using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room_Menu : Photon.MonoBehaviour {

/*
	public enum Team
	{
		None,team1,team2
	}

	public static Room_Menu SP;
	
	public Transform playerPrefab;
	
	public List<PlayerInfoData> playerList;
	public PlayerInfoData localPlayer;
	public Team myTeam;
	public int myPos;
	public bool isResetPos;

	public List<PlayerInfoData> Team1PlayerList;
	public List<PlayerInfoData> Team2PlayerList;

	public int[] teamNum;
	private static int MaxTeamNum = 4;

	private Transform myLocalTransform;
	private Camera mainCamera;

	public SpawnPoints SPScript;

	[System.Serializable]
	public class PlayerInfoData
	{
		public PhotonPlayer networkPlayer;
		public Transform transform;
		public TP_Info InfoScript;
		public int ID;

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

	public void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);

		SP = this;
		playerList = new List<PlayerInfoData>();
		Team1PlayerList = new List<PlayerInfoData>();
		Team2PlayerList = new List<PlayerInfoData>();
		teamNum = new int[2];
		isResetPos = false;
		mainCamera = Camera.main;

		PhotonNetwork.isMessageQueueRunning = true;

		AllocatePlayer();

		//SpawnRoomPlayer();
		//PhotonNetwork.autoCleanUpPlayerObjects = false;

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

	public void OnGUI()
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
	}

	#region Load Game-Scene
	void ForceLoadGame()
	{
		PhotonNetwork.automaticallySyncScene = true;
		//PhotonNetwork.networkingPeer.Service();//Make sure to send the RPC before starting loading ourselves
		photonView.RPC("DoLoadGame", PhotonTargets.AllBuffered);
	}


	[RPC]
	void DoLoadGame()
	{
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel("Game-Scene");
	}
	#endregion

	#region On Photon
	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom (local)");
		WholeGameManager.SP.InGame = false;

		photonView.RPC("RemovePlayer",PhotonTargets.AllBuffered,PhotonNetwork.player);

		PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room

		WholeGameManager.SP.EnableLobby(gameObject);
	}
	
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		RemovePlayer(PhotonNetwork.player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
//	}
	/*
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
		Debug.Log("OnPlayerDisconneced: " + player);
		RemovePlayer(player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
/*
	}
	
	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");
		RemovePlayer(PhotonNetwork.player);
		/*WholeGameManager.SP.NameExisted = false;
		WholeGameManager.SP.InGame = false;
		WholeGameManager.SP.EnableLobby(gameObject);*/
	/*}
	#endregion

	#region Manage Players
	//////////////////////////////
	// Manage players
	
	[RPC]//Add this player into PlayerList and assign this player to be localPlayer if the player isLocal
	void AddPlayer(PhotonPlayer networkPlayer, int id1)
	{
		if (GetPlayer(networkPlayer) != null)
		{
			Debug.LogError("AddPlayer: Player already exists!");
			return;
		}
		
		PlayerInfoData pla = new PlayerInfoData();
		pla.networkPlayer = networkPlayer;
		pla.ID = id1;

		playerList.Add(pla);

		if(Team1PlayerList.Count<=MaxTeamNum)
		{
			Team1PlayerList.Add(pla);
		}
		else if(Team2PlayerList.Count<=MaxTeamNum)
		{
			Team2PlayerList.Add(pla);
		}

		if (pla.IsLocal)
		{
			if (localPlayer.ID == pla.ID) { Debug.LogError("localPlayerInfo already set?"); }
			localPlayer = pla;
		}
		Debug.Log("AddPlayer " + networkPlayer + " : " + + pla.ID);
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
		thePlayer.InfoScript = pTransform.GetComponent<TP_Info>();
		if (thePlayer.IsLocal)
		{
			localPlayer = thePlayer;
		}
	}

	[RPC]//Destroy all the player's Network GOs, Photoview and RPCs if he is master client, also destroy local GO if it's existed, then remove this player from PlayerList
	void RemovePlayer(PhotonPlayer networkPlayer)
	{
		PlayerInfoData thePlayer = GetPlayer(networkPlayer);
		/*if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.DestroyPlayerObjects(networkPlayer);
			PhotonNetwork.RemoveRPCs(networkPlayer);//PhotonNetwork.RemoveAllBufferedMessages(networkPlayer);
		}*/

		/*if(myTeam==Team.team1)
			photonView.RPC("RemoveTeamNum", PhotonTargets.AllBuffered, 0);
		else if(myTeam==Team.team2)
			photonView.RPC("RemoveTeamNum", PhotonTargets.AllBuffered, 1);*/
		/*if (thePlayer.transform)
		{
			Destroy(thePlayer.transform.gameObject);
		}
		playerList.Remove(thePlayer);
	}

	//Send Back this networkPlayer in the PlayerList
	PlayerInfoData GetPlayer(PhotonPlayer networkPlayer)
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

	//When a PhotonView instantiates it has viewID=0 and is unusable.
	//We need to assign the right viewID -on all players(!)- for it to work
	void SetPhotonViewIDs(GameObject go, int id1)
	{
		PhotonView[] nViews = go.GetComponentsInChildren<PhotonView>();
		nViews[0].viewID = id1;
	}
	#endregion

	#region Allocate and Spawn Players
	void AllocatePlayer()
	{
		int id1 = PhotonNetwork.AllocateViewID();

		photonView.RPC("AddPlayer", PhotonTargets.AllBuffered, PhotonNetwork.player, id1);

		//photonView.RPC("DetermineTeamAndPos", PhotonTargets.AllBuffered,PhotonNetwork.player);

		Vector3 pos = transform.position;
		Quaternion rot = Quaternion.identity;
		photonView.RPC("SpawnRoomPlayer", PhotonTargets.AllBuffered, pos, rot, id1, PhotonNetwork.player);

		//photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 0);
	
		/*if(teamNum[0]<MaxTeamNum)
		{
			myTeam = Team.team1;
			Debug.Log("nowNum : " + teamNum[0]);
			myPos = teamNum[0];
			photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 0);
		}
		else if(teamNum[1]<MaxTeamNum)
		{
			//photonView.RPC("RemoveTeamNum", PhotonTargets.AllBuffered, 0);
			myTeam = Team.team2;
			myPos = teamNum[1];
			photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 1);
		}*/
	/*}
	
	[RPC]
	public void RemoveTeamNum(int num)
	{
		teamNum[num]--;
	}

	[RPC]
	public void AddTeamNum(int num)
	{
		teamNum[num]++;
	}

	[RPC]
	public void SpawnRoomPlayer(Vector3 pos, Quaternion rot, int id1, PhotonPlayer np)
	{
		if(!np.isLocal)
			pos += new Vector3(200,200,0);
		Transform newPlayer = Instantiate(playerPrefab, pos, rot) as Transform;

		//Set transform
		SetPlayerTransform(np,newPlayer);

		//Set photonviewID everywhere!
		SetPhotonViewIDs(newPlayer.gameObject, id1);
	}
	
	public void SpawnPlayer()
	{
		Transform[] spawnPoints;
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		SPScript = GameObject.FindGameObjectWithTag("SpawnPoints").GetComponent<SpawnPoints>();

		for(int cnt = 0; cnt<Team1PlayerList.Count;cnt++)
		{
			if(Team1PlayerList[cnt].ID==localPlayer.ID)
			{
				spawnPoints = SPScript.team1SpawnPoints;
				Transform theGO = spawnPoints[cnt];
				pos = theGO.position;
				rot = theGO.rotation;
			}
		}
		if(Team2PlayerList.Count>0)
		{
			for(int cnt = 0; cnt<Team2PlayerList.Count;cnt++)
			{
				if(Team2PlayerList[cnt].ID==localPlayer.ID)
				{
					spawnPoints = SPScript.team2SpawnPoints;
					Transform theGO = spawnPoints[cnt];
					pos = theGO.position;
					rot = theGO.rotation;
				}
			}
		}
		/*if(myTeam==Team.team1)
		{
			spawnPoints = SPScript.team1SpawnPoints;
		}
		else
			spawnPoints = SPScript.team2SpawnPoints;

		Transform theGO = spawnPoints[myPos];
		Vector3 pos = theGO.position;
		Quaternion rot = theGO.rotation;
		localPlayer.transform.position = pos;
		localPlayer.transform.rotation = rot;*/

		/*foreach(PlayerInfoData pla in playerList)
		{
			TP_Info.Team myteam = pla.InfoScript.MyTeam;
			if(myteam==TP_Info.Team.team1)
				Team1PlayerList.Add(pla);
			else if(myteam==TP_Info.Team.team2)
			{
				Team2PlayerList.Add(pla);
			}
		}

		Team1PlayerList.Sort(CompareByID);
		Team2PlayerList.Sort(CompareByID);

		for(int cnt = 0; cnt< Team1PlayerList.Count;cnt++)
		{
			spawnPoints = SPScript.team1SpawnPoints;
			Transform theGO = spawnPoints[cnt];
			Vector3 pos = theGO.position;
			Quaternion rot = theGO.rotation;
			Team1PlayerList[cnt].transform.position = pos;
			Team1PlayerList[cnt].transform.rotation = rot;
		}

		for(int cnt = 0; cnt< Team2PlayerList.Count;cnt++)
		{
			spawnPoints = SPScript.team2SpawnPoints;
			Transform theGO = spawnPoints[cnt];
			Vector3 pos = theGO.position;
			Quaternion rot = theGO.rotation;
			Team2PlayerList[cnt].transform.position = pos;
			Team2PlayerList[cnt].transform.rotation = rot;
		}*/

	/*	photonView.RPC("SpawnGamePlayer", PhotonTargets.AllBuffered, PhotonNetwork.player, pos, rot);
	}

	[RPC]
	public void SpawnGamePlayer(PhotonPlayer np,Vector3 pos, Quaternion rot)
	{
		PlayerInfoData thePlayer = GetPlayer(np);
		thePlayer.transform.position = pos;
		thePlayer.transform.rotation = rot;
		OpenPlayerScripts(thePlayer);


	}


	#endregion
	
	void OpenPlayerScripts(PlayerInfoData pla)
	{
		foreach(PlayerInfoData plas in playerList)
		{
			//if(pla.ID==id1)
		//	{
				Transform myPlayer = plas.transform;
				NetworkController playerNetworkController = myPlayer.GetComponent<NetworkController>();
				playerNetworkController.enabled = true;
				myPlayer.GetComponent<PhotonView>().observed = playerNetworkController;
				myPlayer.GetComponent<TP_Animator>().enabled = true;
				myPlayer.GetComponent<TP_Motor>().enabled = true;
				myPlayer.GetComponent<TP_Controller>().enabled = true;
				myPlayer.GetComponent<TP_Info>().enabled = true;
		//	}
		}
	}

	public static int CompareByID(PlayerInfoData x, PlayerInfoData y)
	{
		if(x==null)
		{
			if(y==null)
				return 0;
			return 1;
		}
		if(y==null)
		{
			return -1;
		}

		int retval = y.ID.CompareTo(x.ID);
		return retval;
	}

	void ResetMyPos()
	{
		if(teamNum[0]<=MaxTeamNum)
		{
			myTeam = Team.team1;
			Debug.Log("nowNum : " + teamNum[0]);
			myPos = teamNum[0];
			photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 0);
		}
		else if(teamNum[1]<MaxTeamNum)
		{
			//photonView.RPC("RemoveTeamNum", PhotonTargets.AllBuffered, 0);
			myTeam = Team.team2;
			myPos = teamNum[1];
			photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 1);
		}
	}

	void Update()
	{
		if(!isResetPos)
		{
			if(teamNum[0]==0)
			{
				photonView.RPC("AddTeamNum", PhotonTargets.AllBuffered, 0);
				photonView.RPC("RemoveTeamNum", PhotonTargets.AllBuffered, 0);
			}
			else if(teamNum[0]!=0)
			{
				ResetMyPos();
				isResetPos = true;
			}
		}
		for(int cnt=0;cnt<teamNum.Length;cnt++)
		{
			Debug.Log("team" + cnt + " : " + teamNum[cnt]);
		}
	}*/
}
