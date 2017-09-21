using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalTouchParticle : MonoBehaviour {

    private Animator anim;
	private RectTransform rectTransform;

	private const float m_fWidth = 450;
	private const float m_fHeight = 450;

    private void Awake()
    {
        anim = GetComponent<Animator>();
		rectTransform = GetComponent<RectTransform> ();
    }

    public void Play()
    {
		int nRandomRotation = Random.Range (0, 360 + 1);

		transform.rotation =  Quaternion.Euler(0, 0, nRandomRotation);

		rectTransform.sizeDelta = new Vector2 (m_fWidth, m_fHeight);

        anim.SetTrigger("bIsTouch");

        StartCoroutine(StartDisappearAfter(2f));
    }

    IEnumerator StartDisappearAfter(float time)
    {
        yield return new WaitForSeconds(time);
        
		CriticalTouchPool.Instance.ReturnObject(gameObject);
    }

    public void ResetAnimation()
    {
        anim.SetTrigger("bIsTouch");
    }
}
