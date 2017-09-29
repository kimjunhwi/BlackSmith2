using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using UnityEngine.EventSystems;

enum E_GOBLIN_INDEX
{
	E_GOBLIN_GOLD = 0,
	E_GOBLIN_HONOR,
	E_GOBLIN_RUBY,
	E_GOBLIN_BUFF_GODL,
	E_GOBLIN_BUFF_HONOR,
	E_GOBLIN_BUFF_ARBAIT,
	E_GOBLIN_BUFF_TOUCH,
	E_MAX,
}

public class Goblin_Guest : Character {

	public double dValue = 0;
	public int nRandomIndex = 0;
	public Sprite[] Goblin_Sprite;

	public string strValue = "";

	private const string sCashTimeSaveName_Gold = "SaveCashConsume_GoldTime";
	private const string sCashTimeSaveName_Honor = "SaveCashConsume_HonorTime";
	private const string sCashTimeSaveName_Staff = "SaveCashConsume_StaffTime";
	private const string sCashTimeSaveName_Attack = "SaveCashConsume_AttackTime";

	public override void Awake ()
	{
		base.Awake();

		boxCollider = gameObject.GetComponent<BoxCollider2D>();
	}

	void OnEnable()
	{
		m_bIsBack = false;

		mySprite.flipX = false;

		m_bIsArrival = false;

		m_bIsFirstBack = false;

		mySprite.sortingOrder = (int)E_SortingSprite.E_WALK;

		m_nIndex = -1;

		E_STATE = ENORMAL_STATE.WALK;

		transform.position = m_VecStartPos;

		boxCollider.isTrigger = false;

		nRandomIndex = Random.Range (0, (int)E_GOBLIN_INDEX.E_MAX);


		switch (nRandomIndex) 
		{
		case (int)E_GOBLIN_INDEX.E_GOBLIN_GOLD:
			dValue = (250 * Mathf.Pow (1.09f, cPlayerData.GetDay () - 1)) * 10;

			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_HONOR:
			dValue = 100;

			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_RUBY:
			dValue = 10;

			break;
		default:
			dValue = 0;
			break;
		}

		BallonObject.SetActive (false);
	}

	void OnDisable()
	{
		strValue = "";
		dValue = 0;
		nRandomIndex = 0;

		m_VecMoveDistance = new Vector3(0.0f, 0.0f, 0.0f);
	}

	void Update()
	{

		StartCoroutine(this.CheckCharacterState());

		StartCoroutine(this.CharacterAction());
	}

	IEnumerator CheckCharacterState()
	{
		yield return new WaitForSeconds(0.3f);

		//시간이 지났거나 m_bIsBack이 트루일경우 돌려보냄
		if (m_bIsBack == true)
			E_STATE = ENORMAL_STATE.BACK;

		//시간이 지나지 않았거나 도착하지 않았다면 Walk
		else if(m_bIsArrival == false)
			E_STATE = ENORMAL_STATE.WALK;

		//그 외에는 대기
		else
			E_STATE = ENORMAL_STATE.WAIT;
	}

	IEnumerator CharacterAction()
	{
		switch (E_STATE) {
		case ENORMAL_STATE.WALK:

			m_anim.SetBool ("bIsWalk", true);

			//Move 함수를 통해 지정된 위치로 자연스럽게 이동하기 위해 MoveTowards 함수를 사용
			transform.position = Vector3.MoveTowards (transform.position, m_VecMoveDistance, fSpeed * Time.deltaTime);

			//만약 도착했다면
			if ((transform.position.x == m_VecMoveDistance.x)) {
				m_bIsArrival = true;
			}
			break;

		case ENORMAL_STATE.WAIT:
			m_anim.SetBool ("bIsWalk", false);

			BallonObject.SetActive (true);
			break;

		case ENORMAL_STATE.BACK:

			//딱 한번만 호출 되야 하는 부분
			if (!m_bIsFirstBack) 
			{
				BallonObject.SetActive (false);

				fSpeed = 3.0f;

				m_bIsFirstBack = true;

				boxCollider.isTrigger = true;

				m_anim.SetBool ("bIsWalk", true);

				//이미지 변경이나 효과 애니메이션 변경등을 진행
				mySprite.flipX = true;

				mySprite.sortingOrder = (int)E_SortingSprite.E_BACK;

				//현재 캐릭터를 지움
				SpawnManager.Instance.DeleteObject(gameObject);
			}

			transform.position = Vector3.MoveTowards (transform.position, m_VecStartPos, fSpeed * Time.deltaTime);

			if (Vector3.Distance (transform.position, m_VecStartPos) < 0.5f) 
			{
				SpawnManager.Instance.m_bIsGoblinCreate = false;

				gameObject.SetActive (false);
			}
			break;

		default:
			break;
		}

		yield return null;
	}

