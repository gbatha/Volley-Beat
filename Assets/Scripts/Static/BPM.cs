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
			time += getBeatFractionTime (fractIn);
		} else if (roundUp && time < getBeatFractionTime (fractIn) * 0.75f) {
			//if the nearest fraction is actually significantly closer than the fraction length and we're rounding,
			//round to the nearest fractionLength. (this is to fix 2/1 rounding to the nearest 2/1 which was actually only half a beat away)

			//get how much of the beat length we didn't use
			float remain = getBeatFractionTime (fractIn) - time;
			float roundFract = getBeatFractionTime (1f/4f);
			//round remainder to the nearest quarter beat
			float add = Mathf.Round(remain / roundFract) * roundFract;

			time += add;
		}
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
