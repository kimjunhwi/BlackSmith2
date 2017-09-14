using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHintTextBlink : MonoBehaviour {


	public Text text;
	public float fTime;


	public void StartBlinkText()
	{
		StartCoroutine (BlinkText ());
	}

	IEnumerator BlinkText()
	{
		while (true) 
		{
			yield return new WaitForSeconds (0.3f);
			Debug.Log ("Blink Text!");

			if (text.isActiveAndEnabled == true)
				text.enabled = false;
			else
				text.enabled = true;

		}
	}

}
