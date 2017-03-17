using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WholeGameManager : MonoBehaviour {
	public static WholeGameManager SP;
	public Lobby_Menu _LBS;
	private bool _isLobbyScript;
	public int _startingLightSource;
	public bool MCLeftRoomWarning;
	public bool isTesting;

	//name existed means clients had name already so they dont have to enter name again when they back to Lobby
	public bool nameExisted;
	public bool NameExisted{get{return nameExisted;}set{nameExisted = value;}}
	public int StartingLightSource{get{return _startingLightSource;}set{_startingLightSource = value;}}

	public bool inGame;
	public bool InGame{get{return inGame;}set{inGame = value;}}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		SP = this;
		_isLobbyScript = true;
		nameExisted = false;
		inGame = false;
		MCLeftRoomWarning = false;
		Application.targetFrameRate = 50;
		Application.LoadLevel("Lobby-Scene");

	}

	public void ChangeLayersRecursively(Transform trans, string Layer)
	{
		if(trans==null)
			return;
		trans.gameObject.layer = LayerMask.NameToLayer(Layer);
		foreach(Transform child in trans)
		{
			ChangeLayersRecursively(child,Layer);
		}
	}

	public int CalculateLevel(int freeExp,int level, int expToLevel)
	{
		if(freeExp<expToLevel)
		{
			return level;
		}
		else
		{
			int FreeExp = freeExp;
			int Level = level;
			int ExpToLevel = expToLevel;

			FreeExp -= expToLevel;
			ExpToLevel = (int)(ExpToLevel * 1.25f);
			Level++;
			return CalculateLevel(FreeExp,Level,ExpToLevel);

		}
	}

	// Use this for initialization
	void Start () {
	
	}

	public void EnableLobby(GameObject roomMenu)
	{	
		Application.LoadLevel("Lobby-Scene");
		GameObject[] plas = GameObject.FindGameObjectsWithTag("monster");
		foreach(GameObject pla in plas)
		{
			Destroy(pla);
		}
		Destroy(roomMenu);
		_isLobbyScript = false;
	}

	void Update()
	{
		if(_isLobbyScript==false)
		{
			if(GameObject.FindGameObjectWithTag("Lobby")!=null)
			{
				GameObject.FindGameObjectWithTag("Lobby").GetComponent<Lobby_Menu>().EnableLobby();
				_isLobbyScript = true;
			}
		}
	}


}
