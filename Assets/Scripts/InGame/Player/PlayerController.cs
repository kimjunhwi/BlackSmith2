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
        //Debug.Log("NormalHit");

        GameObject obj = NormalRepairPool.Instance.GetObject();

		obj.transform.position = normalPosition.position;

        obj.GetComponent<NormalRepairParticle>().Play();
    }

    public void CreateCriticalEffect()
    {
        Debug.Log("CriticalHit");

        GameObject obj = CriticalRepairPool.Instance.GetObject();

		obj.transform.position = criticalPosition.position;

        obj.GetComponent<CriticalRepairParticle>().Play();
    }

	public void NormalTouchSound()
	{
		SoundManager.instance.PlayTouchNormalWeapon ();
	}

	public void GuestSuccessed()
	{

		GameObject obj = GuestSuccessedPool.GetObject();

		obj.transform.position = GuestSuccessedPosition.position;

		obj.GetComponent<SuccessedParticle>().Play(GuestSuccessedPool);
	}

	public void CriticalTouchSound()
	{

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
