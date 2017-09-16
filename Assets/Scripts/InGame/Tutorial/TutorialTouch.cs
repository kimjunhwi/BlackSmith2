using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialTouch : MonoBehaviour ,IPointerDownHandler 
{
	public GameObject tutorialImage_Obj;
	public Image image;
	private bool bIsTouchActive = false;

	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoObj = eventData.pointerEnter;
		if (bIsTouchActive == true)
		{
			tutorialImage_Obj.SetActive (false);
			getInfoObj.SetActive (false);
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
