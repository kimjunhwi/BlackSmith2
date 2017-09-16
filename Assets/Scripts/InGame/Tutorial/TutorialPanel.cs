using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialPanel : MonoBehaviour 
{
	public Text text01;
	public Text text02;

	public GameObject button;
	public GameObject DeActiveObj;

	public GameObject tutorial_Obj;
	public Image tutorial_Image;
	public TutorialTouch [] tutorialTouches;



	private string strText01 = "크흠...";
	private string strText02 = "오늘도 영업을 시작해 볼까?";

	private bool bTextEnd = false;

	private string[] strTutorialImage_path = new string[4];
	private int nTutorialImage_Index = 0;


	private bool bIsDragonRealShow = false;


	public void StartContent()
	{
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

		while (bTextEnd == false) 
		{
			yield return new WaitForSeconds (0.1f);
			//End Condition
			if (nIndex + nIndex2 >= nCount)
			{
				bTextEnd = true;
				button.SetActive (true);
				continue;
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
	}

	public void ShowTutorialImage(int _index)
	{
		tutorial_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strTutorialImage_path[_index]);
		tutorial_Obj.SetActive (true);
		tutorialTouches [_index].gameObject.SetActive (true);
		tutorialTouches [_index].StartBlinkTouchImage ();

	}

}
