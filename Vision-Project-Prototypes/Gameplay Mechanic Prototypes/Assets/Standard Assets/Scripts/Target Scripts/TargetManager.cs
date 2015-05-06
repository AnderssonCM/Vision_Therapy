﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SQLite4Unity3d;

/// <summary>
/// The Target manager controls a group of Targets, most likely all targets in a specific level
/// The class relies on a Game manager type class to provide it with tagets in order to keep coupling low
/// </summary>
public class TargetManager : MonoBehaviour 
{
    private int hits, misses, nearMisses;
    public Movement moveType;
	private ArrayList targets;
    public float nearMissThreshold = 5f;
    private string manID;
	
	// Use this for initialization
	void Start () 
	{
        hits = 0;
        misses = 0;
        nearMisses = 0;
		targets = new ArrayList();
        manID = Guid.NewGuid().ToString();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Touch[] taps = Input.touches;
        if (Input.touchCount > 0)
        {
            foreach (Touch tap in taps)
            {
                if (tap.phase == TouchPhase.Began)
                {
                    bool hit = false;

                    foreach (Target target in targets)
                    {
                        if (target.checkTouch(tap))
                        {
                            hit = true;
                            break;
                        } else {
                            if (target.checkNearMiss(tap, nearMissThreshold))
                            {
                                nearMissAnimation(target);
                                ++nearMisses;
                            }
                        }
                    }
                    if (hit)
                    {
                        hits++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds a target to the list of managed targets.
    /// </summary>
    /// <param name="t">The fully prepared target to be added</param>
	public void addTarget(Target t)
	{
        if (t != null)
        {
            targets.Add(t);
        }
	}

    /// <summary>
    /// Retrieve the average time between target creation and tap
    /// </summary>
    /// <returns>The average hit time for all targets</returns>
	public float getAverage()
	{
		float average = 0f;
		
		foreach (Target t in targets)
		{
			average += t.lapTime;
		}

		return average / targets.Count;
	}

    /// <summary>
    /// Retrieve an ArrayList of all targets
    /// </summary>
    /// <returns></returns>
	public ArrayList getTargets()
	{
		return targets;
	}

    /// <summary>
    /// The total number of targets managed by the TargetManager
    /// </summary>
    /// <returns></returns>
	public int getNumberOfTargets()
	{
		return targets.Count;
    }

    /// <summary>
    /// The animation to be played when a near miss happens
    /// can trigger on multiple objects
    /// </summary>
    /// <param name="target">The target that was almost hit.</param>
    public void nearMissAnimation(Target target)
    {

    }

    /// <summary>
    /// Pack all the data into a IEnumerator to 
    /// </summary>
    /// <returns></returns>
    public IEnumerable packTargetData()
    {
        if (targets.Count == 0) 
        {
            return null;
        }
        ArrayList data = new ArrayList();
        foreach (Target t in targets)
        {
            data.Add(t.packData(manID));
        }

        return data;
    }

    public TargetManData getData(string gameManID)
    {
        TargetManData data = new TargetManData();

        data.targetManID = manID;
        data.GameManID = gameManID;
        data.totalTargets = targets.Count;
        data.hits = hits;
        data.misses = misses;
        data.nearMisses = nearMisses;

        return null;
    }
}


