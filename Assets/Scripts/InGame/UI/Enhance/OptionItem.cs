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
				GameManager.Instance.Window_yesno ("에픽 옵션을 잠금 하시겠습니까?", "50", rt => 
					{
						if(rt == "0")
						{


							WeaponOption.bIsLock = true;
							makingUI.createEpic.bIsLock = true;
							OptionImage.sprite = ActiveEpicSprite;
							LockImage.sprite = ActiveEpicLockSprite;
						}
						else if(rt == "1")
						{
							if(ScoreManager.ScoreInstance.GetRuby() >= 50)
							{
								WeaponOption.bIsLock = true;
								makingUI.createEpic.bIsLock = true;
								OptionImage.sprite = ActiveEpicSprite;
								LockImage.sprite = ActiveEpicLockSprite;
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
						makingUI.createEpic.bIsLock = false;
						OptionImage.sprite = UnActiveEpicSprite;
						LockImage.sprite = UnActiveEpicLockSprite;

					} else if (rt == "1") {
						return;
					}

				}
				);


			}

		} 
		else if (WeaponOption.nIndex <= (int)E_CREATOR.E_MAX) 
		{

		}
	}
}
