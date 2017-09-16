﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{

    public GameObject obj;
	public BossCreator bossCreator;

	public GameObject []uiPanels;
	public Button []uiButtons;
	public GameObject yesNoButton; //BossInfo
	public Sprite [] uiSprite;	   //하이라이트 sprite 조정을 위한 sprite 배열
	Button uiButton ;
	public GameObject uiBossFirstFightMark;
	public ShopCash shopCash;
	public QusetManager questManager;

	void Start()
	{
		//shopCash.StartSetUp ();

		AllDisable ();

		GameManager.Instance.Root_ui = gameObject;
		uiBossFirstFightMark.SetActive (false);
			
	}

	public void ActiveMenu(int nIndex)
	{
		//Tutorial
		if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE03) {
			SpawnManager.Instance.tutorialPanel.ShowTutorialImage (3);

		}

		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		if (uiPanels [nIndex].activeSelf) 
		{
			//보스 패널 닫을시 시간 저장 
			if (nIndex == 3)
			{
				bossCreator.BossPanelInfoSave ();
				yesNoButton.SetActive (false);
			}
			uiPanels [nIndex].SetActive (false);

			//퀘스트 패널 열고 닫을때 저장 정보
			if(nIndex == 5)
			{
				questManager.SaveQuestData ();
			}

			if (nIndex != 5 && nIndex != 6) 
			{
				SpriteState tmpSpriteState = new SpriteState ();
				tmpSpriteState.disabledSprite = uiButtons [nIndex].spriteState.disabledSprite;
				tmpSpriteState.pressedSprite = uiButtons [nIndex].spriteState.pressedSprite;
				tmpSpriteState.highlightedSprite = null;
		
				uiButtons [nIndex].spriteState = tmpSpriteState;

				AllHilightImageAdd (nIndex);
			}
		}
		else
		{
			AllDisable ();
			uiPanels [nIndex].SetActive (true);
			if (nIndex != 5 && nIndex != 6)
			{
				
				SpriteState tmpSpriteState = new SpriteState ();
				tmpSpriteState.disabledSprite = uiButtons [nIndex].spriteState.disabledSprite;
				tmpSpriteState.pressedSprite = uiButtons [nIndex].spriteState.pressedSprite;
				tmpSpriteState.highlightedSprite = uiSprite [nIndex];

				uiButtons [nIndex].spriteState = tmpSpriteState;

			}
			yesNoButton.SetActive (false);



			//보스 패널 열시 시간 로드 
			if (nIndex == 3) 
			{

				Debug.Log ("BossPanelOn!!");
				bossCreator.CheckCurDaysAndBossUnlock ();
				bossCreator.BossPanelSetUp ();
				bossCreator.bossConsumeItemInfo.BossInviteMentLoadTime ();
				bossCreator.bossConsumeItemInfo.bossRegenTimer.BossRegenTimeLoad ();

			}
		
		}
	}

	//활성화된 버튼 빼고 나머지는 하이라이트 이미지를 넣는다.
	public void AllHilightImageAdd(int _nIndex )
	{
		for(int i  =0; i < 5; i++)
		{
			if (i != _nIndex) {
				SpriteState tmpSpriteState = new SpriteState ();
				tmpSpriteState.disabledSprite = uiButtons [i].spriteState.disabledSprite;
				tmpSpriteState.pressedSprite = uiButtons [i].spriteState.pressedSprite;
				tmpSpriteState.highlightedSprite = uiSprite [i];

				uiButtons [i].spriteState = tmpSpriteState;
			}
		}

	}

	public void AllDisable()
	{
		
		foreach (GameObject obj in uiPanels) 
		{
			obj.SetActive (false);	
		}
	}


}
