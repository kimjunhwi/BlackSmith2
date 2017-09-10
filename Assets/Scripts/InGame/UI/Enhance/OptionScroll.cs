using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class OptionScroll : MonoBehaviour {

	public Image BackgroundImage;
	public Text OptionText;

	public Sprite EpicSprite;
	public Sprite BossSprite;
	public Sprite NormalSprite;

	public void Awake()
	{
		BackgroundImage = gameObject.GetComponent<Image> ();
		OptionText = transform.GetChild (0).GetComponent<Text> ();
	}

	public void Setting(CGameMainWeaponOption _option)
	{
		if (_option.nIndex == (int)E_CREATOR.E_EPIC)
			BackgroundImage.sprite = EpicSprite;
		
		else if (_option.nIndex >= (int)E_CREATOR.E_BOSS_ICE)
			BackgroundImage.sprite = BossSprite;
		
		else
			BackgroundImage.sprite = NormalSprite;

		OptionText.text = _option.strOptionExplain;
	}
}