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
			tutorialImage_Obj.SetActive (false);
			getInfoObj.SetActive (false);

			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE01) 
			{
				Debug.Log ("Cur State :" + tutorialPanel.eTutorialState);
				tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_IMAGE02;
			}

		

			//제작 하는 버튼 쪽으로 옮겨야 함
			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE04)
			{
				Debug.Log ("Cur State :" + tutorialPanel.eTutorialState);
				tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_FINISH;
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
