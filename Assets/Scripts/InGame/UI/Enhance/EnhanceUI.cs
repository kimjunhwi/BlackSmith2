﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class EnhanceUI : MonoBehaviour {

	public int nLevel = 0;
	public float fCostGold = 0.0f;

	public float fEnhanceValue = 0.0f;

	public string strEnhanceName = null;

	public Text EnhanceText;
	public Text CostGoldText;
	public Image EnhanceImage;
	public Button EnhanceButton;

	public Player cPlayer;

	public SmithEnhance[] enhanceDatas;

	public RepairObject repairObject;

	protected virtual void Awake()
	{
		cPlayer = GameManager.Instance.player;

		enhanceDatas = GameManager.Instance.cSmithEnhance;

		EnhanceButton.onClick.AddListener (EnhanceButtonClick);
	}

	protected virtual void EnhanceButtonClick() { }

}
