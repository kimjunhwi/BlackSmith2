using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RepairObject : MonoBehaviour
{
	public Slider ComplateSlider;
    public Slider TemperatureSlider;
    public Slider WaterSlider;

    public Text ComplateText;

    public Transform CanvasTransform;

    //진행중인 오브젝트
	public GameObject AfootObject;
	GameObject WeaponObject;

	float fWaterSlideTime = 0.0f;
	float fComplateSlideTime = 0.0f;
	float fTemperatureSlideTime = 0.0f;

	private double dCurrentComplate = 0;				//현재완성도
	private string strMaxComplate = "";				//맥스 완성도 
    private float fWeaponDownTemperature = 0;		//무기 수리시 올라가는 온도
    private float fMaxTemperature = 100;					//최대 온도
    private float fCurrentTemperature= 0;			//현재 온도
    private float fDownTemperature = 0;				//떨어지는 온도3

    public float fMaxWater;         //최고치
    public float fCurrentWater;     //남은 량
    public float fUseWater;         //사용되는 가능 용량
    public float fPlusWater;        //충전되는 량


    private float fMinusTemperature;
    private float fMinusWater;

	public Image WeaponSprite;
    public Image WeaponAlphaSpirte;

	public Sprite main_Touch_Sprite;

	private CGameWeaponInfo weaponData;

	Player player;

	//Boss
	public BossCharacter bossCharacter;		//보스 캐릭터 받는 것
	BossIce bossIce;
	private double dBossMaxComplete;		//보스 캐릭터 최대 완성도
	GameObject waterObject;				//물 오브젝트
	public GameObject waterPaching;
	GameObject bossWeaponObject;		//보스 무기 버튼
	GameObject bossWaterObject;			//보스 물 버튼 

	Image BossWeaponAlphaSprite;
	Image BossWeaponSprite;
	public Animator bossWeaponAnimator;
	public RuntimeAnimatorController[] bossWeaponController;

	//BossSasinText
	//tmp value
	private GameObject textObj;
	BossMissText bossMissText;
	public RectTransform textRectTrasnform;
	//textPool
	public SimpleObjectPool textObjectPool;
	//RandomPosition
	private float fRandomXPos;
	private float fRandomYPos;
	private float fXPos;
	private float fYPos;

	int nChancePercent = 50;									//미스 확률
	double dCalcValue = 0.0f; 


	//BossMusic
	private BossMusic bossMusic;
	private Vector3 randomDir;									//루시오 보스무기가 갈 랜덤 방향
	private Vector3 getReflectDir;
	private float fRandomX;										//랜덤 방향 X
	private float fRandomY;										//랜덤 방향 Y
	private float canvasWidth = 655f;							//체크 할 
	private float canvasHeight = 1100f;
	private float fMoveSpeed;

	private RectTransform bossWeaponRectTransform;
	public RectTransform bossNoteRectTransform;
	private NoteObject noteObj;
	private Note2Object note2Obj;
	private Note3Object note3Obj;

	//private Transform noteGameObject;							//물 사용시 없어질 노트 obj
	private Vector3 bossWeaponObjOriginPosition;				//원래 수리 패널에 있을때의 무기 위치
	private Vector2 bossWeaponObjOriginSize;					//원래 수리 패널에 터치 인식 범위의 크기
	private Vector2 bossWeaponSize;								//무기 이미지 만큼의 크기

    private Vector3 mouseTouchPosition;
	public bool isMoveWeapon = false;
	private float translateValue;

	//BossFire
	public float fSmallFireMinusWater = 0f;
	public float fSmallFirePlusTemperatrue = 0f;
	public float fOriWaterPlus = 0f;

	//WaterFx
	public GameObject waterFxObj;
	public Animator bossWaterCat_animator;						//보스무기일때의 고양이
	public Animator weaponWaterCat_animator;					//그냥무기일때의 고양이
	public Animator CatWater_animator;							//물 이펙트

    
	public bool isTouchWaterAvailable;							//물이 사용가능한가??
	public bool isTouchWater;									//물을 터치 했는가?
	private bool isBossIcePassive01Active = false;				//얼음보스 패시브가 발동 되었는가?
	private bool isBossMusicPassive01Active = false;			//음악보스 패시브01가 발동 되었는가?
	private bool isBossMusicPassive02Active = false;			//음악보스 패시브02가 발동 되었는가?

	public RectTransform waterBottle;							//물 비커의 크기
	public RectTransform waterAvailableArrow;					//물 사용가능 최저 화살표 표시

	public PlayerController m_PlayerAnimationController; 


    // 07.20 피버
    private bool m_bIsFever = false;
    private const float m_fFeverTime = 10.0f;

    private const float m_fNormalCretaeTime = 5f;
    private const float m_fFeverCreateTime = 5f;

    private const float m_fNormalSpeed = 1.2f;
    private const float m_fFeverSpeed = 3.6f;

	private float m_fMinusTemperature = 0.0f;
	private const float m_fMinusDefault = 0.1f;

	//MissText
	public GameObject textParent;
	private const int nEnableTime = 1;
	public SimpleObjectPool damageTextPool;

	//WeaponBoom
	public GameObject weaponBoomTransform;
	//WeaponShake
	public BossWeaponShake bossWeaponShake;
	public WeaponShakeIt normalWeaponShake;

	public GameObject SucceessedObject;
	public Animator SuccessedAnim;

	public PlayerSpecificInfo PlayerInfo;

	//Tutorial
	private int nTouchCount = 0;


	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee" }; 

	void Start()
	{
		isTouchWater = false;

		bossWeaponObjOriginSize = new Vector2 (590f, 470f);
		bossWeaponSize = new Vector2 (380f, 270f);
		fXPos = textRectTrasnform.position.x;
		fYPos = textRectTrasnform.position.y;

		fRandomXPos = 0;
		fRandomYPos = 0;
		WaterSlider.minValue = 0;
		ComplateSlider.minValue = 0;
		TemperatureSlider.minValue = 0;

		WaterSlider.maxValue = 0;
		ComplateSlider.maxValue = 0;

		//Add GameObject(button)
		WeaponObject = transform.Find("WeaponButton").gameObject;
		waterObject = transform.Find ("WaterButton").gameObject;

		bossWeaponObject = transform.Find ("BossWeaponButton").gameObject;
		bossWaterObject = transform.Find ("BossWaterButton").gameObject;

        WeaponAlphaSpirte = WeaponObject.transform.GetChild(0).GetComponent<Image>();
        WeaponSprite = WeaponObject.transform.GetChild(1).GetComponent<Image>();


		BossWeaponAlphaSprite = bossWeaponObject.transform.GetChild (0).GetComponent<Image> ();
		BossWeaponSprite = bossWeaponObject.transform.GetChild (1).GetComponent<Image> ();

		//BossWeaponAnimator
		bossWeaponAnimator = BossWeaponSprite.gameObject.GetComponent<Animator>();

		this.StartCoroutine (this.StartWaterFx ());

		this.StartCoroutine (this.ChangeSlider ());

		this.StartCoroutine (this.OneSecondPlay ());
	

		player = GameManager.Instance.player;

		player.PlayerStatsSetting ();

		fCurrentWater = 0f;
		//fUseWater  = 10.0f;

		fPlusWater = player.GetWaterPlus ();
		fMaxWater = GameManager.Instance.GetPlayer().GetBasicMaxWater();

		TemperatureSlider.maxValue = fMaxTemperature;
		TemperatureSlider.value = 0;

		WaterSlider.maxValue = fMaxWater;
		WaterSlider.value = 0;

		bossWeaponObjOriginPosition = bossWeaponObject.transform.position;


		bossWeaponRectTransform = bossWeaponObject.GetComponent<RectTransform> ();

		bossWeaponObject.SetActive (false);
		bossWaterObject.SetActive (false);
		WeaponObject.SetActive (true);
		waterObject.SetActive (true);
		waterPaching.SetActive (false);
	}

	public IEnumerator StartWaterFx()
	{
		while (true)
		{
			if (bossCharacter != null) 
			{
				//BossWepaonWater
				if (isTouchWater == true)
				{
					waterFxObj.transform.SetAsLastSibling ();
					Debug.Log("BossWeaponWaterFx !!");
					bossWaterCat_animator.SetBool ("isTouchWater", true);
					CatWater_animator.SetBool ("isTouchWater", true);

					if (CatWater_animator.GetCurrentAnimatorStateInfo (0).IsName ("Water_Fx_spread"))
					{
						yield return new WaitForSeconds (0.5f);
						//Debug.Log ("BossWeaponWaterFinish !!");
						waterFxObj.transform.SetAsFirstSibling ();
						bossWaterCat_animator.SetBool ("isTouchWater", false);
						CatWater_animator.SetBool ("isTouchWater", false);
						isTouchWater = false;
						bossWaterCat_animator.Play ("WaterCat_Idle");
						CatWater_animator.Play ("Water_Fx_Idle");
						
					}
				}
			} 
			else 
			{
				//normalWeaponWater
				if (isTouchWater == true)
				{
					waterFxObj.transform.SetAsLastSibling ();
					//Debug.Log("NormalWeaponWater !!");
					weaponWaterCat_animator.SetBool ("isTouchWater", true);		//CatAnimation go
					CatWater_animator.SetBool ("isTouchWater", true);			//WaterAnmation go

					if (CatWater_animator.GetCurrentAnimatorStateInfo (0).IsName ("Water_Fx_spread"))
					{
						yield return new WaitForSeconds (0.5f);
						Debug.Log ("WeaponWaterFinish !!");
						waterFxObj.transform.SetAsFirstSibling ();
						weaponWaterCat_animator.SetBool ("isTouchWater", false);
						CatWater_animator.SetBool ("isTouchWater", false);
						isTouchWater = false;
						weaponWaterCat_animator.Play ("WaterCat_Idle");
						CatWater_animator.Play ("Water_Fx_Idle");
					} 

				}
			}
			yield return null;

		}
			
	}

	/*
	public void StartBossMusiceWeaponMove()
	{
		StartCoroutine (BossMusicWeaponMove ());
	}

	public IEnumerator BossMusicWeaponMove()
	{
		isMoveWeapon = true;
		bossWeaponRectTransform.sizeDelta = bossWeaponSize;
		//무기 초기 스피드
		fMoveSpeed = 600f;		
		
		fRandomX = Random.Range ( -0.2f, 1.0f);
		fRandomY = Random.Range ( -0.2f, 1.0f);

		randomDir = new Vector3 (fRandomX, fRandomY, 0);
		randomDir = randomDir.normalized;
		while (true)
		{
			yield return null;
			//4면 충돌 확인
			//방향은 달라도 속도는 일정해야한다
			//Right Collision
			if (bossWeaponRectTransform.anchoredPosition.x >= ((canvasWidth) - bossWeaponRectTransform.sizeDelta.x) - 120f)
			{
				randomDir = new Vector3 (-1.0f, Random.Range ( -0.5f, 0.5f), 0f);
			} 
			//left
			else if (bossWeaponRectTransform.anchoredPosition.x <= -((canvasWidth - bossWeaponRectTransform.sizeDelta.x) - 75f)) 
			{
				randomDir = new Vector3 (1.0f, Random.Range ( -0.5f, 0.5f), 0f);
			} 
			//top
			else if(bossWeaponRectTransform.anchoredPosition.y >= (canvasHeight) - (bossWeaponRectTransform.sizeDelta.y) - 50f)
			{
				randomDir = new Vector3 (Random.Range ( -0.5f, 0.5f), -1.0f, 0f);
			}
			//bottom
			else if (bossWeaponRectTransform.anchoredPosition.y <= -((canvasHeight) - ((bossWeaponRectTransform.sizeDelta.y * 3f) + 190f)))
			{
				randomDir = new Vector3 (Random.Range ( -0.5f, 0.5f), 1.0f, 0f);
			}
			randomDir = randomDir.normalized;

			#if UNITY_EDITOR
			bossWeaponObject.transform.Translate (fMoveSpeed * randomDir * Time.deltaTime);

			#elif UNITY_ANDROID
			bossWeaponObject.transform.Translate (fMoveSpeed * randomDir * Time.deltaTime * 2.5f);

			#endif

			if (isMoveWeapon == false)
				yield break;

		}

	}
*/
	IEnumerator ChangeSlider()
	{
		while(true)
		{
			yield return null;

			if (TemperatureSlider.value != fCurrentTemperature) 
			{
				fTemperatureSlideTime += Time.deltaTime;

				TemperatureSlider.value = Mathf.Lerp (TemperatureSlider.value, fCurrentTemperature, fTemperatureSlideTime);

				if(TemperatureSlider.value >= fMaxTemperature)
				{
					fCurrentTemperature = 0.0f;
					TemperatureSlider.value = fCurrentTemperature;

					//얼음 보스시 온도가 터지면 모든 아르바이트 빙결 해제 
					if(bossIce != null)
						bossIce.DefreezeAllArbait ();


					if (dBossMaxComplete == 0.0f)
						dCurrentComplate = (dCurrentComplate) - weaponData.dMaxComplate * 0.3f;

					else 
					{
						//SpawnManager.Instance.ComplateCharacter(AfootObject, weaponData.fMaxComplate);
						//무기 실패취급으로 리턴
						if(bossCharacter != null)
							dCurrentComplate -= (dBossMaxComplete * 0.3f);  
						else
							dCurrentComplate = (dCurrentComplate) - weaponData.dMaxComplate * 0.3f;

						if (dCurrentComplate <= 0) {
							SpawnManager.Instance.bIsBossCreate = false;
							continue;
						}
					}

                    if (dCurrentComplate > 0)
                    {
                        GameObject obj = TemperatureBoomPool.Instance.GetObject();

						obj.transform.SetParent(CanvasTransform,false);

                        obj.GetComponent<TemperatureBoomParticle>().Play();

                        SpawnManager.Instance.CheckComplateWeapon(AfootObject, dCurrentComplate, fCurrentTemperature);
                    }

                    else
                    {
						if (bossCharacter == null)
						{
							//터지는 파티클
							ShowBreakWeapon ();

						}
                    }
				}
			}

			if (TemperatureSlider.value != 0) 
			{
				m_fMinusTemperature += Time.deltaTime;

				if (m_fMinusTemperature >= m_fMinusDefault) 
				{
					m_fMinusTemperature = 0.0f;

					fCurrentTemperature -= fMaxTemperature * 0.02f;

					if (fCurrentTemperature < 0)
						fCurrentTemperature = 0;
				}
			}

			if (fMaxWater > fCurrentWater) 
			{
				fWaterSlideTime += Time.deltaTime;
				WaterSlider.value = Mathf.Lerp (WaterSlider.value, fCurrentWater, fWaterSlideTime);

				if (fCurrentWater >= 1000f) 
				{
					isTouchWaterAvailable = true;
					waterPaching.SetActive (true);
				} 
				else 
				{
					isTouchWaterAvailable = false;
					waterPaching.SetActive (false);
				}
			} 
			else
				fCurrentWater = fMaxWater;

			if (ComplateSlider.value != dCurrentComplate) 
			{
				fComplateSlideTime += Time.deltaTime;

				ComplateSlider.value = Mathf.Lerp (ComplateSlider.value, (float)dCurrentComplate, fComplateSlideTime);

				ComplateText.text = string.Format("{0} / {1}", ChangeValue(ComplateSlider.value), ChangeValue(ComplateSlider.maxValue));
			}
		}
	}

	public void ShowBreakWeapon()
	{
		//터지는 파티클
		SoundManager.instance.PlaySound (eSoundArray.ES_WeaponExplosionSound);
		GameObject obj = BreakBoomPool.Instance.GetObject ();
		obj.transform.SetParent (weaponBoomTransform.transform, false);

		obj.GetComponent<BreakBoomParticle> ().Play ();

		GameObject BoomObject = TemperatureBoomPool.Instance.GetObject ();

		BoomObject.transform.SetParent (weaponBoomTransform.transform, false);

		BoomObject.GetComponent<TemperatureBoomParticle> ().Play ();

		SpawnManager.Instance.ComplateCharacter (AfootObject, dCurrentComplate);
	}

	IEnumerator OneSecondPlay()
	{
		while (true) 
		{

			yield return new WaitForSeconds (1.0f);

			fPlusWater = player.GetWaterPlus ();

			fWaterSlideTime = 0.0f;
			fTemperatureSlideTime = 0.0f;

			fCurrentWater += (fPlusWater  - (fPlusWater * fSmallFireMinusWater));

			if (fCurrentWater / player.GetBasicMaxWaterPlus () > 0.7)
				SpawnManager.Instance.WaterCheck ();
			
			else
				SpawnManager.Instance.UnWaterCheck ();

			//Debug.Log ("CurPlusWater / " +GameManager.Instance.player.GetWaterPlus() + "WaterMax " + fMaxWater );
			if (fCurrentTemperature > 0) 
			{
				fDownTemperature = (fMaxTemperature - fCurrentTemperature) * 0.05f;

				fCurrentTemperature -= fDownTemperature;

				if (fCurrentTemperature < 0)
					fCurrentTemperature = 0;
			}

			//WaterAvailable Arrow 
			int waterLevel = player.GetMaxWaterLevel();
			waterAvailableArrow.anchoredPosition = new Vector2(waterAvailableArrow.anchoredPosition.x, waterBottle.sizeDelta.y /( waterLevel + 1));
		
		}
	}


    IEnumerator StartFever(float _fTime)
    {
        yield return new WaitForSeconds(_fTime);

        m_bIsFever = false;
		SucceessedObject.SetActive (false);

        //SpawnManager.Instance.SettingFever(m_fNormalCretaeTime, m_fNormalSpeed);
    }

	public void GetWeapon(GameObject obj, CGameWeaponInfo data, double _dComplate, float _fTemperator)
    {
		bossWeaponObject.SetActive (false);
		bossWaterObject.SetActive (false);
		WeaponObject.SetActive (true);
		waterObject.SetActive (true);

        if (AfootObject == obj)
            return;

        if(weaponData == null)
        {
            AfootObject = obj;
            
            weaponData = data;
        }
        else
        {
			SpawnManager.Instance.ReturnInsertData(AfootObject,false,false, dCurrentComplate, fCurrentTemperature);

            AfootObject = obj;

            weaponData = data;
        }

		PlayerInfo.SetGuestWeapon (weaponData);

		dCurrentComplate = _dComplate;

		ComplateSlider.maxValue = (float)weaponData.dMaxComplate;
		ComplateSlider.value = (float)dCurrentComplate;

		strMaxComplate = ChangeValue (weaponData.dMaxComplate);

        fCurrentTemperature = _fTemperator;
		WeaponSprite.sprite = weaponData.WeaponSprite;

		if(_dComplate != 0)
			ComplateText.text = string.Format("{0} / {1}", ChangeValue(_dComplate), strMaxComplate);

		else
			ComplateText.text = string.Format("{0} / {1}", 0, strMaxComplate);
    }

	public void GetBossWeapon(Sprite _sprite, double _dMaxBossComplete ,double _dComplate,
		float _fTemperator , BossCharacter _bossData)
	{
		bossWeaponObject.SetActive (true);
		bossWaterObject.SetActive (true);

		WeaponObject.SetActive (false);
		waterObject.SetActive (false);

		if (_bossData.nIndex == 0) {
			bossCharacter = _bossData;
			bossIce = (BossIce)bossCharacter;

			//input Image
			BossWeaponSprite.sprite = _sprite;

			//Change AnimController 
			bossWeaponAnimator.runtimeAnimatorController = bossWeaponController [0];

		} else if (_bossData.nIndex == 1)
		{
			bossCharacter = _bossData;

			//input Image
			BossWeaponSprite.sprite = _sprite;

			//Change AnimController 
			bossWeaponAnimator.runtimeAnimatorController = bossWeaponController [1];
		}
		else if (_bossData.nIndex == 2) 
		{
			bossCharacter = _bossData;

			//input Image
			BossWeaponSprite.sprite = _sprite;
			fOriWaterPlus = GameManager.Instance.GetPlayer ().changeStats.fWaterPlus;
			//Change AnimController 
			bossWeaponAnimator.runtimeAnimatorController = bossWeaponController [2];
		}
		else if (_bossData.nIndex == 3)
		{
			bossMusic = (BossMusic)_bossData;
			bossCharacter = _bossData;

			//input Image
			BossWeaponSprite.sprite = _sprite;
		
			//Change AnimController 
			bossWeaponAnimator.runtimeAnimatorController = bossWeaponController [3];
		}
		else
			return;

		dCurrentComplate = _dComplate;
		dBossMaxComplete = _dMaxBossComplete ;

		strMaxComplate = ChangeValue (dBossMaxComplete);


		ComplateSlider.maxValue = (float)_dMaxBossComplete;
		ComplateSlider.value = (float)dCurrentComplate;

		fCurrentTemperature = _fTemperator;
		ComplateText.text = string.Format("{0} / {1}", _dComplate, strMaxComplate);
	}

	public void ShowDamage(double _dDamage,Vector3 _position)
	{
		string strDamage = ChangeValue (_dDamage);

		//int nRandomX = Random.Range (0, 20);
		//int nRandomY = Random.Range (0, 10);
		//fRandomXPos = Random.Range (textParent.transform.position.x - nRandomX,textParent.transform.position.x + nRandomX);
		//fRandomYPos = Random.Range (textParent.transform.position.y - nRandomY,textParent.transform.position.y + nRandomY);

		GameObject damageText = damageTextPool.GetObject ();

		damageText.transform.SetParent (textParent.transform,false);
		damageText.transform.localScale = Vector3.one;
		damageText.transform.position = new Vector3(_position.x,_position.y + 20,_position.z);
		damageText.name = "Damage";

		DamageTextPool damagePool = damageText.GetComponent<DamageTextPool> ();
		damagePool.Damage (strDamage);
		damagePool.textObjPool = damageTextPool;
		damagePool.leftSecond = nEnableTime;
	}


    //무기터치
	public void TouchWeapon(Vector3 _position)
	{
        if (weaponData == null)
			return;

		//Tutorial
		if (SpawnManager.Instance.bIsTutorial == true)
		{
			if (SpawnManager.Instance.bIsTutorial_ShowTempAndWater == false) {
				nTouchCount++;

				if (nTouchCount >= 10) {
					WaterSlider.value = WaterSlider.maxValue;
					SpawnManager.Instance.bIsTutorial_ShowTempAndWater = true;
			
					SpawnManager.Instance.tutorialPanel.ShowTutorialImage (1);
				}

			}
		}

        Debug.Log("Touch");
		normalWeaponShake.Shake (12.0f, 0.12f);
        fComplateSlideTime = 0.0f;

        //피버일경우 크리 데미지로 완성도를 증가시킴
        if (m_bIsFever)
        {
			m_PlayerAnimationController.UserBigSuccessedRepair ();

			dCalcValue = (player.GetRepairPower () +(player.GetRepairPower () * weaponData.dMinusRepair * 0.01));

			dCalcValue *= player.GetCriticalDamage() * 0.01;

			ShowDamage (dCalcValue,_position);

			dCurrentComplate += dCalcValue;

            GameObject obj = CriticalTouchPool.Instance.GetObject();

			obj.transform.SetParent(CanvasTransform,false);

			obj.transform.position = _position;

            obj.GetComponent<CriticalTouchParticle>().Play();

            //완성이 됐는지 확인 밑 오브젝트에 진행사항 전달
			if (SpawnManager.Instance.CheckComplateWeapon (AfootObject, dCurrentComplate, fCurrentTemperature)) 
			{
				ComplateSlider.value = 0;
				TemperatureSlider.value = 0;

				ComplateText.text = string.Format ("{0:#### / {1}", (int)ComplateSlider.value, 0);
				return;
			}
            return;
        }

		if (Random.Range (0, 100) >= Mathf.Round (player.GetAccuracyRate () - player.GetAccuracyRate () * weaponData.fMinusAccuracy * 0.01f)) {

			GameObject damageText = damageTextPool.GetObject ();

			damageText.transform.SetParent (textParent.transform,false);
			damageText.transform.localScale = Vector3.one;
			damageText.transform.position = _position;
			damageText.name = "Damage";

			DamageTextPool damagePool = damageText.GetComponent<DamageTextPool> ();
			damagePool.Damage ("Miss");
			damagePool.textObjPool = damageTextPool;
			damagePool.leftSecond = nEnableTime;

			return;
		}


        //크리티컬 확률 
		if (Random.Range(0, 100) <= Mathf.Round(player.GetCriticalChance() + (player.GetCriticalChance() * weaponData.fMinusCriticalChance *0.01f)))
        {

            GameObject obj = CriticalTouchPool.Instance.GetObject();

			obj.transform.SetParent(CanvasTransform,false);

			obj.transform.position = _position;

            obj.GetComponent<CriticalTouchParticle>().Play();

			m_PlayerAnimationController.UserCriticalRepair();

            SpawnManager.Instance.PlayerCritical();

			dCalcValue = (player.GetRepairPower () +(player.GetRepairPower () * weaponData.dMinusRepair * 0.01));

			dCalcValue *= 1.5;

			ShowDamage (dCalcValue,_position);

			dCurrentComplate += dCalcValue;
        }
        else
        {
            Debug.Log("Nor!!!!");

            GameObject obj = NormalTouchPool.Instance.GetObject();

			obj.transform.SetParent(CanvasTransform, false);

			obj.transform.position = _position;

            obj.GetComponent<NormalTouchParticle>().Play();

            m_PlayerAnimationController.UserNormalRepair();

			dCalcValue = (player.GetRepairPower () +(player.GetRepairPower () * weaponData.dMinusRepair * 0.01f));

			ShowDamage (dCalcValue,_position);

			dCurrentComplate += dCalcValue;
        }
        //공식에 따른 온도 증가

        //fCurrentTemperature += ((fWeaponDownDamage * fMaxTemperature) / weaponData.fMaxComplate) * (1 + (fCurrentTemperature / fMaxTemperature) * 1.5f);

		fCurrentTemperature += fMaxTemperature * 0.06f;


        //완성이 됐는지 확인 밑 오브젝트에 진행사항 전달
		if (SpawnManager.Instance.CheckComplateWeapon (AfootObject, dCurrentComplate,fCurrentTemperature)) {


            //만약 완성됐을때 빅 성공인지를 체크
            if (Random.Range(0.0f, 100.0f) <= Mathf.Round(player.GetBigSuccessed()) && m_bIsFever == false)
            {
                Debug.Log("Fever!!");

                m_bIsFever = true;

				SpawnManager.Instance.cameraShake.Shake (0.05f, 0.5f);

				SucceessedObject.SetActive (true);

                //SpawnManager.Instance.SettingFever(m_fFeverCreateTime, m_fFeverSpeed);

				this.StartCoroutine(StartFever(player.GetBasicFeverTime()));
            }

			ComplateSlider.value = 0;
			TemperatureSlider.value = 0;

			ComplateText.text = string.Format("{0:#### / {1}", (int)ComplateSlider.value, 0);

			return;
		}
    }

	public void TouchBossWeapon(Vector3 _position)
	{
		if (bossCharacter == null)
			return;

		bossWeaponShake.Shake (12.0f, 0.12f);
		//Ice
		if (bossCharacter.nIndex == 0)
		{ 
			if (bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_01)
			{
				//Debug.Log ("IcePhase00");
			}
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_01 && bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_02)
			{
				//Debug.Log ("IcePhase01");
				//크리티컬 확률 감소o
				if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance () - 30.0f)) {
					//Debug.Log ("Cri!!!");
					GameObject obj = CriticalTouchPool.Instance.GetObject ();

					obj.transform.SetParent (CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<CriticalTouchParticle> ().Play ();

					SpawnManager.Instance.PlayerCritical ();

					dCurrentComplate = dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);

					m_PlayerAnimationController.UserCriticalRepair ();



				} else 
				{
					//Debug.Log ("Nor!!!!");
				
					GameObject obj = NormalTouchPool.Instance.GetObject();

					obj.transform.SetParent(CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<NormalTouchParticle>().Play();

					m_PlayerAnimationController.UserNormalRepair();

				
					dCurrentComplate = dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);


					m_PlayerAnimationController.UserNormalRepair ();
				
				}

				fCurrentTemperature += fMaxTemperature * 0.08f;
				return;

			} 
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_02)
			{
				//Debug.Log ("IcePhase02");
				//아르바이트 공속 감소 들어가야함
				if (isBossIcePassive01Active == false)
				{
					for(int i = 0; i < SpawnManager.Instance.array_ArbaitData.Length ; i++)
					{
						SpawnManager.Instance.array_ArbaitData [i].SetAttackSpeed (0.5f);
					}
					isBossIcePassive01Active = true;
				}

				//크리티컬 확률 감소o
				if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance () - 30.0f)) {
					//Debug.Log ("Cri!!!");
					GameObject obj = CriticalTouchPool.Instance.GetObject ();

					obj.transform.SetParent (CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<CriticalTouchParticle> ().Play ();

					SpawnManager.Instance.PlayerCritical ();

					dCurrentComplate = dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);

					m_PlayerAnimationController.UserCriticalRepair ();
				}
				else 
				{
					//Debug.Log ("Nor!!!!");

					GameObject obj = NormalTouchPool.Instance.GetObject();

					obj.transform.SetParent(CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<NormalTouchParticle>().Play();

					m_PlayerAnimationController.UserNormalRepair();

					dCurrentComplate =  dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);

					m_PlayerAnimationController.UserNormalRepair ();

				}

				fCurrentTemperature += fMaxTemperature * 0.08f;
				return;

			}
		}

		//Sasin
		int nRandom = Random.Range (0, 100);	
		fRandomXPos = Random.Range (fXPos - (textRectTrasnform.sizeDelta.x / 2), fXPos + (textRectTrasnform.sizeDelta.x / 2));
		fRandomYPos = Random.Range (fYPos - (textRectTrasnform.sizeDelta.y / 2), fYPos + (textRectTrasnform.sizeDelta.y / 2));

		if (bossCharacter.nIndex == 1) 
		{ 
			if (bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_01)
				Debug.Log ("SasinPhase00");
			 
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_01 && bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_02)
			{
				Debug.Log ("SasinPhase01");
				//명중률 50% 감소
				if (nRandom <= nChancePercent) 
					Debug.Log ("Attack To Sasin Success");
				else
				{
					Debug.Log ("Attack To Sasin Miss");

					textObj = textObjectPool.GetObject ();
					textObj.transform.SetParent (textRectTrasnform.transform, false);
					textObj.transform.localScale = Vector3.one;
					textObj.transform.position = new Vector3 (fRandomXPos, fRandomYPos, textObj.transform.position.z);
					textObj.name = "Miss";

					bossMissText = textObj.GetComponent<BossMissText> ();
					bossMissText.textObjPool = textObjectPool;
					bossMissText.leftSecond = 2.0f;
					bossMissText.parentTransform = textRectTrasnform;
					return;
				}
			} 
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_02) 
			{
				Debug.Log ("SasinPhase02");
				//수리력 30% 감소
				if (nRandom <= nChancePercent) 
				{
					//크리티컬 확률 
					if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance ())) {
						Debug.Log ("Cri!!!");
						GameObject obj = CriticalTouchPool.Instance.GetObject ();

						obj.transform.SetParent (CanvasTransform, false);

						obj.transform.position = _position;

						obj.GetComponent<CriticalTouchParticle> ().Play ();

						SpawnManager.Instance.PlayerCritical ();

						dCurrentComplate = dCurrentComplate + ((player.GetRepairPower () * 1.5f) * 0.7f);

						ShowDamage (((player.GetRepairPower () * 1.5f) * 0.7f),_position);

						m_PlayerAnimationController.UserCriticalRepair ();

					}
					else
					{
						Debug.Log ("Nor!!!!");

						GameObject obj = NormalTouchPool.Instance.GetObject();

						obj.transform.SetParent(CanvasTransform, false);

						obj.transform.position = _position;

						obj.GetComponent<NormalTouchParticle>().Play();

						m_PlayerAnimationController.UserNormalRepair(); 

					
						dCurrentComplate = dCurrentComplate + (player.GetRepairPower () * 0.7f);

						ShowDamage ((player.GetRepairPower () * 0.7f),_position);


						m_PlayerAnimationController.UserNormalRepair ();

					
					}
					fCurrentTemperature += fMaxTemperature * 0.08f;

					return;
				} 
				else 
				{
					Debug.Log ("Miss");
					textObj = textObjectPool.GetObject ();
					textObj.transform.SetParent (textRectTrasnform.transform, false);
					textObj.transform.localScale = Vector3.one;
					textObj.transform.position = new Vector3 (fRandomXPos, fRandomYPos, 0);
					textObj.name = "Miss";

					bossMissText = textObj.GetComponent<BossMissText> ();
					bossMissText.textObjPool = textObjectPool;
					bossMissText.leftSecond = 2.0f;
					bossMissText.parentTransform = textRectTrasnform;
					return;
				}
			}
		}

		//Fire
		if (bossCharacter.nIndex == 2) 
		{
			//Phase00
			if (bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_01) 
			{
				fCurrentTemperature += (fMaxTemperature * 0.08f) + ((fMaxTemperature * 0.08f) * fSmallFirePlusTemperatrue);
			}
			//Phase01
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_01 && bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_02) 
			{
				//크리데미지 50% 감소
				//Player의 기본 능력치에 따른 크리 and 노말 평타
				if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance ())) 
				{
					//Debug.Log ("Cri!!!");
					GameObject obj = CriticalTouchPool.Instance.GetObject ();

					obj.transform.SetParent (CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<CriticalTouchParticle> ().Play ();

					SpawnManager.Instance.PlayerCritical ();

					dCurrentComplate = dCurrentComplate + ((player.GetRepairPower () * 0.75f) * 0.5f  );

					ShowDamage ((player.GetRepairPower () * 1.5f),_position);

					m_PlayerAnimationController.UserCriticalRepair ();
				}
				else 
				{
					//Debug.Log ("Nor!!!!");
					GameObject obj = NormalTouchPool.Instance.GetObject();

					obj.transform.SetParent(CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<NormalTouchParticle>().Play();

					m_PlayerAnimationController.UserNormalRepair();

					dCurrentComplate =  dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);


					m_PlayerAnimationController.UserNormalRepair ();
				}

				//불씨에 따른 온도 상승량
				fCurrentTemperature += (fMaxTemperature * 0.08f) + ((fMaxTemperature * 0.08f) * fSmallFirePlusTemperatrue);
				return;
			} 
			//Phase02
			else
			{
				//물 충전량 50% 감소
				GameManager.Instance.player.SetBasicWaterPlus(	GameManager.Instance.player.GetBasicWaterPlus() * 0.5f);
				//불씨에 따른 온도 상승량
				fCurrentTemperature += (fMaxTemperature * 0.08f) + ((fMaxTemperature * 0.08f) * fSmallFirePlusTemperatrue);
			}

		}

		//MusicMan
		if (bossCharacter.nIndex == 3)
		{ 
			if (bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_01) 
			{
				Debug.Log ("MusicPhase00");
				//Player의 기본 능력치에 따른 크리 and 노말 평타



			}
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_01 && bossCharacter.eCureentBossState < Character.EBOSS_STATE.PHASE_02)
			{
				Debug.Log ("MusicPhase01");
				//아르바이트의 수리력이 50% 감소, 무기 움직임 시작
				if (isBossMusicPassive01Active == false) {
					
					for (int i = 0; i < SpawnManager.Instance.array_ArbaitData.Length; i++) {
						SpawnManager.Instance.array_ArbaitData [i].SetArbaitRepair (0.5f);
					}
					isBossMusicPassive01Active = true;
				}



			}
			else if (bossCharacter.eCureentBossState >= Character.EBOSS_STATE.PHASE_02)
			{
				Debug.Log ("MusicPhase02");
				if (isBossMusicPassive02Active == false) {

					for (int i = 0; i < SpawnManager.Instance.array_ArbaitData.Length; i++) {
						SpawnManager.Instance.array_ArbaitData [i].SetArbaitRepair (0);
					}
					isBossMusicPassive02Active = true;
				}
			}


			//반사 상태
			if (bossMusic.isReflect == true && bossMusic.isSwitch == true)
			{
				if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance ()))
				{
					//Debug.Log ("Cri!!!");
					GameObject obj = CriticalTouchPool.Instance.GetObject ();

					obj.transform.SetParent (CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<CriticalTouchParticle> ().Play ();

					SpawnManager.Instance.PlayerCritical ();

					dCurrentComplate = dCurrentComplate - (player.GetRepairPower () * 1.5f);

					ShowDamage ((player.GetRepairPower () * 1.5f),_position);

					m_PlayerAnimationController.UserCriticalRepair ();
				} else {
					//Debug.Log ("Nor!!!!");
					GameObject obj = NormalTouchPool.Instance.GetObject();

					obj.transform.SetParent(CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<NormalTouchParticle>().Play();

					m_PlayerAnimationController.UserNormalRepair();

					dCurrentComplate =  dCurrentComplate - player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);


					m_PlayerAnimationController.UserNormalRepair ();
				}
			}

			//반사 상태x
			if (bossMusic.isReflect == false && bossMusic.isSwitch == false)
			{
				if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance ()))
				{
					//Debug.Log ("Cri!!!");
					GameObject obj = CriticalTouchPool.Instance.GetObject ();

					obj.transform.SetParent (CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<CriticalTouchParticle> ().Play ();

					SpawnManager.Instance.PlayerCritical ();

					dCurrentComplate = dCurrentComplate + (player.GetRepairPower ()  * 1.5f);

					ShowDamage (player.GetRepairPower (),_position);


					m_PlayerAnimationController.UserCriticalRepair ();

				} 
				else
				{
					//Debug.Log ("Nor!!!!");
					GameObject obj = NormalTouchPool.Instance.GetObject();

					obj.transform.SetParent(CanvasTransform, false);

					obj.transform.position = _position;

					obj.GetComponent<NormalTouchParticle>().Play();

					m_PlayerAnimationController.UserNormalRepair();

					dCurrentComplate = dCurrentComplate + player.GetRepairPower ();

					ShowDamage (player.GetRepairPower (),_position);


					m_PlayerAnimationController.UserNormalRepair ();
				}
			}
				
			fCurrentTemperature += fMaxTemperature * 0.08f;
			return;
		}


		//Player의 기본 능력치에 따른 크리 and 노말 평타
		if (Random.Range (1, 100) <= Mathf.Round (player.GetCriticalChance ())) 
		{
			//Debug.Log ("Cri!!!");
			GameObject obj = CriticalTouchPool.Instance.GetObject ();

			obj.transform.SetParent (CanvasTransform, false);

			obj.transform.position = _position;

			obj.GetComponent<CriticalTouchParticle> ().Play ();

			SpawnManager.Instance.PlayerCritical ();

			dCurrentComplate = dCurrentComplate + (player.GetRepairPower () * 1.5f);

			ShowDamage ((player.GetRepairPower () * 1.5f),_position);


			m_PlayerAnimationController.UserCriticalRepair ();
		} 
		else
		{
			//Debug.Log ("Nor!!!!");
			GameObject obj = NormalTouchPool.Instance.GetObject();

			obj.transform.SetParent(CanvasTransform, false);

			obj.transform.position = _position;

			obj.GetComponent<NormalTouchParticle>().Play();

			m_PlayerAnimationController.UserNormalRepair();


			dCurrentComplate = dCurrentComplate + player.GetRepairPower ();

			ShowDamage (player.GetRepairPower (),_position);


			m_PlayerAnimationController.UserNormalRepair ();
		}

		fCurrentTemperature += fMaxTemperature * 0.08f;
	}

    public void TouchWater()
	{
		if (weaponData == null)
			return;

		if (isTouchWaterAvailable == true) 
		{
			SoundManager.instance.PlaySound (eSoundArray.ES_WaterActiveSound);

			isTouchWaterAvailable = false;
			isTouchWater = true;

			fWeaponDownTemperature = fMaxTemperature * 0.3f;
		
			// 수리력 = 수리력 * ( 현재온도 * 11 * 0.00556) * ( 1 - 물수치(플레이어의 무기 + 장비의 물수치))
			dCurrentComplate += GameManager.Instance.player.GetRepairPower() * (fCurrentTemperature  * 11f * 0.00556f) * 
				(1f + (weaponData.fMinusUseWater * 0.05f));

			//Debug.Log ("물 수치 Plus 수치 : " + GameManager.Instance.player.GetWaterPlus() + "물 최대 수치 : " + GameManager.Instance.player.GetBasicMaxWater() ); 
			
			//온도 감소량
			fCurrentTemperature -= fMaxTemperature * 0.5f;

			if (fCurrentTemperature < 0)
				fCurrentTemperature = 0;
			
			//물 감소량
			fCurrentWater -=  1000f ;

			if (fCurrentWater < 0) {
				fCurrentWater = 0;
			}

			float resultWaterValue =  (weaponData.fMinusUseWater * 0.01f);
			Debug.Log ("Before fCurrent : " + dCurrentComplate);          

			Debug.Log ("After fCurrent : " + dCurrentComplate);     

			Debug.Log ("TouchWater!!");
			//bossWaterCat_animator.SetBool ("isTouchWater", true);

			SpawnManager.Instance.UseWater ();

			WaterSlider.value = fCurrentWater;

			if (dCurrentComplate > weaponData.dMaxComplate)
				dCurrentComplate = weaponData.dMaxComplate;

			TemperatureSlider.value = fCurrentTemperature;

			if (dCurrentComplate >= weaponData.dMaxComplate) 
			{
				PlayerInfo.SetGuestWeapon (null);
				SpawnManager.Instance.ComplateCharacter (AfootObject, weaponData.dMaxComplate);
			}
		}
	}

	public void TouchBossWater()
	{

		if (bossCharacter == null)
			return;
	
		if (isTouchWaterAvailable == true ) 
		{
			//useWater
			Debug.Log ("TouchBossWater!!");
			isTouchWaterAvailable = false;
			isTouchWater = true;

			//waterPaching.SetActive (true);
			//물 터치시 노트 한단계씩 떨어진다.
			SoundManager.instance.PlaySound (eSoundArray.ES_WaterActiveSound);

			//온도 감소 수치
			fCurrentTemperature -= fMaxTemperature * 0.3f;

			if (fCurrentTemperature < 0)
				fCurrentTemperature = 0;

			//물 감소 수치
			fCurrentWater -= 1000f;

			if (fCurrentWater < 0)
				fCurrentWater = 0;

			fWeaponDownTemperature = fMaxTemperature * 0.3f;

			//플레이어가 장비하고 있는 무기 물수치의 1%
			// 수리력 = 수리력 * ( 현재온도 * 11 * 0.00556) * ( 1 - 물수치(플레이어의 무기 + 장비의 물수치))
			// 플레이어 수리력 추가 해야됨
			double completeValueResult = 0;

			completeValueResult = (GameManager.Instance.player.GetRepairPower () * (fCurrentTemperature * 11f * 0.00556));
			dCurrentComplate += completeValueResult;

			if (bossCharacter.nIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC) 
			{
				
				int nChildCount = bossNoteRectTransform.childCount;
				Debug.Log ("CurCount = " + nChildCount);
				for (int i = 0; i < nChildCount; i++)
				{
					Transform noteGameObject = null;		

					if (bossNoteRectTransform.transform.GetChild (0).name == "Note") {
						noteGameObject = bossNoteRectTransform.transform.GetChild (0);
						noteObj = noteGameObject.gameObject.GetComponent<NoteObject> ();
						noteObj.CreateNote ();
					} 
					else if(bossNoteRectTransform.transform.GetChild (0).name == "Note2") 
					{
						noteGameObject = bossNoteRectTransform.transform.GetChild (0);
						note2Obj = noteGameObject.gameObject.GetComponent<Note2Object> ();
						note2Obj.EraseObj ();

					} 
					/*else if (bossNoteRectTransform.transform.GetChild (0).name == "Note3") {
						noteGameObject = bossNoteRectTransform.transform.GetChild (0);
						note3Obj = noteGameObject.gameObject.GetComponent<Note3Object> ();
						note3Obj.EraseObj ();
					}
					*/
				}
			}
			//얼음 보스시. 물을 사용하면 화면을 얼게 한다
			if (bossCharacter.nIndex == (int)E_BOSSNAME.E_BOSSNAME_ICE)
			{
				bossIce.ActiveIceWall ();
			}
				
		



			Debug.Log ("FireBossWaterValue : " + completeValueResult);

			//불 보스 일 경우에만  물수치가 50%감소


			SpawnManager.Instance.UseWater ();

			WaterSlider.value = fCurrentWater;

			if (dCurrentComplate > dBossMaxComplete)
				dCurrentComplate = dBossMaxComplete;

			if (fCurrentWater < 0)
				fCurrentWater = 0;

			if (fCurrentTemperature < 0)
				fCurrentTemperature = 0;

			TemperatureSlider.value = fCurrentTemperature;


			if (dCurrentComplate >= dBossMaxComplete) 
			{
				SpawnManager.Instance.bIsBossCreate = false;
				//bossCharacter.
			}
		}

	}

	//값을 수치로 표기하기 위한 함수 
	public string ChangeValue(double _dValue)
	{ 
		//
		int[] cVal = new int[10]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		if (_dValue < 10000)
			return strValue;

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				int intLast4 = int.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = int.Parse (strValue); 
				break; 
			} 

			index++; 
		} 

		//1000,00
		//1000,000,00
		//1000,000,000,00

		while (_dValue >= 1000) 
		{
			_dValue *= 0.001f;
		}

		if (index > 0) { 

			if (_dValue >= 100) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else if (_dValue >= 10) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 1);

				string strSecondValue = nResult.ToString ().Substring (1, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			}
		} 

		return strValue; 
	}


    //만약 수리중에 대기시간이 다 지나서 되돌아갈때 확인함
    public void CheckMyObject(GameObject _obj)
    {
        if (_obj == AfootObject)
        {
			SpawnManager.Instance.ReturnInsertData(AfootObject,false,false, dCurrentComplate, TemperatureSlider.value);
            InitWeaponData();
        }
    }

    //전체 초기화
    private void InitWeaponData()
    {
        weaponData = null;
        AfootObject = null;

		WeaponSprite.sprite =  main_Touch_Sprite;

        
		dCurrentComplate = 0;

        ComplateSlider.value = 0;
        ComplateSlider.maxValue = 0;
        TemperatureSlider.value = 0;
        fCurrentTemperature = 0;

		ComplateText.text = string.Format("{0} / {1}", dCurrentComplate, ComplateSlider.maxValue);
    }

	//현재무기의 완성도를 가져온다
	public double GetCurCompletion()
	{
		return dCurrentComplate;
	}
	public void SetCurCompletion(double _value)
	{
		dCurrentComplate += _value;
	}
	public bool isCurTemperatureOver()
	{
		if (fCurrentTemperature >= fMaxTemperature)
			return true;
		else
			return false;
	}
	public void SetMaxTempuratrue()
	{
		fCurrentTemperature = fMaxTemperature;
		dCurrentComplate = (dCurrentComplate) - (dBossMaxComplete * 0.3);
	}

	public void SetFinishBoss()
	{
		

		bossWeaponObject.transform.position = bossWeaponObjOriginPosition;
		bossWeaponRectTransform.sizeDelta = bossWeaponObjOriginSize;

		bossWeaponObject.SetActive (false);
		bossWaterObject.SetActive (false);
		WeaponObject.SetActive (true);
		waterObject.SetActive (true);

		dCurrentComplate = 0;
		WaterSlider.minValue = 0;
		ComplateSlider.minValue = 0;
		TemperatureSlider.minValue = 0;

		WaterSlider.maxValue = 0;
		ComplateSlider.maxValue = 0;
		TemperatureSlider.maxValue = 0;

		fCurrentWater = 0f;
		fCurrentTemperature = 0f;

		GameManager.Instance.GetPlayer ().changeStats.fWaterPlus = fOriWaterPlus;
		fMaxWater = player.GetBasicMaxWaterPlus ();

		//FireBoss
		fSmallFireMinusWater = 0f;				
		fSmallFirePlusTemperatrue = 0f;


		TemperatureSlider.maxValue = fMaxTemperature;
		TemperatureSlider.value = 0;

		WaterSlider.maxValue = fMaxWater;
		WaterSlider.value = 0;

		WeaponSprite.sprite = main_Touch_Sprite;

		ComplateText.text = string.Format ("{0} / {1}", dCurrentComplate, ComplateSlider.maxValue);

		//얼음 보스시 초기화 (아르바이트 어택 스피드, bool)
		for(int i = 0; i < SpawnManager.Instance.array_ArbaitData.Length ; i++)
			SpawnManager.Instance.array_ArbaitData [i].SetAttackSpeed (1.0f);
		
		isBossIcePassive01Active = false;

		//불 보스시 패시브 
		//물 충전량 50% 감소
		GameManager.Instance.player.SetBasicWaterPlus(	GameManager.Instance.player.GetBasicWaterPlus() * 2f);

		//음악 보스 패시브 
		for (int i = 0; i < SpawnManager.Instance.array_ArbaitData.Length; i++)
			SpawnManager.Instance.array_ArbaitData [i].SetArbaitRepair (1f);
		
		isBossMusicPassive01Active = false;
		isBossMusicPassive02Active = false;

		//음악 노트 모두 제거 
		if (bossCharacter.nIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC) 
		{
			int nChildCount = bossNoteRectTransform.childCount;

			while (nChildCount != 0) 
			{
				Transform noteGameObject =null;	
				Debug.Log ("CurCount = " + nChildCount);
				if (bossNoteRectTransform.Find ("Note"))
				{

					noteGameObject = bossNoteRectTransform.Find ("Note");
					noteObj = noteGameObject.gameObject.GetComponent<NoteObject> ();
					noteObj.EraseObj ();

				} else if (bossNoteRectTransform.Find ("Note2")) {
					noteGameObject = bossNoteRectTransform.Find ("Note2");
					note2Obj = noteGameObject.gameObject.GetComponent<Note2Object> ();
					note2Obj.EraseObj ();
				}
				nChildCount--;
			}
			isMoveWeapon = false;
		}

		bossCharacter = null;

		bossIce = null;
		bossMusic = null;
	}
}
