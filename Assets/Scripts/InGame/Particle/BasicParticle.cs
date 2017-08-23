using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicParticle : MonoBehaviour {

	public void Play(float _fTime = 2.0f)
	{
		StartCoroutine(StartDisappearAfter(2f));
	}

	IEnumerator StartDisappearAfter(float time)
	{
		yield return new WaitForSeconds(time);

		if (gameObject.activeSelf == false)
			yield break;
		
		gameObject.SetActive (false);
	}
}
