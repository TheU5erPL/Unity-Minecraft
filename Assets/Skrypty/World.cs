using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	
	public int ch_width = 20;
	public int ch_height = 20;
	public int randomSeed = 0;
	
	public static World activeWorld;
	
	// inicjalizacja
	void Awake () {
		activeWorld = this;
		if(randomSeed == 0)
			randomSeed = Random.Range(0, int.MaxValue);
	}
	
}