using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class OptionItem : MonoBehaviour {

	private Button LockButton;

	public Image OptionImage;
	public Image LockImage;

	public Sprite UnActiveEpicSprite;
	public Sprite ActiveEpicSprite;

	public Sprite UnActiveBossSprite;

	public Sprite UnActiveNormalSprite;

	public Sprite UnActiveEpicLockSprite;
	public Sprite ActiveEpicLockSprite;

	public Sprite NoneResetSprite;
	public Sprite ResetSprite;

	public Text OptionText;

	private double dCostGold;

	private MakingUI makingUI;
	private CGameMainWeaponOption WeaponOption;

	void Awake()
	{
		LockButton = transform.Find("LockButton").GetComponent<Button>();

		LockButton.onClick.AddListener(HandleClick);
	}

	void OnEnable()
	{

	}

	public void SetInit()
	{
		WeaponOption = null;

		OptionText.text = null;

		OptionImage.sprite = UnActiveNormalSprite;
		LockImage.sprite = ResetSprite;
	}

	public void Setup(CGameMainWeaponOption _WeaponOption,MakingUI _makingUI)
	{
		makingUI = _makingUI;
		WeaponOption = _WeaponOption;

		OptionText.text = WeaponOption.strOptionExplain;

		if (_WeaponOption.nIndex == (int)E_CREATOR.E_EPIC) {
			if (WeaponOption.bIsLock) {
				OptionImage.sprite = ActiveEpicSprite;
				LockImage.sprite = ActiveEpicLockSprite;
			} else {
				OptionImage.sprite = UnActiveEpicSprite;
				LockImage.sprite = UnActiveEpicLockSprite;
			}

			OptionText.text = _WeaponOption.strOptionExplain;

		} else if (_WeaponOption.nIndex >= (int)E_CREATOR.E_BOSS_ICE) 
		{
			OptionImage.sprite = UnActiveBossSprite;
			LockImage.sprite = NoneResetSprite;
		} 
		else 
		{
			OptionImage.sprite = UnActiveNormalSprite;

			LockImage.sprite = ResetSprite;
		}
	}

	//버튼 클릭시 아이템을 보여주기 위함
	public void HandleClick()
	{
		if (WeaponOption.nIndex == (int)E_CREATOR.E_EPIC) 
		{


			if (WeaponOption.bIsLock == false) 
			{
				GameManager.Instance.Window_yesno ("에픽 옵션을 잠금 하시겠습니까?", "50",ObjectCashing.Instance.LoadSpriteFromCache("Store/GoldShop/quest_popup_reset_ruby"), rt => 
					{
						if(rt == "0")
						{
							GameManager.Instance.ShowRewardedAd_EpicChange(this);
						}
						else if(rt == "1")
						{
							if(ScoreManager.ScoreInstance.GetRuby() >= 50)
							{
								ScoreManager.ScoreInstance.RubyPlus(-50);

								WeaponOption.bIsLock = true;
								OptionImage.sprite = ActiveEpicSprite;
								LockImage.sprite = ActiveEpicLockSprite;
								GameManager.Instance.GetPlayer().GetCreatorWeapon().bIsLock = true;
							}
							else
							{
								GameManager.Instance.Window_notice("루비가 부족합니다",null);
							}

						}
					}
				);
			} 
			else 
			{

				GameManager.Instance.Window_Check ("에픽 옵션 잠금을 해제 하시겠습니까?", rt => {
					if (rt == "0") {
						WeaponOption.bIsLock = false;
						OptionImage.sprite = UnActiveEpicSprite;
						LockImage.sprite = UnActiveEpicLockSprite;
						GameManager.Instance.GetPlayer().GetCreatorWeapon().bIsLock = false;
					} else if (rt == "1") {
						return;
					}

				}
				);


			}

		} 
		else if (WeaponOption.nIndex <= (int)E_CREATOR.E_MAX) 
		{
			dCostGold = 250 * Mathf.Pow (1.09f, GameManager.Instance.GetPlayer().GetCreatorWeapon().nCostDay - 1) * 5 * GameManager.Instance.GetPlayer().GetCreatorWeapon().nOptionChangeCount;

			string strCostGold = ScoreManager.ScoreInstance.ChangeMoney (dCostGold);


			GameManager.Instance.Window_yesno ("옵션을 변경 하시겠습니까?", strCostGold, ObjectCashing.Instance.LoadSpriteFromCache ("Store/GoldShop/popup_or_gold"), rt => 
				{
					//광고
					if(rt == "0")
					{
						GameManager.Instance.ShowRewardedAd_Option(this);
					}
					//골드
					else if(rt == "1")
					{
						if(ScoreManager.ScoreInstance.GetGold() >= dCostGold)
						{
							ScoreManager.ScoreInstance.GoldPlus(-dCostGold);

							ChageOption();
						}
						else
						{
							GameManager.Instance.Window_notice("골드가 부족합니다",null);
						}
					}
				}
			);
		}
	}

	public void Lock_EpicOption()
	{
		WeaponOption.bIsLock = true;
		OptionImage.sprite = ActiveEpicSprite;
		LockImage.sprite = ActiveEpicLockSprite;
		GameManager.Instance.GetPlayer().GetCreatorWeapon().bIsLock = true;
	}

	public void ChageOption()
	{

		CreatorWeapon weapon = GameManager.Instance.GetPlayer ().GetCreatorWeapon ();

		weapon.nOptionChangeCount++;

		int nRandomOptionLength = 1;

		//옵션 셋팅 
		while(nRandomOptionLength > 0)
		{
			int nInsertIndex = 0;

			nInsertIndex = Random.Range((int)E_CREATOR.E_REPAIRPERCENT, (int)E_CREATOR.E_MAX);

			if (makingUI.CheckData(weapon, nInsertIndex, (int)WeaponOption.nValue ))
				nRandomOptionLength--;
		}

		switch (WeaponOption.nIndex) {
		case (int)E_CREATOR.E_REPAIRPERCENT:
			weapon.fRepairPercent = 0;
			break;
		case (int)E_CREATOR.E_ARBAIT:
			weapon.fArbaitRepair = 0;
			break;
		case (int)E_CREATOR.E_HONOR:
			weapon.fPlusHonorPercent = 0;
			break;
		case (int)E_CREATOR.E_GOLD:
			weapon.fPlusGoldPercent = 0;
			break;
		case (int)E_CREATOR.E_WATERCHARGE:
			weapon.fWaterPlus = 0;
			break;
		case (int)E_CREATOR.E_CRITICAL:
			weapon.fCriticalChance = 0;
			break;
		case (int)E_CREATOR.E_CRITICALD:
			weapon.fCriticalDamage = 0;
			break;
		case (int)E_CREATOR.E_BIGCRITICAL:
			weapon.fBigSuccessed = 0;
			break;
		case (int)E_CREATOR.E_ACCURACY:
			weapon.fAccuracyRate = 0;
			break;
		}

		makingUI.DeleteOption (WeaponOption);
	}
}
