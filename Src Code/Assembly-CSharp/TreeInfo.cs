using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000154 RID: 340
[ExecuteInEditMode]
public class TreeInfo : MonoBehaviour
{
	// Token: 0x0600117D RID: 4477 RVA: 0x000B7C8C File Offset: 0x000B5E8C
	private void Init()
	{
		this.cam = CameraController.MainCamera;
		Terrain activeTerrain = Terrain.activeTerrain;
		this.td = ((activeTerrain == null) ? null : activeTerrain.terrainData);
		this.instances = ((this.td == null) ? null : this.td.treeInstances);
		this.TotalTrees = ((this.instances == null) ? 0 : this.instances.Length);
		this.PerType = ((this.td == null) ? null : new int[this.td.treePrototypes.Length]);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x000B7D26 File Offset: 0x000B5F26
	private void OnEnable()
	{
		this.CalculateDistances = false;
		this.Init();
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x000B7D38 File Offset: 0x000B5F38
	private void Update()
	{
		if (this.instances == null || this.td == null || this.instances.Length != this.td.treeInstanceCount)
		{
			this.Init();
		}
		if (this.td == null)
		{
			return;
		}
		if (this.cam == null)
		{
			return;
		}
		if (this.cam.transform.position == this.cam_pos && this.cam.transform.rotation == this.cam_rot)
		{
			return;
		}
		this.cam_pos = this.cam.transform.position;
		this.cam_rot = this.cam.transform.rotation;
		this.InGameView = 0;
		for (int i = 0; i < this.PerType.Length; i++)
		{
			this.PerType[i] = 0;
		}
		this.in_view.Clear();
		this.MinDist = -1f;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(this.cam);
		foreach (TreeInstance treeInstance in this.instances)
		{
			Vector3 vector = Vector3.Scale(treeInstance.position, this.td.size);
			Bounds bounds = new Bounds(vector, Vector3.one);
			if (GeometryUtility.TestPlanesAABB(planes, bounds))
			{
				this.InGameView++;
				if (treeInstance.prototypeIndex < this.PerType.Length)
				{
					this.PerType[treeInstance.prototypeIndex]++;
				}
				TreeInfo.Inst inst = new TreeInfo.Inst();
				inst.pos = vector;
				inst.type = treeInstance.prototypeIndex;
				inst.min_dist = -1f;
				if (this.CalculateDistances)
				{
					for (int k = 0; k < this.in_view.Count; k++)
					{
						TreeInfo.Inst inst2 = this.in_view[k];
						float magnitude = (vector - inst2.pos).magnitude;
						if (this.MinDist < 0f || magnitude < this.MinDist)
						{
							this.MinDist = magnitude;
						}
						if (inst.min_dist < 0f || magnitude < inst.min_dist)
						{
							inst.min_dist = magnitude;
						}
						if (inst2.min_dist < 0f || magnitude < inst2.min_dist)
						{
							inst2.min_dist = magnitude;
						}
					}
				}
				this.in_view.Add(inst);
			}
		}
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x000B7FBC File Offset: 0x000B61BC
	public void ClearObjects()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		foreach (GameObject obj2 in list)
		{
			Object.DestroyImmediate(obj2);
		}
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x000B805C File Offset: 0x000B625C
	public void SpawnObjects()
	{
		this.ClearObjects();
		Vector3 localScale = new Vector3(1f, 2f, 1f);
		foreach (TreeInfo.Inst inst in this.in_view)
		{
			if (this.ObjectsPrefab == null)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.position = inst.pos + Vector3.up;
				gameObject.transform.localScale = localScale;
			}
			else
			{
				GameObject gameObject2 = Common.Spawn(this.ObjectsPrefab, false, false);
				gameObject2.transform.SetParent(base.transform);
				gameObject2.transform.position = inst.pos;
			}
		}
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x000B8148 File Offset: 0x000B6348
	private void OnDrawGizmosSelected()
	{
		if (!this.Visualize)
		{
			return;
		}
		Vector3 size = new Vector3(1f, 2f, 1f);
		foreach (TreeInfo.Inst inst in this.in_view)
		{
			Gizmos.color = ((inst.min_dist >= 0f && inst.min_dist <= this.DenseThreshold) ? Color.red : Color.blue);
			Gizmos.DrawWireCube(inst.pos + Vector3.up, size);
		}
	}

	// Token: 0x04000B93 RID: 2963
	public int TotalTrees;

	// Token: 0x04000B94 RID: 2964
	public int InGameView;

	// Token: 0x04000B95 RID: 2965
	public int[] PerType;

	// Token: 0x04000B96 RID: 2966
	public bool CalculateDistances;

	// Token: 0x04000B97 RID: 2967
	public float MinDist = -1f;

	// Token: 0x04000B98 RID: 2968
	public float DenseThreshold = 0.2f;

	// Token: 0x04000B99 RID: 2969
	public bool Visualize = true;

	// Token: 0x04000B9A RID: 2970
	public GameObject ObjectsPrefab;

	// Token: 0x04000B9B RID: 2971
	private TerrainData td;

	// Token: 0x04000B9C RID: 2972
	private TreeInstance[] instances;

	// Token: 0x04000B9D RID: 2973
	private Camera cam;

	// Token: 0x04000B9E RID: 2974
	private Vector3 cam_pos = Vector3.zero;

	// Token: 0x04000B9F RID: 2975
	private Quaternion cam_rot = Quaternion.identity;

	// Token: 0x04000BA0 RID: 2976
	private List<TreeInfo.Inst> in_view = new List<TreeInfo.Inst>();

	// Token: 0x02000670 RID: 1648
	private class Inst
	{
		// Token: 0x04003593 RID: 13715
		public Vector3 pos;

		// Token: 0x04003594 RID: 13716
		public int type;

		// Token: 0x04003595 RID: 13717
		public float min_dist;
	}
}
