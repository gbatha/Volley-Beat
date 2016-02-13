using UnityEngine;
using System.Collections;

public class PositionToScreenPerc : MonoBehaviour {
	public Vector2 targetPosition = new Vector2(0f,0f);

	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;
		Vector3 targetPos = Utils.screenPercToWorldPoint (targetPosition);
		pos.x = targetPos.x;
		pos.y = targetPos.y;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
