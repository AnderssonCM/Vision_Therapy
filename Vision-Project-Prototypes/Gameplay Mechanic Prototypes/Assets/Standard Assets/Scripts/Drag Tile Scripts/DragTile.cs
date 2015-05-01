﻿using UnityEngine;
using System.Collections;

public class DragTile : MonoBehaviour 
{

	private bool pickedUp = false;
	protected float maxFollowSpeed = 200f;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

			if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos)
				&& touch.phase == TouchPhase.Began)
			{
				pickedUp = true;
				pickUpBehavior();
			}
			else if (pickedUp && touch.phase == TouchPhase.Ended)
			{
				pickedUp = false;
				releaseBehavior();
			}	

			if (pickedUp)
			{
				transform.position = Vector2.MoveTowards(transform.position, touchPos, maxFollowSpeed);
			}
		}
	}

	protected virtual void pickUpBehavior() { }

	protected virtual void releaseBehavior() { }
}