using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void AttackStateDelegate(string name);

public class Player_Attack : MonoBehaviour {
	public static Player_Attack Instance;
	//public AttackStateDelegate setAttackState = null;
	
	//Declare weapon's state
	private float attackDistance;
	private float attackAngle;
	public BattleScene.Weapon_Type UsingWeapon;
	
	public bool IsAttack; //if true: star check collision
	public bool CheckDistance;
	public List<string> enemylist;//record those enemies got hit to avoid double collision check
	
	void Awake() 
	{
		Instance = this;
		IsAttack = false;
		CheckDistance = false;
		enemylist = new List<string>();
		enemylist.Clear();
	}
	
	// Use this for initialization
	void Start () {
		UsingWeapon = BattleScene.Weapon_Type.None;
	}
	
	// Update is called once per physic frame
	void FixedUpdate() 
	{	
		if(IsAttack)
		{
			CheckDistance = false;
			SetAttackState();
			CheckDistanceCollision();
		}
		else
		{
			ClearAttackState();
		}
	}
	
	//Assign weapon's distence & angle based on type of weapon
	void DetermineWeaponRange()
	{
		switch(UsingWeapon)
		{
		case BattleScene.Weapon_Type.Sword:
			attackDistance = 2.8f;//depends on Sword Size
			attackAngle = -0.3f;
			break;
		}
	}
	
	//Set Weapon's state then start checking weapon's collision
	void SetAttackState()
	{
		if(attackDistance == 0)
			DetermineWeaponRange();
	}
	
	//After stop checking collision, reset Weapon's state 
	void ClearAttackState()
	{
		UsingWeapon = BattleScene.Weapon_Type.None;
		attackDistance = 0;
		attackAngle = 0;
		CheckDistance = false;
		enemylist.Clear();
	}
	
	private void CheckDistanceCollision(){
		//Distance and Angle Checking method
		GameObject[] enemies;
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		
		foreach(GameObject enemy in enemies)
		{
			Vector3 dir = (enemy.transform.position - transform.position).normalized;
			float direction = Vector3.Dot(dir,transform.forward);
			//Debug.Log(Vector3.Distance(transform.position, enemy.transform.position));
			if(enemy!=null && !enemylist.Contains(enemy.name) && Vector3.Distance(transform.position, enemy.transform.position)<attackDistance && direction>attackAngle)
			{
				//enemy.GetComponent<EnemyAI>().GetHit(BattleScene.Weapon_Type.Sword);
				BattleScene.Instance.Enemy = enemy;
				BattleScene.Instance.SendGameMessage<GameObject>(BattleScene.Message_Type.EnemyGetHit,BattleScene.Weapon_Type.Sword);
				enemylist.Add(enemy.name);
			}
		}
		CheckDistance = true;
	}
	
	public void GetHit(int damage)
	{
		Debug.Log("Get Damage : " + damage);
	}
}
