using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverView_Menu : MonoBehaviour {

	public static OverView_Menu SP;
	public int winner;
	public List<InRoom_Menu.PlayerInfoData> playerList = new List<InRoom_Menu.PlayerInfoData>();
	public bool playerMusic;
	public bool result;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		SP = this;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Game_Manager.SP.MyGameState == GameState.OverView)
		{
			if(!playerMusic)
			{
				if(result)
					GameObject.Find("Victory Music").SetActive(true);
				else
					GameObject.Find("Defeat Music").SetActive(true);
			}
		}
	}
}