	public override void RetreatCharacter(float _fSpeed,bool _bIsBack,  bool _bIsAllBack = false)
	{
		fSpeed = _fSpeed;

		m_bIsBack = _bIsBack;
	}

	//지정한 인덱스로 손님을 이동시키기 위함
	public override void Move(int _nIndex)
	{
		if (m_nIndex == _nIndex)
			return;

		m_nIndex = _nIndex;

		m_bIsArrival = false;

		float fDistance = 0.8f;

		fDistance *= _nIndex;

		m_VecMoveDistance = new Vector3(m_VecEndPos.x + fDistance, transform.position.y, 0);
	}

	void OnMouseDown()
	{
		if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE01 || SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE02 ||
			SpawnManager.Instance.questManager.isQuestWindowOn == true)
			return;

		if (Input.GetMouseButtonDown (0) && (E_STATE == ENORMAL_STATE.WAIT))
		{
			if (nRandomIndex >= (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_GODL)
				strValue = "";
			
			else
				strValue = ScoreManager.ScoreInstance.ChangeMoney (dValue);

			GameManager.Instance.Window_Goblin_yesno ("", strValue, Goblin_Sprite [nRandomIndex], rt => {

				//광고 보고 2배
				if (rt == "0") {
					GameManager.Instance.ShowRewardedAd_Goblin(this);
				} 
				// 그냥 받기 
				else if (rt == "1") {

					switch(nRandomIndex)
					{
					case (int)E_GOBLIN_INDEX.E_GOBLIN_GOLD:
						ScoreManager.ScoreInstance.GoldPlus(dValue);
						break;
					case (int)E_GOBLIN_INDEX.E_GOBLIN_HONOR:
						ScoreManager.ScoreInstance.HonorPlus(dValue );
						break;
					case (int)E_GOBLIN_INDEX.E_GOBLIN_RUBY:
						ScoreManager.ScoreInstance.RubyPlus( (int)(dValue ));
						break;
					case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_GODL:

						SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD,5,0);
							
						break;

					case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_HONOR:
						SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR,5,0);
							
						break;
					case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_ARBAIT:
						SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF,5,0);
							
						break;
					case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_TOUCH:
						SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK,5,0);
							
						break;
					}

					m_bIsBack = true;

					BallonObject.SetActive(false);
				}
			}
			);
		}
	}

	public void Goblin_Show_Ads()
	{
		switch(nRandomIndex)
		{
		case (int)E_GOBLIN_INDEX.E_GOBLIN_GOLD:
			ScoreManager.ScoreInstance.GoldPlus(dValue * 2);
			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_HONOR:
			ScoreManager.ScoreInstance.HonorPlus(dValue * 2);
			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_RUBY:
			ScoreManager.ScoreInstance.RubyPlus( (int)(dValue * 2));
			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_GODL:

			SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD,10,0);

			break;

		case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_HONOR:
			SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR,10,0);
			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_ARBAIT:
			SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF,10,0);
			break;
		case (int)E_GOBLIN_INDEX.E_GOBLIN_BUFF_TOUCH:
			SpawnManager.Instance.shopCash.LoadBooster(E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK,10,0);
			break;
		}

		m_bIsBack = true;

		BallonObject.SetActive(false);
	}
}
