﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerTalk : MonoBehaviour ,IPointerDownHandler
{
	public Text m_playerTalk_Text;	//player의 말풍선 텍스트
	float m_fContinueTextTime;		//텍스트 지속되는 시간 이 시간이 지나면 자동으로 다음으로 넘어간다
	public TutorialPanel tutorialPanel;
	//플레이어 말들(튜토리얼)
	private  string [] m_sPlayerText = new string[8];
	private bool isTextShowAll;		//텍스트가 모두 보여졌는가?
	private bool isPlayerText01AndGuestShow;

	public Image arrow_Image;
	private bool isTextBlink = false;
	private bool isTextTouchAvailable = false;


	void Start()
	{
		isTextShowAll = false;
		isPlayerText01AndGuestShow = false;
		m_sPlayerText[0] = "어서오세요!";
		m_sPlayerText[1] = "다했다!!!!\n밥 먹어야지~";
		m_sPlayerText[2] = "아조씨 영업시간 끝났어요...";
		m_sPlayerText[3] = "ㅠㅠ\n끝났어요..";
		m_sPlayerText[4] = "어이쿠...";
		m_sPlayerText[5] = "^_^"; 
		m_sPlayerText[6] = "부들부들...";
		m_sPlayerText[7] = "기억해두겠어!!";
		gameObject.SetActive (false);
	}


	public void StartPlayerTalk(int _index)
	{
		m_fContinueTextTime = 2f;
		StartCoroutine (SetPlayerTalkText (_index));
	}


	//플레이어의 말 세팅
	public IEnumerator SetPlayerTalkText(int  _index)
	{
		isTextShowAll = false;
		string getString = m_sPlayerText [_index];
		m_playerTalk_Text.text = "";
		int nStrIndex = 0;
		while (true)
		{
			if (isTextShowAll == true)
			{
				m_fContinueTextTime -= Time.deltaTime;

				//일정시간이 지나면 다음 껄로 넘어간다
				if (m_fContinueTextTime <= 0) 
				{
					//텍스트를 클릭하면 넘어간다
					if (isTextBlink == true)
						yield break;
					
					//첫번째 플레이어 말풍선후 손님들 등장
					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX01 && isPlayerText01AndGuestShow == false) {
						tutorialPanel.StartGuestShow ();	
						TalkBoxOnOff (false);
						yield break;
					}

					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX02)
					{
						TalkBoxOnOff (false);
						tutorialPanel.DeActiveObj.SetActive (true);
						SpawnManager.Instance.repairObject.AllDebuffIconInit ();
						tutorialPanel.StartTutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEIN);
						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK02;
					
						yield break;
					}

					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX03) 
					{
						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03;
						TalkBoxOnOff (false);
						yield break;
					}

					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX04)
					{
						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORAIL_START_DRAGONREPAIR;
						TalkBoxOnOff (false);
						yield break;
					}

					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX05) {

						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04;
						TalkBoxOnOff (false);
						yield break;
					}
					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX06) 
					{

						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05;
						TalkBoxOnOff (false);
						yield break;
					}

					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX07) 
					{

						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX08;
						TalkBoxOnOff (false);
						yield break;
					}



					if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX08) 
					{

						tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;
						TalkBoxOnOff (false);
						yield break;
					}



				} 
				else
					yield return null;
			}
			else
			{
				if (nStrIndex < getString.Length) 
				{
					m_playerTalk_Text.text += getString [nStrIndex];
					nStrIndex++;
				} 
				else
				{
					isTextShowAll = true;
					StartTextAvailableBlink ();
				}


				yield return new WaitForSeconds (0.075f);
			}
		}
	}

	public void StartTextAvailableBlink()
	{
		isTextTouchAvailable = false;
		StartCoroutine (TextAvailableBlink ());
	}

	public IEnumerator TextAvailableBlink()
	{
		isTextTouchAvailable = true;
		isTextBlink = false;

		while (true) 
		{
			if (arrow_Image.isActiveAndEnabled == true)
				arrow_Image.enabled = false;
			else
				arrow_Image.enabled = true;

			yield return new WaitForSeconds(0.2f);
		}

		yield return null;

	}


	//말풍선 창 On/off
	public void TalkBoxOnOff(bool _isOnOff)
	{
		gameObject.SetActive (_isOnOff);
	}

	//말풍선
	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoGameObject = eventData.pointerEnter;

		if (isTextTouchAvailable == true) {
			isTextTouchAvailable = false;
			isTextBlink = true;				//Blink될때 클릭을 했으니 위에 것은 return;
			if (getInfoGameObject.name == "PlayerTouchCheckImage")

			{
				//ShowGuset를 하면 손님 5명이 나오는 동시에 튜토이미지 1,2 실행
				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX01) {
					tutorialPanel.StartGuestShow ();
					isPlayerText01AndGuestShow = true;
					TalkBoxOnOff (false);
					return;
				}

				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX02)
				{
					TalkBoxOnOff (false);
					tutorialPanel.DeActiveObj.SetActive (true);
					tutorialPanel.StartTutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEIN);
					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK02;
					return;
				
				}


				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX03) 
				{
					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03;
					TalkBoxOnOff (false);
					return;
				}

				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX04)
				{
					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORAIL_START_DRAGONREPAIR;
					TalkBoxOnOff (false);
					return;
				}

				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX05) {

					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04;
					TalkBoxOnOff (false);
					return;
				}
				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX06) 
				{

					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05;
					TalkBoxOnOff (false);
					return;
				}

				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX07) 
				{

					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX08;
					TalkBoxOnOff (false);
					return;
				}



				if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX08) 
				{

					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;
					TalkBoxOnOff (false);
					return;
				}


				//다음 대화로 넘어간다
				//특정 조건에 따라 연속적인 텍스트도 필요
			}
		}

	}

}
