using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000186 RID: 390
public class TerrainPath : MonoBehaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x0600156D RID: 5485 RVA: 0x000DA4E0 File Offset: 0x000D86E0
	public static TerrainPath First()
	{
		return TerrainPath.first;
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x000DA4E7 File Offset: 0x000D86E7
	public static TerrainPath Last()
	{
		return TerrainPath.last;
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x000DA4EE File Offset: 0x000D86EE
	public static int Count()
	{
		return TerrainPath.count;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x000DA4F5 File Offset: 0x000D86F5
	public TerrainPath Prev()
	{
		return this.prev;
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000DA4FD File Offset: 0x000D86FD
	public TerrainPath Next()
	{
		return this.next;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000DA505 File Offset: 0x000D8705
	public bool Initted()
	{
		return this.initted;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000DA510 File Offset: 0x000D8710
	public void Clean(bool destroy_lines)
	{
		this.waypoints = null;
		this.UnregisterFromTI();
		this.Unregister();
		if (this.pf != null)
		{
			this.pf.Clean();
			this.pf = null;
		}
		this.initted = false;
		if (!destroy_lines)
		{
			return;
		}
		this.path_len = 0f;
		this.path = null;
		this.path_points = null;
		Transform transform = base.transform.Find("_lines");
		if (transform != null)
		{
			Common.DestroyObj(transform.gameObject);
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		if (this.lines_mesh == null && component != null)
		{
			this.lines_mesh = component.sharedMesh;
		}
		if (component != null)
		{
			component.mesh = (component.sharedMesh = null);
		}
		Common.DestroyObj(this.lines_mesh);
		this.lines_mesh = null;
		if (component2 != null)
		{
			component2.enabled = false;
		}
		if (this.lines == null)
		{
			this.lines = base.GetComponent<LineRenderer>();
		}
		if (this.lines != null)
		{
			this.lines.enabled = false;
		}
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x000DA634 File Offset: 0x000D8834
	private bool BeginNextSegment()
	{
		if (this.waypoints.Length >= 1)
		{
			while (this.cur_waypoint < this.waypoints.Length)
			{
				Vector3 position = this.waypoints[this.cur_waypoint].transform.position;
				this.start_thickness = this.waypoints[this.cur_waypoint].transform.localScale.x;
				this.cur_waypoint++;
				Vector3 position2;
				if (this.cur_waypoint < this.waypoints.Length)
				{
					position2 = this.waypoints[this.cur_waypoint].transform.position;
					this.end_thickness = this.waypoints[this.cur_waypoint].transform.localScale.x;
				}
				else
				{
					if (!(this.dest != null) || this.cur_waypoint != this.waypoints.Length)
					{
						return false;
					}
					position2 = this.dest.transform.position;
					this.end_thickness = 1f;
				}
				if (!(position == position2))
				{
					if (this.thicknesses == null)
					{
						this.thicknesses = new List<float>();
					}
					this.pf = new TerrainPathFinder();
					this.pf.Begin(this.settings, position, position2, 0f);
					return true;
				}
			}
			return false;
		}
		this.start_thickness = 1f;
		this.end_thickness = 1f;
		this.thicknesses = null;
		if (this.cur_waypoint > 0)
		{
			return false;
		}
		this.cur_waypoint++;
		if (this.dest == null)
		{
			return false;
		}
		Vector3 position3 = base.transform.position;
		Vector3 position4 = this.dest.transform.position;
		if (position3 == position4)
		{
			return false;
		}
		this.pf = new TerrainPathFinder();
		this.pf.Begin(this.settings, position3, position4, 0f);
		return true;
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x000DA809 File Offset: 0x000D8A09
	public void Begin()
	{
		this.Clean(true);
		this.settings.minHeight = MapData.GetWaterLevel();
		this.waypoints = base.GetComponentsInChildren<RoadWaypoint>();
		this.cur_waypoint = 0;
		this.thicknesses = null;
		this.BeginNextSegment();
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000DA844 File Offset: 0x000D8A44
	public void Recalc()
	{
		this.Begin();
		int num = this.waypoints.Length;
		for (int i = 0; i < num; i++)
		{
			RoadWaypoint roadWaypoint = this.waypoints[i];
			Common.SnapToTerrain(roadWaypoint.transform, 0f, null, -1f);
			roadWaypoint.transform.hasChanged = false;
		}
		while (!this.Process(0))
		{
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000DA8A0 File Offset: 0x000D8AA0
	private void AddPath()
	{
		if (this.path == null)
		{
			this.path = this.pf.path;
		}
		else
		{
			for (int i = 1; i < this.pf.path.Count; i++)
			{
				this.path.Add(this.pf.path[i]);
			}
		}
		if (this.step < 0.1f)
		{
			this.step = 0.1f;
		}
		float num = 0f;
		if (this.path_points == null)
		{
			this.path_points = new List<Vector3>();
			this.path_len = 0f;
		}
		else
		{
			num += this.step;
		}
		PathWalker pathWalker = new PathWalker();
		pathWalker.SetPath(this.pf);
		Vector3 vector;
		Vector3 vector2;
		while (pathWalker.GetPathPoint(num, out vector, out vector2, true, 0f))
		{
			vector.y = Common.GetTerrainHeight(vector, null, false);
			if (this.path_points.Count > 0)
			{
				this.path_len += (vector - this.path_points[this.path_points.Count - 1]).magnitude;
			}
			this.path_points.Add(vector);
			if (this.thicknesses != null)
			{
				float item;
				if (this.start_thickness != this.end_thickness)
				{
					item = Common.map(num, 0f, pathWalker.path_len, this.start_thickness, this.end_thickness, false);
				}
				else
				{
					item = this.start_thickness;
				}
				this.thicknesses.Add(item);
			}
			num += this.step;
		}
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x000DAA2C File Offset: 0x000D8C2C
	public bool Process(int steps)
	{
		if (this.pf == null)
		{
			return true;
		}
		if (this.pf.IsDone())
		{
			return true;
		}
		if (!this.pf.Process(steps))
		{
			return false;
		}
		if (this.pf.path == null || this.pf.path_points == null || this.pf.path_points.Count < 2)
		{
			this.pf.Clean();
			this.pf = null;
			this.path = null;
			this.path_points = null;
			this.path_len = 0f;
			this.Init(true);
			return true;
		}
		this.AddPath();
		this.pf.Clean();
		this.pf = null;
		if (!this.BeginNextSegment())
		{
			this.Init(true);
			return true;
		}
		return false;
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x000DAAF0 File Offset: 0x000D8CF0
	public void Register()
	{
		if (!this.redundant)
		{
			TerrainPath.count++;
		}
		this.prev = TerrainPath.last;
		if (TerrainPath.last != null)
		{
			TerrainPath.last.next = this;
		}
		else
		{
			TerrainPath.first = this;
		}
		TerrainPath.last = this;
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000DAB44 File Offset: 0x000D8D44
	public void Unregister()
	{
		if (!this.initted)
		{
			return;
		}
		if (!this.redundant)
		{
			TerrainPath.count--;
		}
		if (this.next != null)
		{
			this.next.prev = this.prev;
		}
		else
		{
			TerrainPath.last = this.prev;
		}
		if (this.prev != null)
		{
			this.prev.next = this.next;
		}
		else
		{
			TerrainPath.first = this.next;
		}
		this.prev = (this.next = null);
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000DABD6 File Offset: 0x000D8DD6
	public void Init(bool recreate_lines)
	{
		if (this.initted)
		{
			return;
		}
		this.initted = true;
		this.Register();
		this.RegisterInTI();
		if (recreate_lines)
		{
			this.SetRedundant(false);
			this.CreateLines();
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x000DAC04 File Offset: 0x000D8E04
	public void SetRedundant(bool redundant)
	{
		if (redundant == this.redundant)
		{
			return;
		}
		this.redundant = redundant;
		if (!this.initted)
		{
			return;
		}
		if (redundant)
		{
			TerrainPath.count--;
			return;
		}
		TerrainPath.count++;
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000DAC3C File Offset: 0x000D8E3C
	public void CalcRedundant()
	{
		if (this.path == null || this.path.Count < 2)
		{
			return;
		}
		bool flag = true;
		int num = this.path.Count;
		for (int i = 0; i < num; i++)
		{
			if (TerrainInfo.Get(this.path[i], 0).roads == 1)
			{
				flag = false;
				break;
			}
		}
		this.SetRedundant(flag);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000023FD File Offset: 0x000005FD
	public void RegisterInTI()
	{
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000023FD File Offset: 0x000005FD
	public void UnregisterFromTI()
	{
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x000DACA0 File Offset: 0x000D8EA0
	public void CreateLines()
	{
		if (this.path_points == null || this.path_points.Count < 2)
		{
			return;
		}
		if (this.mat == null)
		{
			return;
		}
		if (this.path_colors != null && this.path_colors.Count != this.path_points.Count)
		{
			Debug.LogError("Path points count and path colors count must be equal!");
			return;
		}
		float num = this.useLineRenderer ? 0.1f : 0.3f;
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.path_points.Count; i++)
		{
			Vector3 position = this.path_points[i];
			position.y += num;
			list.Add(base.transform.InverseTransformPoint(position));
		}
		if (this.useLineRenderer)
		{
			if (this.lines == null)
			{
				this.lines = base.GetComponent<LineRenderer>();
			}
			if (this.lines == null)
			{
				this.lines = base.gameObject.AddComponent<LineRenderer>();
				this.lines.numCapVertices = 2;
				this.lines.shadowCastingMode = ShadowCastingMode.Off;
				this.lines.textureMode = LineTextureMode.Tile;
				this.lines.sortingLayerName = "OnTop";
				this.lines.sortingOrder = 5;
			}
			this.lines.enabled = true;
			this.lines.useWorldSpace = false;
			this.lines.material = this.mat;
			this.lines.startWidth = (this.lines.endWidth = this.width);
			this.lines.positionCount = list.Count;
			this.lines.SetPositions(list.ToArray());
			return;
		}
		MeshFilter meshFilter = base.GetComponent<MeshFilter>();
		if (this.lines_mesh == null && meshFilter != null)
		{
			this.lines_mesh = meshFilter.sharedMesh;
		}
		Common.DestroyObj(this.lines_mesh);
		this.lines_mesh = MeshUtils.CreateLinesMesh(list, this.width, this.vertices, true, false, this.thicknesses, this.path_colors);
		this.lines_mesh = MeshUtils.SnapMeshToTerrain(this.lines_mesh, base.transform, 0.1f, false, null);
		this.lines_mesh.RecalculateNormals();
		this.lines_mesh.RecalculateBounds();
		if (this.alpha_blend_dist > 0f && this.path_colors == null)
		{
			Vector3 b = list[0];
			Vector3 b2 = list[list.Count - 1];
			b.y = 0f;
			b2.y = 0f;
			Vector3 vector = new Vector3(this.alpha_blend_dist, 0f, 0f);
			float magnitude = base.transform.InverseTransformVector(vector).magnitude;
			Color[] array = new Color[this.lines_mesh.vertices.Length];
			Color a = new Color(1f, 1f, 1f, 0f);
			for (int j = 0; j < array.Length; j++)
			{
				Vector3 a2 = this.lines_mesh.vertices[j];
				a2.y = 0f;
				float a3 = Vector3.Distance(a2, b);
				float b3 = Vector3.Distance(a2, b2);
				float num2 = Mathf.Min(a3, b3);
				Color color = Color.Lerp(a, Color.white, num2 / magnitude);
				array[j] = color;
			}
			this.lines_mesh.SetColors(array);
			this.mat.EnableKeyword("ALPHA_FADE");
		}
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		meshFilter.mesh = (meshFilter.sharedMesh = this.lines_mesh);
		MeshRenderer meshRenderer = base.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material = this.mat;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.enabled = true;
		this.bounds = default(Bounds);
		if (this.path_points.Count >= 2)
		{
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			float num5 = float.MaxValue;
			float num6 = float.MinValue;
			float num7 = float.MinValue;
			float num8 = float.MinValue;
			for (int k = 0; k < this.path_points.Count; k++)
			{
				if (this.path_points[k].x < num3)
				{
					num3 = this.path_points[k].x;
				}
				if (this.path_points[k].y < num4)
				{
					num4 = this.path_points[k].y;
				}
				if (this.path_points[k].z < num5)
				{
					num5 = this.path_points[k].z;
				}
				if (this.path_points[k].x > num6)
				{
					num6 = this.path_points[k].x;
				}
				if (this.path_points[k].y > num7)
				{
					num7 = this.path_points[k].y;
				}
				if (this.path_points[k].z > num8)
				{
					num8 = this.path_points[k].z;
				}
			}
			this.bounds.center = new Vector3(num3 + num6, num4 + num7, num5 + num8) / 2f;
			this.bounds.size = new Vector3(Mathf.Abs(num3 - num6), Mathf.Abs(num4 - num7), Mathf.Abs(num5 - num8)) * 1.5f;
		}
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x000DB242 File Offset: 0x000D9442
	public void VisibilityChanged(bool visible)
	{
		base.enabled = visible;
		if (this.rend != null)
		{
			this.rend.enabled = visible;
		}
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x000DB265 File Offset: 0x000D9465
	private void Start()
	{
		this.Init(false);
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000023FD File Offset: 0x000005FD
	public void Update()
	{
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000DB26E File Offset: 0x000D946E
	private void OnEnable()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x04000DBD RID: 3517
	public GameObject src;

	// Token: 0x04000DBE RID: 3518
	public GameObject dest;

	// Token: 0x04000DBF RID: 3519
	public Material mat;

	// Token: 0x04000DC0 RID: 3520
	public Renderer rend;

	// Token: 0x04000DC1 RID: 3521
	public float width = 0.5f;

	// Token: 0x04000DC2 RID: 3522
	public int vertices = 2;

	// Token: 0x04000DC3 RID: 3523
	public bool useLineRenderer = true;

	// Token: 0x04000DC4 RID: 3524
	public string culture;

	// Token: 0x04000DC5 RID: 3525
	public TerrainPathFinder pf;

	// Token: 0x04000DC6 RID: 3526
	public TerrainPathFinder.Settings settings;

	// Token: 0x04000DC7 RID: 3527
	public bool disable_pathfinding;

	// Token: 0x04000DC8 RID: 3528
	public static int stepsPerFrame = 10;

	// Token: 0x04000DC9 RID: 3529
	public float alpha_blend_dist = -1f;

	// Token: 0x04000DCA RID: 3530
	private LineRenderer lines;

	// Token: 0x04000DCB RID: 3531
	private Mesh lines_mesh;

	// Token: 0x04000DCC RID: 3532
	[HideInInspector]
	public Bounds bounds;

	// Token: 0x04000DCD RID: 3533
	private int TIVersion;

	// Token: 0x04000DCE RID: 3534
	private bool initted;

	// Token: 0x04000DCF RID: 3535
	public bool redundant;

	// Token: 0x04000DD0 RID: 3536
	private static TerrainPath first = null;

	// Token: 0x04000DD1 RID: 3537
	private static TerrainPath last = null;

	// Token: 0x04000DD2 RID: 3538
	private static int count = 0;

	// Token: 0x04000DD3 RID: 3539
	private TerrainPath prev;

	// Token: 0x04000DD4 RID: 3540
	private TerrainPath next;

	// Token: 0x04000DD5 RID: 3541
	[HideInInspector]
	public List<TerrainInfo.Coords> path;

	// Token: 0x04000DD6 RID: 3542
	[HideInInspector]
	public List<Vector3> path_points;

	// Token: 0x04000DD7 RID: 3543
	[HideInInspector]
	[NonSerialized]
	public List<Color> path_colors;

	// Token: 0x04000DD8 RID: 3544
	public float path_len;

	// Token: 0x04000DD9 RID: 3545
	public float straight_dist;

	// Token: 0x04000DDA RID: 3546
	private RoadWaypoint[] waypoints;

	// Token: 0x04000DDB RID: 3547
	private int cur_waypoint;

	// Token: 0x04000DDC RID: 3548
	private float start_thickness = 1f;

	// Token: 0x04000DDD RID: 3549
	private float end_thickness = 1f;

	// Token: 0x04000DDE RID: 3550
	[HideInInspector]
	public List<float> thicknesses;

	// Token: 0x04000DDF RID: 3551
	public float step = 1f;
}
