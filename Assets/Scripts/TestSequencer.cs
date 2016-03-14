using UnityEngine;
using System.Collections;

public class TestSequencer : MonoBehaviour {
	[SerializeField]
	GameObject prefab;

	MeshRenderer renderer;
	bool playing = false;
	// Use this for initialization
	void Start () {
		renderer = GetComponent<MeshRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			playing = !playing;
			if(playing){
				GetComponent<AudioSource>().Play();
				BPM.controller.startMusicNow();
				StartCoroutine(delayInvertRender(BPM.controller.timeUntilNextBeatFraction(1f/1f)));
				Debug.Log(BPM.controller.timeUntilNextBeatFraction(1f/1f));
			}else{
				GetComponent<AudioSource>().Stop();
			}
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			StartCoroutine(delaySpawnPrefab(BPM.controller.timeUntilNextBeatFraction(1f/2f)));
//			GameObject.Find("Ball").GetComponent<BallBehavior>().TweenTo(Utils.screenPercToWorldPoint(new Vector2(0.5f, 1f)), BPM.controller.getBeatTime(), BPM.controller.timeUntilNextBeatFraction(1f/2f));

//			StartCoroutine(delaySpawnPrefab(BPM.controller.timeUntilNextBeatFraction(1f/4f) + BPM.controller.getBeatFractionTime(1f/4f)));
//			StartCoroutine(delaySpawnPrefab(BPM.controller.timeUntilNextBeatFraction(1f/4f) + BPM.controller.getBeatFractionTime(1f/4f)*2f));
		}


		if (Input.GetKeyDown (KeyCode.Z))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (1,0);
		if (Input.GetKeyDown (KeyCode.A))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (2,0);
		if (Input.GetKeyDown (KeyCode.S))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (3,0);

		if (Input.GetKeyDown (KeyCode.UpArrow))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (4,1);
		if (Input.GetKeyDown (KeyCode.RightArrow))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (5,1);
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			GameObject.Find ("Ball").GetComponent<BallBehavior> ().Hit (6,1);
	}

	IEnumerator delaySpawnPrefab(float delay){
		yield return new WaitForSeconds(delay);
		GameObject newObj = Instantiate (prefab);
		newObj.transform.parent = transform;
		newObj.transform.position = Random.insideUnitSphere * 5f;
		newObj.GetComponent<MeshRenderer> ().material.color = new Color (Random.value, Random.value, 1f);
	}

	IEnumerator delayInvertRender(float delay){
		yield return new WaitForSeconds(delay);
		if (playing) {
//			Debug.Log("I: "+Time.time);
			renderer.enabled = !renderer.enabled;
			StartCoroutine(delayInvertRender(BPM.controller.timeUntilNextBeatFraction(1f/1f)));
		}
	}
}
