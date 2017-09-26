using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using UnityEngine.EventSystems;

[System.Serializable]
public class Character : MonoBehaviour 
{
    public enum ENORMAL_STATE
    {
        WALK,
        WAIT,
        BACK,
    }

	public enum EBOSS_STATE
	{
		CREATEBOSS = 0,		//보스생성
		WAIT,				//대기(등장 연출)
		PHASE_00,			//스킬 기본 시작 
		PHASE_01,			//스킬1 시작
		PHASE_02,			//스킬2 시작
		DIE,				//죽음
		RESULT,				//결과
		FINISH,				//끝
	}

    public enum CHARACTER_GRADE
    {
        NORMAL = 0,
        MAGIC,
        LEGEND,
    }
	public double dGold;
    //완성도
	public double m_dComplate { get; set; }
    //온도
    public float m_fTemperator { get; set; }

	//캐릭터 상태
	public ENORMAL_STATE E_STATE;

    //Max온도치
	protected const int n_nMinTemperature = 0;

	//캐릭터(무기) 등급
    protected CHARACTER_GRADE E_GRADE;

    public bool m_bIsRepair = false;
    
    //움직이는 속도
	public float fSpeed = 1.0f;

    //캐릭터 내부 시간
    protected float m_fCharacterTime = 0.0f;

    //캐릭터가 기다리는 시간
    protected float m_fCharacterWaitTime = 0.0f;

    //캐릭터 애니메이터 (추후 추가)
    protected Animator m_anim;

    //저장될 무기 데이터
    public CGameWeaponInfo weaponData { get; set; }

    //수리하는곳을 보여주는 오브젝트
    protected RepairObject RepairShowObject;

    //무기 이미지
    protected SpriteRenderer weaponsSprite;

    //자신 이미지
    protected SpriteRenderer mySprite;

    protected BoxCollider2D boxCollider;

	public Sprite NoneSpeech;

	public Sprite ArbaitOneSpeech;

	public Sprite ArbaitThreeSpeech;

	public Sprite ArbaitTwoSpeech;

	public Sprite PlayerRepairSpeech;

	public Player cPlayerData;

	//도착 지점
	public Vector3 m_VecEndPos;

	//활성화 됐을때 위치
	public Vector3 m_VecStartPos;

	//움직일 이동 거리
	public Vector3 m_VecMoveDistance;

	//생성되는 위치
	public Transform spawnTransform;

	//캐릭터 인덱스
	public int m_nIndex = -1;

	public bool m_bIsBack = false;

	//캐릭터가 지정한 위치에 도달했는가
	public bool m_bIsArrival = false;

	//뒤로 가는 부분에 처음 부분만 실행하기 위함
	public bool m_bIsFirstBack = false;

	public GameObject BallonObject;

	public virtual void Awake()
	{
		E_GRADE = CHARACTER_GRADE.NORMAL;

		mySprite = GetComponent<SpriteRenderer> ();

		m_VecEndPos = GameObject.Find("EndPoint").transform.position;

		spawnTransform = GameObject.Find("SpawnPoint").gameObject.transform;

		m_VecStartPos = spawnTransform.position;

		m_anim = gameObject.GetComponent<Animator> ();

		cPlayerData = GameManager.Instance.player;
	}

	public virtual void Move (int _nIndex){}

	public virtual void RetreatCharacter(float _fSpeed, bool _bIsBack, bool _bIsAllBack){}

	public virtual bool CheckComplate(double _dComplate,float _fTemperator) {return false; }
    
	public virtual void Complate(double _dComplate = 0.0f) { }

    
}
