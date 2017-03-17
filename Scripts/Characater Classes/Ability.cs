using UnityEngine;
[System.Serializable]
public class Ability : ModifiedStat {
	[SerializeField]private int _curValue;		//This is the current Value
	[SerializeField]private AbilityName _name;	//This is the name of the ability
	
	public Ability(){
		_curValue = 0;
		ExpToLevel = 50;
		LevelModifier = 1.1f;
	}
	
	public int CurValue{
	    get{
			if(_curValue > AdjustedBaseValue)   //EX: 100% health
				_curValue = AdjustedBaseValue; 
			return _curValue;
		}
		set{ _curValue = value;}
	}
	
	 public AbilityName Name{
		get{return _name;}
		set{_name = value;}
	}
}

public enum AbilityName{
	MeleeAttack,
	AttackSpeed,
	MoveSpeed,
	Defense,
	Crits,
	SkillAttack,
	BuffPower,
	None
}