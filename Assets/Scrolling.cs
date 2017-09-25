using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBackgroundMat
{
	E_BackgroundMat_Main = 0,
	E_BackgroundMat_Boss,
	E_BackgroundMat_Night,
}

public class Scrolling : MonoBehaviour 
{
	private Vector3 startPos;

	//오버레이 할시의  뒷 배경과 앞배경
	public Renderer QuadBack;
	public Renderer QuadFront;
	//public Renderer QuadNight;

	public Material [] backGroundMaterials;

	public bool isQuadBack = false;
	public bool isQuadChangeFinsihed = false;
	private bool isFirstOn = false;

	private float fQuadBackAlpha = 0f;
	private float fQuadFrontAlpha = 0f;

	private float fChangedValue = 0.5f;

	private float fAlphaMax = 0f;

	// Use this for initialization
	void Start () 
	{
		startPos = transform.position;
		fAlphaMax = backGroundMaterials [0].color.a;
		Debug.Log ("Max Alpha = " + fAlphaMax);

		isQuadBack = false;
		isQuadChangeFinsihed = true;
		isFirstOn = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 offset = new Vector2 (Time.time * 0.3f, 0);

		if (isQuadChangeFinsihed == true) 
		{
			if (gameObject.transform.GetChild (2).name == "Quad_Front")
				QuadFront.material.mainTextureOffset = offset;
			else
				QuadBack.material.mainTextureOffset = offset;
		}
		else 
		{
			//overLay
			QuadFront.material.mainTextureOffset = offset;
			QuadBack.material.mainTextureOffset = offset;
		}

	}

	//배경 체인지시 바꿀배경을 뒤(BackQuad)에다 두고 현재 배경은 앞(QuadFront)에다 둔다



	public void StartChangeBackground(eBackgroundMat _changeBackground)
	{
		StartCoroutine (ChangeBackground (_changeBackground));
	}

	public IEnumerator ChangeBackground(eBackgroundMat _changeBackground)
	{
		//처음에는  무조건 false 다음 부터는 On/Off
		if (isFirstOn == false) {

			isFirstOn = true;
			isQuadBack = false;
		}
		else
		{
			if (isQuadBack == false)
				isQuadBack = true;
			else
				isQuadBack = false;

		}
		Color QuadBackColor;
		Color QuadFrontColor;

		Material QuadBackMaterial;
		Material QuadFrontMaterial;

		isQuadChangeFinsihed = false;	//아직 바뀌지 않았다.

		yield return null;

		if (isQuadBack == false)
		{
			
			QuadBackMaterial = backGroundMaterials [(int)_changeBackground];
			QuadBackMaterial.color = new Color (QuadBackMaterial.color.r, QuadBackMaterial.color.g, QuadBackMaterial.color.b, fAlphaMax);
			QuadBack.material = QuadBackMaterial;
			QuadFrontMaterial = QuadFront.material;
		}
		else
		{
			QuadBackMaterial = backGroundMaterials [(int)_changeBackground];
			QuadBackMaterial.color = new Color (QuadBackMaterial.color.r, QuadBackMaterial.color.g, QuadBackMaterial.color.b, fAlphaMax);
			QuadFront.material = QuadBackMaterial;
			QuadFrontMaterial = QuadBack.material;
		}

		while (true) 
		{
			

			if (isQuadBack == false) 
			{
				fQuadBackAlpha = QuadBackMaterial.color.a;
				fQuadFrontAlpha = QuadFrontMaterial.color.a;

				fQuadBackAlpha += Time.deltaTime * fChangedValue;
				fQuadFrontAlpha -= Time.deltaTime * fChangedValue;

				QuadBackColor = new Color (QuadBack.materials [0].color.r, QuadBack.materials [0].color.g, QuadBack.materials [0].color.b, 
					fQuadBackAlpha);


				QuadFrontColor = new Color (QuadFront.materials [0].color.r, QuadFront.materials [0].color.g, QuadFront.materials [0].color.b,
					fQuadFrontAlpha);
			}
			else 
			{
				fQuadBackAlpha = QuadBackMaterial.color.a;
				fQuadFrontAlpha = QuadFrontMaterial.color.a;

				fQuadBackAlpha += Time.deltaTime * fChangedValue;
				fQuadFrontAlpha -= Time.deltaTime * fChangedValue;

				QuadFrontColor = new Color (QuadFront.materials [0].color.r, QuadFront.materials [0].color.g, QuadFront.materials [0].color.b, 
					fQuadBackAlpha);


				QuadBackColor = new Color (QuadBack.materials [0].color.r, QuadBack.materials [0].color.g, QuadBack.materials [0].color.b,
					fQuadFrontAlpha);
			}



			QuadBack.materials [0].color = QuadBackColor;
			QuadFront.materials [0].color = QuadFrontColor;



			if (fQuadFrontAlpha <= 0) 
			{
				Debug.Log ("배경바꾸기 완료");
				//앞뒤 바뀐다
			

				if (isQuadBack == false) 
				{
					QuadBack.gameObject.transform.SetAsLastSibling ();
					isQuadChangeFinsihed = true;

				}
				else
				{
					QuadFront.gameObject.transform.SetAsLastSibling ();
					isQuadChangeFinsihed = true;
				}

				yield break;
			}

			yield return null;

		}

	}

	public void SetMidderQuad(Material _material)
	{
		//isQuadMiddle = true;
		//QuadMiddle.material = _material;
		//QuadMiddle.enabled = true;
		//QuadMiddle.gameObject.transform.SetAsLastSibling ();
	}

}
