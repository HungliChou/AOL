using UnityEngine;
using System.Collections;

public class WildTrigger : MonoBehaviour {

	public Transform MonsterBoss;
	public GameObject FaceTarget;

	void Start()
	{
		FaceTarget = transform.FindChild("FaceTarget").gameObject;
	}

	void OnTriggerEnter(Collider enemy)
	{
		if(enemy.tag=="team1"||enemy.tag=="team2")
		{
			if(MonsterBoss!=null)
			{
				MonsterScript MS = MonsterBoss.GetComponent<MonsterScript>();
				if(MS.Enemy==null)
					MS.Enemy = enemy.gameObject;
			}
		}
	}
	void OnTriggerExit(Collider enemy)
	{
		if(enemy.tag=="team1"||enemy.tag=="team2")
		{
			if(MonsterBoss!=null)
			{
				MonsterScript MS = MonsterBoss.GetComponent<MonsterScript>();
				if(MS.Enemy==enemy.gameObject)
					MS.Enemy = null;
			}
		}
	}
}
