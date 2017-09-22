using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialOption
{
	E_TUTORIAL_OPTION_FADEIN,
	E_TUTORIAL_OPTION_FADEOUT,
}

public enum TutorialOrder
{
	E_TUTORIAL_START_FULLSCREENTALK01 = 0,			//대화 화면 01
	E_TUTORIAL_START_PLAYERTALKBOX01,				//플레이어 말풍선 01
	E_TUTORIAL_START_IMAGE01,						//tuto image 01	repair
	E_TUTORIAL_START_IMAGE02,						//tuto image 02	water
	E_TUTORIAL_START_PLAYERTALKBOX02,				//플레이어 말풍선 02
	E_TUTORIAL_START_FULLSCREENTALK02,				//대화 화면 02 노 터치(시간으로만)
	E_TUTORIAL_WAIT_DRAGONSHOW,
	E_TUTORIAL_START_DRAGONSHOW,
	E_TUTORIAL_START_DRAGONTALKBOX01,				//dragon 말풍선 01
	E_TUTORIAL_START_DRAGONTALKBOX02,				//dragon 말풍선 02
	E_TUTORIAL_START_PLAYERTALKBOX03,
	E_TUTORIAL_START_DRAGONTALKBOX03,				//dragon 말풍선 02
	E_TUTORIAL_START_PLAYERTALKBOX04,
	E_TUTORAIL_START_DRAGONREPAIR,					//용 무기 수리 시작...
	E_TUTORIAL_START_IMAGE03,
	E_TUTORIAL_START_IMAGE04,
	E_TUTORIAL_END_DRAGONREPAIR,
	E_TUTORIAL_START_PLAYERTALKBOX05,
	E_TUTORIAL_START_DRAGONTALKBOX04,				//dragon 말풍선 02
	E_TUTORIAL_START_PLAYERTALKBOX06,
	E_TUTORIAL_START_DRAGONTALKBOX05,				//dragon 말풍선 02
	E_TUTORIAL_START_DRAGONBREATH,					//용 숨결
	E_TUTORIAL_START_PLAYERTALKBOX07,
	E_TUTORIAL_START_PLAYERTALKBOX08,
	E_TUTORIAL_START_DAYS,							//몇일 후....
	E_TUTORIAL_START_SHOWCONSTRUCT,
	E_TUTORIAL_FINISH,
	E_TUTORIAL_NONE = 9999,
}


public class TutorialPanel : MonoBehaviour 
{
	public Text text01;
	public Text text02;

	public GameObject button;


	public GameObject tutorialImage_Obj;
	public Image tutorial_Image;
	public TutorialTouch [] tutorialTouches;
	public PlayerTalk playerTalk;
	public TutorialOrder eTutorialState = TutorialOrder.E_TUTORIAL_NONE;


	//FullScreenText Order
	private const string strText01 = "최고의 대장장이 였던 아버지의 명성을 이어받은 나";
	private const string strText02 = "오늘도 하루 일을 시작해볼까?";
	private const string strText03 = "언제나 평화로운 대장간이였다.";
	private const string strText04 = "하지만..."; 
	private const string strText05 = "모든걸 잃은 나는...";
	private const string strText06 = "처음부터 시작하는 대장간 라이프";

	private bool bTextEnd = false;

	private string[] strTutorialImage_path = new string[4];
	private int nTutorialImage_Index = 0;


	private bool bIsDragonRealShow = false;

	public GameObject DeActiveObj;
	public Image DeActiveObj_Image;
	private float m_fAlphaValue = 0.4f;
	private float m_fTutorialFullScrreneAlpha;
	Color originColor;

	public GameObject DragonWepaonBlack_Obj;



	public void StartContent()
	{
		if (eTutorialState == TutorialOrder.E_TUTORIAL_NONE)
		{
			button.GetComponent<Button> ().onClick.AddListener (StartGoShowOpen);
			eTutorialState = TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK01;
			playerTalk.tutorialPanel = this;
		}
		else
			eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;
			
		strTutorialImage_path[0] = 	"Tutorial/Tutorial01";
		strTutorialImage_path[1] = 	"Tutorial/Tutorial02";
		strTutorialImage_path[2] = 	"Tutorial/Tutorial03";
		strTutorialImage_path[3] = 	"Tutorial/Tutorial04";
		StartCoroutine (startText ());
	}

	public void StartFullScreenText()
	{
		StartCoroutine (startText ());
	}

	public IEnumerator startText()
	{
		int nCount = strText01.Length + strText02.Length;
		int nIndex = 0;
		int nIndex2 = 0;
		string strGetText01 = "";
		string strGetText02 = "";

		while (true) 
		{
			yield return new WaitForSeconds (0.1f);
		
			if (eTutorialState == TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK01) 
			{
				//Debug.Log("Before Dragon");
				//End Condition
				if (nIndex + nIndex2 >= nCount)
				{
					button.SetActive (true);

					yield break;
				}
				if (nIndex < strText01.Length)
				{
					strGetText01 += strText01 [nIndex];
					text01.text = strGetText01;
					nIndex++;
				} 
				else 
				{
					strGetText02 += strText02 [nIndex2];
					text02.text = strGetText02;
					nIndex2++;
				}
			}
			//
			if (eTutorialState == TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK02)
			{
				if (nIndex + nIndex2 >= strText03.Length + strText04.Length)
				{
					StartFullScreenTime (2f);
					yield break;
				}
				if (nIndex < strText03.Length)
				{
					strGetText01 += strText03 [nIndex];
					text01.text = strGetText01;
					nIndex++;
				} 
				else
				{
					strGetText02 += strText04 [nIndex2];
					text02.text = strGetText02;
					nIndex2++;
				}

			}


			if(eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS)
			{
				//Debug.Log("affer Dragon");
				//End Condition
				if (nIndex + nIndex2 >= strText05.Length + strText06.Length)
				{
					eTutorialState = TutorialOrder.E_TUTORIAL_START_IMAGE03;
					button.GetComponent<Button> ().onClick.RemoveAllListeners ();
					button.GetComponent<Button> ().onClick.AddListener (StartDays10);

					button.SetActive (true);
					yield break;
				}

				if (nIndex < strText05.Length)
				{
					strGetText01 += strText05 [nIndex];
					text01.text = strGetText01;
					nIndex++;
				}

				else
				{
					strGetText02 += strText06 [nIndex2];
					text02.text = strGetText02;
					nIndex2++;
				}
			}
		}
	}



