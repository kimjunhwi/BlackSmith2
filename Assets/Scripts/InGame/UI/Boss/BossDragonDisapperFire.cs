using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDragonDisapperFire : MonoBehaviour 
{

	public Animator BossDragonDisappearAnimator;					

	void Awake()
	{
		BossDragonDisappearAnimator = GetComponent<Animator> ();
		gameObject.SetActive (false);
	}
		
}
