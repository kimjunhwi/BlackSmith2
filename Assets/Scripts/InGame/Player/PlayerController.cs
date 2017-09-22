using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Animator m_Animator;

    public Transform normalPosition;
	public Transform criticalPosition;


	public Transform GuestSuccessedPosition;
	public SuccessedPool GuestSuccessedPool;

	public Transform BigSuccessedPosition;
	public BigSuccesedPool BigSuccessedPool;

	public SimpleObjectPool NormalRepairPool;
	public SimpleObjectPool CriticalRepairPool;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
		GuestSuccessedPool.Init ();
		BigSuccessedPool.Init ();
    }


    public void UserNormalRepair()
    {
        m_Animator.SetTrigger("bIsNormalRepair");

    }

    public void UserCriticalRepair()
    {
        m_Animator.SetTrigger("bIsCriticalRepair");

    }

	public void UserBigSuccessedRepair()
	{
		m_Animator.SetTrigger ("bIsSuccessedRepair");
	}

    public void CreateNormalEffect()
	{

        GameObject obj = NormalRepairPool.GetObject();

		obj.transform.position = normalPosition.position;

		obj.GetComponent<ParticlePlay>().Play(NormalRepairPool);
    }

    public void CreateCriticalEffect()
    {
        GameObject obj = CriticalRepairPool.GetObject();

		obj.transform.position = criticalPosition.position;

		obj.GetComponent<ParticlePlay>().Play(CriticalRepairPool);
    }

	public void NormalTouchSound()
	{
		SoundManager.instance.PlayTouchNormalWeapon ();
	}

	public void GuestSuccessed()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_FixedSound_Success);

		GameObject obj = GuestSuccessedPool.GetObject();

		obj.transform.position = GuestSuccessedPosition.position;

		obj.GetComponent<SuccessedParticle>().Play(GuestSuccessedPool);
	}

	public void CriticalTouchSound()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Cri);
	}

	public void BigSuccessedTouchSound()
	{

	}

	public void MissTouchSound()
	{

	}

	public void CreateBigSuccessedEffect()
    {
		GameObject obj = BigSuccessedPool.GetObject();

		obj.transform.position = BigSuccessedPosition.position;

		obj.GetComponent<ParticlePlay>().Play(BigSuccessedPool);
    }

	public void ResetNormal()
	{
		m_Animator.SetTrigger("bIsNormalRepair");
	}

	public void ResetCritical()
	{
		m_Animator.SetTrigger("bIsCriticalRepair");
	}

	public void ResetBigSuccessed()
	{
		m_Animator.SetTrigger("bIsSuccessedRepair");
	}
}
