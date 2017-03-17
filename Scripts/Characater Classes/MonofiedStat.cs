/// <summary>
/// ModifiedStat.cs
/// Leo Chou
/// 
/// This is the base class for all stats that will be modifiable by attributes
/// </summary>
using UnityEngine;
using System.Collections.Generic;              //Generic was added so we can use the List<>
[System.Serializable]
public class ModifiedStat : BaseStat {
	[SerializeField]private List<ModifyingAttribute> _mods;    //A list of Attribute that modify this stat
	[SerializeField]private int _modValue;                      //The amount added to the baseValue from the modifiers
	[SerializeField]private int _damageValue;	//This is the damage value
	[SerializeField]private int _maxValue;	//This is the current value
	[SerializeField]private List<ModifyingAbility> _mods_abi;
	[SerializeField]private int _modValue_abi;

	public int DamageValue{
		get{return _damageValue;} set{_damageValue = value;}
	}

	public int MaxValue{
		get{return _maxValue;} set{_maxValue = value;}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ModifiedStat"/> class.
	/// </summary>
	public ModifiedStat(){
		_mods = new List<ModifyingAttribute>();
		_modValue = 0;
		_mods_abi = new List<ModifyingAbility>();
		_modValue_abi = 0;
	}
	/// <summary>
	/// Add a modifyingAttribute to our list of mods for this ModifiedStat.
	/// </summary>
	/// <param name='mod'>
	/// Mod.
	/// </param>
	public void AddModifier(ModifyingAttribute mod){
		_mods.Add(mod);
	}
	
	public void AddModifier_Abi(ModifyingAbility mod){
		_mods_abi.Add(mod);
	}
	
	/// <summary>
	/// Reset _modValue to 0.
	/// Check to see if we have at least one modifyingAttribute in our list mods.
	/// If we do, then interate through the list and add the AdjustedBaseValue * ratio to our _modValue.
	/// </summary>
	private void CalculateModValue(){
		_modValue = 0;
		
		if(_mods.Count > 0)
			foreach(ModifyingAttribute att in _mods)
				_modValue += (int)(att.attribute.AdjustedBaseValue * att.ratio);
		
		_modValue_abi = 0;
		
		if(_mods_abi.Count > 0)
			foreach(ModifyingAbility abi in _mods_abi)
				_modValue_abi += (int)(abi.ability.AdjustedBaseValue * abi.ratio);
	}
	
	/// <summary>
	/// This function is overriding the AdjustedBaseValue in the BaseStat class.
	/// Calculate the AdjustedBaseValue from the BaseValue + BuffValue + +modValue
	/// </summary>
	/// <value>
	/// The adjusted base value.
	/// </value>
	/// "be used in CharacterGenerator to show the value"
	public new int AdjustedBaseValue {
		get{ return BaseValue + BuffValue + _modValue +_modValue_abi - _damageValue; }
	}

	public new int AdjustedMaxValue {
		get{ return BaseValue + BuffValue + _modValue +_modValue_abi; }
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update(){
		CalculateModValue();
	}
	
	public string GetModifyingAttributesString()
    {
        string temp = "";

        for(int cnt = 0; cnt < _mods.Count; cnt++)
        {
            temp += _mods[cnt].attribute.Name;
            temp += "_";
            temp += _mods[cnt].ratio;

            if(cnt < _mods.Count - 1)//if we have more than one attribute
            {
            temp += "|";
            }

         }
         return temp;
     }
	
	public string GetModifyingAbilitiesString()
    {
        string temp = "";

        for(int cnt = 0; cnt < _mods_abi.Count; cnt++)
        {
            temp += _mods_abi[cnt].ability.Name;
            temp += "_";
            temp += _mods_abi[cnt].ratio;

            if(cnt < _mods_abi.Count - 1)//if we have more than one ability
            {
            temp += "|";
            }

         }
         return temp;
     }
}

/// <summary>
/// A structure that will hold an Attribute and a ratio that will be as a modifying attribute to our ModifiedStats
/// </summary>
public struct ModifyingAttribute
{
	public Attribute attribute;  // the attribute to be used as a modifier
	public float ratio;          // the percent of the attributes adjustedBaseValue that will be applied to the modifiedStat
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ModifyingAttribute"/> struct.
	/// </summary>
	/// <param name='att'>
	/// Att.   the attribute to be used
	/// </param>
	/// <param name='rat'>
	/// Rat.   the ratio to use
	/// </param>
	public ModifyingAttribute(Attribute att, float rat){
		attribute = att;
		ratio = rat;
	}
}

public struct ModifyingAbility
{
	public Ability ability;      // the ability to be used as a modifier
	public float ratio;          // the percent of the ablilties adjustedBaseValue that will be applied to the modifiedStat
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ModifyingAbility"/> struct.
	/// </summary>
	/// <param name='att'>
	/// Abi.   the ability to be used
	/// </param>
	/// <param name='rat'>
	/// Rat.   the ratio to use
	/// </param>
	public ModifyingAbility(Ability abi, float rat){
		ability = abi;
		ratio = rat;
	}
}
