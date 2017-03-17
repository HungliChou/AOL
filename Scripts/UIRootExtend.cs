﻿using UnityEngine;
using System.Collections;

public class UIRootExtend : MonoBehaviour {
	
	public int ManualWidth = 1080;
	public int ManualHeight = 1920;
	
	private UIRoot _UIRoot;
	
	void Awake()
	{
		_UIRoot = this.GetComponent<UIRoot>();
	}
	
	void FixedUpdate()
	{
		if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
			_UIRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
		else
			_UIRoot.manualHeight = ManualHeight;
	}
}