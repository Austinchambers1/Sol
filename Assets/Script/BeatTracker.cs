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
	public float offset = 0f;
	float QuarterNoteInterval;
	float EigthNote;
	float SixteenthNote;
	public float LastTime = 0.0f;
	public float currTime = 0.0f;
	public float QuarterNoteActionTime = 0.0f;
	public OnBeatEventHandler onBeat;
	public int beatNo;
	bool nextFrame;
	List<Beats> allBeats = new List<Beats>();


	// Use this for initialization
	void Start () {
		beatNo = 0;
		QuarterNoteInterval = Tempo / 60.0f / 4.0f;
		EigthNote = QuarterNoteInterval / 4.0f;
		SixteenthNote = EigthNote / 4.0f;
	}

	public void addBeatObj(Beats obj) {
		allBeats.Add (obj);
	}
	public void removeBeatObj(Beats obj){
		allBeats.Remove (obj);
	}
	
	// Update is called once per frame
	void Update () {
		float diff = QuarterNoteActionTime - (Time.time + offset);
		if (diff < Time.deltaTime / 2.0f || nextFrame) {
			beatNo++;
			if (beatNo > 4) {
				beatNo = 1;
			}
			QuarterNoteActionTime += QuarterNoteInterval;
			float randomx = Random.value;
			Vector3 temp = this.gameObject.transform.position;
			temp.x = Random.value;
			transform.position = temp;
			foreach (Beats b in allBeats) { 
				b.onBeat (beatNo);
			}
			//BroadcastMessage ("reactToBeat", beatNo);
			//Debug.Log (beatNo);
			nextFrame = false;
		} else if (diff < Time.deltaTime) {
			nextFrame = true;
		}
	}
}
