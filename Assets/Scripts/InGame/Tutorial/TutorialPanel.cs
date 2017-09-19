using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialOrder
{
	E_TUTORIAL_START_TEXT01 = 0,		//시작 텍스트
	E_TUTORIAL_START_IMAGE01,
	E_TUTORIAL_START_IMAGE02,
	E_TUTORIAL_WAIT_DRAGONSHOW,
	E_TUTORIAL_START_DRAGONSHOW,
	E_TUTORIAL_START_DAYS,
	E_TUTORIAL_START_IMAGE03,
	E_TUTORIAL_START_IMAGE04,
	E_TUTORIAL_START_SHOWCONSTRUCT,
	E_TUTORIAL_FINISH,
	E_TUTORIAL_NONE = 9999,
}


public class TutorialPanel : MonoBehaviour 
{
	public Text text01;
	public Text text02;

	public GameObject button;
	public GameObject DeActiveObj;

	public GameObject tutorialImage_Obj;
	public Image tutorial_Image;
	public TutorialTouch [] tutorialTouches;

	public TutorialOrder eTutorialState = TutorialOrder.E_TUTORIAL_NONE;

	private string strText01 = "크흠...";
	private string strText02 = "오늘도 영업을 시작해 볼까?";
	private string strText03 = "10일후...";

	private bool bTextEnd = false;

	private string[] strTutorialImage_path = new string[4];
	private int nTutorialImage_Index = 0;


	private bool bIsDragonRealShow = false;


	public void StartContent()
	{
		if (eTutorialState == TutorialOrder.E_TUTORIAL_NONE)
		{
			button.GetComponent<Button> ().onClick.AddListener (StartGuestShow);
			eTutorialState = TutorialOrder.E_TUTORIAL_START_TEXT01;
		}
		else
			eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;
			
		strTutorialImage_path[0] = 	"Tutorial/Tutorial01";
		strTutorialImage_path[1] = 	"Tutorial/Tutorial02";
		strTutorialImage_path[2] = 	"Tutorial/Tutorial03";
		strTutorialImage_path[3] = 	"Tutorial/Tutorial04";
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
		
			if (eTutorialState == TutorialOrder.E_TUTORIAL_START_TEXT01) 
			{
				Debug.Log("Before Dragon");
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
				} else 
				{
					strGetText02 += strText02 [nIndex2];
					text02.text = strGetText02;
					nIndex2++;
				}
			}
			if(eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS)
			{
				Debug.Log("affer Dragon");
				//End Condition
				if (nIndex >= strText03.Length)
				{
					button.GetComponent<Button> ().onClick.RemoveAllListeners ();
					button.GetComponent<Button> ().onClick.AddListener (StartDays10);

					button.SetActive (true);
					yield break;
				}

				if (nIndex < strText03.Length) {
					strGetText01 += strText03 [nIndex];
					text01.text = strGetText01;
					nIndex++;
				}
			}
		}
	}
	public void StartGuestShow()
	{
		text01.text = "";
		text02.text = "";
		button.SetActive (false);

		StartCoroutine (GuestShow ());
	}

	public void StartDays10()
	{
		DeActiveObj.SetActive (false);
		ScoreManager.ScoreInstance.SetCurrentDays (11);

		SpawnManager.Instance.tutorialPanel.tutorialImage_Obj.SetActive (true);
		SpawnManager.Instance.tutorialPanel.tutorial_Image.gameObject.SetActive (true);
		SpawnManager.Instance.tutorialPanel.tutorial_Image.enabled = true;
		ShowTutorialImage (2);

		if (eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS) {
			eTutorialState = TutorialOrder.E_TUTORIAL_START_IMAGE03;


		}
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

	public void ShowTutorialImage(int _index)
	{
		
		tutorial_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strTutorialImage_path[_index]);
		tutorial_Image.enabled = true;
		tutorialTouches [_index].gameObject.SetActive (true);
		tutorialTouches [_index].StartBlinkTouchImage ();

	}

}
