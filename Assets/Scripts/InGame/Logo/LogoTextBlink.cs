using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoTextBlink : MonoBehaviour 
{

	public Text text;
	public float fTime;

	// Update is called once per frame

	public void StartBlinkText()
	{
		StartCoroutine (BlinkText ());
	}

	IEnumerator BlinkText()
	{
		while (true) 
		{
			yield return new WaitForSeconds (0.5f);
			Debug.Log ("Blink Text!");

			if (text.isActiveAndEnabled == true)
				text.enabled = false;
			else
				text.enabled = true;



		}
	}

}
