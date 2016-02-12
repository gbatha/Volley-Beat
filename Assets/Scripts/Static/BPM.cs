using UnityEngine;
using System.Collections;

public class BPM : MonoBehaviour {
	public static BPM controller;

	float bpm = 85f;
	float musicStartTime = 0f;

	float roundUpThreshold = 0.1f;

	void Awake () {
		if(controller == null){
			DontDestroyOnLoad(gameObject);
			controller = this;
		}else if(controller != this){
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// --- GETTERS --- //
	public float getBPM(){
		return bpm;
	}

	//returns how long in seconds this fraction of a beat is. to be called like getBeatFractionTime(1f/4f) for quarter beats
	public float getBeatFractionTime(float fractIn){
		return 60f / bpm * fractIn;
	}

	//returns how long in seconds one beat is
	public float getBeatTime(){
		return 60f / bpm;
	}

	float getCurrentMusicTime(){
		return Time.time - musicStartTime;
	}

	//returns the amount of seconds until the next fraction
	public float timeUntilNextBeatFraction(float fractIn, bool roundUp = false){
		float time = getBeatFractionTime (fractIn) - (getCurrentMusicTime () % getBeatFractionTime (fractIn));

		//if the next beat is like rn, and we want the next sounding one, add the beat length to this time to round up
		if (roundUp && time < roundUpThreshold) {
			time += getBeatFractionTime(fractIn);
		}
		//ERROR this only works for when it's a few ms off
		//when we call 2/1 it rounds to the nearest 2/1, even if that's only 1/2 beat away
		//do another check to see if we're less than 3/4 of the time away, and if we are then use modulo to find the nearest one to time
		return time;
	}

	// --- GETTERS --- //
	public void setBPM(float bpmIn){
		bpm = bpmIn;
	}

	public void setMusicStartTime(float timeIn){
		musicStartTime = timeIn;
	}

	public void startMusicNow(){
		setMusicStartTime (Time.time);
	}
}
