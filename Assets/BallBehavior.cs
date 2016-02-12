using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallBehavior : MonoBehaviour {
	AudioSource sound;
	List<TweenTarget> targets = new List<TweenTarget>();
	bool moving = false;
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
	
	}

	public void TweenTo(Vector3 target, float time, float delay){
		StartCoroutine (DoTweenTo (transform.position, target, time, delay));
	}

	public void Hit(int angle){
		bool startAfterQueue = false;
		//if we didn't have any targets before this, queue up the tweens now
		if (targets.Count == 0) {
			startAfterQueue = true;
		}

		//ok this is super fucking temporary. we'll use actual angles in the future
		//top player hits down
		if (angle == 1) {
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, 0f)), 2f / 1f));
		} else if (angle == 2) {
			//bottom player hits up
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, 1f)), 2f / 1f));
		} else if (angle == 3) {
			//top player hits a single reflect (to the left i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0f, 0.5f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, 0f)), 1f / 2f));
		}else if (angle == 4) {
			//bottom player hits a single reflect (to the right i guess)
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (1f, 0.5f)), 1f / 2f));
			targets.Add (new TweenTarget (
				Utils.screenPercToWorldPoint (new Vector2 (0.5f, 1f)), 1f / 2f));
		}


		//queue it up!
		if (startAfterQueue) {
			StartCoroutine (DoTweenTo (transform.position,
			                           targets[0].position,
			                           BPM.controller.getBeatFractionTime( targets[0].fraction ),
			                           BPM.controller.timeUntilNextBeatFraction(1f/1f)-0.05f)); //try to make it sound slightly earlier?
		}
	}

	IEnumerator DoTweenTo(Vector3 start, Vector3 end, float time, float delay){
		if(time < 0.3f) Debug.Log ("TIME LOW:" + time);
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
		Debug.Log("H: "+Time.time);
		sound.Play();

		//assuming we're tweening from the target list, remove this from the list
		targets.RemoveAt (0);
		//tween the next right now one if we have one
		if (targets.Count > 0) {
			StartCoroutine (DoTweenTo (transform.position,
			                           targets[0].position,
			                           BPM.controller.timeUntilNextBeatFraction( targets[0].fraction, true ),
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