using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum LobbyUIButton
{
	None,Join,Create,BackToMenu,JoinRandomRoom,JoinRoomByName,CreateRoom
}

public enum LobbyUILabel
{
	NoGamingLabel,RoomCount,PlayerCapacity,StartingLightSource
}

public class Lobby_Menu : Photon.MonoBehaviour
{
	
	//GUI vars
	public static Lobby_Menu SP;
	private string currentGUIMethod = "join";
	
	private Vector2 JoinScrollPosition;
	
	private string joinRoomName;
	public UILabel joinRoomNameLabel;
	private string createRoomName;
	public UILabel createRoomLabel;
	public HUDText HudWarningLabel;
	public List<UILabel> warningLabel = new List<UILabel>();
	private string failConnectMesage = "";
	bool isConnectingToRoom = false;

	public List<GameObject> lobbyUIButton = new List<GameObject>();
	public List<GameObject> lobbyUIRightPage = new List<GameObject>();
	public List<UILabel> lobbyUILabel = new List<UILabel>();
	public List<GameObject> roomPrefabsList = new List<GameObject>();

	public GameObject LobbyUI;
	public UIGrid Grid;
	public UIScrollView ScrollView;

	public GameObject roomPrefab;
	public bool CanDoUpdate;
	public bool RoomUpdated;
	public bool show;
	
	void Awake()
	{
		SP = this;
		show = false;
		CanDoUpdate = false;
		RoomUpdated = false;

		AssignButtonListener();
		CheckMCLeftRoomWarning();
		//Default join values
		joinRoomName = "";
		createRoomName = "";

		//Default host values
		hostTitle = PlayerPrefs.GetString("hostTitle", "Default");

	}

