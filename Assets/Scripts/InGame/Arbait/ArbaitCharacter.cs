using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class ArbaitCharacter : MonoBehaviour {

    public int nIndex;
    private int nEnhanceCost;
    private int nGetBatchIndex;

    protected bool m_bIsBatch = false;

	private Button m_BuyButtonEvent;
	private Button m_EnhanceButtonEvent;

    protected Sprite m_NoneImage;
    protected Sprite m_CharacterImage;

	public Sprite m_NoneActiveSprite;
	public Sprite m_ActiveSprite;
    
	public Sprite BuyGoldImage;
	public Sprite BuyHonorImage;

	public Image BuyImage;
    protected Image m_CharacterImageObject;
    
	protected ArbaitData m_CharacterData = null;
	protected CGameArbaitGrade[] ArbaitEnhanceData; 


    private Toggle m_SettingToggle;

    private GameObject m_BuyButton;
    private GameObject m_SettingPanel;
    private SpawnManager spawnManager;

	public GameObject TextPanel;
	public GameObject PurchasingObject;

	public Text LevelText;
	public Text NameText;
	public Text SkillExplainText;
	public Text RepairPowerText;
	public Text AttackSpeedText;
	public Text CriticalText;
	public Text AccuracyText;
	public Text GoldText;
	public Text PurchasingText;
	public Text BuyText;

	public GameObject BossNameObject;
	public GameObject PurchasingNameObject;
	public Text CharacterNameText;
	public Text EnhaceText;

	public Player playerData;

    public virtual void Upgrade(){}

	// Use this for initialization
	protected void Awake () {
		
        m_BuyButton = transform.Find("BuyButton").gameObject;

		m_BuyButtonEvent = m_BuyButton.GetComponent<Button> ();

		m_BuyButtonEvent.onClick.AddListener (BuyCharacter);
        
		TextPanel = transform.Find ("TextPanel").gameObject;

		LevelText = TextPanel.transform.Find ("LevelText").GetComponent<Text>();
		NameText = TextPanel.transform.Find ("NameText").GetComponent<Text>();
		SkillExplainText = TextPanel.transform.Find ("SkillExplain").GetComponent<Text>();
		RepairPowerText = TextPanel.transform.Find ("RepairPower").GetComponent<Text>();
		AttackSpeedText = TextPanel.transform.Find ("AttackSpeed").GetComponent<Text>();
		CriticalText = TextPanel.transform.Find ("Critical").GetComponent<Text>();
		AccuracyText = TextPanel.transform.Find ("Accuracy").GetComponent<Text>();
		BuyText = m_BuyButton.transform.Find ("BuyText").GetComponent<Text> ();

		m_SettingPanel = transform.Find("SettingPanel").gameObject;

		spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        
		m_CharacterImageObject = transform.Find("CharacterImage").GetComponent<Image>();
        
		m_SettingToggle = m_SettingPanel.transform.Find("SettingToggle").GetComponent<Toggle>();

		m_EnhanceButtonEvent = m_SettingPanel.transform.Find ("EnhanceButton").GetComponent<Button> ();

		GoldText = m_EnhanceButtonEvent.transform.Find ("CostText").GetComponent<Text>();

		m_SettingToggle.onValueChanged.AddListener (OnBatchToggle);

		m_EnhanceButtonEvent.onClick.AddListener (EnhanceEvent);

		playerData = GameManager.Instance.GetPlayer ();
	}

	void OnEnable()
	{
		if(m_CharacterData != null)
			ChangeArbaitText ();
	}

	public void SetUp(int _nIndex,Sprite _sprite)
	{
		nIndex = _nIndex;

		m_CharacterImageObject.sprite = _sprite;

		m_CharacterData = GameManager.Instance.GetArbaitData(nIndex);

        CheckBuyCharacter();

		if (m_CharacterData.nScoutGold != 0) 
		{
			BuyImage.sprite = BuyGoldImage;
		} 
		else 
		{
			BuyImage.sprite = BuyHonorImage;
		}

		BuyText.text = (m_CharacterData.nScoutGold != 0) ? m_CharacterData.nScoutGold.ToString() : m_CharacterData.nScoutHonor.ToString();
		GoldText.text =  (m_CharacterData.nBasicHonor + ((m_CharacterData.level - 1) * 0.4 * m_CharacterData.nBasicHonor)).ToString();

		spawnManager.array_ArbaitData [nIndex].Purchasing ();
		spawnManager.array_ArbaitData [nIndex].Setting ();

		CharacterNameText.text = m_CharacterData.name;
		SkillExplainText.text = m_CharacterData.strExplains;
		PurchasingText.text = m_CharacterData.strPurchasing;
		NameText.text = m_CharacterData.name;

		if (m_CharacterData.index >= 10) {
			PurchasingNameObject.SetActive (true);
			CharacterNameText.text = m_CharacterData.name;
			BossNameObject.SetActive (true);
		} else {
			PurchasingNameObject.SetActive (false);
			BossNameObject.SetActive (false);
		}

		ChangeArbaitText ();
	}

    public void BuyCharacter()
    {
        //스코어 매니저를 만들었을 경우 개선

		if (m_CharacterData.nScoutGold <= ScoreManager.ScoreInstance.GetGold() && m_CharacterData.nScoutHonor <= ScoreManager.ScoreInstance.GetHonor())
        {
			ScoreManager.ScoreInstance.GoldPlus (-m_CharacterData.nScoutGold);
			ScoreManager.ScoreInstance.HonorPlus (-m_CharacterData.nScoutHonor);

            //추후 추가
            m_CharacterData.level = 1;

        }
        CheckBuyCharacter();
    }

	public void CheckArbaitScoutCount(bool _bIsBoss = false)
	{
		if (_bIsBoss) 
		{
			if (m_CharacterData.index < 10)
				return;

			m_CharacterData.nScoutCount++;

			spawnManager.array_ArbaitData [nIndex].Purchasing ();

			CheckBuyCharacter ();

//			if (m_CharacterData.nMaxScoutCount <= m_CharacterData.nScoutCount) 
//			{
//				m_BuyButton.SetActive (true);
//				m_SettingPanel.SetActive (false);
//
//				gameObject.GetComponent<Image>().sprite = m_ActiveSprite;
//			}
		} 
		else 
		{
			if (m_CharacterData.index >= 10)
				return;

			m_CharacterData.nScoutCount++;

			spawnManager.array_ArbaitData [nIndex].Purchasing ();

			CheckBuyCharacter ();

//			if (m_CharacterData.level == 0 && m_CharacterData.nMaxScoutCount <= m_CharacterData.nScoutCount) 
//			{
//				m_BuyButton.SetActive (true);
//				m_SettingPanel.SetActive (true);
//
//				gameObject.GetComponent<Image>().sprite = m_ActiveSprite;
//			}
		}
	}

    public void CheckBuyCharacter()
    {
        //구매하지 않았을 경우
		if (m_CharacterData.nScoutCount < m_CharacterData.nMaxScoutCount) 
		{
			m_BuyButton.SetActive (false);
			m_SettingPanel.SetActive (false);
			PurchasingObject.SetActive (true);
			BossNameObject.SetActive (true);

			gameObject.GetComponent<Image> ().sprite = m_NoneActiveSprite;
		}
        else if (m_CharacterData.level == 0)
        {
            m_BuyButton.SetActive(true);
            m_SettingPanel.SetActive(false);
			PurchasingObject.SetActive (false);
			BossNameObject.SetActive (false);

			gameObject.GetComponent<Image>().sprite = m_ActiveSprite;

			BuyText.text = (m_CharacterData.nScoutGold != 0) ? m_CharacterData.nScoutGold.ToString() : m_CharacterData.nScoutHonor.ToString(); 
            //m_CharacterImageObject.sprite = m_CharacterNoneSprite;
        }

        //구매했을경우
		else if(m_CharacterData.level != 0)
        {
            m_BuyButton.SetActive(false);
            m_SettingPanel.SetActive(true);
			PurchasingObject.SetActive (false);
			BossNameObject.SetActive (false);

            gameObject.GetComponent<Image>().sprite = m_ActiveSprite;

            ChangeArbaitText();

            if (m_CharacterData.batch != -1)
            {
                m_SettingToggle.isOn = true;

                spawnManager.AddArbait(m_CharacterData.index, m_CharacterData.batch, gameObject, m_CharacterData);

                m_bIsBatch = true;
            }
            else
                m_SettingToggle.isOn = false;

			GoldText.text =  (m_CharacterData.nBasicHonor + ((m_CharacterData.level - 1) * 0.4 * m_CharacterData.nBasicHonor)).ToString();
        }
    }

    //아르바이트 배치 및 해제 부분
    public void OnBatchToggle(bool _bIsToggle)
    {
        if (m_CharacterData == null)
            return;

        bool bIsToggle = _bIsToggle;

        Debug.Log(m_SettingToggle.isOn);

        //만약 배치를 해야하고 배치상태가 아니라면
        if (bIsToggle && m_bIsBatch == false)
        {
            //배치 될 수 있는지를 확인
            nGetBatchIndex = spawnManager.AddArbaitCheck();

            //만약 배치 할 수 있다면 현재 인덱스, 배치 인덱스, 현재 페널, 캐릭터 데이터를 보낸후 배치
            if (nGetBatchIndex != (int)E_CHECK.E_FAIL)
            {
                spawnManager.AddArbait(m_CharacterData.index, nGetBatchIndex, gameObject, m_CharacterData);
                m_bIsBatch = true;
				m_SettingToggle.isOn = true;
            }
            else
            {
                m_SettingToggle.isOn = false;
            }
        }
        //해제를 원하고 배치상태라면
        else if ((bIsToggle == false) && m_bIsBatch)
        {
            //배치된 아르바이트를 지움
            spawnManager.DeleteArbait(gameObject);

			playerData.PlayerStatsSetting ();

            m_CharacterData.batch = -1;

            m_bIsBatch = false;
        }

        Debug.Log(m_SettingToggle.isOn);
    }

    public void EnhanceEvent()
    {
		if (m_CharacterData.nBasicHonor + ((m_CharacterData.level - 1) * 0.4 * m_CharacterData.nBasicHonor) <= ScoreManager.ScoreInstance.GetHonor ()) 
		{
			m_CharacterData.fAttackSpeed -= m_CharacterData.fAttackSpeed * 1 * 0.01f;
			m_CharacterData.fCritical += m_CharacterData.fCritical * 1 * 0.01f;

			m_CharacterData.level++;
			m_CharacterData.nPlayerGetRepair++;

			spawnManager.array_ArbaitData [nIndex].EnhacneArbait();

			ChangeArbaitText ();

			GoldText.text =  (m_CharacterData.nBasicHonor + ((m_CharacterData.level - 1) * 0.4 * m_CharacterData.nBasicHonor)).ToString();
		}
    }

    public void ChangeArbaitText()
    {
        LevelText.text = m_CharacterData.level.ToString();
        NameText.text = m_CharacterData.name;
        SkillExplainText.text = m_CharacterData.strExplains;
		PurchasingText.text = m_CharacterData.strPurchasing;
		RepairPowerText.text = ScoreManager.ScoreInstance.ChangeMoney(m_CharacterData.dRepairPower);
        AttackSpeedText.text = m_CharacterData.fAttackSpeed.ToString("F1");
        CriticalText.text = m_CharacterData.fCritical.ToString("F1");
        AccuracyText.text = m_CharacterData.fAccuracyRate.ToString("F1");
    }
}
