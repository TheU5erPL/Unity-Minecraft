using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class Chunk : MonoBehaviour {
	
	public Mesh v_Mesh;
	protected MeshCollider meshCollider;
	protected MeshRenderer meshRenderer;
	protected MeshFilter meshFilter;
	public byte [,,] map;

	void Start () {
		
		meshCollider = GetComponent<MeshCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		
		map = new byte[World.activeWorld.ch_width, World.activeWorld.ch_width, World.activeWorld.ch_height];
		
		for(int x = 0; x < World.activeWorld.ch_width; x++)
		{
			for (int z = 0; z < World.activeWorld.ch_width; z++)
			{
				map[x, 0, z] = 1;
				map[x, 1, z] = (byte)Random.Range (0,1);
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
}