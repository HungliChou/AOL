/// <summary>
/// BuffCondition.cs
/// Leo Chou
/// 
/// This class contain all the condition of buff
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;
[System.Serializable]
public class BuffCondition {
	[SerializeField]private BuffConditionName _buffName;//This is the name of the buff
	[SerializeField]private float _duration;
	[SerializeField]private float _timer;
	[SerializeField]private float _force;
	[SerializeField]private List<AffectedAbility> _affectedBuff;//This are the names of the ability that be affected

	public BuffConditionName BuffName{get{return _buffName;}set{_buffName = value;}}
	public float Duration{get{return _duration;}set{_duration = value;}}
	public float Timer{get{return _timer;}set{_timer = value;}}
	public float Force{get{return _force;}set{_force = value;}}
	public List<AffectedAbility> AffectedBuff{get{return _affectedBuff;}set{_affectedBuff = value;}}

	/// <summary>
	/// Initializes a new instance of the BuffCondition class.
	/// </summary>
	public BuffCondition(float duration){
		_duration = duration;
		_timer = duration;
		_force = 0;
		_affectedBuff = new List<AffectedAbility>();
	}

	public void AddAffectAbility(AffectedAbility abi)
	{
		_affectedBuff.Add(abi);
	}

	public void Freeze(TP_Animator player,bool freeze)
	{
		if(freeze)
		{
			player.Freeze();
			if(player.Mode == TP_Animator.CharacterMode.Skilling)
			{
				player.Mode = TP_Animator.CharacterMode.None;
				player.playerController.SkillType = 0;
			}
		}
		else
		{
			if(player.State!=TP_Animator.CharacterState.Dead)
			{
				player.State = TP_Animator.CharacterState.Idle;
				player.Idle();
			}
		}
	}

	public void Dizzy(TP_Animator player, bool dizzy)
	{
		if(dizzy)
		{
			player.Dizzy();
			if(player.Mode == TP_Animator.CharacterMode.Skilling)
			{
				player.Mode = TP_Animator.CharacterMode.None;
				player.playerController.SkillType = 0;
			}
		}
		else
		{
			if(player.State!=TP_Animator.CharacterState.Dead)
			{
				player.State = TP_Animator.CharacterState.Idle;
			}
		}
	}

}

[System.Serializable]
public class AffectedAbility{
	[SerializeField]private AbilityName _abiName;//This is the name of the ability
	[SerializeField]private int _force;

	public AbilityName AbiName{get{return _abiName;}set{_abiName = value;}}
	public int Force{get{return _force;}set{_force = value;}}

	public AffectedAbility(){
	
	}
}

public enum BuffConditionName
{
	Theia_AddDefense,Darkman_Freeze,Darkman_AddDefense,Mars_DizzyFire,Mars_AddAttack,Persia_DecreaseDefense,Hyperion_Unbeatable,Hyperion_Intimidate,Steropi_Roar,HealthWater,ManaWater,Buff_SpeedUp,Buff_AttackUp
}