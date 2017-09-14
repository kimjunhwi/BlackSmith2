using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour {

	private float m_fSpeed = 0.5f;
	private Renderer render;
	// Use this for initialization
	void Start () {
		render = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 offset = new Vector2 (Time.time * m_fSpeed, 0);

		render.material.mainTextureOffset = offset;

	}
}
