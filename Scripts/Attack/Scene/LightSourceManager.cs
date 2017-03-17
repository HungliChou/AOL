using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightSourceManager : MonoBehaviour {

	private const int MaxLightSource = 1000;
	public int _lightSource;
	public List<Transform> teamList;
	public List<Transform> enemyList;
	private Transform myTransform;
	private string myTag;
	public GameObject GameOverCamera;

	public int LightSource{get{return _lightSource;}set{_lightSource = value;}}

	void Awake()
	{
		_lightSource = MaxLightSource;
		teamList = new List<Transform>();
		enemyList = new List<Transform>();
		myTransform = transform;
		myTag = myTransform.tag;
	}

	void CheckIfAIAndSetAtWhere(Transform colTrans, int where, bool atOrLeave)
	{
		if(where==1)//Home
		{
			TP_Info playerInfo = colTrans.GetComponent<TP_Info>();
			if(playerInfo.isAI)
			{
				AIScript aiScript = colTrans.GetComponent<AIScript>();
				aiScript._atHome = atOrLeave;
			}
		}
		else if(where==2)//enemy Home
		{
			TP_Info playerInfo = colTrans.GetComponent<TP_Info>();
			if(playerInfo.isAI)
			{
				AIScript aiScript = colTrans.GetComponent<AIScript>();
				aiScript._atEnemyHome = atOrLeave;
			}
		}
	}

	void Update()
	{
		CheckLightSource();
	}

	void CheckLightSource()
	{
		if(_lightSource<=0)
		{
			int winner = 0;

			if(myTag=="team1Light")
				winner = 2;
			else
				winner = 1;
			StartCoroutine(GameOver(winner));
			//OverView_Menu.SP.playerList = InRoom_Menu.SP.playerList;

		}
	}

	IEnumerator GameOver(int winner)
	{
		GameUIManager.SP.gameUIPage[0].SetActive(false);
		GameUIManager.SP.gameUIPage[1].SetActive(false);
		GameUIManager.SP.gameUIPage[3].SetActive(false);
		InRoomChat.SP.IsVisible = false;
		bool result = false;
		GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
		foreach(GameObject camera in cameras)
		{
			camera.SetActive(false);
		}
		GameOverCamera.SetActive(true);
		myTransform.GetComponent<Animator>().enabled = true;
		if(PhotonNetwork.isMasterClient)
		{
			Game_Manager.SP.StopAddAllMoney();
		}
		yield return new WaitForSeconds(7);

		if(InRoom_Menu.SP.localPlayer.team==winner)
			result = true;
		InRoom_Menu.SP.StopGame(result);
	}

	#region trigger area for detecting enemy 
	void OnTriggerEnter(Collider col)
	{
		Transform colTrans = col.transform;
		string colTag = col.tag;
		if(colTrans!=myTransform)
		{
			if(myTag=="team1Light")
			{
				if(colTag=="team2")
				{
					if(!enemyList.Contains(colTrans))
					{
						enemyList.Add(colTrans);
						if(colTrans==InRoom_Menu.SP.localPlayer.transform)
						{
							GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(true);
						}
						CheckIfAIAndSetAtWhere(colTrans,2,true);
					}
				}
			}
			else if(myTag=="team2Light")
			{
				if(colTag=="team1")
				{
					if(!enemyList.Contains(colTrans))
					{
						enemyList.Add(colTrans);
						if(colTrans==InRoom_Menu.SP.localPlayer.transform)
						{
							GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(true);
						}
						CheckIfAIAndSetAtWhere(colTrans,2,true);
					}
				}
			}
		}
		//attackList
		//-----------------------------------------------------
		//teammateList
		if(myTag=="team1Light")
		{
			if(colTag=="team1")
			{
				if(!teamList.Contains(colTrans))
				{
					teamList.Add(colTrans);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.SetActive(true);
					}
					CheckIfAIAndSetAtWhere(colTrans,1,true);
				}
			}
		}
		else if(myTag=="team2Light")
		{
			if(colTag=="team2")
			{
				if(!teamList.Contains(colTrans))
				{
					teamList.Add(colTrans);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.SetActive(true);
					}
					CheckIfAIAndSetAtWhere(colTrans,1,true);
				}
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		Transform colTrans = col.transform;
		string colTag = col.tag;
		if(myTag=="team1Light")
		{
			if(colTag=="team2")
			{
				if(enemyList.Contains(colTrans))
				{
					CheckIfAIAndSetAtWhere(colTrans,2,false);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(false);
					}
					enemyList.Remove(colTrans);
				}
			}
		}
		else if(myTag=="team2Light")
		{
			if(colTag=="team1")
			{
				if(enemyList.Contains(colTrans))
				{
					CheckIfAIAndSetAtWhere(colTrans,2,false);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(false);
					}
					enemyList.Remove(colTrans);
				}
			}
		}
		//attackList
		//-----------------------------------------------------
		//teammateList
		if(myTag=="team1Light")
		{
			if(colTag=="team1")
			{
				if(teamList.Contains(colTrans))
				{
					CheckIfAIAndSetAtWhere(colTrans,1,false);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.SetActive(false);
					}
					teamList.Remove(colTrans);
				}
					
			}
		}
		else if(myTag=="team2Light")
		{
			if(colTag=="team2")
			{
				if(teamList.Contains(colTrans))
				{
					CheckIfAIAndSetAtWhere(colTrans,1,false);
					if(colTrans==InRoom_Menu.SP.localPlayer.transform)
					{
						GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanUseLightSourceHint].gameObject.SetActive(false);
					}
					teamList.Remove(colTrans);
				}
			}
		}
	}
	#endregion
}
