using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using UnityEngine.UI;

[System.Serializable]
public class Player 
{	
	//변화될 값 
	public CGamePlayerData changeStats;

	//플레이어 제작 무기  
	private CreatorWeapon creatorWeapon = null;
	private EpicOption epicOpion = null;

    public List<CGameEquiment> List_items;

	//플레이어가 장비한 무기
	public CGameEquiment WeaponEquipment = null;
	public CGameEquiment GearEquipmnet = null;
    public CGameEquiment AccessoryEquipmnet = null;

	private SpawnManager spawnManager = null;

	//플레이어가 가질 인벤토리
    public Inventory inventory;

	//플레이어 정보 표시 스크 
	public PlayerSpecificInfo PlayerInfo = null;

	public SpriteRenderer MoruImage = null;

	public int GetSmithLevel() { return changeStats.nBlackSmithLevel; }
	public void SetSmithLevel(int _nValue) { changeStats.nBlackSmithLevel = _nValue; }

	public int GetRepairLevel() {return changeStats.nEnhanceRepaireLevel; }
	public void SetRepairLevel(int _nValue){changeStats.nEnhanceRepaireLevel = _nValue;}

	public int GetMaxWaterLevel(){return changeStats.nEnhanceMaxWaterLevel;}
	public void SetMaxWaterLevel(int _nValue) {changeStats.nEnhanceMaxWaterLevel = _nValue; }

	public int GetWaterPlusLevel(){ return changeStats.nEnhanceWaterPlusLevel;}
	public void SetWaterPlusLevel(int _nValue){changeStats.nEnhanceWaterPlusLevel = _nValue;}

	public int GetAccuracyRateLevel(){return changeStats.nEnhanceAccuracyRateLevel;}
	public void SetAccuracyRateLevel(int _nValue){changeStats.nEnhanceAccuracyRateLevel = _nValue;}

	public int GetCriticalLevel() {return changeStats.nEnhanceCriticalLevel; }
	public void SetCriticalLevel(int _nValue){changeStats.nEnhanceCriticalLevel = _nValue;}

	public int GetArbaitEnhanceLevel() {return changeStats.nEnhanceArbaitLevel; }
	public void SetArbaitEnhancLevel(int _nValue){changeStats.nEnhanceArbaitLevel = _nValue;}

	public int GetFeverLevel() { return changeStats.nFeverTimeLevel; }
	public void SetFeverLevel(int _nValue) { changeStats.nFeverTimeLevel = _nValue; }

	public int GetBigSuccessedLevel() { return changeStats.nEnhaceBigSuccessedLevel;}
	public void SetBigSuccessedLevel(int _nValeu){ changeStats.nEnhaceBigSuccessedLevel = _nValeu;}

	//ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
	public double GetBasicRepairPower(){ return changeStats.dRepairPower; }
	public void SetBasicRepairPower(double _dValue,bool _bIsNoneSet = false) { 
		changeStats.dRepairPower = _dValue;  

		SetRepairPower ();
	}

	public float GetBasicMaxWater() {return changeStats.fMaxWater;}
	public void SetBasicMaxWater(float _fValue){ changeStats.fMaxWater = _fValue; }


	public float GetBasicArbaitRepairPower() {return changeStats.fArbaitsPower;}
	public void SetBasicArbaitRepairPower(float _fValue){ 
		changeStats.fArbaitsPower = _fValue;
	
		changeStats.fArbaitsPower = Mathf.Round (changeStats.fArbaitsPower * 100) * 0.01f;

		SetArbaitRepairPower ();
	}

	public float GetBasicWaterPlus() { return changeStats.fWaterPlus; }
	public void SetBasicWaterPlus(float _fValue) { 
		changeStats.fWaterPlus = _fValue; 

		SetWaterPlus ();
	}

	public double GetBasicGoldPlusPercent() {return changeStats.dGoldPlusPercent;}
	public void SetBasicGoldPlusPercent(double _dValue){ 
		changeStats.dGoldPlusPercent = _dValue;

		SetGoldPlusPercent ();
	}

	public double GetBasicHonorGoldPlusPercent() {return changeStats.dHornorPlusPercent;}
	public void SetBasicHonorPlusPercent(double _dValue){ 
		changeStats.dHornorPlusPercent = _dValue;

		SetHonorPlusPercent ();
	}

