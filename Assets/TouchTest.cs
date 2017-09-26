using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TouchTest : MonoBehaviour ,IPointerDownHandler
{
	GameObject getInfoGameObject;
	public Animator animator;
	public int nTouchCount =3;

	private float value = 0.3f;
	RectTransform rectTrasform;

	public void OnPointerDown (PointerEventData eventData)
	{
		getInfoGameObject = eventData.pointerEnter;

		if (getInfoGameObject.gameObject == null)
			return;

		if (getInfoGameObject.gameObject.name == "SmallFire") 
		{
			nTouchCount--;
			if (nTouchCount == 2) {
				rectTrasform = gameObject.GetComponent<RectTransform> ();
				rectTrasform.sizeDelta = new Vector2 (rectTrasform.sizeDelta.x - 100f, rectTrasform.sizeDelta.y - 100f);
			}
			else if (nTouchCount == 1) {
				
			}
			else 
			{
				animator.SetBool ("isTouched1", false);
				animator.SetBool ("isTouched2", false);
				animator.SetBool ("isTouched1Fin", false);
				animator.SetBool ("isTouched2Fin", false);
				Destroy (gameObject);
			}
		}

	}

	public void StartFireShulink(int _index)
	{
		StartCoroutine (FireShulink (_index));
	}

	public  IEnumerator FireShulink(int _index)
	{
		yield return null;

		if (_index == 2) 
		{
			animator.SetBool ("isTouched1", true);

			yield return new WaitForSeconds (0.1f);

			animator.SetBool ("isTouched1Fin", true);

		}

		if (_index == 1) 
		{

			animator.SetBool ("isTouched2", true);

			yield return new WaitForSeconds (0.1f);

			animator.SetBool ("isTouched2Fin", true);
		}




	}
}
