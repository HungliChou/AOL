/// <summary>
/// Battle scene.cs
/// Leo Chou
/// 
/// This is the singleton script for dealing with event between player and enemy
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleScene : MonoBehaviour {
	public static BattleScene Instance;
	public GameObject playerPrefab;
	private static GameObject _player;
	private GameObject _enemy;
	private delegate void GetHitDelegate();
	//private static event GetHitDelegate GetHitListener;
	public int _enemyCount;
	
	public enum Weapon_Type{
		None,Sword
	}
	
	public enum Message_Type
	{
		PlayerGetHit, EnemyGetHit
	}
	
	void Awake(){
		Instance = this;
		_enemyCount = 0;
		
	}
	void Start () {	
		_player = Instantiate(playerPrefab, transform.position, Quaternion.identity) as GameObject;
		_player.transform.parent = transform;
		_player.name = "Player";
	}
	
	public GameObject Player
	{
		get{return _player;}
		set{_player = value;}
	}
	
	public GameObject Enemy
	{
		get{return _enemy;} 
		set{_enemy = value;}
	}
	
	public int EnemyCount
	{
		get{return _enemyCount;}
		set{_enemyCount = value;}
	}
	
	public void SendGameMessage<T>(Message_Type type, Weapon_Type weapon)
	{
		switch(type)
		{
		case Message_Type.EnemyGetHit:
			if(_enemy)
				_enemy.SendMessage("GetHit", weapon);
			break;
		}
	}
	public void SendGameMessageToPlayer<T>(Message_Type type, int damage)
	{
		switch(type)
		{
		case Message_Type.PlayerGetHit:
			if(_player)
				_player.SendMessage("GetHit", damage);
			break;
		}
	}
	
	/*public void EnemyGetHit(Weapon_Type weapon, Vector3 hitpoint, string hitpart)
	{
		if(_enemy!=null)
			_enemy.GetComponent<EnemyAI>().GetHit(weapon,hitpoint,hitpart);
	}*/
	
	/*public bool IsAttackArea()
	{
		if(_player && _enemy)
		{
			CharacterController playerController = _player.GetComponent<CharacterController>();
			RVOController enemyController = _enemy.GetComponent<RVOController>();
			float dist = Vector3.Distance(_player.transform.position, _enemy.transform.position);
			if(dist <= enemyController.radius + playerController.radius + 1.0f)
				return true;
		}
		return false;
	}*/
}
