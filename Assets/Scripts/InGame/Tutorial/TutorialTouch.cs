using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialTouch : MonoBehaviour ,IPointerDownHandler 
{
	public GameObject tutorialImage_Obj;
	public Image image;
	public int nTutorialIndex = 0;
	private bool bIsTouchActive = false;
	public TutorialPanel tutorialPanel;

	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoObj = eventData.pointerEnter;
		if (bIsTouchActive == true)
		{
			//터치 영어 이미지
			tutorialImage_Obj.SetActive (false);
			getInfoObj.SetActive (false);

			tutorialPanel.tutorial_Image.enabled = false;
			tutorialPanel.tutorialImage_Obj.SetActive (false);
		

			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE01) 
			{
				Debug.Log ("Cur State :" + tutorialPanel.eTutorialState);
				tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_IMAGE02;
			}

			//3번째 이미지 터치시
			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE03) 
			{
				Debug.Log ("Cur State :" + tutorialPanel.eTutorialState);
				SpawnManager.Instance.uiManager.ActiveMenu (0);

			}



		

			//제작 하는 버튼 쪽으로 옮겨야 함
			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_SHOWCONSTRUCT)
			{
				Debug.Log ("Cur State :" + tutorialPanel.eTutorialState);
				SpawnManager.Instance.makingUI.MakeWeapon ();
				//tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_FINISH;
				//tutorialPanel.TutorialBlock_Obj.SetActive (false);
			}

				
			
		}

	}


	public void StartBlinkTouchImage()
	{
		bIsTouchActive = false;
		StartCoroutine (BlinkImage());
	}

	IEnumerator BlinkImage()
	{
		yield return new WaitForSeconds (1.0f);

		bIsTouchActive = true;
		while (true) 
		{
			if (tutorialImage_Obj.activeSelf == true) 
			{
				if (image.isActiveAndEnabled == true)
					image.enabled = false;
				else
					image.enabled = true;
			}
			yield return new WaitForSeconds (0.3f);
		}
	}


}
