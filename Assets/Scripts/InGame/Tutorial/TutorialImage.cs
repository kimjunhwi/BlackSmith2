using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TutorialImage : MonoBehaviour 
{
	public TutorialPanel tutorialPanel;

	public void StartBlinkTouchImage()
	{
		StartCoroutine (BlinkImage());
	}

	IEnumerator BlinkImage()
	{
		yield return new WaitForSeconds (1.0f);
		while (true) 
		{

			Debug.Log ("Blink Text!");

			if (tutorialPanel.tutorial_Obj.activeSelf == true)
			//	text.enabled = false;
			//else
			//	text.enabled = true;

			yield return new WaitForSeconds (0.3f);
		}
	}
}