	void AssignButtonListener()
	{
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.Join-1]).onClick = OpenLobbyPage;
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.Create-1]).onClick = OpenLobbyPage;
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.BackToMenu-1]).onClick = BackToMenu;
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.JoinRandomRoom-1]).onClick = JoinRandomRoom;
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.JoinRoomByName-1]).onClick = JoinRoomByName;
		UIEventListener.Get(lobbyUIButton[(int)LobbyUIButton.CreateRoom-1]).onClick = CreateRoom;
	}

	void CheckMCLeftRoomWarning()
	{
		if(WholeGameManager.SP.MCLeftRoomWarning)
		{
			HudWarningLabel.Add("The room is closed because the room master just left.",Color.white,5);
			WholeGameManager.SP.MCLeftRoomWarning = false;
		}
	}

	public void EnableLobby()
	{
		show = true;
	}
	
	void OnConnectedToPhoton()
	{
		//Debug.Log("This client has connected to a server");
		failConnectMesage = "";
	}
	void OnDisconnectedFromPhoton()
	{
		//Debug.Log("This client has disconnected from the server");
		failConnectMesage = "Disconnected from Photon";
		HudWarningLabel.Add(failConnectMesage,Color.red,5);
	}
	
	void OnFailedToConnectToPhoton(ExitGames.Client.Photon.StatusCode status)
	{
		//Debug.Log("Failed to connect to Photon: " + status);
		failConnectMesage = "Failed to connect to Photon: " + status;
		HudWarningLabel.Add(failConnectMesage,Color.red,5);
	}

	void Update()
	{
		if(!Cursor.visible)
		{
			Cursor.visible = true;
		}
		if(Screen.lockCursor)
		{
			Screen.lockCursor = false;
		}

		if(PhotonNetwork.connected)
		{
			if(show)
			{
				//open LobbyUI
				LobbyUI.SetActive(true);
				if (currentGUIMethod == "join")
					JoinMenu();
			}
			else
			{
				if (LobbyUI.activeSelf)
				{
					if(CanDoUpdate)
					{
						CancelInvoke("UpdateRoomList");
						CanDoUpdate = false;
					}
					LobbyUI.SetActive(false);
				}
			}
		}
	}
	
	void OnGUI()
	{
		if (!PhotonNetwork.connected)
		{
			//GUILayout.Label("Connecting..");
			if(!warningLabel[0].gameObject.activeSelf)
				warningLabel[0].gameObject.SetActive(true);
			if (failConnectMesage != "")
			{
				//GUILayout.Label("Error message: " + failConnectMesage);
				if(!warningLabel[1].gameObject.activeSelf)
				{
					warningLabel[1].gameObject.SetActive(true);
					warningLabel[1].text = "Error message: " + failConnectMesage;
				}
				if (GUILayout.Button("Retry"))
				{
					failConnectMesage = "";
					PhotonNetwork.ConnectUsingSettings("1.0");
				}
			}
		}
		else
		{
			if(warningLabel[1].gameObject.activeSelf)
			{
				warningLabel[1].gameObject.SetActive(false);
			}
			if(warningLabel[0].gameObject.activeSelf)
				warningLabel[0].gameObject.SetActive(false);
			if(show)
			{
				if (currentGUIMethod == "join")
					JoinMenuGUI();
			}
		}
	}

	#region Button Event
	void OpenLobbyPage(GameObject button)
	{
		for(int cnt = 0; cnt < 2;cnt++)
		{
			if(button==lobbyUIButton[cnt])
			{
				if(!lobbyUIRightPage[cnt].activeSelf)
					lobbyUIRightPage[cnt].SetActive(true);
				if(cnt==0)
					currentGUIMethod = "join";
				else if(cnt==1)
					currentGUIMethod = "create";
			}
			else
				lobbyUIRightPage[cnt].SetActive(false);
		}
	}

	void BackToMenu(GameObject button)
	{
		foreach(GameObject GO in lobbyUIRightPage)
		{
			GO.SetActive(false);
		}
		show = false;
		Name_Menu.SP.RequirePlayerName = true;
	}

	void JoinRandomRoom(GameObject button)
	{
		if(PhotonNetwork.GetRoomList().Length > 0)
			PhotonNetwork.JoinRandomRoom();
	}

	void JoinRoom(GameObject button)
	{
		string RoomName = button.transform.Find("Room/2.ID").GetComponent<UILabel>().text;
		//Debug.Log(RoomName);
		 
		foreach (RoomInfo room in PhotonNetwork.GetRoomList())
		{
			if(room.name==RoomName)
			{
				if(room.playerCount < room.maxPlayers || room.maxPlayers<=0)
				{	
					PhotonNetwork.JoinRoom(RoomName);
				}
			}
		}
	}

	void JoinRoomByName(GameObject button)
	{
		joinRoomName = joinRoomNameLabel.text;
		PhotonNetwork.JoinRoom(joinRoomName);
	}

	void CreateRoom(GameObject button)
	{
		createRoomName = createRoomLabel.text;
		int createCapacity =  Convert.ToInt32(lobbyUILabel[(int)LobbyUILabel.PlayerCapacity].text);
		int startingLightSource = Convert.ToInt32(lobbyUILabel[(int)LobbyUILabel.StartingLightSource].text);
		if(createRoomName.Length>1&&createRoomName!="Please enter a title!"&&createRoomName!="You can type here")
		{
			if(createCapacity>1&&createCapacity<9)
			{
				if(startingLightSource>=100&&startingLightSource<=3000)
					StartHostingGame(createRoomName, createCapacity,startingLightSource);
				else
					lobbyUILabel[(int)LobbyUILabel.StartingLightSource].text = "100~3000";
			}
			else
				lobbyUILabel[(int)LobbyUILabel.PlayerCapacity].text = "2~8";
		}
		else
		{
			createRoomLabel.text = "Please enter a title!";
		}
	}
	#endregion

	void JoinMenu()
	{
		/*if(!LobbyUI.activeSelf)
			return;*/

		if(!CanDoUpdate)
		{
			InvokeRepeating("UpdateRoomList",0.01f,3);
			CanDoUpdate = true;
		}

		if(ScrollView.transform.position.y>10)
		{
			ScrollView.ResetPosition();
		}
		/*else
		{
			//Masterlist
			/*GUILayout.BeginHorizontal();
			GUILayout.Label("Game list:");
			
			
			GUILayout.FlexibleSpace();
			if ( PhotonNetwork.GetRoomList().Length > 0 &&
			    GUILayout.Button("Join random game"))
			{
				PhotonNetwork.JoinRandomRoom();
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(2);
			GUILayout.BeginHorizontal();
			GUILayout.Space(24);
			
			GUILayout.Label("Title", GUILayout.Width(200));
			GUILayout.Label("Players", GUILayout.Width(55));
			GUILayout.EndHorizontal()
			
			
			JoinScrollPosition = GUILayout.BeginScrollView(JoinScrollPosition);
			foreach (RoomInfo room in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				
				
				if ((room.playerCount < room.maxPlayers || room.maxPlayers<=0) &&
				    GUILayout.Button("" + room.name, GUILayout.Width(200)))
				{
					PhotonNetwork.JoinRoom(room.name);
				}
				GUILayout.Label(room.playerCount + "/" + room.maxPlayers, GUILayout.Width(55));
				
				
				
				
				GUILayout.EndHorizontal();
			}



			GUILayout.EndScrollView();
			

			
			//DIRECT JOIN
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Join by name:");
			GUILayout.Space(5);
			GUILayout.Label("Room name");
			joinRoomName = (GUILayout.TextField(joinRoomName + "", GUILayout.Width(50)) + "");
			
			if (GUILayout.Button("Connect"))
			{
				PhotonNetwork.JoinRoom(joinRoomName);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(4);
			
		}*/
	}

	void JoinMenuGUI()
	{
		if (isConnectingToRoom)
		{
			GUILayout.Label("Trying to connect to a room.");
			
		}
		else if (failConnectMesage != "")
		{
			GUILayout.Label("The game failed to connect:\n" + failConnectMesage);
			GUILayout.Space(10);
			if (GUILayout.Button("Cancel"))
			{
				failConnectMesage = "";
			}
		}
	}

	void UpdateRoomList()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();

		if (roomList.Length == 0)
		{
			lobbyUILabel[(int)LobbyUILabel.NoGamingLabel].gameObject.SetActive(true);
			foreach(GameObject room in roomPrefabsList)
			{
				Destroy(room);
			}
			roomPrefabsList.Clear();
		}
		else
		{
			lobbyUILabel[(int)LobbyUILabel.NoGamingLabel].gameObject.SetActive(false);

			foreach (GameObject room in roomPrefabsList)
			{
				Destroy(room);
			}
			roomPrefabsList.Clear();

			for(int cnt = 0;cnt<roomList.Length;cnt++)
			{
				if((roomList[cnt].playerCount < roomList[cnt].maxPlayers || roomList[cnt].maxPlayers<=0) && roomList[cnt].playerCount>0)
				{
					GameObject clone = Instantiate(roomPrefab)as GameObject;
					clone.transform.parent = Grid.transform;
					clone.transform.localPosition = Vector3.zero;
					clone.transform.localRotation = Quaternion.identity;
					clone.transform.localScale = new Vector3(1,1,1);
					clone.transform.Find("Room/1.No.").GetComponent<UILabel>().text = (cnt+1).ToString();
					clone.transform.FindChild("Room/2.ID").GetComponent<UILabel>().text = roomList[cnt].name;
					clone.transform.FindChild("Room/3.Scene").GetComponent<UILabel>().text = "Bridge";
					clone.transform.FindChild("Room/4.Players").GetComponent<UILabel>().text = roomList[cnt].playerCount + " / 8"; 
					roomPrefabsList.Add(clone);
					UIEventListener.Get(clone).onClick = JoinRoom;
				}
			}
			if(Grid!=null)
			{
				Grid.GetComponent<UIGrid>().Reposition();
			}
		}

		int text = roomList.Length;
		lobbyUILabel[(int)LobbyUILabel.RoomCount].text = text.ToString();
	}

	void OnPhotonCreateRoomFailed()
	{
		Debug.Log("A CreateRoom call failed, most likely the room name is already in use.");
		failConnectMesage = "Could not create new room, the name is already in use.";
		HudWarningLabel.Add(failConnectMesage,Color.red,5);
	}
	
	void OnPhotonJoinRoomFailed()
	{
		Debug.Log("A JoinRoom call failed, most likely the room name does not exist or is full.");
		failConnectMesage = "Could not connect to the desired room, this room does no longer exist or all slots are full.";
		HudWarningLabel.Add(failConnectMesage,Color.red,5);
	}
	
	void OnPhotonRandomJoinFailed()
	{
		Debug.Log("A JoinRandom room call failed, most likely there are no rooms available.");
		failConnectMesage = "Could not connect to random room; no rooms were available.";
		HudWarningLabel.Add(failConnectMesage,Color.red,5);
	}
	
	
	void OnJoinedRoom()
	{
		//Stop communication until in the game
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel + 1);
	}
	
	
	
	
	private string hostTitle;
	//private string hostDescription;
	//private int hostMaxPlayers;
	
	
	/*void HostMenu()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Host a new game:");
		GUILayout.EndHorizontal();
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Title:");
		GUILayout.FlexibleSpace();
		hostTitle = GUILayout.TextField(hostTitle, GUILayout.Width(200));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
        GUILayout.Label("Server description");
        GUILayout.FlexibleSpace();
        hostDescription = GUILayout.TextField(hostDescription, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Max players (1-8)");
		GUILayout.FlexibleSpace();
		hostMaxPlayers = int.Parse(GUILayout.TextField(hostMaxPlayers + " ", GUILayout.Width(50)) + "");
		GUILayout.EndHorizontal();
		
		CheckHostVars();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start server", GUILayout.Width(150)))
		{
			StartHostingGame(hostTitle, 8);
		}
		GUILayout.EndHorizontal();
	}*/
	
	/*void CheckHostVars()
	{
		hostMaxPlayers = Mathf.Clamp(hostMaxPlayers, 1, 8);
	}*/
	
	
	void StartHostingGame(string hostSettingTitle, int hostPlayers, int startingLightSource)
	{
		/*if (hostSettingTitle == "")
		{
			hostSettingTitle = "NoTitle";
		}*/
		//hostPlayers = Mathf.Clamp(hostPlayers, 0, 8);
		WholeGameManager.SP.StartingLightSource = startingLightSource;
		PhotonNetwork.CreateRoom(hostSettingTitle,true,true,hostPlayers);
		//PhotonNetwork.CreateRoom(hostSettingTitle, true, true, hostPlayers, addAPlayer);
	}
	
	
	
	
	//
	// CUSTOM HOST LIST
	//
	// You could use this to implement custom sorting, or adding custom fields.
	//
	
	/*
    private List<MyRoomData> hostDataList = new List<MyRoomData>();

     
    void OnReceivedRoomList()
    {
        Debug.Log("We received a new room list, total rooms: " + PhotonNetwork.GetRoomList().Length);
        ReloadHostList();
    }

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("We received a room list update, total rooms now: " + PhotonNetwork.GetRoomList().Length);
        ReloadHostList();
    }



    void ReloadHostList()
    {        
        hostDataList =new List<MyRoomData>();
        foreach(Room room in PhotonNetwork.GetRoomList())
        {
            MyRoomData cHost= new MyRoomData();
            cHost.room = room;
      
            
            hostDataList.Add(cHost);
            
        }
    }



    public class MyRoomData
    {
        public Room room;

        public string title
        {
            get { return room.name; }
        }
        public int connectedPlayers
        {
            get { return room.playerCount; }
        }
        public int maxPlayers
        {
            get { return room.maxPlayers; }
        }

        //Example custom fields
        public int gameVersion; // You could 
    }
     */
	
	
}