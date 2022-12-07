using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class PathArrows : MonoBehaviour
{
	// Token: 0x060010B4 RID: 4276 RVA: 0x000B1734 File Offset: 0x000AF934
	public static PathArrows Create(Movement movement, int step, bool dark = false)
	{
		PathArrows pathArrows = new GameObject("PathArrows").AddComponent<PathArrows>();
		pathArrows.step = step;
		pathArrows.material = new Material(Assets.Get<Material>("assets/misc/selection/wv_arrows.mat"));
		pathArrows.crossing_prefab = Assets.Get<GameObject>("assets/misc/selection/wv_crossing.prefab");
		pathArrows.movement = movement;
		pathArrows.dark = dark;
		float num = 0.5f;
		pathArrows.path_points = new List<PathArrows.PointSeg>();
		pathArrows.segments = new List<PathArrows.Segment>();
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int count = movement.path.segments.Count;
		for (int i = 0; i < count; i++)
		{
			PPos pt = movement.path.segments[i].pt;
			int num5 = num4;
			num4 = 0;
			if (pt.paID > 0)
			{
				bool flag;
				if (movement == null)
				{
					flag = (null != null);
				}
				else
				{
					Game game = movement.game;
					if (game == null)
					{
						flag = (null != null);
					}
					else
					{
						Logic.PathFinding path_finding = game.path_finding;
						if (path_finding == null)
						{
							flag = (null != null);
						}
						else
						{
							PathData data = path_finding.data;
							flag = (((data != null) ? data.pas : null) != null);
						}
					}
				}
				if (flag && movement.game.path_finding.data.pas.Length >= pt.paID && movement.game.path_finding.data.pas[pt.paID - 1].type == PathData.PassableArea.Type.Teleport)
				{
					num4 = pt.paID;
				}
			}
			bool river = pt.paID == -1;
			Vector3 vector = global::Common.SnapToTerrain(pt, num, null, -1f, false);
			vector.y = pt.Height(movement.game, vector.y, num);
			if (num5 != num4)
			{
				PathArrows.Segment item = default(PathArrows.Segment);
				item.idx = num3;
				item.cnt = num2;
				item.obj = null;
				pathArrows.segments.Add(item);
				num3 += num2;
				num2 = 0;
				pathArrows.path_points.Add(new PathArrows.PointSeg
				{
					position = vector,
					area = num4,
					river = river
				});
				num2++;
			}
			else
			{
				num2++;
				if (num2 > 1 && (num2 > 100 || i == count - 1))
				{
					PathArrows.Segment item2 = default(PathArrows.Segment);
					item2.idx = num3;
					item2.cnt = num2;
					item2.obj = null;
					pathArrows.segments.Add(item2);
					num3 += num2 + 1;
					num2 = -1;
				}
				pathArrows.path_points.Add(new PathArrows.PointSeg
				{
					position = vector,
					area = num4,
					river = river
				});
			}
		}
		return pathArrows;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x000B19B4 File Offset: 0x000AFBB4
	private Bounds CalcBounds(int idx, int cnt)
	{
		Bounds result = new Bounds(this.path_points[idx].position, Vector3.zero);
		for (;;)
		{
			idx++;
			cnt--;
			if (cnt <= 0)
			{
				break;
			}
			result.Encapsulate(this.path_points[idx].position);
		}
		return result;
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x000B1A08 File Offset: 0x000AFC08
	private GameObject CreateMesh(int start_idx, int count)
	{
		if (this.material == null)
		{
			return null;
		}
		if (this.path_points == null)
		{
			return null;
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		GameObject gameObject = new GameObject("PathArrows");
		gameObject.layer = base.gameObject.layer;
		gameObject.transform.SetParent(base.transform, true);
		int i = start_idx + count - 1;
		Vector3 a = this.path_points[i].position;
		bool flag = false;
		while (i >= start_idx)
		{
			PathArrows.PointSeg pointSeg = this.path_points[i];
			if (pointSeg.river)
			{
				int index = i + 1;
				PathArrows.PointSeg pointSeg2 = this.path_points[index];
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.crossing_prefab, base.transform);
				Vector3 position = pointSeg.position;
				Vector3 position2 = (position + pointSeg2.position) / 2f;
				float num = Vector3.Distance(position, pointSeg2.position);
				Vector3 forward = pointSeg2.position - position;
				gameObject2.transform.position = position2;
				gameObject2.transform.rotation = Quaternion.LookRotation(forward);
				float num2 = Mathf.Min(num * 0.5f, 15f);
				gameObject2.transform.localScale = new Vector3(num2, num2, num * 0.5f);
				MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
				pointSeg = this.path_points[i];
				pointSeg.r = component;
				this.path_points[i] = pointSeg;
				a = position;
			}
			else if (pointSeg.area != 0)
			{
				for (int j = i - 1; j >= start_idx; j--)
				{
					PathArrows.PointSeg pointSeg3 = this.path_points[j];
					if (PathData.IsGroundPAid(pointSeg3.area) || j == start_idx)
					{
						Vector3 position3 = pointSeg3.position;
						Vector3 vector = a - position3;
						position3 + vector * 0.75f;
						PathData.PassableArea passableArea = this.movement.game.path_finding.data.pas[pointSeg.area - 1];
						GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.crossing_prefab, base.transform);
						Point3 point = Point3.Zero;
						float num3 = float.MinValue;
						for (int k = 0; k < 4; k++)
						{
							Point3 cornerVertex = passableArea.GetCornerVertex(k);
							point += cornerVertex;
							for (int l = 0; l < 4; l++)
							{
								if (k != l)
								{
									Point3 cornerVertex2 = passableArea.GetCornerVertex(l);
									float num4 = cornerVertex.SqrDist(cornerVertex2);
									if (num4 > num3)
									{
										num3 = num4;
									}
								}
							}
						}
						num3 = Mathf.Sqrt(num3);
						point /= 4f;
						gameObject3.transform.position = point;
						gameObject3.transform.rotation = Quaternion.LookRotation(vector);
						float num5 = Mathf.Min(num3 * 0.5f, 15f);
						gameObject3.transform.localScale = new Vector3(num5, num5, num3 * 0.5f);
						MeshRenderer component2 = gameObject3.GetComponent<MeshRenderer>();
						if (j == start_idx)
						{
							pointSeg = this.path_points[j];
							pointSeg.r = component2;
							this.path_points[j] = pointSeg;
							i--;
						}
						else
						{
							pointSeg = this.path_points[j + 1];
							pointSeg.r = component2;
							this.path_points[j + 1] = pointSeg;
						}
						a = position3;
						i = j;
						break;
					}
				}
			}
			else if (i % this.step == 0)
			{
				Vector3 position4 = pointSeg.position;
				Vector3 vector2 = a - position4;
				Vector3 a2 = position4 + vector2 * 0.75f;
				float magnitude = vector2.magnitude;
				if (magnitude > 0.0001f)
				{
					Vector3 vector3 = new Vector3(vector2.z, 0f, -vector2.x);
					vector3 *= this.width * 0.5f / magnitude;
					int count2 = list.Count;
					list.Add(position4 + vector3);
					list.Add(position4 - vector3);
					list.Add(a2 + vector3);
					list.Add(a2 - vector3);
					float num6 = flag ? 0.5f : 0f;
					flag = !flag;
					list2.Add(new Vector2((float)i, num6));
					list2.Add(new Vector2((float)i, 0.5f + num6));
					list2.Add(new Vector2((float)(i + 1), num6));
					list2.Add(new Vector2((float)(i + 1), 0.5f + num6));
					list3.Add(count2);
					list3.Add(count2 + 1);
					list3.Add(count2 + 2);
					list3.Add(count2 + 1);
					list3.Add(count2 + 3);
					list3.Add(count2 + 2);
					a = position4;
				}
			}
			i--;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = this.material;
		return gameObject;
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x000B1F78 File Offset: 0x000B0178
	private void Start()
	{
		for (int i = 0; i < this.segments.Count; i++)
		{
			PathArrows.Segment segment = this.segments[i];
			segment.bounds = this.CalcBounds(segment.idx, segment.cnt);
			this.segments[i] = segment;
		}
		this.Update();
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x000B1FD4 File Offset: 0x000B01D4
	private void Update()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(CameraController.MainCamera);
		for (int i = 0; i < this.segments.Count; i++)
		{
			PathArrows.Segment segment = this.segments[i];
			if (!(segment.obj != null) && segment.cnt != 0 && GeometryUtility.TestPlanesAABB(planes, segment.bounds))
			{
				segment.obj = this.CreateMesh(segment.idx, segment.cnt);
				this.segments[i] = segment;
			}
		}
		Color value = Color.black;
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			if (this.movement.obj.IsAllyOrOwn(BaseUI.LogicKingdom()))
			{
				Movement movement = this.movement;
				if (((movement != null) ? movement.path : null) != null && this.movement.path.flee)
				{
					value = baseUI.selectionSettings.retreatingColor;
				}
				else
				{
					value = (this.dark ? baseUI.selectionSettings.secondaryFriendColor : baseUI.selectionSettings.friendColor);
				}
			}
			else
			{
				value = baseUI.GetStanceColor(this.movement.obj, true);
			}
		}
		this.material.SetColor("_Color", value);
		for (int j = 0; j < this.path_points.Count; j++)
		{
			if (this.path_points[j].r != null)
			{
				Logic.Army army = this.movement.obj as Logic.Army;
				this.path_points[j].r.material.SetColor("_Color", value);
				if (army != null && army.water_crossing.running && army.water_crossing.start_segment == j)
				{
					float value2 = army.water_crossing.Progress();
					this.path_points[j].r.material.SetFloat("_T", value2);
				}
				else if (this.movement.path.segment_idx > j)
				{
					UnityEngine.Object.Destroy(this.path_points[j].r.gameObject);
				}
			}
		}
		this.material.SetFloat("_T", (float)this.movement.path.segment_idx);
	}

	// Token: 0x04000B1A RID: 2842
	public Material material;

	// Token: 0x04000B1B RID: 2843
	public float width = 2f;

	// Token: 0x04000B1C RID: 2844
	public const int segmentPoints = 100;

	// Token: 0x04000B1D RID: 2845
	public int step = 1;

	// Token: 0x04000B1E RID: 2846
	private bool dark;

	// Token: 0x04000B1F RID: 2847
	private List<PathArrows.PointSeg> path_points;

	// Token: 0x04000B20 RID: 2848
	private Movement movement;

	// Token: 0x04000B21 RID: 2849
	private List<PathArrows.Segment> segments = new List<PathArrows.Segment>();

	// Token: 0x04000B22 RID: 2850
	private GameObject crossing_prefab;

	// Token: 0x02000658 RID: 1624
	private struct PointSeg
	{
		// Token: 0x04003525 RID: 13605
		public Vector3 position;

		// Token: 0x04003526 RID: 13606
		public int area;

		// Token: 0x04003527 RID: 13607
		public MeshRenderer r;

		// Token: 0x04003528 RID: 13608
		public bool river;
	}

	// Token: 0x02000659 RID: 1625
	private struct Segment
	{
		// Token: 0x04003529 RID: 13609
		public int idx;

		// Token: 0x0400352A RID: 13610
		public int cnt;

		// Token: 0x0400352B RID: 13611
		public Bounds bounds;

		// Token: 0x0400352C RID: 13612
		public GameObject obj;
	}
}
