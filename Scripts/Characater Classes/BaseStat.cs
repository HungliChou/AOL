using UnityEngine;
[System.Serializable]
public class BaseStat {
	public const int STARTING_EXP_COST = 100;
	
    [SerializeField]private int _baseValue;               //the base value of this stat
    [SerializeField]private int _buffValue;               //the amount of the buff to this stat
	[SerializeField]private int _expToLevel;              //the total amount of exp needed to raise the skill
	[SerializeField]private float _levelModifier;         //the modifier applied to the exp needed to raise the skill
	
	public BaseStat()
	{
		_baseValue = 0;
		_buffValue = 0;
		_levelModifier = 1.1f;
		_expToLevel = STARTING_EXP_COST;
	}
	
#region Basic setters and Getters
	/// <summary>
	/// Gets or sets the _bassValue.
	/// </summary>
	/// <value>
	/// The _baseValue.
	/// </value>
	public int BaseValue{
		get{ return _baseValue;}
		set{ _baseValue = value;}
	}
	/// <summary>
	/// Gets or sets the _buffValue.
	/// </summary>
	/// <value>
	/// The _buffValue.
	/// </value>
	public int BuffValue{
	    get{ return _buffValue;}
		set{ _buffValue = value;}
	}
	/// <summary>
	/// Gets or sets the _expToLevel.
	/// </summary>
	/// <value>
	/// The _expToLevel.
	/// </value>
	public int ExpToLevel{
	    get{ return _expToLevel;}
		set{ _expToLevel = value;}
	}
	/// <summary>
	/// Gets or sets the _levelModifier.
	/// </summary>
	/// <value>
	/// The _levelModifier.
	/// </value>
	public float LevelModifier{
	    get{ return _levelModifier;}
		set{ _levelModifier = value;}
	}
#endregion
	
    //we can use it if we want to level up
    /// <summary>
    /// Calculates the exp to level.
    /// </summary>
    /// <returns>
    /// The exp to level.
    /// </returns>
    private int CalculateExpToLevel(){
		return (int)(_expToLevel * _levelModifier);
	}	
	/// <summary>
	/// Assign the new value to _expTolevel and then increase the _baseValue by one.
	/// </summary>
	public void LevelUP(){
		_expToLevel = CalculateExpToLevel();
		_baseValue++;
	}
	/// <summary>
	/// Recaalculate the adjusted base value and return it.
	/// </summary>
	/// <value>
	/// The adjusted base value.
	/// </value>
	public int AdjustedBaseValue{
		get{ return _baseValue + _buffValue; }
	}
}

