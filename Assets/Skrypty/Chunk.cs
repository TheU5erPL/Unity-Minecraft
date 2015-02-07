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
	
	// Use this for initialization
	void Start () {
		
		meshCollider = GetComponent<MeshCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		
		map = new byte[World.activeWorld.ch_width, World.activeWorld.ch_width, World.activeWorld.ch_height];
		
		for(int x = 0; x < World.activeWorld.ch_width; x++)
		{
			float perlinX = (float)x / 15;
			for (int y = 0; y < World.activeWorld.ch_height; y++)
			{
				float perlinY = (float)y / 15;
				for (int z = 0; z < World.activeWorld.ch_width; z++)
				{
					float perlinZ = (float)z / 15;
					float perlin = Noise.Generate(perlinX,perlinY,perlinZ);
					
					perlin += (10f - (float)y) / 10;
					
					if( perlin > 0.2f)
						map[x,y,z] = 1;
				}
			}
		}
		GenerateMesh();
	}
	
	public virtual void GenerateMesh() {
		v_Mesh = new Mesh();
		
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> UVs = new List<Vector2>();
		List<int> tris = new List<int>();
		
		for(int x = 0; x < World.activeWorld.ch_width; x++)
		{
			for(int y = 0; y < World.activeWorld.ch_height; y++)
			{
				for (int z = 0; z < World.activeWorld.ch_width; z++)
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
		    (x >= World.activeWorld.ch_width) ||
		    (y >= World.activeWorld.ch_height) ||
		    (z >= World.activeWorld.ch_width) ||
		    ( x < 0) || ( y < 0) || (z < 0)
		    )
			return 0;
		return map[x,y,z];
	}
}
