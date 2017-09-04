﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class EpicOption {

	//에픽 무기 인덱스 
	public int nIndex = 0;

	//세이브를 위한 일차
	public int nSaveDay = 0;

	//제작에 소비된 일차 
	public int nCostDay = 0;

	public bool bIsLock = false;

	public const float fDivisionDay = 0.1f;

	public const float fPlusOption = 50f;

	//에픽 무기 설명 
	public string strExplain = "";

	public Player cPlayerData;

	public virtual void Init(int _nDay,Player _player) { }

	public virtual string GetExplain() {return null;}

	public virtual bool CheckOption(){ return false; }

	public virtual void Relive(){ }
}