	public float GetBasicMaxWaterPlus() { return changeStats.fMaxWater; }
	public void SetBasicMaxWaterPlus(float _fValue) { 
		changeStats.fMaxWater = _fValue; 

		spawnManager.repairObject.WaterSlider.maxValue = changeStats.fMaxWater;
	}

	public float GetBasicAccuracyRate() { return changeStats.fAccuracyRate; }
	public void SetBasicAccuracyRate(float _fValue) { 

		changeStats.fAccuracyRate = _fValue; 

		changeStats.fAccuracyRate = Mathf.Round (changeStats.fAccuracyRate * 100) * 0.01f;
	
		SetAccuracyRate ();
	}

    public float GetBasicBigSuccessedPercent() { return changeStats.fBigSuccessed; }
	public void SetBasicBigSuccessedPercent(float _fValue) { 

		changeStats.fBigSuccessed = _fValue;

		changeStats.fBigSuccessed = Mathf.Round (changeStats.fBigSuccessed * 100) * 0.01f;
	
		SetBigSuccessed ();
	}

    public float GetBasicCriticalChance() { return changeStats.fCriticalChance; }
	public void SetBasicCriticalChance(float _fValue) { 

		changeStats.fCriticalChance = _fValue; 
	
		SetCriticalChance ();
	}

	public float GetBasicFeverTime() { return changeStats.fFeverTime; }
	public void SetBasicFeverTime(float _fValue) { 

		changeStats.fFeverTime = _fValue; 

		SetFeverTime ();
	}

    public int GetSasinMaterial() { return changeStats.nSasinMaterial; }
    public void SetSasinMaterial(int _nValue) { changeStats.nSasinMaterial = _nValue; }
    public int GetRusiuMaterial() { return changeStats.nRusiuMaterial; }
    public void SetRusiuMaterial(int _nValue) { changeStats.nRusiuMaterial = _nValue; }
    public int GetIceMaterial() { return changeStats.nIceMaterial; }
    public void SetIceMaterial(int _nValue) { changeStats.nIceMaterial = _nValue; }
    public int GetFireMaterial() { return changeStats.nFireMaterial; }
    public void SetFireMaterial(int _nValue) { changeStats.nFireMaterial = _nValue; }

    public int GetDay() { return changeStats.nDay; }
    public void SetDay(int _nValue) { 
		changeStats.nDay = _nValue; 
	}

	public int GetMaxDay() { return changeStats.nMaxDay; }
	public void SetMaxDay(int _nValue) { changeStats.nMaxDay = _nValue; }

    public int GetGuestCount() { return changeStats.nGuestCount; }

	public int GetSuccessedGuestCount() { return changeStats.nSuccessedGuest; }
	public void SetSuccessedGuestCount(int _nValue){changeStats.nSuccessedGuest = _nValue;}
	
	//08.09
	//플레이어 제작 
	public void SetCreatorWeapon(CreatorWeapon _weapon,EpicOption _EpicOption){

		creatorWeapon = null;

		creatorWeapon = _weapon; 

		epicOpion = _EpicOption;

		double dCurComplete;

		if (changeStats.nEnhanceRepaireLevel <= 10) {
			dCurComplete = _weapon.dRepair + changeStats.nEnhanceRepaireLevel;
		} else {
			dCurComplete = _weapon.dRepair * Mathf.Pow (1.022f, (Mathf.Floor ((changeStats.nEnhanceRepaireLevel - 11) * 0.1f))) * (1 + ((changeStats.nEnhanceRepaireLevel - 10) * 0.03f));

			dCurComplete += changeStats.nEnhanceRepaireLevel;
		}

		SetBasicRepairPower (dCurComplete);

		PlayerStatsSetting ();
	}

	public void SetPlayerInfo(PlayerSpecificInfo _Info)
	{
		PlayerInfo = _Info;
	}

	public CreatorWeapon GetCreatorWeapon()
	{
		return creatorWeapon;
	}

	public EpicOption GetEpicOption()
	{
		return epicOpion;
	}

	public double GetGold(){ return changeStats.dGold; }
	public double GetHonor(){ return changeStats.dHonor; }
	public int GetRuby(){ return changeStats.nRuby; }

