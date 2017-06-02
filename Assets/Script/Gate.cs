using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int enemyCount = 0;
		Object[] unitList;
		unitList = FindObjectsOfType(typeof (Attackable)) as Attackable[];
		foreach (Attackable enemy in unitList){
			if (enemy.faction == "enemy") {
				enemyCount++;
			}
		}

		if (enemyCount == 0) {
			Debug.Log ("Open Gate");
		}
	}
}
