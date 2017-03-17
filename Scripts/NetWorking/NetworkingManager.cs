using UnityEngine;
using System.Collections;

public class NetworkingManager : Photon.MonoBehaviour {

	private PhotonView myPhotonView;
	private PlayersInfo playersInfo;

	public GameObject myRole;
	
	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("alpha 0.1");
	}

	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}

	void OnJoinedRoom()
	{
		//playersInfo = GameObject.Find("_SCRIPTS").GetComponent<PlayersInfo>();
		//CreateRole(0);
	}

	public void CreateRole(int ID)
	{
		myRole = PhotonNetwork.Instantiate("Darkman Solider", Random.onUnitSphere * 5f, Quaternion.identity, 0 );
		if(ID==0)
		{
			playersInfo.myID = PhotonNetwork.room.playerCount;
			GameObject myPlayer = GameObject.Find("Player" + PhotonNetwork.room.playerCount.ToString());
			myRole.transform.parent = myPlayer.transform;
			
			myPhotonView = myPlayer.GetComponent<PhotonView>();
			myPhotonView.observed = myRole.GetComponent<NetworkController>();
		}
		playersInfo.alive[ID] = true;

		
		//AddTag(myPhotonView);
		if(myPhotonView.isMine)
		{
			TP_Animator playerAnimator = myRole.GetComponent<TP_Animator>();
			TP_Motor playerMotor = myRole.GetComponent<TP_Motor>();
			TP_Controller playerController = myRole.GetComponent<TP_Controller>();
			TP_Info playerInfo = myRole.GetComponent<TP_Info>();
			playerAnimator.enabled = true;
			playerMotor.enabled = true;
			playerController.enabled = true;
			playerInfo.enabled = true;
		}
	}
}
