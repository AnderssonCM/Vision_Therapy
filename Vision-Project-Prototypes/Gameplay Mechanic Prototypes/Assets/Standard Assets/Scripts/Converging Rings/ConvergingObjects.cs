﻿using UnityEngine;
using System.Collections;

public class ConvergingObjects : MonoBehaviour 
{

	// List of boomerangs associated with this convergence point
	private ArrayList boomerangs;
	// The time it takes for all of the boomerangs to converge
	private float convergeTime = 0f;
	// The scale of the each boomerang
	private float scale = 1f;
	// The point where all of the boomerangs converge
	private Vector2 centerPoint;

	// Use this for initialization
	void Start () 
	{
		boomerangs = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void converge()
	{
		// If the first boomerang reaches its max distance, turn
		// all of them boomerangs around and go the other way
		if (Vector2.Distance(((Boomerang)boomerangs[0]).transform.position, centerPoint)
			>= ((Boomerang)boomerangs[0]).distance)
		{
			turnAround();
		}
		
		// Move the boomerangs towards the center point
		foreach (Boomerang b in boomerangs)
		{
			b.transform.position += b.transform.up * b.speed * Time.smoothDeltaTime;
		}
	}

	private void turnAround()
	{
		// Turn all of the boomerangs around to face the center point
		foreach (Boomerang b in boomerangs)
		{
			Vector2 convergeDirection = centerPoint - (Vector2)b.transform.position;
			b.transform.LookAt(b.transform.position + new Vector3(0, 0, 1), convergeDirection);
		}
	}

	public void set(int numberOfObjects, Vector2 centerPoint, float time)
	{
		// Create and position the center point
		GameObject center = Instantiate(Resources.Load("Converging Rings Prefabs/Center", typeof(GameObject))) as GameObject;
		center.transform.position = centerPoint;
		this.centerPoint = centerPoint;

		// Create the amount of boomerangs specified by numberOfObjects
		for (int i = 0; i < numberOfObjects; i++)
		{
			// Instantiate boomerang
			Boomerang boomerang = Instantiate(Resources.Load("Converging Rings Prefabs/Ring", typeof(Boomerang))) as Boomerang;
			
			convergeTime = time;

			// Find the x coordinate of the boomerang relative to the center point
			float height = Camera.main.orthographicSize - Mathf.Abs(center.transform.position.y - scale / 2);
			float y = Random.Range(center.transform.position.y - height, center.transform.position.y + height);

			// Find the y coordinate of the boomerang relative to the center point
			float width = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(height, 2) - Mathf.Pow(y, 2)));
			float x = Random.Range(-width, width);

			// Position the boomerang
			boomerang.transform.position = new Vector2(x, y);

			// Turn the point to face the center point
			Vector2 convergeDirection = centerPoint - (Vector2)boomerang.transform.position;
			boomerang.transform.LookAt(boomerang.transform.position + new Vector3(0, 0, 1), convergeDirection);

			// The distance between this boomerang and the center point
			float distance = Vector2.Distance(boomerang.transform.position, centerPoint);

			// The speed that this boomerang needs to travel to reach the center point
			// in time
			float speed = distance / time;

			// Set variables in this boomerang
			boomerang.distance = distance;
			boomerang.speed = speed;

			boomerangs.Add(boomerang);
		}
	}
}
