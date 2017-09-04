using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class OptionItem : MonoBehaviour {

	private Button LockButton;

	public Image OptionImage;
	public Image LockImage;

	public Sprite OptionSprite;
	public Sprite UnOptionSprite;

	public Sprite LockSprite;
	public Sprite UnLockSprite;

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

		OptionImage.sprite = UnOptionSprite;
		LockImage.sprite = UnLockSprite;
	}

	public void Setup(CGameMainWeaponOption _WeaponOption,MakingUI _makingUI)
	{
		makingUI = _makingUI;
		WeaponOption = _WeaponOption;

		if (_WeaponOption.nIndex != (int)E_CREATOR.E_EPIC)
			OptionText.text = string.Format ("{0} : {1}", WeaponOption.strOptionName, WeaponOption.nValue);
		
		else
			OptionText.text = WeaponOption.strOptionName;

		if (WeaponOption.bIsLock) 
		{
			OptionImage.sprite = OptionSprite;
			LockImage.sprite = LockSprite;
		} 
		else 
		{
			OptionImage.sprite = UnOptionSprite;
			LockImage.sprite = UnLockSprite;
		}

	}

	//버튼 클릭시 아이템을 보여주기 위함
	public void HandleClick()
	{
		if (WeaponOption.nIndex == (int)E_CREATOR.E_EPIC) 
		{
			if (WeaponOption.bIsLock == false) 
			{
				WeaponOption.bIsLock = true;
				makingUI.createEpic.bIsLock = true;
				OptionImage.sprite = OptionSprite;
				LockImage.sprite = LockSprite;
			} 
			else 
			{
				WeaponOption.bIsLock = false;
				makingUI.createEpic.bIsLock = false;
				OptionImage.sprite = UnOptionSprite;
				LockImage.sprite = UnLockSprite;
			}

		} 
		else if (WeaponOption.nIndex <= (int)E_CREATOR.E_MAX) 
		{

		}
	}
}
