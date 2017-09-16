using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossElement : MonoBehaviour 
{
	public BossCreator bossCreator;

	public Text bossLevel_Text;
	public Text BossLeftCount_Text;
	public Button bossLevelRight_Button;
	public Button bossLevelLeft_Button;

	public GameObject ReloadButton_Obj;			//충전 버튼
	public Button ReloadButton;

	public Button BossExplian_Button;	//보스설명버튼

	public GameObject BossUnlock_Obj;	//보스 락 이미지

	public int nBossIndex = 0;

	public int curLevel = 1;	//현재래벨
	private int maxLevel = 100;	//최대래벨
	private int minLevel = 1;	//최소래벨



	public void AddLevel()
	{
		if (curLevel < maxLevel)
			curLevel++;
		bossLevel_Text.text = string.Format ("Lv {0}", curLevel);
	}

	public void MinusLevel()
	{
		if (curLevel > minLevel)
			curLevel--;
		bossLevel_Text.text = "Lv " + curLevel.ToString ();
	}

	public void ShowBossInfo(int _nIndex)
	{
		Image tmpImage = bossCreator.bossHint_Obj.GetComponent<Image> ();
		tmpImage.sprite = bossCreator.bossInfoSprite [_nIndex];
		bossCreator.bossHint_Obj.GetComponent<Image> ().sprite = tmpImage.sprite;
		bossCreator.bossHint_Obj.SetActive (true);
		bossCreator.bossHint_Obj.GetComponent<BossHintTextBlink> ().StartBlinkText ();
	}

}
