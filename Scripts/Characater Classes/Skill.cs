/// <summary>
/// Skill.cs
/// Leo Chou
/// 
/// This class contain all the extra functions that are needed for the skill
/// </summary>
using UnityEngine;
[System.Serializable]
public class Skill : ModifiedStat {
	[SerializeField]private float _curValue;	//This is the current value
	[SerializeField]private bool _known;	//Does player learned this skill or not
	[SerializeField]private float _cDTime;
	[SerializeField]private int _mana;
	/// <summary>
	/// Initializes a new instance of the <see cref="Skill"/> class.
	/// </summary>
	public Skill(){
		_known = false;
		ExpToLevel = 25;
		LevelModifier =1.1f;
		_cDTime = 10f;
		_mana = 10;
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Skill"/> is known.
	/// </summary>
	/// <value>
	/// <c>true</c> if known; otherwise, <c>false</c>.
	/// </value>
	public float CurValue{
	    get{
			if(_curValue > AdjustedBaseValue)   
				_curValue = AdjustedBaseValue; 
			return _curValue;
		}
		set{ _curValue = value;}
	}
	
	public bool Known{
		get{return _known;}
		set{_known = value;}
	}

	public float CDTime{
		get{return _cDTime;}
		set{_cDTime = value;}
	}

	public int Mana{
		get{return _mana;}
		set{_mana = value;}
	}

}

[System.Serializable]
public class MeleeSkill : Skill{
	[SerializeField]private MeleeSkillName _meleeName;//This is the name of the meleeskill
	
	public MeleeSkillName MeleeName
	{
		get{return _meleeName;}
		set{_meleeName = value;}
	}
}
[System.Serializable]
public class MagicSkill : Skill{
	[SerializeField]private MagicSkillName _magicName;//This is the name of the magicskill
	
	public MagicSkillName MagicName
	{
		get{return _magicName;}
		set{_magicName = value;}
	}
}
[System.Serializable]
public class BuffSkill : Skill{
	[SerializeField]private BuffSkillName _buffName;//This is the name of the buffskill
	
		public BuffSkillName BuffName
	{
		get{return _buffName;}
		set{_buffName = value;}
	}
}
[System.Serializable]
public class SuperSkill : Skill{
	[SerializeField]private SuperSkillName _superName;//This is the name of the buffskill

	public SuperSkillName SuperName
	{
		get{return _superName;}
		set{_superName = value;}
	}
}

public enum MeleeSkillName
{
	Melee_Offense001
}

public enum MagicSkillName
{
	Darkman_01,Darkman_02,Darkman_03,Darkman_04,
	Theia_CrossFire,Theia_Meteorite,Theia_Pray,Theia_Crusade,
	Persia_ThunderousWave,Persia_Hail,Persia_GroundedLightning,Persia_Curse,
	Steropi_SnowBall,Steropi_RollingStrike,Steropi_SlamCrush,Steropi_Roar,
	Hyperion_HotRock,Hyperion_Unbeatable,Hyperion_BombSpreading,Hyperion_Intimadate,
	Mars_AresBlessing,Mars_Assault,Mars_FightLight,Mars_Stun
}

public enum BuffSkillName
{
	None,Buff_Speed,Buff_Power
}

public enum SuperSkillName
{
	DarkMan,Mars,Persia,Theia,Steropi,Hyperion
}