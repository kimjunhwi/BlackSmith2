using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanabiTest : MonoBehaviour {

	const int nMaxLength = 7;

	const float fMinSize = 0.8f;
	const float fMaxSize = 1.0f;

	public GameObject UpgradeEffect;

	public Transform[] hanabiTransform;

	public void Play()
	{
		for (int nLengh = 0; nLengh < nMaxLength * 2; nLengh++) {

			int nRandom = Random.Range (0, nMaxLength);

			hanabiTransform [nRandom].SetAsFirstSibling ();
		}

		SpawnManager.Instance.scrolling.StartChangeBackground (eBackgroundMat.E_BackgroundMat_Night,2);

		StartCoroutine (Test());
	}


	IEnumerator Test()
	{
		int nLength = 0;

		while (nLength != nMaxLength) 
		{
			

			float fRandomSize = Random.Range (fMinSize, fMaxSize);

			transform.GetChild (nLength).transform.localScale = new Vector3 (fRandomSize, 1, fRandomSize);

			transform.GetChild (nLength).gameObject.SetActive (true);

			nLength++;

			yield return new WaitForSeconds (0.2f);
		}


		yield return new WaitForSeconds (1.4f);

		for(int nObjectLength = 0; nObjectLength < nMaxLength; nObjectLength++)
			transform.GetChild(nObjectLength).gameObject.SetActive(false);


		UpgradeEffect.SetActive (false);


		SpawnManager.Instance.scrolling.StartChangeBackground (eBackgroundMat.E_BackgroundMat_Main);

		yield break;
	}
}
