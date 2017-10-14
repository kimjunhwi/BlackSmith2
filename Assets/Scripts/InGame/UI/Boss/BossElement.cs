using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BossElement : MonoBehaviour 
{
	public BossCreator bossCreator;

	public Text bossLevel_Text;
	public Text BossLeftCount_Text;
	public Text BossHealth_Text;

	public Button bossLevelRight_Button;
	public Button bossLevelLeft_Button;

	public GameObject ReloadButton_Obj;			//충전 버튼
	public Button ReloadButton;

	public Button BossExplian_Button;	//보스설명버튼

	public GameObject BossUnlock_Obj;	//보스 락 이미지



	public int nBossIndex = 0;

	public int curLevel = 1;	//현재래벨
	public int maxLevel = 1;	//최대래벨
	private int minLevel = 1;	//최소래벨



	public void AddLevel()
	{
		
		SoundManager.instance.PlaySound(eSoundArray.ES_TouchSound_Menu);
		if (curLevel < maxLevel)
		{
			if (curLevel != 1 || maxLevel != 1) {
				curLevel++;
				if (curLevel >= maxLevel) 
				{
					maxLevel = curLevel;
					bossLevelRight_Button.gameObject.SetActive (false);
				}
			}
		}
		bossLevel_Text.text = string.Format ("Lv {0}", curLevel);

		ShowBossHealth ();
	}

	public void MinusLevel()
	{

		SoundManager.instance.PlaySound(eSoundArray.ES_TouchSound_Menu);
		if (curLevel > minLevel) {
			curLevel--;
			bossLevelRight_Button.gameObject.SetActive (true);
		}
		bossLevel_Text.text = "Lv " + curLevel.ToString ();
		ShowBossHealth ();
	}

	public void ShowBossInfo(int _nIndex)
	{
		Image tmpImage = bossCreator.bossHint_Obj.GetComponent<Image> ();
		tmpImage.sprite = bossCreator.bossInfoSprite [_nIndex];
		bossCreator.bossHint_Obj.GetComponent<Image> ().sprite = tmpImage.sprite;
		bossCreator.bossHint_Obj.SetActive (true);
		bossCreator.bossHint_Obj.GetComponent<BossHintTextBlink> ().StartBlinkText ();
	}


	public void  ShowBossHealth()
	{
		float fOriValue = (24 + (curLevel * 5));
		float fMinusValue = (Mathf.Floor( (24f + (float)curLevel * 5f) * 0.1f ) ) * 10;
		float result = fOriValue - fMinusValue;
		double dCurComplete = 0;
		if(nBossIndex == 0)
			dCurComplete = ( GameManager.Instance.bossInfo [nBossIndex].dComplate * Math.Pow (2, (Mathf.Floor( Mathf.Max (((24 + (curLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 15;
		else if(nBossIndex == 1)
			dCurComplete = ( GameManager.Instance.bossInfo [nBossIndex].dComplate * Math.Pow (2, (Mathf.Floor( Mathf.Max (((44 + (curLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 15;
		else if(nBossIndex == 2)
			dCurComplete = ( GameManager.Instance.bossInfo [nBossIndex].dComplate * Math.Pow (2, (Mathf.Floor( Mathf.Max (((64 + (curLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 15;
		else
			dCurComplete = ( GameManager.Instance.bossInfo [nBossIndex].dComplate * Math.Pow (2, (Mathf.Floor( Mathf.Max (((84 + (curLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 15;


		string strCurBossHealth =  SpawnManager.Instance.repairObject.ChangeValue (dCurComplete);

		BossHealth_Text.text = string.Format ("{0}", strCurBossHealth);
	}
}
