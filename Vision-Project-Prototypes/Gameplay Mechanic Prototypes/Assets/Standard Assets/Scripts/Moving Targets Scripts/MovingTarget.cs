﻿using UnityEngine;
using System.Collections;

public class MovingTarget : Target 
{

	private float timeout;
	private float scale;
	private float opacity;

	GameManager gameMan;

	protected override void Start()
	{
		base.Start();

		gameMan = GameObject.Find("GameManager").GetComponent<GameManager>();
		
		timeout = gameMan.GetComponent<DifficultySettings>().targetTimeout;

		scale = gameMan.GetComponent<DifficultySettings>().targetScale;
		transform.localScale = new Vector2(scale, scale);

		opacity = gameMan.GetComponent<DifficultySettings>().targetOpacity;
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
	}

	// Update is called once per frame
	void Update () 
	{
		if (timer.lap() >= timeout && gameMan.getState() == GameManager.state.PLAY)
		{
			gameObject.SetActive(false);
		}
	}

	protected override void tapBehavior()
	{
		gameObject.SetActive(false);
	}
}
