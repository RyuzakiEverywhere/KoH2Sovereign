using System;
using System.Collections.Generic;
using Logic;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class PassableArea : MonoBehaviour
{
	// Token: 0x0600120C RID: 4620 RVA: 0x000BD400 File Offset: 0x000BB600
	public void SetVisible(bool state)
	{
		if (this.rend == null)
		{
			return;
		}
		this.rend.enabled = state;
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x000BD420 File Offset: 0x000BB620
	public void ApplyCorners(Game game, int area_id)
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component)
		{
			this.mesh = component.mesh;
		}
		if (base.transform.childCount >= 4)
		{
			int num = Math.Min(PathData.PassableArea.numNodes, base.transform.childCount);
			for (int i = 0; i < num; i++)
			{
				Transform cornerTransform = this.GetCornerTransform(i);
				if (!(cornerTransform == null))
				{
					this.cornerPoints[i] = cornerTransform.position;
				}
			}
			for (int j = num - 1; j >= 0; j--)
			{
				Transform child = base.transform.GetChild(j);
				if (child.name.Contains("Corner"))
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}
		}
		this.GenerateNodePoints(area_id);
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000BD4E8 File Offset: 0x000BB6E8
	private void GenerateNodePoints(int area_id)
	{
		for (int i = 0; i < this.nodePoints.Count; i++)
		{
			PathData.PassableAreaNode.Type type = this.nodePoints[i].type;
			if (type == PathData.PassableAreaNode.Type.Edge)
			{
				type = PathData.PassableAreaNode.Type.Normal;
			}
			this.nodePoints[i] = new PassableArea.Node(Vector3.Lerp(this.cornerPoints[i], this.cornerPoints[(i + 1) % this.nodePoints.Count], 0.5f), this.cornerPoints[i], this.cornerPoints[(i + 1) % this.nodePoints.Count], area_id, (area_id - 1) * 4 + i, type);
			Vector3 vector = new Vector3((float)((int)this.nodePoints[i].pos.x), (float)((int)this.nodePoints[i].pos.y), (float)((int)this.nodePoints[i].pos.z));
			PassableArea.Node node;
			NativeMultiHashMapIterator<Vector3> nativeMultiHashMapIterator;
			if (!this.pam.map.TryGetFirstValue(vector, out node, out nativeMultiHashMapIterator))
			{
				this.pam.map_coords.Add(vector);
			}
			this.pam.map.Add(vector, this.nodePoints[i]);
		}
		this.BuildMesh();
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x000BD634 File Offset: 0x000BB834
	private void BuildMesh()
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.cornerPoints.Count; i++)
		{
			list.Add(base.transform.InverseTransformPoint(this.cornerPoints[i]));
		}
		bool flag = (this.area_type & (PathData.PassableArea.Type)278) == PathData.PassableArea.Type.None;
		if (flag)
		{
			for (int j = 0; j < this.cornerPoints.Count; j++)
			{
				list.Add(base.transform.InverseTransformPoint(global::Common.SnapToTerrain(this.cornerPoints[j], 0f, null, -1f, false)));
			}
		}
		Vector3[] array = list.ToArray();
		int[] triangles;
		if (Vector3.Angle(Vector3.Cross(array[0] - array[1], array[0] - array[2]), Vector3.up) <= 90f)
		{
			if (flag)
			{
				triangles = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					0,
					5,
					1,
					0,
					4,
					5,
					1,
					6,
					2,
					1,
					5,
					6,
					2,
					7,
					3,
					2,
					6,
					7,
					3,
					4,
					0,
					3,
					7,
					4
				};
			}
			else
			{
				triangles = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3
				};
			}
		}
		else if (flag)
		{
			triangles = new int[]
			{
				0,
				2,
				1,
				0,
				3,
				2,
				0,
				1,
				5,
				0,
				5,
				4,
				1,
				2,
				6,
				1,
				6,
				5,
				2,
				3,
				7,
				2,
				7,
				6,
				3,
				0,
				4,
				3,
				4,
				7
			};
		}
		else
		{
			triangles = new int[]
			{
				0,
				2,
				1,
				0,
				3,
				2
			};
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
			this.mesh.vertices = array;
			this.mesh.triangles = triangles;
			this.mesh.RecalculateNormals();
			this.mesh.RecalculateBounds();
			if (this.area_type != PathData.PassableArea.Type.Ladder && this.area_type != PathData.PassableArea.Type.LadderExit)
			{
				base.gameObject.layer = LayerMask.NameToLayer("Terrain");
			}
			else
			{
				base.gameObject.layer = LayerMask.NameToLayer("Physics");
			}
			base.gameObject.AddComponent<MeshFilter>().mesh = this.mesh;
			base.gameObject.AddComponent<MeshCollider>();
		}
		else
		{
			this.mesh.vertices = array;
			this.mesh.triangles = triangles;
			this.mesh.RecalculateNormals();
			this.mesh.RecalculateBounds();
			base.gameObject.GetComponent<MeshCollider>().sharedMesh = this.mesh;
		}
		this.rend = base.gameObject.GetComponent<MeshRenderer>();
		if (this.rend == null)
		{
			this.rend = base.gameObject.AddComponent<MeshRenderer>();
		}
		this.rend.materials[0] = this.mat;
		this.SetVisible(false);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x000BD8B2 File Offset: 0x000BBAB2
	public Transform GetCornerTransform(int corner)
	{
		if (base.transform.childCount <= corner)
		{
			return null;
		}
		return base.transform.Find("Corner " + corner);
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x000BD8E0 File Offset: 0x000BBAE0
	public void SetCornerTransform(Vector3 pos, int corner)
	{
		Transform cornerTransform = this.GetCornerTransform(corner);
		if (cornerTransform == null)
		{
			return;
		}
		cornerTransform.position = pos;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x000BD908 File Offset: 0x000BBB08
	public static bool WeldCorners(PassableArea a, PassableArea b, float max_dist = 0.5f, List<PassableArea> snapped = null)
	{
		for (int i = 0; i < 4; i++)
		{
			Transform cornerTransform = a.GetCornerTransform(i);
			if (cornerTransform == null)
			{
				return false;
			}
			Vector3 position = cornerTransform.position;
			int num = -1;
			float num2 = max_dist;
			bool flag = false;
			for (int j = 0; j < 4; j++)
			{
				if (a.nodePoints[i].type != PathData.PassableAreaNode.Type.Ground || b.nodePoints[j].type != PathData.PassableAreaNode.Type.Ground)
				{
					Transform cornerTransform2 = b.GetCornerTransform(j);
					if (cornerTransform2 == null)
					{
						return false;
					}
					if (a.node_connections != null)
					{
						bool flag2 = false;
						for (int k = 0; k < 4; k++)
						{
							PassableArea.NodeConnection nodeConnection = a.node_connections[k];
							if (nodeConnection.connected_to != null && nodeConnection.connected_to.Contains(cornerTransform))
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							flag = true;
							break;
						}
					}
					Vector3 position2 = cornerTransform2.position;
					float num3 = Vector3.Distance(position, position2);
					if (num3 < num2)
					{
						num2 = num3;
						num = j;
					}
				}
			}
			if (!flag && num != -1)
			{
				a.SetCornerTransform(b.GetCornerTransform(num).position, i);
				PassableArea.ConnectNodes(a, b, i, num, false);
				a.snapped = true;
			}
		}
		return true;
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000BDA40 File Offset: 0x000BBC40
	public static bool WeldAllCorners(List<PassableArea> areas = null)
	{
		if (areas == null)
		{
			areas = new List<PassableArea>(UnityEngine.Object.FindObjectsOfType<PassableArea>());
		}
		for (int i = 0; i < areas.Count; i++)
		{
			for (int j = 0; j < areas.Count; j++)
			{
				if (i != j && !areas[j].snapped)
				{
					PassableArea.WeldCorners(areas[i], areas[j], areas[i].weld_dist, null);
				}
			}
		}
		return true;
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x000BDAB4 File Offset: 0x000BBCB4
	public static void InitConnections(PassableArea area)
	{
		if (area.node_connections == null)
		{
			area.node_connections = new List<PassableArea.NodeConnection>();
		}
		for (int i = area.node_connections.Count; i < 4; i++)
		{
			area.node_connections.Add(new PassableArea.NodeConnection());
		}
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x000BDAFC File Offset: 0x000BBCFC
	public static void ConnectNodes(PassableArea a, PassableArea b, int a_index, int b_index, bool exclusive = false)
	{
		PassableArea.InitConnections(a);
		PassableArea.InitConnections(b);
		Transform cornerTransform = a.GetCornerTransform(a_index);
		Transform cornerTransform2 = b.GetCornerTransform(b_index);
		cornerTransform2.position = cornerTransform.position;
		if (exclusive)
		{
			a.node_connections[a_index].connected_to.Clear();
			b.node_connections[b_index].connected_to.Clear();
			a.node_connections[a_index].connected_to.Add(cornerTransform2);
			b.node_connections[b_index].connected_to.Add(cornerTransform);
			return;
		}
		if (!b.node_connections[b_index].connected_to.Contains(cornerTransform))
		{
			for (int i = 0; i < b.node_connections[b_index].connected_to.Count; i++)
			{
				Transform transform = b.node_connections[b_index].connected_to[i];
				if (!(transform == null))
				{
					transform.position = cornerTransform2.position;
				}
			}
			b.node_connections[b_index].connected_to.Add(cornerTransform);
		}
		if (!a.node_connections[a_index].connected_to.Contains(cornerTransform2))
		{
			for (int j = 0; j < a.node_connections[a_index].connected_to.Count; j++)
			{
				Transform transform2 = a.node_connections[a_index].connected_to[j];
				if (!(transform2 == null))
				{
					transform2.position = cornerTransform.position;
				}
			}
			a.node_connections[a_index].connected_to.Add(cornerTransform2);
		}
	}

	// Token: 0x04000C26 RID: 3110
	public int id;

	// Token: 0x04000C27 RID: 3111
	public List<Vector3> cornerPoints = new List<Vector3>
	{
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3)
	};

	// Token: 0x04000C28 RID: 3112
	public List<PassableArea.Node> nodePoints = new List<PassableArea.Node>
	{
		default(PassableArea.Node),
		default(PassableArea.Node),
		default(PassableArea.Node),
		default(PassableArea.Node)
	};

	// Token: 0x04000C29 RID: 3113
	public PathData.PassableArea.Type area_type = PathData.PassableArea.Type.Normal;

	// Token: 0x04000C2A RID: 3114
	[HideInInspector]
	public Vector3 normal;

	// Token: 0x04000C2B RID: 3115
	[NonSerialized]
	public bool snapped;

	// Token: 0x04000C2C RID: 3116
	[HideInInspector]
	public int battle_side = -1;

	// Token: 0x04000C2D RID: 3117
	public List<PassableArea.NodeConnection> node_connections = new List<PassableArea.NodeConnection>();

	// Token: 0x04000C2E RID: 3118
	public Mesh mesh;

	// Token: 0x04000C2F RID: 3119
	public PassableAreaManager pam;

	// Token: 0x04000C30 RID: 3120
	private MeshRenderer rend;

	// Token: 0x04000C31 RID: 3121
	public Material mat;

	// Token: 0x04000C32 RID: 3122
	public float weld_dist = 0.75f;

	// Token: 0x02000689 RID: 1673
	[Serializable]
	public struct Node
	{
		// Token: 0x060047EB RID: 18411 RVA: 0x00215E4E File Offset: 0x0021404E
		public Node(Vector3 pos, Vector3 corner1, Vector3 corner2, int area_id, int id, PathData.PassableAreaNode.Type type)
		{
			this.pos = pos;
			this.area_id = area_id;
			this.id = id;
			this.type = type;
			this.cornerPoint1 = corner1;
			this.cornerPoint2 = corner2;
		}

		// Token: 0x040035DD RID: 13789
		public Vector3 pos;

		// Token: 0x040035DE RID: 13790
		public Vector3 cornerPoint1;

		// Token: 0x040035DF RID: 13791
		public Vector3 cornerPoint2;

		// Token: 0x040035E0 RID: 13792
		public int area_id;

		// Token: 0x040035E1 RID: 13793
		public int id;

		// Token: 0x040035E2 RID: 13794
		public PathData.PassableAreaNode.Type type;
	}

	// Token: 0x0200068A RID: 1674
	[Serializable]
	public class NodeConnection
	{
		// Token: 0x040035E3 RID: 13795
		public List<Transform> connected_to = new List<Transform>();
	}
}
