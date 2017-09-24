﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class ResultEpicUI : MonoBehaviour {

	public Image WeaponImage;

	public Button SuccessedButton;
	public Button AdButton;

	public Text WeaponNameText;
	public Text RepairText;

	//기본 값
	const int m_nBasicGold = 1200;
	const int m_nBasicHonor = 300;
	const int m_nBasicMinRepair = 6;
	const int m_nBasicMaxRepair = 10;
	const int m_nBasicMinOption = 1;
	const int m_nBasicMaxOption = 3;

	//일차 별로 증가하는 값, 단 추가옵션과 보스옵션 같은 경우 10 레벨 마다 증가 한다.
	const int m_nPlusGoldPercent = 20;
	const int m_nPlusHonorPercent = 10;
	const int m_nPlusRepairMinPercent = 10;
	const int m_nPlusRepairMaxPercent = 10;
	const int m_nPlusOptionMinPercent = 10;
	const int m_nPlusOptionMaxPercent = 10;

	//Calc Data
	int nCalcGoldCost = 0;
	int nCalcHonorCost = 0;
	double dCalcMinRepair = 0;
	double dCalcMaxRepair = 0;
	int nCalcAddMinOption = 0;
	int nCalcAddMaxOption = 0;

	int nInsertValue;

	private string[] strBossExplains = {"명중률 증가" ,"수리력 증가", "크리확률 증가","알바 수리력 증가"};

	public Transform contentPanel;

	Player playerData;
	CreatorWeapon createWeapon = null;
	List<CGameMainWeaponOption> LIST_OPTION = new List<CGameMainWeaponOption> ();

	public MakingUI makingUI;
	public BossSoul[] BossSoulSlots;
	public SimpleObjectPool OptionPool;
	public EpicOption createEpic = null;

	void Awake()
	{
		playerData = GameManager.Instance.GetPlayer ();

		SuccessedButton.onClick.AddListener (Successed);
		AdButton.onClick.AddListener (ShowAdButton );

		gameObject.SetActive (false);
	}

	public void Init(Player _player, BossSoul[] _bossSoul)
	{
		gameObject.SetActive (true);

		AdButton.gameObject.SetActive (true);

		playerData = _player;

		BossSoulSlots = _bossSoul;

		CreateWeapon ();

		makingUI.CreateWeapon (createWeapon, createEpic, LIST_OPTION);

		RefreshDisplay ();
	}

	public void RefreshDisplay()
	{
		RemoveButtons();
		AddButtons();
	}

	private void RemoveButtons()
	{
		while (contentPanel.childCount > 0)
		{
			GameObject toRemove = contentPanel.GetChild(0).gameObject;
			OptionPool.ReturnObject(toRemove);
		}
	}

	//옵션 정렬 방식이다
	private void AddButtons()
	{
		//등급이 높은 것을 정렬
		LIST_OPTION.Sort(delegate(CGameMainWeaponOption A, CGameMainWeaponOption B)
			{
				if (A.nIndex < B.nIndex) return 1;
				else if (A.nIndex > B.nIndex) return -1;
				else return 0;
			});

		for (int i = 0; i < LIST_OPTION.Count; i++) {
			CGameMainWeaponOption item = LIST_OPTION [i];

			GameObject newButton = OptionPool.GetObject ();
			newButton.transform.SetParent (contentPanel, false);
			newButton.transform.localScale = Vector3.one;

			OptionScroll sampleButton = newButton.GetComponent<OptionScroll> ();
			sampleButton.Setting (item);
		}
	}

	public void CreateWeapon()
	{
		int nDight = 0;
		int nDightCost = playerData.GetDay ();

		float fCostDay = (float)nDightCost;

		while (fCostDay >= 1) 
		{
			fCostDay *= 0.1f;
			nDight++;
		}

		createWeapon = new CreatorWeapon ();

		int nDay = playerData.GetDay ();
		float fOriValue = nDay - 1;
		float fMinusValue = Mathf.Floor( (nDay - 1) * 0.1f ) * 10;
		float result = fOriValue - fMinusValue;

		double dCurComplete = 3 * Mathf.Max( Mathf.Pow (1.85f, (Mathf.Floor((nDay + 9) * 0.1f))),1) * (1 + (result) * 0.05f);

		dCalcMinRepair = dCurComplete - dCurComplete * 0.1f;

		int nPlusCount = 0;

		while (dCalcMinRepair >= 400000000) 
		{
			dCalcMinRepair *= 0.1;
			dCurComplete *= 0.1;

			nPlusCount++;
		}

		float fResult = Random.Range ((float)dCalcMinRepair, (float)dCurComplete);

		dCurComplete = fResult * Mathf.Pow(10,nPlusCount);

		RepairText.text = ScoreManager.ScoreInstance.ChangeMoney (dCurComplete);

		//수리력 설정
		createWeapon.dRepair = dCurComplete;

		int nOptionLength = 3;

		//리스트에서 지움 
		LIST_OPTION.Clear();

		//추가 옵션 범위 
		nCalcAddMinOption = Mathf.RoundToInt(m_nBasicMinOption + (float)(m_nBasicMinOption *(m_nPlusOptionMinPercent * nDight * 0.01f)));
		nCalcAddMaxOption = Mathf.RoundToInt(m_nBasicMaxOption + (float)(m_nBasicMaxOption *(m_nPlusOptionMaxPercent * nDight * 0.01f)));

		//옵션 셋팅 
		while(nOptionLength > 0)
		{
			int nInsertIndex = 0;

			nInsertIndex = Random.Range((int)E_CREATOR.E_REPAIRPERCENT, (int)E_CREATOR.E_MAX);
			nInsertValue = Random.Range (nCalcAddMinOption, nCalcAddMaxOption + 1);

			if (CheckData(createWeapon, nInsertIndex, nInsertValue))
				nOptionLength--;
		}

		//보스 옵션 셋팅 
		for (int nIndex = 0; nIndex < BossSoulSlots.Length; nIndex++) 
		{
			//만약 체크가 됐다면 
			if (BossSoulSlots [nIndex].bIsCheck) {
				nInsertValue = Random.Range (nCalcAddMinOption, nCalcAddMaxOption + 1);

				if (nIndex == (int)E_BOSSNAME.E_BOSSNAME_ICE)
					createWeapon.fIceBossValue = nInsertValue;

				else if (nIndex == (int)E_BOSSNAME.E_BOSSNAME_SASIN)
					createWeapon.fSasinBossValue = nInsertValue;

				else if (nIndex == (int)E_BOSSNAME.E_BOSSNAME_FIRE)
					createWeapon.fFireBossValue = nInsertValue;

				else if (nIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC)
					createWeapon.fRusiuBossValue = nInsertValue;

				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				plusItem.nIndex = nIndex + (int)E_CREATOR.E_BOSS_ICE;
				plusItem.strOptionName = strBossExplains [nIndex];
				plusItem.nValue = nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
			}
		}

		//만약 전 에픽이 있을 경우 락을 했는지 안했는지 넣어 줌
		if (createEpic != null) 
		{
			createWeapon.bIsLock = createEpic.bIsLock;
			createWeapon.nCostDay = createEpic.nCostDay;
			createWeapon.nEpicIndex = createEpic.nIndex;
		}

		//락이 아닐 경우 다시 에픽 확률을 계산 해서 넣어 줌 
		if (createWeapon.bIsLock == false) {

			if (createEpic != null)
				createEpic.Relive ();
			
			//특수 옵션 미정
			//if (Random.Range (0, 100) <= 5) {
			int nRandomIndex = (int)Random.Range (0, (int)E_EPIC_INDEX.E_EPIC_MAX);

				createEpic = EpicFactory (nRandomIndex);

				createWeapon.nEpicIndex = nRandomIndex;

				createWeapon.nCostDay = playerData.GetDay ();

				if (createEpic != null)
					createEpic.Init (playerData.GetDay (), playerData);	
			//}
			//else 
			//{
			//	createEpic = null;
			//}
		} 
		//락 일경우 추가 된 일차로 옵션 수치만 다시 설정 해준다.
		else 
			createEpic.Init (playerData.GetDay (), playerData);

		if (createEpic != null) {
			//에픽 옵션 추가 
			CGameMainWeaponOption EpicOption = new CGameMainWeaponOption ();
		
			EpicOption.nIndex = (int)E_CREATOR.E_EPIC;
			EpicOption.strOptionName = createEpic.strExplain;
			EpicOption.nValue = (int)createEpic.fValue;
			EpicOption.strOptionExplain = createEpic.strExplain;
			EpicOption.bIsLock = false;
			createEpic.bIsLock = false;
		
			LIST_OPTION.Add (EpicOption);

			createWeapon.WeaponSprite = Resources.Load<Sprite> ("Crafts/CreatorWeapon/" + createEpic.nIndex);

			createWeapon.strName = GameManager.Instance.cHammerNames [createEpic.nIndex].strName;

			SoundManager.instance.PlaySound (eSoundArray.ES_CraftSound_GreatFinish);
		} 
		else 
		{
			int nRandomWeaponIndex = (int)Random.Range (9, 28);

			createWeapon.WeaponSprite = Resources.Load<Sprite> ("Crafts/CreatorWeapon/" + nRandomWeaponIndex);

			createWeapon.strName = GameManager.Instance.cHammerNames [nRandomWeaponIndex].strName;

			SoundManager.instance.PlaySound (eSoundArray.ES_CraftSound_Finish);
		}

		createWeapon.nOptionChangeCount = 1;

		WeaponImage.sprite = createWeapon.WeaponSprite;
		WeaponNameText.text = createWeapon.strName;
	}

	private bool CheckData(CreatorWeapon _equiment, int nIndex, int _nInsertValue)
	{
		switch(nIndex)
		{
		case (int)E_CREATOR.E_REPAIRPERCENT:
			if (_equiment.fRepairPercent == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fRepairPercent = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_REPAIRPERCENT;
				plusItem.strOptionName = "수리력";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}

			break;
		case (int)E_CREATOR.E_ARBAIT:
			if (_equiment.fArbaitRepair == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fArbaitRepair = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "아르바이트 수리력";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_HONOR:
			if (_equiment.fPlusHonorPercent == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fPlusHonorPercent = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_HONOR;
				plusItem.strOptionName = "명예 추가 증가량";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_GOLD:
			if (_equiment.fPlusGoldPercent == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fPlusGoldPercent = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_GOLD;
				plusItem.strOptionName = "골드 추가 증가량";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_WATERCHARGE:
			if (_equiment.fWaterPlus == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fWaterPlus = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_WATERCHARGE;
				plusItem.strOptionName = "물 증가량";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;

		case (int)E_CREATOR.E_WATERUSE:
			if (_equiment.fActiveWater == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fActiveWater = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_WATERUSE;
				plusItem.strOptionName = "물 추가 증가량";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_CRITICAL:
			if (_equiment.fCriticalChance == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fCriticalChance = _nInsertValue;


				plusItem.nIndex = (int)E_CREATOR.E_CRITICAL;
				plusItem.strOptionName = "크리티컬 확률 증가";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_CRITICALD:
			if (_equiment.fCriticalDamage == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fCriticalDamage = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_CRITICALD;
				plusItem.strOptionName = "크리티컬 데미지 증가";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_BIGCRITICAL:
			if (_equiment.fBigSuccessed == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fBigSuccessed = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_BIGCRITICAL;
				plusItem.strOptionName = "대성공 확률 증가";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);

				return true;
			}
			break;
		case (int)E_CREATOR.E_ACCURACY:
			if (_equiment.fAccuracyRate == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fAccuracyRate = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_ACCURACY;
				plusItem.strOptionName = "명중률 증가";
				plusItem.nValue = _nInsertValue;
				plusItem.strOptionExplain = string.Format ("{0} : {1}%", plusItem.strOptionName, plusItem.nValue);
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
				return true;
			}
			break;
		}

		return false;
	}

	public EpicOption EpicFactory(int _nIndex)
	{
		EpicOption resultOption = null;

		switch (_nIndex) 
		{
		case (int)E_EPIC_INDEX.E_EPIC_MAGIC: 			resultOption = new MagicStick (); break;
		case (int)E_EPIC_INDEX.E_EPIC_KO_HAMMER: 		resultOption = new KoHammer (); break;
		case (int)E_EPIC_INDEX.E_EPIC_GOLD_HAMMER: 		resultOption = new GoldHammer (); break;
		case (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA: 	resultOption = new FreezingTuna (); break;
		case (int)E_EPIC_INDEX.E_EPIC_RUBBER_CHICKEN: 	resultOption = new RubberChicken ();break;
		case (int)E_EPIC_INDEX.E_EPIC_ENGINE_HAMMER: 	resultOption = new EngineHammer (); break;
		case (int)E_EPIC_INDEX.E_EPIC_ICEPUNCH:			resultOption = new IcePunch (); break;
		case (int)E_EPIC_INDEX.E_EPIC_GOBLIN_HAMMER:	resultOption = new GoblinHammer ();break;
		case (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER:		resultOption = new SledeHammer (); break;

		default:
			break;
		}

		return resultOption;
	}

	void Successed()
	{
		SpawnManager.Instance.SetDayInitInfo (playerData.GetDay ());

		gameObject.SetActive (false);

		SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_FINISH;
	}

	void ShowAdButton()
	{
		GameManager.Instance.ShowRewardedAd_Creator (this);
	}

	public void ShowAd()
	{
		//감소를 시켜 놨기 때문에 현재 일차에서 10일을 추가 시켜준다.
		playerData.SetDay (playerData.GetDay () + 10);

		//무기 제작
		CreateWeapon ();

		//결과를 넣어줌
		makingUI.CreateWeapon (createWeapon, createEpic, LIST_OPTION);

		playerData.SetDay (playerData.GetDay () - 10);

		//옵션 재설
		RefreshDisplay ();

		AdButton.gameObject.SetActive (false);
	}
}

