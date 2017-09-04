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

	Player playerData;

	EpicOption createEpic = null;

	//기본 값
	const int m_nBasicGold = 1200;
	const int m_nBasicHonor = 300;
	const int m_nBasicMinRepair = 6;
	const int m_nBasicMaxRepair = 10;
	const int m_nBasicMinOption = 3;
	const int m_nBasicMaxOption = 1;

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


	//08.07

	private const int nMaxOption = 7;

	public Transform contentPanel;

	List<CGameMainWeaponOption> LIST_OPTION = new List<CGameMainWeaponOption> (); 

	public SimpleObjectPool optionPanelPool;

	private string[] strBossExplains = {"명중률 증가" ,"수리력 증가", "크리확률 증가","알바 수리력 증가"};

	void Awake()
	{
		playerData = GameManager.Instance.GetPlayer ();
		MakingButton.onClick.AddListener (MakeWeapon);

		//playerData.SetDay (11);

		MakeWeapon ();
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
			sampleButton.Setup (item);
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

		int nDight = 0;
		int nDightCost = playerData.GetDay ();

		float fCostDay = (float)nDightCost;

		while (fCostDay >= 10) 
		{
			fCostDay *= 0.1f;
			nDight++;
		}

		CreatorWeapon createWeapon = new CreatorWeapon ();

		nCalcMinRepair = Mathf.RoundToInt(m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay() * 0.01f)));
		nCalcMaxRepair = Mathf.RoundToInt(m_nBasicMaxRepair + (float)(m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay() * 0.01f)));

		//수리력 
		createWeapon.fRepair = 10.0f; //Mathf.RoundToInt (Random.Range (nCalcMinRepair, nCalcMaxRepair + 1));

		int nOptionLength = 3;

		//추가 옵션 범위 
		nCalcAddMinOption = Mathf.RoundToInt(m_nBasicMinOption + (float)(m_nBasicMinOption *(m_nPlusOptionMinPercent * nDight * 0.01f)));
		nCalcAddMaxOption = Mathf.RoundToInt(m_nBasicMaxOption + (float)(m_nBasicMaxOption *(m_nPlusOptionMaxPercent * nDight * 0.01f)));

		for (int nIndex = 0; nIndex < LIST_OPTION.Count; nIndex++) 
		{
			if (LIST_OPTION [nIndex].bIsLock == false)
				LIST_OPTION.Remove (LIST_OPTION [nIndex--]);

			else 
			{
				nOptionLength--;

				nInsertValue = Random.Range (nCalcAddMinOption, nCalcAddMaxOption + 1);

				switch (LIST_OPTION [nIndex].nIndex) 
				{
				case (int)E_CREATOR.E_ARBAIT:
					createWeapon.fArbaitRepair = nInsertValue;
					break;
				case (int)E_CREATOR.E_HONOR:
					createWeapon.fPlusHonorPercent = nInsertValue;
					break;
				case (int)E_CREATOR.E_GOLD:
					createWeapon.fPlusGoldPercent = nInsertValue;
					break;
				case (int)E_CREATOR.E_WATERCHARGE:
					createWeapon.fWaterPlus = nInsertValue;
					break;
				case (int)E_CREATOR.E_WATERUSE:
					createWeapon.fActiveWater = nInsertValue;
					break;
				case (int)E_CREATOR.E_CRITICAL:
					createWeapon.fCriticalChance = nInsertValue;
					break;
				case (int)E_CREATOR.E_CRITICALD:
					createWeapon.fCriticalDamage = nInsertValue;
					break;
				case (int)E_CREATOR.E_BIGCRITICAL:
					createWeapon.fBigSuccessed = nInsertValue;
					break;
				case (int)E_CREATOR.E_ACCURACY:
					createWeapon.fAccuracyRate = nInsertValue;
					break;
				}
			}
		}

		int nInsertIndex = 0;

		while(nOptionLength > 0)
		{
			nInsertIndex = Random.Range((int)E_CREATOR.E_ARBAIT, (int)E_CREATOR.E_MAX);
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
				plusItem.bIsLock = false;

				BossSoulSlots [nIndex].ReSetting ();

				LIST_OPTION.Add (plusItem);
			}
		}

		//특수 옵션 미정

		if (Random.Range (0, 100) <= 100) 
		{
			if (createEpic != null) 
			{
				
			} 
			else 
			{
				int nRandomIndex = (int)E_EPIC_INDEX.E_EPIC_GOLD_HAMMER; //Random.Range (0, (int)E_EPIC_INDEX.E_EPIC_MAX);

				createEpic = EpicFactory (nRandomIndex);

				if (createEpic != null) 
				{
					createEpic.Init (playerData.GetDay(),playerData);
				}
			}
		}

		playerData.SetDay (playerData.GetDay () - 10);

		float fRepair = Mathf.RoundToInt (createWeapon.fRepair);

		NowRepairPower.text = string.Format("{0}", fRepair);

		playerData.SetCreatorWeapon (createWeapon);

		float fEnhanceValue = playerData.GetCreatorWeapon().fRepair * Mathf.Pow (1.125f, playerData.GetRepairLevel());

		playerData.SetBasicRepairPower (fEnhanceValue);

		SpawnManager.Instance.SetDayInitInfo (playerData.GetDay ());

		RefreshDisplay ();

		CostDayText.text = string.Format("{0}",playerData.GetDay());

		nCalcMinRepair = (int)Mathf.Round(m_nBasicMinRepair + (float)(m_nBasicMinRepair * (m_nPlusRepairMinPercent * playerData.GetDay() * 0.01f)));
		nCalcMaxRepair = (int)Mathf.Round(m_nBasicMaxRepair + m_nBasicMaxRepair * (m_nPlusRepairMaxPercent * playerData.GetDay() * 0.01f));

		RandomRepairPower.text = string.Format("제작시 수리력 {0:F1} ~ {1:F1}",nCalcMinRepair,nCalcMaxRepair);
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
