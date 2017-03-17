using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState{
	None,Playing,DeadLock,CanChoosing,Shopping,OverView,Esc
}

public class Game_Manager : Photon.MonoBehaviour {
	public static Game_Manager SP;

	public GameState _myGameState;
	public InRoom_Menu _roomMenuScript;
	public LightSourceManager L_LSManager;
	public LightSourceManager D_LSManager;
	public int currentTeam;

	public bool showGodWeapon;
	public bool ismaster;
	public GameObject[] WildHarnesses;
	public Transform[] WildMonsterAnchor;
	public bool[] WildExist;
	public float[] WildTimer;
	private bool _setAllPlayerInfoID;
	public bool SetAllPlayerInfoID{get{return _setAllPlayerInfoID;}set{_setAllPlayerInfoID = value;}}
	public GameState MyGameState{get{return _myGameState;}set{_myGameState = value;}}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SP = this;
		PhotonNetwork.isMessageQueueRunning = true;
		WholeGameManager.SP.InGame = true;
		InRoomChat.SP.OnGameAlignment();
		_setAllPlayerInfoID = false;
		_roomMenuScript = GameObject.Find("RoomMenu").GetComponent<InRoom_Menu>();

		#region error Detection
		//if can't find Room_Menu, it must be wrong cuz it should not be destroy when loading the game scene
		if(_roomMenuScript==null)
		{
			Debug.LogError("Can't Find PlayerList");
			WholeGameManager.SP.InGame = false;
			Application.LoadLevel("Lobby-Scene");
			return;
		}

		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			WholeGameManager.SP.NameExisted = false;
			WholeGameManager.SP.InGame = false;
			Application.LoadLevel("Lobby-Scene");
			return;
		}
		#endregion

		if(_myGameState!=GameState.OverView)
		{
			if(PhotonNetwork.isMasterClient)
			{
				_roomMenuScript.AllocateAndSpawnMonster();
				//_roomMenuScript.SpawnAI();
			}

			_roomMenuScript.SpawnPlayer();

			if(PhotonNetwork.isMasterClient)
			{
				ismaster = true;
				InRoom_Menu.SP.AssignTeamNumInput();
				InvokeRepeating("AddAllPlayerMoney",5f,1f);
			}

			_myGameState = GameState.Playing;
		}


	}

	public void StopAddAllMoney()
	{
		if(ismaster)
			CancelInvoke("AddAllPlayerMoney");
	}

	void AddAllPlayerMoney()
	{
		_roomMenuScript.AddAllPlayerMoney(5);
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating("SetPlayerInfoIDInput",0.2f,0.1f);
		GameUIManager.SP.myPlayer = InRoom_Menu.SP.localPlayer.transform;
		GameUIManager.SP.playerInfo = GameUIManager.SP.myPlayer.GetComponent<TP_Info>();

		L_LSManager = GameObject.FindGameObjectWithTag("team1Light").GetComponent<LightSourceManager>();
		D_LSManager = GameObject.FindGameObjectWithTag("team2Light").GetComponent<LightSourceManager>();
		InRoom_Menu.SP.L_LSManager = L_LSManager;
		InRoom_Menu.SP.D_LSManager = D_LSManager;
		currentTeam = InRoom_Menu.SP.localPlayer.team;
		if(PhotonNetwork.isMasterClient)
		{
			InRoom_Menu.SP.SetLightSourceInput(WholeGameManager.SP.StartingLightSource);
		}
	}

	void SetPlayerInfoIDInput()
	{
		if(_setAllPlayerInfoID)
			CancelInvoke("SetPlayerInfoIDInput");
		_roomMenuScript.SetPlayerInfoID();

	}

	// Update is called once per frame
	void FixedUpdate () {
		if(_myGameState!=GameState.OverView)
		{
			if(PhotonNetwork.isMasterClient)
			{
				for(int cnt=0;cnt<WildExist.Length;cnt++)
				{
					if(WildExist[cnt]==false)
					{
						if(WildTimer[cnt]>0)
						{
							WildTimer[cnt] -= Time.fixedDeltaTime;
						}
						else if(WildTimer[cnt]<=0)
						{
							WildTimer[cnt] = 0;
							InRoom_Menu.SP.RespawnMonster(cnt);
							WildExist[cnt] = true;
						}
					}
				}
			}
		}
	}

}
