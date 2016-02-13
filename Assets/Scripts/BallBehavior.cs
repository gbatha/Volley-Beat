using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallBehavior : MonoBehaviour {
	AudioSource sound;
	List<TweenTarget> targets = new List<TweenTarget>();
	bool moving = false;
	public bool CPU = true;

	//timing variables for hits
	int playerTurn = 1; //top player is 0, bottom is 1. 1 always serves so we default to that
	float nextHitTime = 0f; //the exact moment the ball will land on the target. calc the window from this
	bool hitWindow = false;
	bool delayNextHit = true; //this is for if we hit late, set false to trigger the hit immediately

	//positional variables
	float[] goalY = { 0.9f,0.1f }; //in screen percentages

	[SerializeField]
	MeshRenderer[] goalRenderers = new MeshRenderer[2];

	// Use this for initialization
	void Start () {
		sound = GetComponent<AudioSource> ();

		//math for calculating angles
		Vector2 from = new Vector2(0.5f, 1f);
		Vector2 to = new Vector2(1f, 1f);
		
		float angle = Mathf.Atan2(to.y-from.y, to.x-from.x)*180 / Mathf.PI;
		Debug.Log (angle);
	}
	
	// Update is called once per frame
	void Update () {
		//if hit window is set to false
		//but we have a hit time
		//and the current time is within the window frame
		if (!hitWindow && nextHitTime > 0f &&
			Time.time >= nextHitTime - BPM.controller.getBeatFractionTime (1f / 8f) &&
			Time.time <= nextHitTime + BPM.controller.getBeatFractionTime (1f / 8f)) {
			hitWindow = true;
			goalRenderers[playerTurn].material.color = Color.green;
		} else if(hitWindow &&
		          (Time.time < nextHitTime - BPM.controller.getBeatFractionTime (1f / 8f) ||
		          Time.time > nextHitTime + BPM.controller.getBeatFractionTime (1f / 8f))) {
			//otherwise hit window is true and shouldn't be anymore
			hitWindow = false;
			goalRenderers[playerTurn].material.color = Color.white;
			if(Time.time > nextHitTime + BPM.controller.getBeatFractionTime (1f / 8f)){
				goalRenderers[playerTurn].material.color = Color.red;
			}
		}
	}

	public void TweenTo(Vector3 target, float time, float delay){
		StartCoroutine (DoTweenTo (transform.position, target, time, delay));
	}

	public void Hit(int angle, int player){
		//if this isn't the right player to be hitting
		//or it is the right player but we're not inside the hit window
		//QUIT!
		if (player != playerTurn || ( nextHitTime > 0f && !hitWindow)) {
			return;
		}
		bool startAfterQueue = false;
		//if we didn't have any targets before this, queue up the tweens now
		if (targets.Count == 0) {
			startAfterQueue = true;
		}

		//check if we should quantize this hit or trigger it immediately upon adding
		if (nextHitTime > 0f && Time.time > nextHitTime) {
//			delayNextHit = false;
			Debug.Log("TRIGGER HIT NOW");
		} else {
			delayNextHit = true;
		}

		float timeOfAnimation = 0f; //the cumulative time the ball will take to reach its target once it's hit
		//ok this is super fucking temporary. we'll use actual angles in the future
		//top player hits down
		if (angle == 1) {
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[1])), 2f / 1f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(2f/1f);
		} else if (angle == 4) {
			//bottom player hits up
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[0])), 2f / 1f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(2f/1f);
		} else if (angle == 2) {
			//top player hits a single reflect (to the left i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0f, 0.5f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[1])), 1f / 2f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(1f/2f + 1f/2f);
		}else if (angle == 5) {
			//bottom player hits a single reflect (to the right i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (1f, 0.5f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[0])), 1f / 2f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(1f/2f + 1f/2f);
		}else if (angle == 3) {
			//top player hits a double reflect (to the right i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (1f, 0.6333f)), 1f / 4f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0f, 0.3666f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[1])), 1f / 4f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(1f/4f + 1f/2f + 1f/4f);
		}else if (angle == 6) {
			//bottom player hits a double reflect (to the left i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0f, 0.3666f)), 1f / 4f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (1f, 0.6333f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, goalY[0])), 1f / 4f));

			timeOfAnimation = BPM.controller.getBeatFractionTime(1f/4f + 1f/2f + 1f/4f);
		}


		//this is a serve, so start at the next beat!
		if (startAfterQueue) {
			float delay = BPM.controller.timeUntilNextBeatFraction (1f / 1f) - 0.05f;
			StartCoroutine (DoTweenTo (transform.position,
			                           targets [0].position,
			                           BPM.controller.getBeatFractionTime (targets [0].fraction),
			                           delay)); //try to make it sound slightly earlier?

			nextHitTime = Time.time + delay + timeOfAnimation;
			//set up computer player to play
			if(CPU && player == 1){
				StartCoroutine(delayHit(Random.Range(1,4), 0, delay + timeOfAnimation - BPM.controller.getBeatFractionTime(1f/32f)));
			}
		} else {
			//this was a hit, so time it as if it were hit exactly at the calculated (quantized) hit time
			nextHitTime = nextHitTime + timeOfAnimation;
			//set up computer player to play
			if(CPU && player == 1){
				StartCoroutine(delayHit(Random.Range(1,4), 0, timeOfAnimation - BPM.controller.getBeatFractionTime(1f/32f)));
			}
		}

		//set up window variables
		goalRenderers[playerTurn].material.color = Color.white;
		playerTurn = (player == 0) ? 1 : 0;
	}

	IEnumerator delayHit(int angle, int player, float delay){
		yield return new WaitForSeconds (delay);
		Hit (angle, player);
	}

	IEnumerator DoTweenTo(Vector3 start, Vector3 end, float time, float delay){
//		if(time < 0.3f) Debug.Log ("TIME LOW:" + time);
		if (delay > 0f) {
			yield return new WaitForSeconds(delay);
			sound.Play();
		}

		float t = 0.0f;
		float duration = time;
		while (t < duration) {
			t += Time.deltaTime;
			float p = Mathf.Clamp01 (t / duration);
			transform.position = Vector3.Lerp(start, end, p);
			yield return null;
		}
//		Debug.Log("H: "+Time.time);
		sound.Play();

		//assuming we're tweening from the target list, remove this from the list
		targets.RemoveAt (0);
		//tween the next right now one if we have one
		if (targets.Count > 0) {
			float delayTime = BPM.controller.timeUntilNextBeatFraction (targets [0].fraction, true);
			//if we want to trigger this hit immediately, set delay to 0 and reset the delayNextHit flag to true
			if(!delayNextHit){
				delayTime = 0f;
				delayNextHit = true;
			}
			StartCoroutine (DoTweenTo (transform.position,
			                           targets[0].position,
			                           delayTime,
			                           0f));
		}
	}
}

//struct for tween targets
public struct TweenTarget {
	public Vector3 position;
	public float fraction;
	public float delay;

	public TweenTarget(Vector3 posIn, float fraction, float delay){
		this.position = posIn;
		this.fraction = fraction;
		this.delay = delay;
	}
	public TweenTarget(Vector3 posIn, float fraction){
		this.position = posIn;
		this.fraction = fraction;
		this.delay = 0f;
	}
}