	public void StartGoShowOpen()
	{
		StartCoroutine( TutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEOUT));
		text01.text = "";
		text02.text = "";
		button.SetActive (false);
	}

	public void StartGuestShow()
	{
		StartCoroutine (GuestShow ());
	}

	public IEnumerator GuestShow()
	{
		DeActiveObj.SetActive (false);
		int nGuestCount = 0;

		while (nGuestCount != 5) 
		{
			SpawnManager.Instance.CreateCharacter ();
			yield return new WaitForSeconds (0.3f);
			nGuestCount++;
		}

		ShowTutorialImage (0);

		eTutorialState = TutorialOrder.E_TUTORIAL_START_IMAGE01;
	}


	public void StartFullScreenText02()
	{
		StartCoroutine (startText ());
	}
	public void StartFullScreenTime(float _time)
	{
		StartCoroutine(FullScreenText02StartTime(_time));
	}

	public IEnumerator FullScreenText02StartTime(float _time)
	{
		float fTime = _time;
		while (true) 
		{
			fTime -= Time.deltaTime;
			if (fTime <= 0)
			{
				if (eTutorialState == TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK02)
				{
					text01.text = "";
					text02.text = "";
					eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONSHOW;
					StartTutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEOUT);


				}
				yield break;
			}

			yield return null;
		}
		yield return null;
	}

	public void StartDays10()
	{
		StartTutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEOUT);
		ScoreManager.ScoreInstance.SetCurrentDays (11);
		button.SetActive (false);
		text01.text = "";
		text02.text = "";
	}


	public void ShowTutorialImage(int _index)
	{
		
		tutorial_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strTutorialImage_path[_index]);
		tutorial_Image.enabled = true;
		tutorialTouches [_index].gameObject.SetActive (true);
		tutorialTouches [_index].StartBlinkTouchImage ();

	}

	//true 0 -> 1 fade in
	//false 

	public void StartTutorialFullScreenTextPanelAlpha(TutorialOption _option)
	{
		StartCoroutine(TutorialFullScreenTextPanelAlpha(_option));
	}

	public IEnumerator TutorialFullScreenTextPanelAlpha(TutorialOption _option)
	{
		
		m_fTutorialFullScrreneAlpha = DeActiveObj_Image.color.a;
		while (true) 
		{

			//fade in
			if (_option == TutorialOption.E_TUTORIAL_OPTION_FADEIN) 
			{
				Debug.Log ("Fade In!!");
				m_fTutorialFullScrreneAlpha += Time.deltaTime * m_fAlphaValue;
				originColor = new Color (DeActiveObj_Image.color.r, DeActiveObj_Image.color.g, DeActiveObj_Image.color.b, m_fTutorialFullScrreneAlpha);
				DeActiveObj_Image.color = originColor;


				if (m_fTutorialFullScrreneAlpha >= 1f && eTutorialState == TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK02) 
				{
					//Text go
					StartFullScreenText();
					yield break;
				}


				if (m_fTutorialFullScrreneAlpha >= 1f && eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS) {

					SpawnManager.Instance.tutorialPanel.StartContent ();
					yield break;
				}
			} 
			//fade out
			else 
			{
				Debug.Log ("Fade Out!!");
				m_fTutorialFullScrreneAlpha -= Time.deltaTime * m_fAlphaValue;
				originColor = new Color (DeActiveObj_Image.color.r, DeActiveObj_Image.color.g, DeActiveObj_Image.color.b, m_fTutorialFullScrreneAlpha);
				DeActiveObj_Image.color = originColor;
				if (m_fTutorialFullScrreneAlpha <= 0f && eTutorialState == TutorialOrder.E_TUTORIAL_START_FULLSCREENTALK01)
				{
					eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX01;
					playerTalk.TalkBoxOnOff (true);
					playerTalk.StartPlayerTalk (0);
					yield break;
				}



				//보스등장
				if (m_fTutorialFullScrreneAlpha <= 0f && eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONSHOW)
				{
					playerTalk.TalkBoxOnOff (false);
					DeActiveObj.SetActive (false);

					SpawnManager.Instance.StartBossCreate(4);
					eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX01;
					yield break;
				}

				if (m_fTutorialFullScrreneAlpha <= 0f && eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE03)
				{
					DeActiveObj.SetActive (false);

					SpawnManager.Instance.tutorialPanel.tutorialImage_Obj.SetActive (true);
					SpawnManager.Instance.tutorialPanel.tutorial_Image.gameObject.SetActive (true);
					SpawnManager.Instance.tutorialPanel.tutorial_Image.enabled = true;
					ShowTutorialImage (2);
					yield break;

				}


			}
			yield return null;

		}


	}

}
