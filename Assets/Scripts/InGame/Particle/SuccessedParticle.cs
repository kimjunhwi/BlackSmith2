using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessedParticle : MonoBehaviour {
	
	private SuccessedPool successedPool;

	public void Play(SuccessedPool _successedPool)
	{
		successedPool = _successedPool;

		StartCoroutine(StartDisappearAfter(2f));
	}

	IEnumerator StartDisappearAfter(float time)
	{
		yield return new WaitForSeconds(time);

		successedPool.ReturnObject(gameObject);
	}
}
