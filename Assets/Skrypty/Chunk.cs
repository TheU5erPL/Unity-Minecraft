using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimplexNoise;

[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class Chunk : MonoBehaviour {
	
	
	public Mesh v_Mesh;
	protected MeshCollider meshCollider;
	protected MeshRenderer meshRenderer;
	protected MeshFilter meshFilter;
	public byte [,,] map;

	public static int chunkWidth {
		get { return World.activeWorld.ch_width; }
	}
	
	public static int chunkHeight {
		get { return World.activeWorld.ch_height; }
	}

	public static List<Chunk> chunks = new List<Chunk>();
	
	void Start () {
		
		chunks.Add (this);
		
		meshCollider = GetComponent<MeshCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		
		map = new byte[chunkWidth, chunkHeight, chunkWidth];
		
		Random.seed = World.activeWorld.randomSeed;
		Vector3 offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
		
		for(int x = 0; x < chunkWidth; x++)
		{
			float perlinX = Mathf.Abs((float)(x + transform.position.x + offset.x) / 30);
			for (int y = 0; y < chunkHeight; y++)
			{
				float perlinY = Mathf.Abs((float)(y + transform.position.y + offset.y) / 50);
				for (int z = 0; z < chunkWidth; z++)
				{
					float perlinZ = Mathf.Abs((float)(z +transform.position.z + offset.z) / 30);
					float perlin = Noise.Generate(perlinX,perlinY,perlinZ);
					
					perlin += (10f - (float)y) / 10;
					
					if( perlin > 0.05f)
						map[x,y,z] = 1;
				}
			}
		}
		StartCoroutine(GenerateMesh());//StartCoroutine(GenerateMesh());
	}
	
	public virtual IEnumerator GenerateMesh() { //IEnumerator
		v_Mesh = new Mesh();
		
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> UVs = new List<Vector2>();
		List<int> tris = new List<int>();
		
		for(int x = 0; x < chunkWidth; x++)
		{
			for(int y = 0; y < chunkHeight; y++)
			{
				for (int z = 0; z < chunkWidth; z++)
				{
					if(map[x, y, z] == 0)
						continue;
					byte block = map[x,y,z];
					
					// lewa sciana
					if( Visible (x - 1, y, z))
						GenerateFace(block, new Vector3(x,y,z), Vector3.up, Vector3.forward, false, verts, UVs, tris);
					
					// prawa sciana
					if( Visible (x + 1, y, z))
						GenerateFace(block, new Vector3(x + 1,y,z), Vector3.up, Vector3.forward, true, verts, UVs, tris);
					
					// tyl
					if ( Visible (x, y, z - 1))
						GenerateFace(block, new Vector3(x,y,z), Vector3.up, Vector3.right, true, verts, UVs, tris);
					
					// przod
					if ( Visible (x, y, z + 1))
						GenerateFace(block, new Vector3(x,y,z + 1), Vector3.up, Vector3.right, false, verts, UVs, tris);
					
					// podloga
					if ( Visible (x, y - 1, z))
						GenerateFace(block, new Vector3(x,y,z), Vector3.forward, Vector3.right, false, verts, UVs, tris);
					
					// sufit
					if ( Visible (x, y + 1, z))
						GenerateFace(block, new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, true, verts, UVs, tris);
				}
			}
		}
		
		v_Mesh.vertices = verts.ToArray();
		v_Mesh.uv = UVs.ToArray();
		v_Mesh.triangles = tris.ToArray();
		v_Mesh.RecalculateBounds();
		v_Mesh.RecalculateNormals();
		
		meshFilter.mesh = v_Mesh;
		meshCollider.sharedMesh = v_Mesh;

		yield return new WaitForEndOfFrame();

	}
	
	public virtual void GenerateFace(byte block, Vector3 corner, Vector3 up, Vector3 right, bool rev, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
	{
		int index = verts.Count;
		verts.Add (corner);
		verts.Add (corner + up);
		verts.Add (corner + up + right);
		verts.Add (corner + right);
		
		uvs.Add (new Vector2(0,0));
		uvs.Add (new Vector2(0,1));
		uvs.Add (new Vector2(1,1));
		uvs.Add (new Vector2(1,0));
		
		if(rev)
		{
			tris.Add (index + 0);
			tris.Add (index + 1);
			tris.Add (index + 2);
			tris.Add (index + 2);
			tris.Add (index + 3);
			tris.Add (index + 0);
		} else {
			tris.Add (index + 1);
			tris.Add (index + 0);
			tris.Add (index + 2);
			tris.Add (index + 3);
			tris.Add (index + 2);
			tris.Add (index + 0);
		}
	}
	
	public virtual bool Visible (int x, int y, int z)
	{
		byte block = GetBlock(x,y,z);
		switch (block)
		{
		default:
		case 0:
			return true;
		case 1:
			return false;
		}
	}
	
	public virtual byte GetBlock(int x, int y, int z)
	{
		if ( 
		    (x >= chunkWidth) ||
		    (y >= chunkHeight) ||
		    (z >= chunkWidth) ||
		    ( x < 0) || ( y < 0) || (z < 0)
		    )
			return 0;
		return map[x,y,z];
	}

	public static Chunk GetChunk(Vector3 position)
	{
		for (int i = 0; i < chunks.Count; i++)
		{
			Vector3 chunkPos = chunks[i].transform.position;
			if ( ( position.x < chunkPos.x) || (position.z < chunkPos.z) ||
			    (position.x >= chunkPos.x + chunkWidth) || (position.z >= chunkPos.z + chunkWidth)) 
				continue;
			return chunks[i];
		}
		return null;
	}
}
