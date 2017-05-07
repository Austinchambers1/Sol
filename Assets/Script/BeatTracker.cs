using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTracker : MonoBehaviour {

	[System.Serializable]
	public class OnBeatEventHandler : UnityEngine.Events.UnityEvent
	{

	}
	public float Tempo;
	public float timePassed = 0f;
	float QuarterNoteInterval;
	float EigthNote;
	float SixteenthNote;
	public float LastTime = 0.0f;
	public float currTime = 0.0f;
	public float QuarterNoteActionTime = 0.0f;
	public OnBeatEventHandler onBeat;
	int beatNo;


	// Use this for initialization
	void Start () {
		beatNo = 1;
		QuarterNoteInterval = Tempo / 60.0f / 4.0f;
		EigthNote = QuarterNoteInterval / 4.0f;
		SixteenthNote = EigthNote / 4.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > QuarterNoteActionTime) {
			beatNo++;
			if (beatNo > 4) {
				beatNo = 1;
			}
			QuarterNoteActionTime += QuarterNoteInterval;
			float randomx = Random.value;
			Vector3 temp = this.gameObject.transform.position;
			temp.x = Random.value;
			transform.position = temp;
			BroadcastMessage ("reactToBeat", beatNo);
			Debug.Log (beatNo);
		}
	}



}