	private double m_dRepairPower;
	private float m_fArbaitRepairPower;
	private double m_dGoldPlusPercent;
	private double m_dHonorPlusPercent;
	private float m_fWaterPlus;
	private float m_fCriticalChance;
	private double m_dCriticalDamage;
	private float m_fBigSuccessed;
	private float m_fAccuracyRate;
	private float m_fFeverTime;

	public double GetRepairPower(){ return m_dRepairPower; }

	public float GetAccuracyRate(){ return m_fAccuracyRate;}

	public float GetArbaitRepairPower(){ return m_fArbaitRepairPower;}

	public double GetGoldPlusPercent(){	return m_dGoldPlusPercent;}

	public double GetHonorPlusPercent(){ return m_dHonorPlusPercent;}

	public float GetWaterPlus(){ return m_fWaterPlus;}

	public float GetCriticalChance(){ return m_fCriticalChance;}

	public double GetCriticalDamage(){ return m_dCriticalDamage;}

	public float GetBigSuccessed(){ return m_fBigSuccessed;}

	public float GetFeverTime(){return m_fFeverTime;}

	//아이템 이나 캐릭터의 스텟이 바뀌었을때 호출해서 다시 세팅해준다.
	public void PlayerStatsSetting()
	{
		SetRepairPower ();
		SetAccuracyRate ();
		SetArbaitRepairPower ();
		SetGoldPlusPercent ();
		SetHonorPlusPercent ();
		SetWaterPlus ();
		SetCriticalChance ();
		SetCriticalDamage ();
		SetBigSuccessed ();
		SetFeverTime ();
	}

