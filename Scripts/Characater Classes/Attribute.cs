/// <summary>
/// Attribute.cs
/// Leo Chou
/// 
/// This is the class for all of the character attributes in-game.
/// </summary>
using UnityEngine;
[System.Serializable]
public class Attribute : BaseStat {
	new public const int STARTING_EXP_COST = 50;  //This is the starting cost for all of our attribute
	[SerializeField]private AttributeName _name;  //This is the name of the attribute
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Attribute"/> class.
	/// </summary>
	public Attribute(){
		ExpToLevel = 50;
		LevelModifier = 1.05f;		
	}
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public AttributeName Name{
		get{return _name;}
		set{_name = value;}
	}
}

public enum AttributeName{
	Strength,
	Nimbleness,
	Constitution,
	Concentration,
	Willpower,
}