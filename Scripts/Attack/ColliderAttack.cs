	using UnityEngine;
using System.Collections;

public class ColliderAttack : Photon.MonoBehaviour {
	
	public Transform parentTrans;
	public PhotonView playerView;
	public string _enemyTeam;
	public int _force;
	public string EnemyTeam{get{return _enemyTeam;}set{_enemyTeam = value;}}
	public int Force{get{return _force;}set{_force = value;}}
	public float lifetime;
	public GameObject explosion;
	private bool isExploded;

	void Start()
	{
		isExploded = false;
		Destroy(gameObject,lifetime);
	}

	void CreateExplosion()
	{
		//create explosion
		if(explosion!=null)
		{
			GameObject clone;
			clone = (GameObject)Instantiate(explosion,transform.position,Quaternion.identity);
			Destroy(clone,1);
			Destroy(gameObject);
			isExploded = true;
		}
	}
	
	void OnTriggerEnter(Collider enemy)
	{
		string enemyTag = enemy.tag;
		if(enemy.transform!=parentTrans	&& enemyTag!="team1Light"&&enemyTag!="team2Light")
		{
			{
				if(enemyTag == _enemyTeam || enemyTag=="monster")
				{
					if(!isExploded)
					{
						CreateExplosion();
					}
					Debug.Log(enemy.name);
					PhotonView enemyPV = enemy.GetComponent<PhotonView>();
					if(enemyPV!=null)
					{
						//hit
						int enemyID =enemyPV.viewID;
						TP_Info enemyInfo = enemy.GetComponent<TP_Info>();
						if((int)enemyInfo.GetVital((int)VitalName.Health).CurValue > 0)
						{
							if(playerView.isMine)
							{
								InRoom_Menu.SP.Hit(playerView.viewID,enemyID, _force,TP_Animator.HitWays.BeHit, HitSound.None);
							}
							//roomMenu.Hit(enemyPlayer,force);
						}
					}
				}
				else
				{

					if(!isExploded)
					{
						CreateExplosion();
					}
				}
			}
		}
	}
}
