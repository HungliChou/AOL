using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HomeArea : MonoBehaviour {

	public List<Transform> enemyList;
	private Transform myTransform;
	private string myTag;
	
	void Awake()
	{
		enemyList = new List<Transform>();
		myTransform = transform;
		myTag = myTransform.tag;
	}
	
	/*void CheckIfAIAndSetAtWhere(Transform colTrans, int where, bool atOrLeave)
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
	}*/
	
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

						colTrans.GetComponent<TP_Info>().WarningForEnemyInput(true);
						/*if(colTrans==InRoom_Menu.SP.localPlayer.transform)
						{
							GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(true);
						}*/
						//CheckIfAIAndSetAtWhere(colTrans,2,true);
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
						colTrans.GetComponent<TP_Info>().WarningForEnemyInput(true);
						/*if(colTrans==InRoom_Menu.SP.localPlayer.transform)
						{
							GameUIManager.SP.WarningLabel[(int)GameUIWarningLabel.CanStealLightSourceHint].gameObject.SetActive(true);
						}*/
						//CheckIfAIAndSetAtWhere(colTrans,2,true);
					}
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
					//CheckIfAIAndSetAtWhere(colTrans,2,false);
					colTrans.GetComponent<TP_Info>().WarningForEnemyInput(false);
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
					//CheckIfAIAndSetAtWhere(colTrans,2,false);
					colTrans.GetComponent<TP_Info>().WarningForEnemyInput(false);
					enemyList.Remove(colTrans);
				}
			}
		}
	}
	#endregion
}