	public void SetRepairPower () {

		double dResultRepairPowerPercent = 0.0f;

		if (WeaponEquipment != null)	dResultRepairPowerPercent += WeaponEquipment.fReapirPower;
		if (GearEquipmnet != null) 		dResultRepairPowerPercent += GearEquipmnet.fReapirPower;
		if (AccessoryEquipmnet != null) dResultRepairPowerPercent += AccessoryEquipmnet.fReapirPower;
		if (creatorWeapon != null) {
			dResultRepairPowerPercent += (double)creatorWeapon.fRepairPercent;
			dResultRepairPowerPercent += (double)creatorWeapon.fSasinBossValue;

			if (epicOpion != null) {

				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA) {
					if (epicOpion.CheckOption ())
						dResultRepairPowerPercent += epicOpion.fResultValue;
				}

				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_ICEPUNCH)
					dResultRepairPowerPercent += epicOpion.fMinusRepiar;

			}
		}

       
		if (spawnManager != null) 
		{
			//로이 아르바이트가 배치중이라면 스킬을 적용
			if (spawnManager.list_ArbaitUI.Count != 0) {
				if (spawnManager.m_BatchArbait [(int)E_ARBAIT.E_ROY].activeSelf == true)
					dResultRepairPowerPercent += (double)spawnManager.array_ArbaitData [(int)E_ARBAIT.E_ROY].m_CharacterChangeData.fSkillPercent;

				//사신이 배치 됐고 버프가 활성화 됐을경우 
				if (spawnManager.m_BatchArbait [(int)E_ARBAIT.E_SASIN].activeSelf == true) 
					if (spawnManager.array_ArbaitData [(int)E_ARBAIT.E_SASIN].bIsSasinCheck) 
						dResultRepairPowerPercent += (double)spawnManager.array_ArbaitData [(int)E_ARBAIT.E_SASIN].m_CharacterChangeData.fSkillPercent;
			}
		}
		m_dRepairPower = changeStats.dRepairPower + (changeStats.dRepairPower * dResultRepairPowerPercent * 0.01);

		if (spawnManager != null) 
			if (spawnManager.list_ArbaitUI.Count != 0) 
				spawnManager.SettingArbaitRepairPower (m_dRepairPower);
			
		

		if (PlayerInfo != null)
			PlayerInfo.SetRepairpowerText ();
	}

	public void SetAccuracyRate()
	{
		float fResultAccuracyRatePercent = 0.0f;

		if (WeaponEquipment != null)	fResultAccuracyRatePercent += WeaponEquipment.fAccuracyRate;
		if (GearEquipmnet != null) 		fResultAccuracyRatePercent += GearEquipmnet.fAccuracyRate;
		if (AccessoryEquipmnet != null) fResultAccuracyRatePercent += AccessoryEquipmnet.fAccuracyRate;
		if (creatorWeapon != null) {
			fResultAccuracyRatePercent += creatorWeapon.fAccuracyRate;
			fResultAccuracyRatePercent += creatorWeapon.fIceBossValue;

			if (epicOpion != null) {
				
				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER)
					fResultAccuracyRatePercent += epicOpion.fSledeAccuracyRate;

			}
		}

		m_fAccuracyRate = changeStats.fAccuracyRate + (changeStats.fAccuracyRate * fResultAccuracyRatePercent * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetAccuracyRateText ();
	}

	public void SetArbaitRepairPower()
	{
		float fResultArbaitRepairPower = 0.0f;

		if (WeaponEquipment != null)	fResultArbaitRepairPower += WeaponEquipment.fArbaitRepair;
		if (GearEquipmnet != null) 		fResultArbaitRepairPower += GearEquipmnet.fArbaitRepair;
		if (AccessoryEquipmnet != null) fResultArbaitRepairPower += AccessoryEquipmnet.fArbaitRepair;
		if (creatorWeapon != null) {
			fResultArbaitRepairPower += creatorWeapon.fArbaitRepair;
			fResultArbaitRepairPower += creatorWeapon.fRusiuBossValue;
		}

		m_fArbaitRepairPower = changeStats.fArbaitsPower + (changeStats.fArbaitsPower * fResultArbaitRepairPower * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetArbaitRepairText ();
	}

	public void SetGoldPlusPercent()
	{
		double dResultGoldPlusPercent = 0;

		if (WeaponEquipment != null)
			dResultGoldPlusPercent += WeaponEquipment.fGoldPlus;
		if (GearEquipmnet != null)
			dResultGoldPlusPercent += GearEquipmnet.fGoldPlus;
		if (AccessoryEquipmnet != null)
			dResultGoldPlusPercent += AccessoryEquipmnet.fGoldPlus;
		if (creatorWeapon != null)
			dResultGoldPlusPercent += creatorWeapon.fPlusGoldPercent;

		m_dGoldPlusPercent = changeStats.dGoldPlusPercent + dResultGoldPlusPercent;

		if (PlayerInfo != null)
			PlayerInfo.SetGoldPlusText ();
	}

	public void SetHonorPlusPercent()
	{
		double dResultHonorPlusPercent = 0;

		if (WeaponEquipment != null)	dResultHonorPlusPercent += WeaponEquipment.fHonorPlus;
		if (GearEquipmnet != null) 		dResultHonorPlusPercent += GearEquipmnet.fHonorPlus;
		if (AccessoryEquipmnet != null) dResultHonorPlusPercent += AccessoryEquipmnet.fHonorPlus;
		if (creatorWeapon != null) 		dResultHonorPlusPercent += creatorWeapon.fPlusHonorPercent;

		m_dHonorPlusPercent = changeStats.dHornorPlusPercent + dResultHonorPlusPercent;

		if (PlayerInfo != null)
			PlayerInfo.SetHonorPlusText ();
	}

	public void SetWaterPlus()
	{
		float fResultWaterPlusPercent = 0.0f;

		if (WeaponEquipment != null)	fResultWaterPlusPercent += WeaponEquipment.fWaterChargePlus;
		if (GearEquipmnet != null) 		fResultWaterPlusPercent += GearEquipmnet.fWaterChargePlus;
		if (AccessoryEquipmnet != null) fResultWaterPlusPercent += AccessoryEquipmnet.fWaterChargePlus;
		if (creatorWeapon != null) {
			fResultWaterPlusPercent += creatorWeapon.fWaterPlus;

			if (epicOpion != null) {
				
				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_ICEPUNCH)
					fResultWaterPlusPercent += epicOpion.fWaterPlus;
			}
		}

		if (spawnManager != null) 
			//오렌지 헤어 아르바이트가 배치중이라면 스킬을 적용
			if (spawnManager.list_ArbaitUI.Count != 0) 
				if (spawnManager.m_BatchArbait [(int)E_ARBAIT.E_MIA].activeSelf == true)
					fResultWaterPlusPercent += spawnManager.array_ArbaitData [(int)E_ARBAIT.E_MIA].m_CharacterChangeData.fSkillPercent;

		//Ice골렘이 배치 됐다면
		if (spawnManager != null)
			if (spawnManager.list_ArbaitUI.Count != 0)
				if (spawnManager.m_BatchArbait [(int)E_ARBAIT.E_ICE].activeSelf == true)
					fResultWaterPlusPercent += spawnManager.array_ArbaitData[(int)E_ARBAIT.E_ICE].m_CharacterChangeData.fSkillPercent;

		m_fWaterPlus = changeStats.fWaterPlus + (changeStats.fWaterPlus * fResultWaterPlusPercent * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetWaterCharginText ();
	}

	public void SetCriticalChance()
	{
		float fResultCriticalChancePercent = 0.0f;

        //각종 장비들을 체크
		if (WeaponEquipment != null)	fResultCriticalChancePercent += WeaponEquipment.fCritical;
		if (GearEquipmnet != null) 		fResultCriticalChancePercent += GearEquipmnet.fCritical;
		if (AccessoryEquipmnet != null) fResultCriticalChancePercent += AccessoryEquipmnet.fCritical;

        //제작무기 장착
        if (creatorWeapon != null) {
			fResultCriticalChancePercent += creatorWeapon.fCriticalChance;
			fResultCriticalChancePercent += creatorWeapon.fFireBossValue;
		}

        //널스 아르바이트가 배치중이라면 스킬을 적용 

		if (spawnManager != null)
			if (spawnManager.list_ArbaitUI.Count != 0)
				if (spawnManager.m_BatchArbait [(int)E_ARBAIT.E_NURSE].activeSelf == true)
					fResultCriticalChancePercent += spawnManager.array_ArbaitData[(int)E_ARBAIT.E_NURSE].m_CharacterChangeData.fSkillPercent;
            

		m_fCriticalChance = changeStats.fCriticalChance + (changeStats.fCriticalChance * fResultCriticalChancePercent * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetCriticalChance ();
	}

	public void SetCriticalDamage()
	{
		double dResultCriticalDamagePercent = 0;

		if (WeaponEquipment != null)	dResultCriticalDamagePercent += WeaponEquipment.fCriticalDamage;
		if (GearEquipmnet != null) 		dResultCriticalDamagePercent += GearEquipmnet.fCriticalDamage;
		if (AccessoryEquipmnet != null) dResultCriticalDamagePercent += AccessoryEquipmnet.fCriticalDamage;
		if (creatorWeapon != null) {
			dResultCriticalDamagePercent += creatorWeapon.fCriticalDamage;

			if (epicOpion != null) {
				

				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_ENGINE_HAMMER) {
					if (epicOpion.bIsApplyBuff)
						dResultCriticalDamagePercent += epicOpion.fValue;
				} else if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_ICEPUNCH)
					dResultCriticalDamagePercent += epicOpion.fCriticalDamage;
				else if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER)
					dResultCriticalDamagePercent += epicOpion.fSledeCriticalDamage;

			}
		}

		m_dCriticalDamage = changeStats.dCriticalDamage + (changeStats.dCriticalDamage * dResultCriticalDamagePercent * 0.01);

		if (PlayerInfo != null)
			PlayerInfo.SetCriticalDamamgeText ();
	}

	public void SetBigSuccessed()
	{
		float fResultBigSuccessedPercent = 0.0f;

		if (WeaponEquipment != null)	fResultBigSuccessedPercent += WeaponEquipment.fBigCritical;
		if (GearEquipmnet != null) 		fResultBigSuccessedPercent += GearEquipmnet.fBigCritical;
		if (AccessoryEquipmnet != null) fResultBigSuccessedPercent += AccessoryEquipmnet.fBigCritical;
		if (creatorWeapon != null) 
		{
			fResultBigSuccessedPercent += creatorWeapon.fBigSuccessed;

			if (epicOpion != null) {
				
				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_RUBBER_CHICKEN)
					fResultBigSuccessedPercent += epicOpion.fResultValue;
			}
		}

		m_fBigSuccessed = changeStats.fBigSuccessed + (changeStats.fBigSuccessed * fResultBigSuccessedPercent * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetBigSuccessedText ();
	}

	public void SetFeverTime ()
	{
		float fResultFeverPercent = 0.0f;

		if (creatorWeapon != null) 
		{
			if (epicOpion != null) {
				
				if (epicOpion.nIndex == (int)E_EPIC_INDEX.E_EPIC_GOLD_HAMMER)
					fResultFeverPercent += 50;
			}
		}

		m_fFeverTime = changeStats.fFeverTime + (changeStats.fFeverTime * fResultFeverPercent * 0.01f);

		if (PlayerInfo != null)
			PlayerInfo.SetFeverTimeText ();
	}


	public void Init(List<CGameEquiment> _itemList, CGamePlayerData _defaultStats,CreatorWeapon _creatorWeapon)
    {
        List_items = _itemList;
		creatorWeapon = _creatorWeapon;
		changeStats = new CGamePlayerData(_defaultStats);
    }

	public void SetInventroy(Inventory _inventory,SpriteRenderer _image)
    {
		MoruImage = _image;

        inventory = _inventory;

        inventory.SetInventory(this, List_items);
    }

	public int GetItemListCount()
	{
		if (inventory == null)
			return 0;

		List_items = inventory.GetItemList ();

		if (List_items == null)
			return 0;

		return List_items.Count;
	}

	public int GetShopMaxCount()
	{
		return changeStats.nShopMaxCount;
	}

	public void SetShopMaxCount(int _nValue)
	{
		changeStats.nShopMaxCount = _nValue;
	}

    /*
	public E_BOSS_WEAPON Check_Equipment()
	{
		if (WeaponEquipment != null)
			return WeaponEquipment.nIndex;

		return null;
	}*/

    //아이템을 장착할 경우
    public void EquipItem(CGameEquiment _item)
    {
        //아이템이 어디 부위인지 확인한다.
        switch (_item.nSlotIndex)
        {
		case (int)E_EQUIMNET_INDEX.E_WEAPON:

                //만약 무기가 있을 경우 그 무기가 현재 플레이어에 적용되는 값을 빼고 아이템을 넣어줌
                //그 후 다시 아이템 효과를 플레이어에게 적용한다.
			if (WeaponEquipment == _item) {

				WeaponEquipment.bIsEquip = false;

				WeaponEquipment = null;

				MoruImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache ("ShopItem/0");

				PlayerStatsSetting ();

				return;
			} else if (WeaponEquipment != null) {
				WeaponEquipment.bIsEquip = false;

			}

			WeaponEquipment = _item;

			MoruImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (WeaponEquipment.strResource);

			WeaponEquipment.bIsEquip = true;

                break;
            case (int)E_EQUIMNET_INDEX.E_WEAR:

                if (GearEquipmnet == _item)
                {
                    GearEquipmnet.bIsEquip = false;

                    GearEquipmnet = null;

                    PlayerStatsSetting();

                    return;
                }
                else if (GearEquipmnet != null)
                    GearEquipmnet.bIsEquip = false;


                GearEquipmnet = _item;

                GearEquipmnet.bIsEquip = true;
                break;
            case (int)E_EQUIMNET_INDEX.E_ACCESSORY:


                if (AccessoryEquipmnet == _item)
                {

                    AccessoryEquipmnet.bIsEquip = false;

                    AccessoryEquipmnet = null;

                    PlayerStatsSetting();

                    return;
                }

                else if (AccessoryEquipmnet != null)
                    AccessoryEquipmnet.bIsEquip = false;


                AccessoryEquipmnet = _item;

                AccessoryEquipmnet.bIsEquip = true;

                break;
        }
        PlayerStatsSetting();

        //아이템을 장착했기에 다시 정렬
        inventory.inventorySlots[_item.nSlotIndex].RefreshDisplay();
    }

	//아이템을 해제 할 경우 
	public void NoneEquipItem(CGameEquiment _item)
	{
		//아이템이 어디 부위인지 확인한다.
		switch (_item.nSlotIndex)
		{
		case (int)E_EQUIMNET_INDEX.E_WEAPON:
			WeaponEquipment = null;
			break;
		case (int)E_EQUIMNET_INDEX.E_WEAR:
			GearEquipmnet = null;
			break;
		case (int)E_EQUIMNET_INDEX.E_ACCESSORY:
			AccessoryEquipmnet = null;
			break;
		}
		PlayerStatsSetting();

		//아이템을 장착했기에 다시 정렬
		inventory.inventorySlots[_item.nSlotIndex].RefreshDisplay();
	}


    //계속해서 받아올 수 없기에 미리 캐싱해둠
    public void GetSpawnManager(SpawnManager _spawnManager)
    {
        spawnManager = _spawnManager;
    }


}




