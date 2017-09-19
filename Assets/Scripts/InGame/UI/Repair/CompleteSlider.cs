using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_SLIDERCOLOR
{
	E_SLIDERCOLOR_RED,
	E_SLIDERCOLOR_ORANGE,
	E_SLIDERCOLOR_YELLOW,
	E_SLIDERCOLOR_GREEN,
	E_SLIDERCOLOR_BLUE,
	E_SLIDERCOLOR_DARKBLUE,
	E_SLIDERCOLOR_PURPLE
}

public class CompleteSlider : MonoBehaviour 
{
	Slider completeSlider;
	public Color [] sliderColor = new Color[7];		//빨 주 노 초 파 남 보
	public Image fillImage;
	public Image fillBackgroundImage;
	public Text LeftCountText;
	public Image fillBackflashImage;

	private int nCurColorIndex =-1;
	public int nLeftLineCount = 0;
	private double fEachSliderValue = 0;
	public double dContinueValue = 0;

	private bool isPunpaSliderOn = false;
	void Awake()
	{
		completeSlider = GetComponent<Slider> ();
	}

	public void SetSliderGuest(float _dCurComplete)
	{
		//최대수치
		//받은 줄의 개수를 받아서 
		int nCurDay = GameManager.Instance.player.GetDay();
		double result = 0;

		if (nCurDay <= 100)
			result = (double) (500 * 0.5f);
		else
			result  =(double) Mathf.Pow (1024, Mathf.Floor((float)nCurDay * 0.01f)) * 500 * 0.5f;

		float nEachSliderTotalCount =  Mathf.Floor((float)_dCurComplete / (float)result);
		nLeftLineCount = (int)nEachSliderTotalCount;
		LeftCountText.text = string.Format ("X{0}", nLeftLineCount);

		fEachSliderValue = result;
		completeSlider.maxValue = (float)fEachSliderValue;


		fillImage.color = sliderColor [(int)E_SLIDERCOLOR.E_SLIDERCOLOR_RED];
		nCurColorIndex = (int)E_SLIDERCOLOR.E_SLIDERCOLOR_RED;
		//SetRandomColor except Red
		/*
		if (nLeftLineCount <= 1)
			fillImage.color = sliderColor [(int)E_SLIDERCOLOR.E_SLIDERCOLOR_RED];
		else 
		{
			int randomIndex = Random.Range (1, 6);
			nCurColorIndex = randomIndex;
			fillImage.color = sliderColor [randomIndex];
		}
*/
		isPunpaSliderOn = false;
		fillBackflashImage.enabled = false;
		dContinueValue = 0;

	}
	//Touch 할때 마다 체크 현재 완성도의 것보다 넘으면 빨간색을 제외한 나머지 색 중에 넣는다
	public void ChangeEachSliderColorAndInitValue(double _dCurDamage)
	{
		dContinueValue += _dCurDamage;
		if (_dCurDamage > fEachSliderValue && isPunpaSliderOn == false)
		{
			isPunpaSliderOn = true;
			StartCoroutine(StartDunPaSlider(nCurColorIndex));
		} 
		else
		{
			nLeftLineCount--;
			if (nCurColorIndex < 0)
				nCurColorIndex = 0;

			if (nCurColorIndex > 6)
				nCurColorIndex = 0;
			//뒷배경 색깔을 바꾼다
			fillBackgroundImage.color = sliderColor [nCurColorIndex];


			if (nLeftLineCount <= 1)
			{
				LeftCountText.text = string.Format ("X{0}", nLeftLineCount);
				fillImage.color = sliderColor [(int)E_SLIDERCOLOR.E_SLIDERCOLOR_RED];
				nCurColorIndex = (int)E_SLIDERCOLOR.E_SLIDERCOLOR_RED;
				return;
			}

			//다음 인덱스로 바꾼다
			int nNextColorIndex = nCurColorIndex + 1;
			if (nNextColorIndex > 6)
				nNextColorIndex = 0;

			LeftCountText.text = string.Format ("X{0}", nLeftLineCount);
			fillImage.color = sliderColor [nNextColorIndex];
			nCurColorIndex = nNextColorIndex;
			completeSlider.value = 0;

		}


		/*

		else 
		{
			//같은 색깔 중복 방지
			while (nPrevColorIndex == nCurColorIndex)
			{
				int randomIndex = Random.Range (1, 6);
				nCurColorIndex = randomIndex;
				fillImage.color = sliderColor [randomIndex];
			}
		}
		*/
	}

	public void StartBlinkFlashBack()
	{
		StartCoroutine (BlinkFlashBack ());
	}

	public IEnumerator BlinkFlashBack()
	{
		while (fillBackflashImage.isActiveAndEnabled == false) 
		{
			fillBackflashImage.enabled = true;
			yield return new WaitForSeconds (0.1f);
		}
		fillBackflashImage.enabled = false;
	}

	public IEnumerator StartDunPaSlider(int _nCurIndex)
	{
		int nPrevColorIndex = _nCurIndex;

		while (dContinueValue > 0) 
		{
			//Debug.Log ("DunpaSlider!!");
			if (_nCurIndex > 6)
				_nCurIndex = 0;
			fillImage.color = sliderColor [_nCurIndex++];
			dContinueValue -= fEachSliderValue;
			nLeftLineCount--;
			LeftCountText.text = string.Format ("X{0}", nLeftLineCount);


			completeSlider.value = Mathf.Lerp (completeSlider.value, (float)fEachSliderValue, 1.0f);

			if (completeSlider.value >= completeSlider.maxValue)
			{
				if (nPrevColorIndex > 6)
					nPrevColorIndex = 0;
				completeSlider.value = 0;
				fillBackgroundImage.color = sliderColor[ nPrevColorIndex];

			}
			nPrevColorIndex = _nCurIndex;


			yield return new WaitForSeconds (0.001f);
		}
		isPunpaSliderOn = false;
		nCurColorIndex = _nCurIndex;
	}

	public void SetSliderBoss(float _maxValue)
	{
		//최대수치
		float fEachSliderValue = _maxValue;

		completeSlider.maxValue = fEachSliderValue;
	}

	public float GetSliderMaxValue()
	{
		return completeSlider.maxValue;
	}
}
