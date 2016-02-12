using UnityEngine;
using System.Collections;

public class Utils {

	public static AudioSource AddAudio(GameObject gameObject, AudioClip clip, bool loop, bool playAwake, float vol) {
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		return newAudio;
	}

	public static void shuffle(int[,] ints){
		for (int i = 0; i < ints.GetLength(0); i++) {
			// Knuth shuffle algorithm :: courtesy of Wikipedia :)
			for (int t = 0; t < ints.GetLength(1); t++ ){
				
				int tmp = ints[i,t];
				
				int r = Random.Range(t, ints.GetLength(1));
				
				ints[i,t] = ints[i,r];
				
				ints[i,r] = tmp;
				
			}		
		}
	}

	public static void shuffle(int[] ints){
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < ints.Length; t++ ){
			
			int tmp = ints[t];
			
			int r = Random.Range(t, ints.Length);
			
			ints[t] = ints[r];
			
			ints[r] = tmp;
			
		}		
	}

	//formats a displaytime to the time that's passed in. displays -:-- if -1 is passed in
	public static void displayFormattedTime(float timeIn, TextMesh timemesh, TextMesh timemeshcenti, bool multByScale = false){
		if(timeIn == -1f){
			timemesh.text = "-:--";
			timemeshcenti.text = "";
		}else{
			int minutes = (int)(timeIn / 60f);
			int seconds = (int)(timeIn % 60);
			int centisecond = (int)((timeIn - Mathf.Floor(timeIn)) * 100f);

			timemesh.text = string.Format("{0}:{1:D2}", minutes, seconds);
			timemeshcenti.text = string.Format("{0:D2}", centisecond);
		}
		
		//position centitext
		Vector3 centiPos = timemeshcenti.transform.localPosition;
		if(multByScale){
			centiPos.x = timemesh.GetComponent<Renderer>().bounds.size.x * (1f/timemesh.transform.lossyScale.x);
			//centiPos.x += 0.1f;
		}else{
			centiPos.x = timemesh.GetComponent<Renderer>().bounds.size.x + 0.1f;	
		}
		
		timemeshcenti.transform.localPosition = centiPos;
	}

	//checks if a given vector2 in world space overlaps with a passed in GO that has a 2D collider
	public static bool pointOverlapsCollider2D(Vector2 inputIn, GameObject gameObjIn){
		int layerMask = 1 << gameObjIn.layer;
		Collider2D col = Physics2D.OverlapPoint(inputIn, layerMask);
		if(col == gameObjIn.GetComponent<Collider2D>()){
			return true;
		}else{
			return false;
		}
	}

	//plays and pauses a particle system
	public static void PlayParticleSystem(ParticleSystem tp, bool letPlay){
		if(letPlay)
		{
			if(!tp.isPlaying)
			{
				tp.Play();
			}
		}else{
			if(tp.isPlaying)
			{
				tp.Stop();
			}
		}
	}

	//takes in a vec2 of 0-1 floats from the bottom left to the top right of the screen and returns a world space coord
	//works for orthographic camera only!
	public static Vector3 screenPercToWorldPoint(Vector2 point){
		//THIS IS A CONSTANT WE SET based on the known camera center point
		//we set this here because we'll be using screen shake
		Vector3 CameraDefaultPos = new Vector3 (0f, 5f, -10f);

		float vertExtent = Camera.main.orthographicSize;
		float horzExtent = vertExtent * Screen.width / Screen.height;

		float minX = CameraDefaultPos.x - horzExtent;
		float maxX = CameraDefaultPos.x + horzExtent;
		float minY = CameraDefaultPos.y - vertExtent;
		float maxY = CameraDefaultPos.y + vertExtent;

		Vector3 final = new Vector3 (0f, 0f, 0f);
		final.x = minX + ((maxX - minX) * point.x);
		final.y = minY + ((maxY - minY) * point.y);

		return final;
	}
}
