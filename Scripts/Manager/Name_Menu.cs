using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NameUIButton
{
	None,Start,Story,HowToPlay,Option,Quit,TeamMember
}

public class Name_Menu : MonoBehaviour {

	public static Name_Menu SP;

	public GameObject NameUI;

	public UILabel nameLabel;

	public List<GameObject> nameUIButton = new List<GameObject>();
	public List<GameObject> nameUIRightPage = new List<GameObject>();

	private Lobby_Menu _lobbyMenuScript;
	
	private bool requirePlayerName = false;
	public bool RequirePlayerName{get{return requirePlayerName;}set{requirePlayerName = value;}}

	public string playerNameInput = "";

	public GameObject RightPagee;


	void Awake()
	{
		SP = this;

		AssignButtonListener();
		playerNameInput = PlayerPrefs.GetString("playerName" + Application.platform, "");

		if(playerNameInput.Length >= 1)
		{
			nameLabel.text = playerNameInput;
		}
		if(WholeGameManager.SP.NameExisted == false)
			requirePlayerName = true;
		
		_lobbyMenuScript = GetComponent<Lobby_Menu>();
		
		OpenMenu("lobbyMenu");

		PhotonNetwork.ConnectUsingSettings("1.5");
	}

	void Update()
	{
		if(requirePlayerName)
		{
			if(!NameUI.activeSelf)
				NameUI.SetActive(true);
			if(NameUI.activeSelf)
				playerNameInput = nameLabel.text;
		}
		else
		{
			if(NameUI.activeSelf)
				NameUI.SetActive(false);
		}
	}

	void AssignButtonListener()
	{
		UIEventListener.Get(nameUIButton[(int)NameUIButton.Start-1]).onClick = GameStart;
		UIEventListener.Get(nameUIButton[(int)NameUIButton.Story-1]).onClick = OpenNamePage;
		UIEventListener.Get(nameUIButton[(int)NameUIButton.HowToPlay-1]).onClick = OpenNamePage;
		UIEventListener.Get(nameUIButton[(int)NameUIButton.Option-1]).onClick = OpenNamePage;
		UIEventListener.Get(nameUIButton[(int)NameUIButton.Quit-1]).onClick = QuitGame;
		UIEventListener.Get(nameUIButton[(int)NameUIButton.TeamMember-1]).onClick = OpenNamePage;
	}

	public void QuitGame(GameObject button)
	{
		Application.Quit();
	}

	public void OpenMenu(string newMenu)
	{
		if (requirePlayerName)
		{
			return;
		}
		
		if (newMenu == "lobbyMenu")
		{
			RightPagee.SetActive(false);
			_lobbyMenuScript.EnableLobby();
		}
		else
		{
			Debug.LogError("Wrong menu:" + newMenu);
		}
	}

	void GameStart(GameObject button)
	{ 
		if(playerNameInput.Length >= 1 && playerNameInput!="Create Your ID")
		{
			foreach(GameObject page in nameUIRightPage)
			{
				page.SetActive(false);
			}
			requirePlayerName = false;
			PlayerPrefs.SetString("playerName" + Application.platform, playerNameInput);
			PhotonNetwork.playerName = playerNameInput;
			WholeGameManager.SP.NameExisted = true;
			OpenMenu("lobbyMenu");
		}
		else
		{
			nameLabel.text = "Enter an ID to Continue...";
		}
	}

	void OpenNamePage(GameObject button)
	{
		RightPagee.SetActive(true);
		for(int cnt = 0; cnt < nameUIButton.Count;cnt++)
		{
			if(button==nameUIButton[cnt])
			{
				if(cnt==5)
				{
					if(!nameUIRightPage[cnt-2].activeSelf)
						nameUIRightPage[cnt-2].SetActive(true);
				}
				else
				{
					if(!nameUIRightPage[cnt-1].activeSelf)
						nameUIRightPage[cnt-1].SetActive(true);
				}
			}
			else
			{
				if(cnt>0&&cnt!=5)
					nameUIRightPage[cnt-1].SetActive(false);
			}
				
		}
	}

	/*void NameMenu(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.Label("Please enter your name");
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		playerNameInput = GUILayout.TextField(playerNameInput);
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		if (playerNameInput.Length >= 1)
		{
			if (GUILayout.Button("Save"))
			{
				requirePlayerName = false;
				PlayerPrefs.SetString("playerName" + Application.platform, playerNameInput);
				PhotonNetwork.playerName = playerNameInput;
				WholeGameManager.SP.NameExisted = true;
				OpenMenu("lobbyMenu");
			}
		}
		else
		{
			GUILayout.Label("Enter a name to continue...");
		}
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		
		GUILayout.EndVertical();
	}*/
}