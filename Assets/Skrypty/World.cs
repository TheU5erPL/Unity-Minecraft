using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	
	public int ch_width = 50;
	public int ch_height = 20;
	public int randomSeed = 0;
	
	public float viewRange = 100;
	public Chunk chunkPrefab;
	
	public static World activeWorld;
	
	// inicjalizacja
	void Awake () {
		activeWorld = this;
		if(randomSeed == 0)
			randomSeed = Random.Range(0, int.MaxValue);
	}
	
	void Update () {
		for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x+= ch_width)
		{
			
			for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += ch_width)
			{
				Vector3 pos = new Vector3(x, 0, z);
				pos.x = Mathf.Floor (pos.x / (float)ch_width) * ch_width;
				pos.z = Mathf.Floor (pos.z / (float)ch_width) * ch_width;
				
				Chunk chunk = Chunk.GetChunk(pos);
				if (chunk != null) continue;
				
				chunk = (Chunk)Instantiate (chunkPrefab, pos, Quaternion.identity);
				
			}
			
		}
	}
	
} 