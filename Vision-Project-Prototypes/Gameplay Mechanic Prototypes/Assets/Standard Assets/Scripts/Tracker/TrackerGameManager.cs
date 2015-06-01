﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Game manager for tracker game
/// </summary>
public class TrackerGameManager : Mechanic 
{
	// The range that the targets' speed can be
	private float minChangeTime;
	private float maxChangeTime;
	// Number of targets that the user needs to track
	private int numberOfTrackTargets;
	// Number of dummy targets in the scene
	private int numberOfDummyTargets;
	// The amount of time that targets are allowed to move around
	private float shuffleTime;
	// The time between tracks spawning, dummies spawning, and the
	// game starting
	private float startUpTime;
	// The max speed of the targets
	private float targetSpeed;
	// The transperancy of the background
	private float backgroundOpacity = 1f;

	private TrackManager trackMan;

	private TextMesh score;
	private GameObject winText;

	private Background background;

	// Prefabs for the dummy and track game objects. These prefabs
	// are set in the Unity scene by dragging the appropriate prefab
	// into either the trackPrefab or dummyPrefab field in the object
	// with this script attached to it.
	public Target trackPrefab;
	public Target dummyPrefab;

	// The current state of the Tracker game
	public static TrackerState CurrentState { get; private set; }

	// The different states for the Tracker game
	public enum TrackerState
	{
		STARTUP,
		PLAY,
		PAUSE,
		WIN,
		LOSE
	}
	
	// Use this for initialization
	void Start () 
	{
		base.Start();
		
		targetScale = TrackerSettings.targetScale;
		targetOpacity = TrackerSettings.targetOpacity;
		minChangeTime = TrackerSettings.minChangeTime;
		maxChangeTime = TrackerSettings.maxChangeTime;
		numberOfTrackTargets = TrackerSettings.numberOfTrackTargets;
		numberOfDummyTargets = TrackerSettings.numberOfDummyTargets;
		shuffleTime = TrackerSettings.shuffleTime;
		startUpTime = TrackerSettings.startUpTime;
		targetSpeed = TrackerSettings.targetSpeed;
		backgroundOpacity = TrackerSettings.backgroundOpacity;

		trackMan = GetComponent<TrackManager>();
		targetMan = trackMan;

		score = GameObject.Find("Score").GetComponent<TextMesh>();

		winText = GameObject.Find("WinText");

		background = GameObject.Find("Background").GetComponent<Background>();
		background.Opacity = backgroundOpacity;

		mechanicType = "Tracker";

		CurrentState = TrackerState.STARTUP;

		StartCoroutine(startUp(startUpTime));
    }
	
	// Update is called once per frame
	void Update () 
	{
		switch (CurrentState)
		{
			case TrackerState.STARTUP:
				break;
			
			case TrackerState.PLAY:
				playBehavior();
				break;

			case TrackerState.WIN:
				winBehavior();
				break;
		}
	}

	protected override void playBehavior()
	{
		// Displays the user's current score
		score.text = trackMan.SuccessfulHits + " / " + numberOfTrackTargets + " targets found";
		
		// Transition to WIN state if the user finds all of the track objects
		if (trackMan.SuccessfulHits == numberOfTrackTargets)
		{
			CurrentState = TrackerState.WIN;
		}
	}

	protected override void winBehavior()
	{
		// Clear the screen
		trackMan.disableAllTargets();
		// Move the win message to the center of the screen
		winText.transform.position = Vector2.zero;

		base.winBehavior();
	}

    private void spawnTrack()
    {
        // Instantiate the track prefab and set attributes
        Target track = Instantiate(trackPrefab) as Target;
		track.Scale = targetScale;
		track.Opacity = targetOpacity;
		track.gameObject.GetComponent<RandomStraightMove>().MinimumChangeTime = minChangeTime;
		track.gameObject.GetComponent<RandomStraightMove>().MaximumChangeTime = maxChangeTime;
		track.gameObject.GetComponent<RandomStraightMove>().Speed = targetSpeed;

        // Generate random x position within camera view
        float worldHeight = Camera.main.orthographicSize - track.Scale / 2;
		float y = Random.Range(-worldHeight, worldHeight);

		// Generate random y position within camera view
		float worldWidth = (Camera.main.orthographicSize / Camera.main.aspect) - track.Scale / 2;
		float x = Random.Range(-worldWidth, worldWidth);

        // Position track object
        track.transform.position = new Vector2(x, y);

        // Add track to target manager
		trackMan.addTarget(track);
    }

	private void spawnDummy()
	{
		// Instantiate the dummy prefab and set attributes
		Target dummy = Instantiate(dummyPrefab) as Target;
		dummy.Scale = targetScale;
		dummy.Opacity = targetOpacity;
		dummy.gameObject.GetComponent<RandomStraightMove>().MinimumChangeTime = minChangeTime;
		dummy.gameObject.GetComponent<RandomStraightMove>().MaximumChangeTime = maxChangeTime;
		dummy.gameObject.GetComponent<RandomStraightMove>().Speed = targetSpeed;

		// Generate random x position within camera view
		float worldHeight = Camera.main.orthographicSize - dummy.Scale / 2;
		float y = Random.Range(-worldHeight, worldHeight);

		// Generate random y position within camera view
		float worldWidth = (Camera.main.orthographicSize * Camera.main.aspect) - dummy.Scale / 2;
		float x = Random.Range(-worldWidth, worldWidth);

		// Position dummy object
		dummy.transform.position = new Vector2(x, y);

		// Add dummy to target manager
		trackMan.addTarget(dummy);
	}

	IEnumerator startUp(float waitTime)
	{
		// Position walls
		float height = Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;

		GameObject leftWall = GameObject.Find("LeftWall");
		leftWall.transform.position = new Vector2(-width - leftWall.transform.localScale.x / 2, 0f);
		leftWall.transform.localScale = new Vector2(1f, height * 2);

		GameObject rightWall = GameObject.Find("RightWall");
		rightWall.transform.position = new Vector2(width + rightWall.transform.localScale.x / 2, 0f);
		rightWall.transform.localScale = new Vector2(1f, height * 2);

		GameObject topWall = GameObject.Find("TopWall");
		topWall.transform.position = new Vector2(0f, height + topWall.transform.localScale.y / 2);
		topWall.transform.localScale = new Vector2(width * 2, 1f);

		GameObject bottomWall = GameObject.Find("BottomWall");
		bottomWall.transform.position = new Vector2(0f, -height - bottomWall.transform.localScale.y / 2);
		bottomWall.transform.localScale = new Vector2(width * 2, 1f);

		// Spawn track objects
		for (int i = 0; i < numberOfTrackTargets; i++)
		{
			spawnTrack();
		}

		// Freeze track objects
		trackMan.freezeTargets();
		
		yield return new WaitForSeconds(waitTime);
		
		// Spawn dummies
		for (int i = 0; i < numberOfDummyTargets; i++)
		{
			spawnDummy();
		}

		// Freeze dummies
		trackMan.freezeTargets();
		
		yield return new WaitForSeconds(waitTime);

		// Unfreeze all targets
		trackMan.unfreezeTargets();

		// Let the targets shuffle around
		yield return new WaitForSeconds(shuffleTime);

		// Freeze targets so user can select the right ones
		trackMan.freezeTargets();

		// Position the score text
		score.transform.position = new Vector3(0f, Camera.main.orthographicSize
			- score.transform.localScale.y, score.transform.position.z);

		gameTime.start();
		CurrentState = TrackerState.PLAY;
	}
}
