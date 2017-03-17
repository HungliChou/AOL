using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : Photon.MonoBehaviour {

	public enum RangeType
	{
		None, Round, Sector
	}

	public Texture2D AimCircle;

	public CharacterController CC;
	public TP_Info playerInfo;
	public TP_Motor playerMotor;
	public InRoom_Menu roomMenu;
	public PhotonView myPhotonView;
	public List<Transform> attackList = new List<Transform>();
	public List<Transform> tempList = new List<Transform>();


	public Transform parentTrans;
	public float attackDistance;
	public bool _lockAttackCheck;//only attack one time
	
	public bool LockAttackCheck{get{return _lockAttackCheck;}set{_lockAttackCheck = value;}}

	void Awake()
	{
		parentTrans = transform.parent;
		CC = parentTrans.GetComponent<CharacterController>();
		playerInfo = parentTrans.GetComponent<TP_Info>();
		playerMotor = parentTrans.GetComponent<TP_Motor>();
		roomMenu = GameObject.FindGameObjectWithTag("RoomMenu").GetComponent<InRoom_Menu>();
		myPhotonView = transform.parent.GetComponent<PhotonView>();
		_lockAttackCheck = false;
		attackDistance = 100;
	}

	// Use this for initialization
	void Start () {

	}

	bool CheckAim()
	{
		bool isAimed = false;

		Ray rayOrigin = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2+20,0));
		RaycastHit[] hits = Physics.RaycastAll(rayOrigin);
		foreach(RaycastHit hit in hits)
		{
			Transform hitTrans = hit.collider.transform;
			if(attackList.Contains(hitTrans))
			{
				isAimed = true;
//				Debug.Log( "SphereCast Hit : " + hitTrans.name);
			}
		}
		return isAimed;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(playerInfo.isAI) return;
		if(Camera.main)
		{
			if(myPhotonView.isMine)
			{
				if(CheckAim())
					roomMenu.isAimed = true;
				else
					roomMenu.isAimed = false;
			}
		}
	}

	void IgnoreCollision(bool ignore)
	{
		UnityEngine.Object[] CCs = GameObject.FindObjectsOfType(typeof(CharacterController));

		if(ignore)
		{
			foreach(CharacterController CC in CCs)
			{
				if(parentTrans.GetComponent<Collider>() != CC && CC.enabled)
				{
					Physics.IgnoreCollision(parentTrans.GetComponent<Collider>(),CC);
				}
			}
		}
		else
		{
			foreach(CharacterController CC in CCs)
			{
				if(parentTrans.GetComponent<Collider>() != CC && CC.enabled)
				{
					Physics.IgnoreCollision(parentTrans.GetComponent<Collider>(),CC,false);
				}
			}
		}

	}
	
	bool IsAIButNotMasterClients()
	{
		if(playerInfo.isAI&&!PhotonNetwork.isMasterClient)
			return true;
		else
			return false;
	}

	public void RangeSkillInput(TP_Animator.Skilltype skillType, RangeColliderAttack RCA, int force, TP_Animator.HitWays hitWay)
	{
		if(skillType==TP_Animator.Skilltype.Attack||skillType==TP_Animator.Skilltype.Darkman_Freeze||skillType==TP_Animator.Skilltype.Mars_Stun||skillType==TP_Animator.Skilltype.Persia_Curse)
		{
			List<Transform> skillAttackList = RCA.attackList;
			tempList.Clear();
			foreach(Transform enemy in skillAttackList)
			{
				if(enemy!=null)
					tempList.Add(enemy);
			}

			foreach(Transform enemy in tempList)
			{
				Debug.Log("enemy: " + enemy.name);
				PhotonView enemyPV = enemy.GetComponent<PhotonView>();
				if(enemyPV!=null)
				{
					int enemyID = enemyPV.viewID;
					TP_Info enemyInfo = enemy.GetComponent<TP_Info>();
					switch(skillType)
					{
					case TP_Animator.Skilltype.Attack:
						RangeAttack(enemyInfo,enemyID,force, hitWay);
						break;
					case TP_Animator.Skilltype.Darkman_Freeze:
						RangeFreeze(enemyID,force);
						break;
					case TP_Animator.Skilltype.Mars_Stun:
						RangeDizzyFire(enemyID,force);
						break;
					case TP_Animator.Skilltype.Persia_Curse:
						RangePersiaDecreaseDefense(enemyID,force);
						break;
					/*case TP_Animator.Skilltype.DecreseDefense:
						break;*/
					}
				}
			}
		}
		else if(skillType==TP_Animator.Skilltype.Darkman_Spurt)
		{
			if(IsAIButNotMasterClients())
				return;
			StartCoroutine(SpurtMove(10,RCA,force,0.4f));
		}
		else if(skillType==TP_Animator.Skilltype.Steropi_RollingStrike)
		{
			if(IsAIButNotMasterClients())
				return;
			StartCoroutine(SpurtMove(15,RCA,force,0.4f));
		}
		else if(skillType==TP_Animator.Skilltype.Mars_Assault)
		{
			if(IsAIButNotMasterClients())
				return;
			StartCoroutine(SpurtMove(10,RCA,force,0.4f));
		}
		else if(skillType==TP_Animator.Skilltype.Darkman_AddDefense||skillType==TP_Animator.Skilltype.Hyperion_AddDefense)
		{
			RoleAddDefense(force,playerInfo.Role);
		}
		else if(skillType==TP_Animator.Skilltype.Hyperion_Unbeatable)
		{
			HyperionUnbeatable(force);
		}
		else if(skillType==TP_Animator.Skilltype.Mars_AddAttack||skillType==TP_Animator.Skilltype.Steropi_AddAttack)
		{
			RoleAddAttack(force,playerInfo.Role);
		}
		else if(skillType==TP_Animator.Skilltype.AddHealth||skillType==TP_Animator.Skilltype.Theia_AddDefense)
		{
			List<Transform> skillHelpList = RCA.TeammateList;
			tempList.Clear();
			foreach(Transform teammate in skillHelpList)
			{
				if(teammate!=null)
					tempList.Add(teammate);
			}
			foreach(Transform teammate in tempList)
			{
				Debug.Log("teammate: " + teammate.name);
				int teammatePlayerID = teammate.GetComponent<PhotonView>().viewID;
				TP_Info teammateInfo = teammate.GetComponent<TP_Info>();
				switch(skillType)
				{
				case TP_Animator.Skilltype.AddHealth:
					RangeAddHealth(teammatePlayerID,force);
					break;
				case TP_Animator.Skilltype.Theia_AddDefense:
					RangeTheiaAddDefense(teammatePlayerID,force);
					break;
				}
			}
		}
		if(skillType!=TP_Animator.Skilltype.Darkman_Spurt&&skillType!=TP_Animator.Skilltype.Steropi_RollingStrike&&skillType!=TP_Animator.Skilltype.Mars_Assault)
		{
			RCA.ClearList();
			RCA.gameObject.SetActive(false);
		}
		else
		{
			StartCoroutine(WaitClearList(RCA));
		}


	}

	IEnumerator WaitClearList(RangeColliderAttack RCA)
	{
		yield return new WaitForSeconds(2);
		RCA.ClearList();
		RCA.gameObject.SetActive(false);
	}

	void RangeAttack(TP_Info enemyInfo, int enemyID, int force, TP_Animator.HitWays hitWay)
	{
		if(IsAIButNotMasterClients())
			return;
		if((int)enemyInfo.GetVital((int)VitalName.Health).CurValue > 0)
		{
			if(myPhotonView.isMine)
				roomMenu.Hit(myPhotonView.viewID,enemyID,force,hitWay,HitSound.None);
		}
	}

	void RangeFreeze(int enemyID, int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.Freeze(myPhotonView.viewID,enemyID,force);
	}

	void RangeDizzyFire(int enemyID, int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.DizzyFire(myPhotonView.viewID,enemyID,force);
	}

	void RangeSpurtAttack(RangeColliderAttack RCA, int force)
	{
		List<Transform> skillAttackList = RCA.attackList;
		IgnoreCollision(false);
		tempList.Clear();
		foreach(Transform enemy in skillAttackList)
		{
			if(enemy!=null)
				tempList.Add(enemy);
		}
		
		foreach(Transform enemy in tempList)
		{
			Debug.Log("enemy: " + enemy.name);
			PhotonView enemyPV = enemy.GetComponent<PhotonView>();
			if(enemyPV!=null)
			{
				int enemyID = enemyPV.viewID;
				TP_Info enemyInfo = enemy.GetComponent<TP_Info>();

				RangeAttack(enemyInfo,enemyID,force,TP_Animator.HitWays.KnockDown);
			}
		}
	}

	IEnumerator SpurtMove(float distance, RangeColliderAttack RCA, int force, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		Vector3 parentPos = parentTrans.position;
		float movement = parentPos.z;
		float goal = movement - distance;
		float speed = 1.5f;	
		float rate = 1.4f;
		Vector3 dir = parentTrans.forward;

		IgnoreCollision(true);

		while(movement>goal)
		{
			movement = movement - rate;

			if(movement<=goal)
			{
				RangeSpurtAttack(RCA,force);
				StopCoroutine("SpurtMove");
			}

			CC.Move(dir * speed);

			yield return null;
		}
		RCA.ClearList();
	

	}

	void RangeAddHealth(int teammatePlayerID, int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.Addhealth(myPhotonView.viewID,teammatePlayerID,force);
	} 

	void RangeTheiaAddDefense(int teammatePlayerID, int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.TheiaAddDefense(myPhotonView.viewID,teammatePlayerID,force);
	}

	void RangePersiaDecreaseDefense(int enemyID, int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.PersiaDecreaseDefense(myPhotonView.viewID,enemyID,force);
	}

	void RoleAddDefense(int force,TP_Info.CharacterRole role)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.RoleAddDefense(myPhotonView.viewID,myPhotonView.viewID,force,role);
	}

	void RoleAddAttack(int force,TP_Info.CharacterRole role)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.RoleAddAttack(myPhotonView.viewID,myPhotonView.viewID,force,role);
	}

	void HyperionUnbeatable(int force)
	{
		if(IsAIButNotMasterClients())
			return;
		if(myPhotonView.isMine)
			roomMenu.HyperionUnbeatable(myPhotonView.viewID,myPhotonView.viewID,force);
	}

	void MeleeAttackCheck(float attDistance, float attAngle, int force, TP_Animator.HitWays hitWay)
	{
		if(IsAIButNotMasterClients())
			return;
		for(int cnt=0;cnt<attackList.Count;cnt++)
		{
			Transform enemy = attackList[cnt];
			
			if(enemy==null)
				attackList.Remove(enemy);
			else
			{
				if(enemy!=parentTrans)
				{
					Vector3 targetDir = enemy.transform.position - parentTrans.position;
					
					Vector3 dir = targetDir.normalized;
					float angle = Vector3.Dot(dir,parentTrans.forward);
//					Debug.Log("Name:" + enemy.name +  " Dis:" + Vector3.Magnitude(targetDir) + "A:" + angle);

					if(Vector3.Magnitude(targetDir)<= attDistance && angle>attAngle)
					{
						if(!tempList.Contains(enemy))
							tempList.Add(enemy);
					}
				}
			}
		}
		
		if(tempList.Count>0)
		{
			HitSound sound = HitSound.None;
			if(playerInfo.Role==TP_Info.CharacterRole.Mars)
			{
				sound = HitSound.Sword1;
			}
			else if(playerInfo.Role==TP_Info.CharacterRole.DarkMan)
			{
				sound = HitSound.Sword2;
			}
			else if(playerInfo.Role==TP_Info.CharacterRole.Steropi)
			{
				sound = HitSound.Fist;
			}
			else if(playerInfo.Role==TP_Info.CharacterRole.Hyperion)
			{
				sound = HitSound.Slap;
			}
			for(int cnt=0;cnt<tempList.Count;cnt++)
			{
				foreach(Transform enemy in tempList)
				{
					//Transform enemy = attackList[cnt];
					if(enemy==null)
						tempList.Remove(enemy);
					else
					{
							PhotonView enemyPV = enemy.GetComponent<PhotonView>();
						if(enemyPV!=null)
						{
							int enemyID = enemyPV.viewID;
							TP_Info enemyInfo = enemy.GetComponent<TP_Info>();
							if((int)enemyInfo.GetVital((int)VitalName.Health).CurValue > 0)
							{
								Debug.Log(enemyInfo.gameObject.name + " got hit!");
								if(myPhotonView.isMine)
									roomMenu.Hit(myPhotonView.viewID,enemyID,force,hitWay,sound);
							}
						}
					}
				}
			}
		}
		tempList.Clear();
	}
	

	IEnumerator MeleeAttackCheckAfterSnapToCamera(float attDistance, float attAngle, int force, TP_Animator.HitWays hitWay)
	{
		while((Camera.main.transform.eulerAngles.y - parentTrans.eulerAngles.y)>5)
		{
			yield return new WaitForSeconds(0.005f);
		}
			MeleeAttackCheck(attDistance, attAngle, force, hitWay);
			StopCoroutine("MeleeAttackCheckAfterSnapToCamera");
	}

	public void MeleeAttackInput(float attDistance, float attAngle, int force, TP_Animator.HitWays hitWay)
	{
		tempList.Clear();
		if(IsAIButNotMasterClients())
			return;
		if(!playerInfo.isAI)
			StartCoroutine(MeleeAttackCheckAfterSnapToCamera(attDistance, attAngle, force, hitWay));
		else
			MeleeAttackCheck(attDistance, attAngle, force, hitWay);
		
	}

	public void Persia_ThunderousWaveInput(int force)
	{	
		playerInfo.PersiaThunderousWaveInput(force);
	}

	public void Steropi_SnowBallInput(int force)
	{	
		playerInfo.SteropiSnowBallInput(force);
	}

	public void Mars_FightLightInput(int force)
	{	
		playerInfo.MarsFightLightInput(force);
	}


	public void Hyperion_HotRockInput(int force)
	{	
		playerInfo.HyperionHotRockInput(force);
	}

	public IEnumerator MagicAttackInput(int force)
	{
		yield return new WaitForSeconds(0.2f);
		playerInfo.MagicAttackInput(force);
	}

	public IEnumerator StoredMagicAttackInput(int force)
	{
		yield return new WaitForSeconds(0.2f);
		playerInfo.StoredMagicAttackInput(force);
	}
	
	void OnGUI()
	{
		if(WholeGameManager.SP.InGame)
		{
			Vector3 ObjToScr = Vector3.zero;
			if(Camera.main!=null)
			{
				if(gameObject!=null)
				{
					ObjToScr = Camera.main.WorldToScreenPoint(gameObject.transform.position);
				}
			}
			if(playerInfo!=null)
			{
				//GUI.Label(new Rect(ObjToScr.x , Screen.height - ObjToScr.y , 200f, 60f), playerInfo.GetVital((int)VitalName.Health).CurValue.ToString());
				//GUI.Label(new Rect(ObjToScr.x , Screen.height - ObjToScr.y -20 , 200f, 60f), playerInfo.GetVital((int)VitalName.Mana).CurValue.ToString());
			}
		}
	}

	#region tools for debuging attackDistance and attackAngle
	void DebugAttack()
	{
		Vector3 targetDir = attackList[0].position - transform.parent.position;
		Debug.Log("Distance: " + Vector3.Magnitude(targetDir) + "Angle: " + Vector3.Angle(targetDir,transform.forward));
	}
	#endregion

	#region trigger area for detecting enemy 
	void OnTriggerEnter(Collider enemy)
	{

		if(enemy.transform!=parentTrans)
		{
			if(parentTrans.tag=="team1")
			{
				if(enemy.tag=="team2"||enemy.tag=="monster")
				{
					if(!attackList.Contains(enemy.transform))
						attackList.Add(enemy.transform);
//					Debug.Log(enemy.name);
				}
			}
			else if(parentTrans.tag=="team2")
			{
				if(enemy.tag=="team1"||enemy.tag=="monster")
				{
					if(!attackList.Contains(enemy.transform))
						attackList.Add(enemy.transform);
				}
			}
			else if(parentTrans.tag=="monster")
			{
				if(enemy.tag=="team1"||enemy.tag=="team2")
				{
					if(!attackList.Contains(enemy.transform))
						attackList.Add(enemy.transform);
				}
			}
		}
	}

	void OnTriggerExit(Collider enemy)
	{
		if(parentTrans.tag=="team1")
		{
			if(enemy.tag=="team2"||enemy.tag=="monster")
				attackList.Remove(enemy.transform);
		}
		else if(parentTrans.tag=="team2")
		{
			if(enemy.tag=="team1"||enemy.tag=="monster")
				attackList.Remove(enemy.transform);
		}
		else if(parentTrans.tag=="monster")
		{
			if(enemy.tag=="team1"||enemy.tag=="team2")
				attackList.Remove(enemy.transform);
		}

	}
	#endregion
}
