﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BossTalkPanel : MonoBehaviour  , IPointerDownHandler
{

	public Text bossTalk_Text;
	public GameObject bossTalkPanel;
	public Image bossTalkTextBlink_Image;

	private bool isTextBlink = false;
	private bool isTextTouchAvailable = false;

	void Start()
	{
		bossTalk_Text.text = "";
		bossTalkPanel.SetActive (false);
	}

	public void StartBossTalkAvailableBlink()
	{
		isTextTouchAvailable = false;

		StartCoroutine (BossTalkAvailableBlink ());
	}

	public IEnumerator BossTalkAvailableBlink()
	{
		isTextTouchAvailable = true;
		isTextBlink = false;

		while (true) 
		{
			if (isTextBlink == true)
				yield break;

			if (bossTalkTextBlink_Image.isActiveAndEnabled == true)
				bossTalkTextBlink_Image.enabled = false;
			else
				bossTalkTextBlink_Image.enabled = true;

			yield return new WaitForSeconds(0.2f);
		}

		yield return null;
	}

	public void StartShowTutorialBossTalkWindow(float _fTime, string _string)
	{
		StartCoroutine(ShowTutorialBossWindow(_fTime, _string));
	}

	public IEnumerator ShowTutorialBossWindow(float _fTime, string _string)
	{
		int nCount = _string.Length;
		int nIndex = 0;
		string strGetText01 = "";
		string getString = _string;
		bossTalkPanel.SetActive (true);

		while (true) 
		{
			

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX01) 
			{
				//Debug.Log("Before Dragon");
				//End Condition
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02) 
			{
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03) 
			{
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_END_DRAGONREPAIR) 
			{
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04) 
			{
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05) 
			{
				if (nIndex >= nCount)
				{
					StartShowBossTimer(2f);
					StartBossTalkAvailableBlink ();
					yield break;
				}
				if (nIndex < getString.Length)
				{
					strGetText01 += getString [nIndex];
					bossTalk_Text.text = strGetText01;
					nIndex++;
				} 
			}


			yield return new WaitForSeconds (0.075f);

		}
	}

	public void StartShowBossTimer(float _time)
	{
		StartCoroutine (ShowBossTimer (_time));
	}

	public IEnumerator ShowBossTimer(float _time)
	{
		float fTime = _time;
		while (true) 
		{
			fTime -= Time.deltaTime;
			if (fTime <= 0) 
			{
				if (isTextBlink == true)
					yield break;

				//ChangeState
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX01) 
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02;
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02) 
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX03;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (2);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX04;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (3);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}

				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_END_DRAGONREPAIR)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX05;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (4);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}

				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX06;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (5);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONBREATH;
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					yield break;
				}
			}
			yield return null;
		}

	}

	public void StartShowBossTalkWindow(float _fTime, string _string)
	{
		StartCoroutine (ShowBossTalkWindow (_fTime, _string));
	}

	public IEnumerator ShowBossTalkWindow(float _fTime, string _string)
	{
		bossTalkPanel.SetActive (true);
		bossTalk_Text.text = _string;

		while (_fTime >= 0) {
			_fTime -= Time.deltaTime;
			yield return null;
		}
		bossTalkPanel.SetActive (false);
		bossTalk_Text.text = "";

		yield break;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoGameObject = eventData.pointerEnter;

		if (isTextTouchAvailable == true) 
		{

			isTextTouchAvailable = false;
			isTextBlink = true;				//Blink될때 클릭을 했으니 위에 것은 return;

			if (getInfoGameObject.name == "BossTouchCheckImage") 
			{
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX01) 
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02;
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02) 
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX03;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (2);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX04;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (3);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}

				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_END_DRAGONREPAIR)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX05;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (4);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}

				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX06;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (5);
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}
				if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05)
				{
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONBREATH;
					bossTalkPanel.SetActive (false);
					bossTalk_Text.text = "";
					return;
				}

				//다음 대화로 넘어간다
				//특정 조건에 따라 연속적인 텍스트도 필요
			}
		}
	}

}
