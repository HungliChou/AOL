using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterRoleEff
{
	None,Mars,DarkMan,Theia,Persia,Hyperion,Steropi,Other,Buff
}

public enum EffectName
{
	None,Theia_CrossFire,Theia_Meteorite,Theia_Pray,Theia_Crusade,Theia_Doom,
	Mars_AresBlessing,Mars_Assault,Mars_FightLight,Mars_HolyRedeemer,Mars_Burn,Mars_Dizzy,
	Persia_ThunderousWave,Persia_Hail,Persia_GroundedLightning,Persia_Curse,Persia_ScatterSpear,Persia_ScatterSpear2,
	Darkman_Spurt,Darkman_IceFreeze,Darkman_TornadoLightning,Darkman_DemonProtection,Darkman_DepravedCrush,Darkman_Frost,
	Steropi_SnowBall,Steropi_RollingStrike,Steropi_SlamCrush,Steropi_Roar,Steropi_LightningStorm,
	Hyperion_HotRock,Hyperion_Unbeatable,Hyperion_BombSpreading,Hyperion_Intimadate,Hyperion_SpiningLight,
	Mars_StoreEnergy,Darkman_StoreEnergy,Theia_StoreEnergy,Persia_StoreEnergy,Hyperion_StoreEnergy,Steropi_StoreEnergy,
	Other_HealthWater,Other_ManaWater,Other_LightGodPower,Other_DarkGodPower,Other_LevelUp,
	Buff_SpeedUp,Buff_AttackUp,Other_GodWeapon
}

[System.Serializable]
public class Effects{
	[SerializeField]private CharacterRoleEff roleName;
	[SerializeField]public List<Effect> effectPrefs;
	
	public Effects()
	{
		effectPrefs = new List<Effect>();
	}
}

[System.Serializable]
public class Effect{
	[SerializeField]private EffectName effectName;
	[SerializeField]public GameObject effectPref;
	
	public Effect()
	{

	}
}
