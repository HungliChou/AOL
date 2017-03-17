using UnityEngine;
using System.Collections;

public class PlayersInfo : MonoBehaviour {

	public bool[] alive;
	public bool regenerate;
	public int myID;

	public NetworkingManager manager;

	void Awake()
	{
		myID = 0;
		regenerate = false;
		alive = new bool[9]; 
		manager = gameObject.GetComponent<NetworkingManager>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*if(regenerate)
		{
			if(myID!=0 && alive[myID]==false)
			{
				manager.CreateRole(myID);
				regenerate = false;
			}
		}*/
	}
}
