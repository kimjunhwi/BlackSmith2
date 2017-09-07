using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class MakingUI : MonoBehaviour {

	public Image WeaponImage;
	public Image BossSlotOne;
	public Image BossSlotTwo;

	public Text CostDayText;
	public Text NowRepairPower;
	public Text RandomRepairPower;

	public Button MakingButton;

	public BossSoul[] BossSoulSlots;
	public ResultEpicUI ResultUI;

	Player playerData;

	public EpicOption createEpic = null;

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
	int nCalcMinRepair = 0;
	int nCalcMaxRepair = 0;
	int nCalcAddMinOption = 0;
	int nCalcAddMaxOption = 0;

	int nInsertValue;

	private string[] strBossExplains = {"명중률 증가" ,"수리력 증가", "크리확률 증가","알바 수리력 증가"};

	//08.07

	private const int nMaxOption = 7;

	public Transform contentPanel;

	List<CGameMainWeaponOption> LIST_OPTION = new List<CGameMainWeaponOption> (); 

	public SimpleObjectPool optionPanelPool;


	void Awake()
	{
		playerData = GameManager.Instance.GetPlayer ();
		MakingButton.onClick.AddListener (MakeWeapon);

		if (playerData.GetCreatorWeapon ().fRepair == 0) {
			playerData.SetDay (11);

			MakeWeapon ();
		} 
		else 
		{
			CreatorWeapon createWeapon = new CreatorWeapon();

			//저장 된 데이터를 미리 캐싱해 둔다 
			CreatorWeapon LoadCreateWeapon = playerData.GetCreatorWeapon ();

			#region SetCreateWeaponSetting

			createWeapon.fRepair = LoadCreateWeapon.fRepair;

			if ( LoadCreateWeapon.fArbaitRepair != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_ARBAIT, (int)LoadCreateWeapon.fArbaitRepair);

			if ( LoadCreateWeapon.fPlusHonorPercent != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_HONOR, (int)LoadCreateWeapon.fPlusHonorPercent);

			if ( LoadCreateWeapon.fPlusGoldPercent != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_GOLD, (int)LoadCreateWeapon.fPlusGoldPercent);

			if ( LoadCreateWeapon.fWaterPlus != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_WATERCHARGE, (int)LoadCreateWeapon.fWaterPlus);

			if ( LoadCreateWeapon.fActiveWater != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_WATERUSE, (int)LoadCreateWeapon.fActiveWater);

			if ( LoadCreateWeapon.fCriticalChance != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_CRITICAL, (int)LoadCreateWeapon.fCriticalChance);

			if ( LoadCreateWeapon.fCriticalDamage != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_CRITICALD, (int)LoadCreateWeapon.fCriticalDamage);

			if ( LoadCreateWeapon.fBigSuccessed != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_BIGCRITICAL, (int)LoadCreateWeapon.fBigSuccessed);

			if ( LoadCreateWeapon.fAccuracyRate != 0)
				CheckData (createWeapon, (int)E_CREATOR.E_ACCURACY, (int)LoadCreateWeapon.fAccuracyRate);

			if ( LoadCreateWeapon.fIceBossValue != 0) 
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				plusItem.nIndex = (int)E_CREATOR.E_BOSS_ICE;
				plusItem.strOptionName = strBossExplains [0];
				plusItem.nValue =  (int)LoadCreateWeapon.fIceBossValue;
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
			}

			if ( LoadCreateWeapon.fRusiuBossValue != 0) 
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				plusItem.nIndex = (int)E_CREATOR.E_BOSS_RUSIU;
				plusItem.strOptionName = strBossExplains [1];
				plusItem.nValue =  (int)LoadCreateWeapon.fRusiuBossValue;
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
			}

			if ( LoadCreateWeapon.fSasinBossValue != 0) 
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				plusItem.nIndex = (int)E_CREATOR.E_BOSS_SASIN;
				plusItem.strOptionName = strBossExplains [2];
				plusItem.nValue =  (int)LoadCreateWeapon.fSasinBossValue;
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
			}

			if ( LoadCreateWeapon.fFireBossValue != 0) 
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				plusItem.nIndex = (int)E_CREATOR.E_BOSS_FIRE;
				plusItem.strOptionName = strBossExplains [3];
				plusItem.nValue =  (int)LoadCreateWeapon.fFireBossValue;
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
			}

			if( LoadCreateWeapon.nEpicIndex != (int)E_EPIC_INDEX.E_EPIC_FAIEL)
			{
				createWeapon.nEpicIndex = LoadCreateWeapon.nEpicIndex;

				createEpic = EpicFactory (createWeapon.nEpicIndex);

				createEpic.Init (createWeapon.nCostDay, playerData);

				//에픽 옵션 추가 
				CGameMainWeaponOption EpicOption = new CGameMainWeaponOption ();

				EpicOption.nIndex = (int)E_CREATOR.E_EPIC;
				EpicOption.strOptionName = createEpic.strExplain;
				EpicOption.nValue = nInsertValue;
				EpicOption.bIsLock = false;
				createEpic.bIsLock = false;

				LIST_OPTION.Add (EpicOption);
			}

			float fRepair = Mathf.RoundToInt (createWeapon.fRepair);

			playerData.SetCreatorWeapon (createWeapon,createEpic);

			float fEnhanceValue = playerData.GetCreatorWeapon().fRepair * Mathf.Pow (1.125f, playerData.GetRepairLevel());

			playerData.SetBasicRepairPower (fEnhanceValue);

			SpawnManager.Instance.SetDayInitInfo (playerData.GetDay ());

			RefreshDisplay ();

			nCalcMinRepair = (int)Mathf.Round(m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay() * 0.01f)));
			nCalcMaxRepair = (int)Mathf.Round(m_nBasicMaxRepair + m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay() * 0.01f));


			NowRepairPower.text = string.Format("{0}", fRepair);

			CostDayText.text = string.Format("{0}",playerData.GetDay());

			RandomRepairPower.text = string.Format("제작시 수리력 {0:F1} ~ {1:F1}",nCalcMinRepair,nCalcMaxRepair);

			#endregion
		}


	}

	void Start()
	{
		for (int nIndex = 0; nIndex < BossSoulSlots.Length; nIndex++) 
		{
			BossSoulSlots [nIndex].SetUp (this, playerData, nIndex);
		}

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
			optionPanelPool.ReturnObject(toRemove);
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

			GameObject newButton = optionPanelPool.GetObject ();
			newButton.transform.SetParent (contentPanel, false);
			newButton.transform.localScale = Vector3.one;

			OptionItem sampleButton = newButton.GetComponent<OptionItem> ();
			sampleButton.Setup (item,this);
		}
	}

	public void OnEnable()
	{
		CostDayText.text = string.Format("{0}",playerData.GetDay());

		nCalcMinRepair = (int)Mathf.Round(m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay() * 0.01f)));
		nCalcMaxRepair = (int)Mathf.Round(m_nBasicMaxRepair + m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay() * 0.01f));

		RandomRepairPower.text = string.Format("제작시 수리력 {0:F1} ~ {1:F1}",nCalcMinRepair,nCalcMaxRepair);
	}

	void MakeWeapon()
	{
		if (playerData.GetDay() <= 10)
			return;

		ResultUI.Init (BossSoulSlots);

//		playerData.SetDay (playerData.GetDay () - 10);
//
//		float fRepair = Mathf.RoundToInt (createWeapon.fRepair);
//
//		playerData.SetCreatorWeapon (createWeapon,createEpic);
//
//		float fEnhanceValue = playerData.GetCreatorWeapon().fRepair * Mathf.Pow (1.125f, playerData.GetRepairLevel());
//
//		playerData.SetBasicRepairPower (fEnhanceValue);
//
//		SpawnManager.Instance.SetDayInitInfo (playerData.GetDay ());
//
//		RefreshDisplay ();
//
//		nCalcMinRepair = (int)Mathf.Round(m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay() * 0.01f)));
//		nCalcMaxRepair = (int)Mathf.Round(m_nBasicMaxRepair + m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay() * 0.01f));
//
//		NowRepairPower.text = string.Format("{0}", fRepair);
//
//		CostDayText.text = string.Format("{0}",playerData.GetDay());
//
//		RandomRepairPower.text = string.Format("제작시 수리력 {0:F1} ~ {1:F1}",nCalcMinRepair,nCalcMaxRepair);
	}

	public void CreateWeapon(CreatorWeapon _weapon,EpicOption _epicOption, List<CGameMainWeaponOption> _List_Option)
	{
		LIST_OPTION = _List_Option;

		playerData.SetDay (playerData.GetDay () - 10);
	
		float fRepair = Mathf.RoundToInt (_weapon.fRepair);
	
		playerData.SetCreatorWeapon (_weapon, _epicOption);
	
		float fEnhanceValue = playerData.GetCreatorWeapon ().fRepair * Mathf.Pow (1.125f, playerData.GetRepairLevel ());
	
		playerData.SetBasicRepairPower (fEnhanceValue);
	
		SpawnManager.Instance.SetDayInitInfo (playerData.GetDay ());
	
		RefreshDisplay ();
	
		nCalcMinRepair = (int)Mathf.Round (m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay () * 0.01f)));
		nCalcMaxRepair = (int)Mathf.Round (m_nBasicMaxRepair + m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay () * 0.01f));
	
		NowRepairPower.text = string.Format ("{0}", fRepair);
	
		CostDayText.text = string.Format ("{0}", playerData.GetDay ());
	
		RandomRepairPower.text = string.Format ("제작시 수리력 {0:F1} ~ {1:F1}", nCalcMinRepair, nCalcMaxRepair);
	}

	private bool CheckData(CreatorWeapon _equiment, int nIndex, int _nInsertValue)
	{
		switch(nIndex)
		{

		case (int)E_CREATOR.E_ARBAIT:
			if (_equiment.fArbaitRepair == 0)
			{
				CGameMainWeaponOption plusItem = new CGameMainWeaponOption ();

				_equiment.fArbaitRepair = _nInsertValue;

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "아르바이트 수리력";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "골드 추가 증가량";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "물 증가량";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "물 추가 증가량";
				plusItem.nValue = _nInsertValue;
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


				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "크리티컬 확률 증가";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "크리티컬 데미지 증가";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "대성공 확률 증가";
				plusItem.nValue = _nInsertValue;
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

				plusItem.nIndex = (int)E_CREATOR.E_ARBAIT;
				plusItem.strOptionName = "명중률 증가";
				plusItem.nValue = _nInsertValue;
				plusItem.bIsLock = false;

				LIST_OPTION.Add (plusItem);
				return true;
			}
			break;
		}

		return false;
	}

	public bool CheckMake()
	{
		int nAmount = 0;

		for (int nIndex = 0; nIndex < BossSoulSlots.Length; nIndex++) {
			if (BossSoulSlots [nIndex].bIsCheck)
				nAmount++;
		}

		if (nAmount < 2)
			return true;

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
		case (int)E_EPIC_INDEX.E_EPIC_GOBLIN_HAMMER:	resultOption = new GoblinHammer ();break;
		case (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER:		resultOption = new SledeHammer (); break;

		default:
			break;
		}

		return resultOption;
	}
}

