using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour {

	private SimpleObjectPool successedPool;

	public void Play(SimpleObjectPool _successedPool)
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
