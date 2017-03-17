/// <summary>
/// Vital.cs
/// Leo Chou
/// 
/// This class contain all the extra functions for a characters vitals
/// </summary>
using UnityEngine;
[System.Serializable]
public class Vital : ModifiedStat {

	[SerializeField]private float _curValue;	//This is the current value
	[SerializeField]private VitalName _name;	//This is the name of the vital
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Vital"/> class.
	/// </summary>
	public Vital(){
		_curValue = 0f;
		ExpToLevel = 50;
		LevelModifier = 1.1f;
	}
	
	/// <summary>
	/// When getting the _curvalue, make sure that it is not greater than our AdjustedBaseValue
	/// If it is, make it the same as our AdjustedBaseValue
	/// </summary>
	/// <value>
	/// The current value.
	/// </value>
	public float CurValue{
	    get{
			if(_curValue > AdjustedMaxValue)   //EX: 100% health
				_curValue = AdjustedMaxValue; 
			return _curValue;
		}
		set{ _curValue = value;}
	}

	public VitalName Name{
		get{return _name;}
		set{_name = value;}
	}
}

public enum VitalName{
	Health,
	Energy,
	Mana
}