using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShoppingHome : MonoBehaviour {

	public Transform image;
	public List<Transform> teamList = new List<Transform>();

	private string myTag;
	private Transform myTransform;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		myTag = myTransform.tag;

	}
	
	// Update is called once per frame
	void Update () {
		image.Rotate(-Vector3.up * Time.deltaTime*100, Space.Self);
	}

	#region trigger area for detecting enemy 
	void OnTriggerEnter(Collider col)
	{
		Transform colTrans = col.transform;
		string colTag = col.tag;

		if(colTrans!=myTransform)
		{
			if(myTag==colTag)
			{
				if(!teamList.Contains(colTrans))
				{
					teamList.Add(colTrans);
					TP_Info playerInfo = colTrans.GetComponent<TP_Info>();
					playerInfo.CanShopping = true;
				}
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		Transform colTrans = col.transform;
		string colTag = col.tag;

		if(myTag==colTag)
		{
			if(teamList.Contains(colTrans))
			{
				TP_Info playerInfo = colTrans.GetComponent<TP_Info>();
				playerInfo.CanShopping = false;
				teamList.Remove(colTrans);
			}
		}
	}
	#endregion
}
