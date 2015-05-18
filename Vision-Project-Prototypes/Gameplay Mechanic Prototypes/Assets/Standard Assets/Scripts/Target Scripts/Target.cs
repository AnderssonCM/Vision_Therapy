﻿using UnityEngine;
using System;
using System.Collections;
using SQLite4Unity3d;

/// <summary>
/// The Target class is a single object who's main purpose
/// is to be tapped quickly. The base class is meant to be tapped once and dissapear
/// The target does not move unless the Movement class is changed
/// </summary>
public class Target : MonoBehaviour 
{
	
    protected StopWatch timer;
	public float LapTime { get; protected set; }
	public bool IsTapped { get; protected set; }
	public float VelocityAtTap { get; protected set; }
	public float TapPrecision { get; protected set; }
	protected float _timeOut;
	protected float _scale;
	protected float _opacity;

    // Use this for initialization
	public virtual void Start () 
    { 
        timer = new StopWatch();
        timer.start();
		TapPrecision = -1f;
    }
	
	// Update is called once per frame
	public virtual void Update () 
    {
       
	}

    /// <summary>
    /// Specifies the way a target reacts upon being tapped. The target 
    /// automatically records the time it took to tap it.
    /// </summary>
	protected virtual void tapBehavior() { }

    /// <summary>
    /// Checks if the touch is overlapping with the target. Sets data if 
    /// </summary>
    /// <param name="tap">The touch object to be checked against the target</param>
    /// <returns>returns true if the target was tapped/touched</returns>
    public bool checkTouch(Touch tap)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(tap.position);
        Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

        if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos)
                && tap.phase == TouchPhase.Began)
        {
            // because the unit has been tapped, set the variables
            LapTime = timer.lap();
            IsTapped = true;

            // Determine how fast the target was moving when it was hit.


            // determine how close the tap was to the target.
            TapPrecision = Vector2.Distance(touchPos, transform.position);

            // If it is an inherited class, we can call the specific tap behavior
            tapBehavior();

            return true;
        }
        return false;   
    }

    /// <summary>
    /// Checks if the target was almost hit, based on its own size and a given threshold
    /// </summary>
    /// <param name="tap">The touch to test against</param>
    /// <param name="threshold">The threshold of what a near miss is</param>
    /// <returns>Returns true if the hit counts as a near miss.</returns>
    public bool checkNearMiss(Touch tap, float threshold)
    {
        // take the given threshold
        float myThreshold = threshold;

        // add the distance from the middle of the object to the edge of the object
        Transform t = GetComponent<Transform>();
        myThreshold += t.localScale.x / 2;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(tap.position);
        Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

        float dist = Vector2.Distance(GetComponent<Transform>().position, touchPos);
        return dist < threshold;
    }

	/// <summary>
	/// Returns a true value if this object has timed out, false otherwise
	/// </summary>
	/// <returns></returns>
	public bool timedOut()
	{
		if (timer.lap() >= _timeOut)
		{
			return true;
		}
		return false;
	}

	public float TimeOut
	{
		get
		{
			return _timeOut;
		}
		set
		{
			if (value >= 0)
			{
				_timeOut = value;
			}
			else
			{
				throw new System.Exception("Value cannot be negative");
			}
		}
	}

	public float Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			if (value >= 0)
			{
				_scale = value;
				transform.localScale = new Vector2(_scale, _scale);
			}
			else
			{
				throw new System.Exception("Value cannot be negative");
			}
		}
	}

	public float Opacity
	{
		get
		{
			return _opacity;
		}
		set
		{
			if (value >= 0)
			{
				_opacity = value;
				GetComponent<SpriteRenderer>().color = new Color(
					GetComponent<SpriteRenderer>().color.r,
					GetComponent<SpriteRenderer>().color.g,
					GetComponent<SpriteRenderer>().color.b,
					_opacity);
			}
			else
			{
				throw new System.Exception("Value cannot be negative");
			}
		}
	}

    /// <summary>
    /// Packs the data up into a Target data object
    /// </summary>
    /// <param name="manID"></param>
    /// <returns></returns>
    public TargetData packData(string manID)
    {
        var data = new TargetData();
        Movement m = GetComponent<Movement>();
        SpriteRenderer r = GetComponent<SpriteRenderer>();

		data.targetID = Guid.NewGuid().ToString();
        data.managerID = manID;
        data.timeAlive = LapTime;
        data.wasHit = IsTapped;
        data.velocity = m.Velocity;
        if (r != null)
        {
            data.opacity = r.color.a;
            data.blue = r.color.b;
            data.red = r.color.r;
            data.green = r.color.g;
        }
        data.scale = GetComponent<Transform>().localScale.x;

        return data;
    }
}