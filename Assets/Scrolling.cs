using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour 
{

	private float m_fSpeed = 0.5f;
	private Vector3 startPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 offset = new Vector2 (Time.time * 0.3f, 0);
		GetComponent<Renderer> ().material.mainTextureOffset = offset;
	}
}
