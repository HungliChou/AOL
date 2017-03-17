using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeColliderAttack : MonoBehaviour {


	public List<Transform> attackList = new List<Transform>();
	public List<Transform> TeammateList = new List<Transform>();
	public Transform myTransform;
	public Transform parentTrans;
	public Transform AiForwardTrans;
	public Transform rotateParentTrans;
	public GameObject RangeModel;
	public bool _triggerExit;
	public bool TriggerExit{get{return _triggerExit;}set{_triggerExit = value;}}
	public float rot;
	public TP_Info playerInfo;

	void Awake()
	{
		myTransform = transform;
		parentTrans = transform.parent.parent;
		AiForwardTrans = transform.parent;
		playerInfo = parentTrans.GetComponent<TP_Info>();
		rotateParentTrans = transform.parent;
		_triggerExit = true;
		SanpToCamera();
	}

	void Update()
	{
		if(!playerInfo.isAI)
			SanpToCamera();
		else
			SnapToForward();
	}

	void SanpToCamera()
	{
		if(myTransform.name=="IceFreeze _skillRoundRange"||myTransform.name=="Stun _skillRoundRange")
			rot = Camera.main.transform.eulerAngles.y - (myTransform.eulerAngles.y + 180);
		else
			rot = Camera.main.transform.eulerAngles.y - myTransform.eulerAngles.y;
		if(rot!=0)
			rotateParentTrans.RotateAround(parentTrans.position,Vector3.up,rot);
	}

	void SnapToForward()
	{
		if(AiForwardTrans.localRotation.y!=0)
			AiForwardTrans.localRotation = new Quaternion(0,0,0,0);
	}

	public void OnModel()
	{
		SanpToCamera();
		RangeModel.SetActive(true);
	}

	public void OffModel()
	{
		RangeModel.SetActive(false);
	}

	public void ClearList()
	{
		attackList.Clear();
		TeammateList.Clear();
	}

	#region trigger area for detecting enemy 
	void OnTriggerEnter(Collider col)
	{
		Transform colTrans = col.transform;
		string colTag = col.tag;
		if(colTrans!=parentTrans)
		{
			if(parentTrans.tag=="team1")
			{
				if(colTag=="team2"||colTag=="monster")
				{
					if(!attackList.Contains(colTrans))
					{
						attackList.Add(colTrans);
					}
				}
			}
			else if(parentTrans.tag=="team2")
			{
				if(colTag=="team1"||colTag=="monster")
				{
					if(!attackList.Contains(colTrans))
					{
						attackList.Add(colTrans);
					}
				}
			}
			else if(parentTrans.tag=="monster")
			{
				if(colTag=="team1"||colTag=="team2")
				{
					if(!attackList.Contains(colTrans))
					{
						attackList.Add(colTrans);
					}
				}
			}
		}
		//attackList
		//-----------------------------------------------------
		//teammateList
		if(parentTrans.tag=="team1")
		{
			if(colTag=="team1")
			{
				if(!TeammateList.Contains(colTrans))
				{
					TeammateList.Add(colTrans);
				}
			}
		}
		else if(parentTrans.tag=="team2")
		{
			if(colTag=="team2")
			{
				if(!TeammateList.Contains(colTrans))
				{
					TeammateList.Add(colTrans);
				}
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		if(_triggerExit)
		{
			Transform colTrans = col.transform;
			string colTag = col.tag;
			if(parentTrans.tag=="team1")
			{
				if(colTag=="team2"||colTag=="monster")
				{
					if(attackList.Contains(colTrans))
						attackList.Remove(colTrans);
				}
			}
			else if(parentTrans.tag=="team2")
			{
				if(colTag=="team1"||colTag=="monster")
				{
					if(attackList.Contains(colTrans))
						attackList.Remove(colTrans);
				}
			}
			else if(parentTrans.tag=="monster")
			{
				if(colTag=="team1"||colTag=="team2")
				{
					if(attackList.Contains(colTrans))
						attackList.Remove(colTrans);
				}
			}
			//attackList
			//-----------------------------------------------------
			//teammateList
			if(parentTrans.tag=="team1")
			{
				if(colTag=="team1")
				{
					if(TeammateList.Contains(colTrans))
						TeammateList.Remove(colTrans);
				}
			}
			else if(parentTrans.tag=="team2")
			{
				if(colTag=="team2")
				{
					if(TeammateList.Contains(colTrans))
						TeammateList.Remove(colTrans);
				}
			}
		}
	}
	#endregion

}
