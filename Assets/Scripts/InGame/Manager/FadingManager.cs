using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingManager : MonoBehaviour {

	public Image FadeImage;

	//추후 쓸수도 있기에 빼둠
	public Slider SliderBar;
	public Text LoadText;

	private bool bIsFading = false;


	public IEnumerator FadeAction(float _fTime,bool _bIsFade)
	{
		bIsFading = true;

		float fTime = 0.0f;
		float fAlpha = 0.0f;

		while(fTime < _fTime)
		{
			fTime += Time.deltaTime;

			if (_bIsFade == false) 
				fAlpha = 1.0f - fTime / _fTime;
			
			else 
				fAlpha = 0.0f + fTime / _fTime;


			FadeImage.color = new Color (0, 0, 0, fAlpha);

			yield return null;
		}
		bIsFading = false;

	}
}
