using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000B8 RID: 184
public class AutoRoads : MonoBehaviour
{
	// Token: 0x0600078B RID: 1931 RVA: 0x0004FD30 File Offset: 0x0004DF30
	private void AddRoad(Transform t1, Transform t2, float max_dist_mul = 1f)
	{
		float magnitude = (t2.position - t1.position).magnitude;
		if (magnitude > this.MaxDistance * max_dist_mul)
		{
			return;
		}
		float waterLevel = MapData.GetWaterLevel();
		if (waterLevel > 0f)
		{
			if (global::Common.GetTerrainHeight(t1.position, null, false) <= waterLevel)
			{
				return;
			}
			if (global::Common.GetTerrainHeight(t2.position, null, false) <= waterLevel)
			{
				return;
			}
		}
		Transform transform;
		Transform transform2;
		if (t1.position.z < t2.position.z || (t1.position.z == t2.position.z && t1.position.x < t2.position.x))
		{
			transform = t1;
			transform2 = t2;
		}
		else
		{
			transform = t2;
			transform2 = t1;
		}
		TerrainPath terrainPath = this.Find(transform.gameObject, transform2.gameObject);
		if (terrainPath != null)
		{
			this.roads.Add(terrainPath);
			return;
		}
		if (terrainPath == null)
		{
			GameObject gameObject = global::Common.Spawn(this.RoadPrefab, false, false);
			gameObject.name = "from " + transform.name + " to " + transform2.name;
			gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
			terrainPath = gameObject.GetComponent<TerrainPath>();
			if (terrainPath == null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
				return;
			}
			this.roads.Add(terrainPath);
		}
		terrainPath.Clean(true);
		terrainPath.transform.position = transform.position;
		terrainPath.src = transform.gameObject;
		terrainPath.dest = transform2.gameObject;
		terrainPath.straight_dist = magnitude;
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0004FEBC File Offset: 0x0004E0BC
	private void CreateRoads()
	{
		if (this.RoadPrefab == null)
		{
			return;
		}
		RoadWaypoint[] componentsInChildren = base.GetComponentsInChildren<RoadWaypoint>();
		this.roads = new List<TerrainPath>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			RoadWaypoint roadWaypoint = componentsInChildren[i];
			for (int j = i + 1; j < componentsInChildren.Length; j++)
			{
				RoadWaypoint roadWaypoint2 = componentsInChildren[j];
				this.AddRoad(roadWaypoint.transform, roadWaypoint2.transform, 1f);
			}
		}
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			foreach (RoadWaypoint roadWaypoint3 in componentsInChildren)
			{
				this.AddRoad(settlement.transform, roadWaypoint3.transform, 1f);
			}
			global::Settlement settlement2 = settlement.Next();
			while (settlement2 != null)
			{
				float max_dist_mul = 1f;
				this.AddRoad(settlement.transform, settlement2.transform, max_dist_mul);
				settlement2 = settlement2.Next();
			}
			settlement = settlement.Next();
		}
		this.SortRoads();
		foreach (TerrainPath terrainPath in this.roads)
		{
			terrainPath.transform.SetParent(base.transform, true);
		}
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x00050014 File Offset: 0x0004E214
	public static int CompareRoads(TerrainPath a, TerrainPath b)
	{
		if (a.straight_dist < b.straight_dist)
		{
			return -1;
		}
		if (a.straight_dist <= b.straight_dist)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00050037 File Offset: 0x0004E237
	private void SortRoads()
	{
		if (this.roads == null)
		{
			return;
		}
		this.roads.Sort(new Comparison<TerrainPath>(AutoRoads.CompareRoads));
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0005005C File Offset: 0x0004E25C
	public void ClearAll()
	{
		this.roads = null;
		this.outgoing = null;
		TerrainPath[] componentsInChildren = base.GetComponentsInChildren<TerrainPath>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			global::Common.DestroyObj(componentsInChildren[i].gameObject);
		}
		TerrainInfo.Clean();
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x000500A0 File Offset: 0x0004E2A0
	public bool IsInCircle(GameObject go, Vector3 pt, float r2)
	{
		if (go == null)
		{
			return false;
		}
		Vector3 vector = pt - go.transform.position;
		vector.y = 0f;
		return vector.sqrMagnitude < r2;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x000500E0 File Offset: 0x0004E2E0
	public bool IsInCircle(Vector3 go, Vector3 pt, float r2)
	{
		Vector3 vector = pt - go;
		vector.y = 0f;
		return vector.sqrMagnitude < r2;
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0005010C File Offset: 0x0004E30C
	public void DeleteInCircle(Vector3 pt, float radius)
	{
		float r = radius * radius;
		foreach (TerrainPath terrainPath in base.GetComponentsInChildren<TerrainPath>())
		{
			if (this.IsInCircle(terrainPath.src, pt, r) || this.IsInCircle(terrainPath.dest, pt, r))
			{
				global::Common.DestroyObj(terrainPath.gameObject);
			}
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00050164 File Offset: 0x0004E364
	public void AddToDict(Dictionary<GameObject, List<TerrainPath>> dict, GameObject go, TerrainPath road)
	{
		if (go == null)
		{
			return;
		}
		List<TerrainPath> list;
		if (!dict.TryGetValue(go, out list))
		{
			list = new List<TerrainPath>();
			dict.Add(go, list);
		}
		list.Add(road);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0005019C File Offset: 0x0004E39C
	public TerrainPath Find(GameObject from, GameObject to)
	{
		if (this.outgoing == null)
		{
			return null;
		}
		List<TerrainPath> list;
		if (!this.outgoing.TryGetValue(from, out list))
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			TerrainPath terrainPath = list[i];
			if (terrainPath.dest == to)
			{
				return terrainPath;
			}
		}
		return null;
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000501F0 File Offset: 0x0004E3F0
	public void AnalyzeExisting()
	{
		this.outgoing = new Dictionary<GameObject, List<TerrainPath>>();
		foreach (TerrainPath terrainPath in base.GetComponentsInChildren<TerrainPath>())
		{
			this.AddToDict(this.outgoing, terrainPath.src, terrainPath);
		}
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x00050234 File Offset: 0x0004E434
	public void Begin()
	{
		this.ClearAll();
		this.CreateRoads();
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00050242 File Offset: 0x0004E442
	public bool IsDone()
	{
		return this.roads == null;
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00050250 File Offset: 0x0004E450
	public void Next()
	{
		if (this.roads == null)
		{
			return;
		}
		for (int i = 0; i < this.roads.Count; i++)
		{
			TerrainPath terrainPath = this.roads[i];
			if (terrainPath.path == null || terrainPath.path.Count < 1)
			{
				terrainPath.Recalc();
				if (terrainPath.path == null || terrainPath.path.Count < 2)
				{
					this.roads.RemoveAt(i);
				}
				return;
			}
		}
		this.roads = null;
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x000502CF File Offset: 0x0004E4CF
	private bool ExistsInRoads(Point pos, List<Point> existing_coords)
	{
		return existing_coords.Contains(pos);
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x000502D8 File Offset: 0x0004E4D8
	public void RefreshAll(Vector3 pt = default(Vector3), float r = 0f, bool clear_meshes = true)
	{
		if (this.bs == null)
		{
			this.bs = base.GetComponent<LinesBatching>();
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x000502F4 File Offset: 0x0004E4F4
	private void Start()
	{
		this.LoadFromBinary(true);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x000502FD File Offset: 0x0004E4FD
	public void ClearTravellers()
	{
		if (this.instanced_roads == null)
		{
			return;
		}
		this.instanced_roads.Clear();
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00050313 File Offset: 0x0004E513
	private void OnDestroy()
	{
		this.ClearTravellers();
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0005031C File Offset: 0x0004E51C
	private void LoadFromBinary(bool reload)
	{
		this.bs = base.GetComponent<LinesBatching>();
		this.bs.Clear();
		if (reload)
		{
			Profile.BeginSection("AutoRoads.LoadPoints");
			this.bs.LoadPoints(null, true);
			string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
			string path = Game.maps_path + str + "/RoadsSegmentData.bin";
			this.roadData.Clear();
			this.roadData2.Clear();
			if (!File.Exists(path))
			{
				return;
			}
			byte[] array = File.ReadAllBytes(path);
			int num = array.Length / 4;
			float[] array2 = Serialization.ToArray<float>(array, num);
			for (int i = 0; i < num; i += 6)
			{
				this.roadData.Add(new float4(array2[i], array2[i + 1], array2[i + 2], array2[i + 3]));
				this.roadData2.Add(new float2(array2[i + 4], array2[i + 5]));
			}
			Profile.EndSection("AutoRoads.LoadPoints");
		}
		Profile.BeginSection("AutoRoads.Rebuild");
		this.bs.Rebuild();
		Profile.EndSection("AutoRoads.Rebuild");
		Profile.BeginSection("AutoRoads.CreatePaths");
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		GameObject gameObject = new GameObject("Travellers");
		global::Common.SetObjectParent(gameObject, GameLogic.instance.transform, "");
		Transform transform = gameObject.transform;
		for (int j = 0; j < this.roadData.Count; j++)
		{
			float4 @float = this.roadData[j];
			float2 float2 = this.roadData2[j];
			float x = @float.x;
			int num2 = (int)@float.y / 3;
			int num3 = (int)@float.z / 3;
			int src = (int)float2.x;
			int dest = (int)float2.y;
			float w = @float.w;
			InstancedPath instancedPath = new InstancedPath();
			if (this.instanced_roads == null)
			{
				this.instanced_roads = new List<InstancedPath>();
			}
			this.instanced_roads.Add(instancedPath);
			instancedPath.path_points = new List<Vector3>();
			Vector3 center;
			if (num3 - num2 > 2)
			{
				center = this.bs.pos_rots_binary[(num3 + num2) / 2];
			}
			else
			{
				center = this.bs.pos_rots_binary[num2];
			}
			for (int k = num2; k <= num3; k++)
			{
				float3 v = this.bs.pos_rots_binary[k];
				instancedPath.path_points.Add(v);
			}
			instancedPath.bounds = new Bounds(center, Vector3.one * x);
			instancedPath.path_len = w;
			instancedPath.TravellersTransform = transform;
			instancedPath.src = src;
			instancedPath.dest = dest;
		}
		Profile.EndSection("AutoRoads.CreatePaths");
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x000505E4 File Offset: 0x0004E7E4
	public void SaveBinaryData()
	{
		int num = 0;
		List<float> list = new List<float>();
		List<float> list2 = new List<float>();
		this.roadData.Clear();
		foreach (TerrainPath terrainPath in this.roads)
		{
			num++;
			List<Vector3> list3 = (terrainPath != null) ? terrainPath.path_points : null;
			if (!terrainPath.redundant && list3 != null)
			{
				list2.Clear();
				int count = list.Count;
				float w = 0f;
				WorldMap.GenerateRoadData(list3, ref list2, out w, 0f);
				list.AddRange(list2);
				this.roadData.Add(new float4(terrainPath.bounds.extents.magnitude, (float)count, (float)(list.Count - 1), w));
				GameObject src = terrainPath.src;
				global::Settlement settlement = (src != null) ? src.GetComponent<global::Settlement>() : null;
				GameObject dest = terrainPath.dest;
				global::Settlement settlement2 = (dest != null) ? dest.GetComponent<global::Settlement>() : null;
				if (settlement != null && settlement2 != null)
				{
					this.roadData2.Add(new float2((float)settlement.nid, (float)settlement2.nid));
				}
				else
				{
					this.roadData2.Add(new float2(-1f, -1f));
				}
			}
		}
		Debug.Log(string.Format("Paths processed: {0}, path segments: {1}", num, list.Count - 1));
		byte[] bytes = Serialization.ToBytes<float>(list.ToArray());
		string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		File.WriteAllBytes("Assets/Maps/" + str + "/RoadsTrapezoidData.bin", bytes);
		float[] array = new float[this.roadData.Count * 6];
		for (int i = 0; i < this.roadData.Count; i++)
		{
			float4 @float = this.roadData[i];
			array[i * 6] = @float.x;
			array[i * 6 + 1] = @float.y;
			array[i * 6 + 2] = @float.z;
			array[i * 6 + 3] = @float.w;
			float2 float2 = this.roadData2[i];
			array[i * 6 + 4] = float2.x;
			array[i * 6 + 5] = float2.y;
		}
		bytes = Serialization.ToBytes<float>(array);
		File.WriteAllBytes("Assets/Maps/" + str + "/RoadsSegmentData.bin", bytes);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0005088C File Offset: 0x0004EA8C
	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.instanced_roads == null)
		{
			return;
		}
		for (int i = 0; i < this.instanced_roads.Count; i++)
		{
			this.instanced_roads[i].UpdateTravellers();
		}
	}

	// Token: 0x04000635 RID: 1589
	public float MaxDistance = 100f;

	// Token: 0x04000636 RID: 1590
	public GameObject RoadPrefab;

	// Token: 0x04000637 RID: 1591
	[NonSerialized]
	public List<TerrainPath> roads;

	// Token: 0x04000638 RID: 1592
	[NonSerialized]
	public List<InstancedPath> instanced_roads;

	// Token: 0x04000639 RID: 1593
	private List<float4> roadData = new List<float4>();

	// Token: 0x0400063A RID: 1594
	private List<float2> roadData2 = new List<float2>();

	// Token: 0x0400063B RID: 1595
	private Dictionary<GameObject, List<TerrainPath>> outgoing;

	// Token: 0x0400063C RID: 1596
	private LinesBatching bs;
}
