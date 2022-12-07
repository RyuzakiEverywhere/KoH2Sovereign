using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class TerrainPathFinder
{
	// Token: 0x06001532 RID: 5426 RVA: 0x000D6EE0 File Offset: 0x000D50E0
	public int GetSteps()
	{
		return this.total_steps;
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x000D6EE8 File Offset: 0x000D50E8
	public bool IsDone()
	{
		return this.done;
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x000D6EF0 File Offset: 0x000D50F0
	public bool Succeeded()
	{
		return this.succeeded;
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000D6EF8 File Offset: 0x000D50F8
	private TerrainPathFinder.Node GetNode(int x, int y)
	{
		return this.nodes[x, y];
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000D6F07 File Offset: 0x000D5107
	private TerrainPathFinder.Node GetNode(TerrainPathFinder.NodeCoords ptg)
	{
		return this.GetNode(ptg.x, ptg.y);
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x000D6F1B File Offset: 0x000D511B
	private Vector3 GridToWorld(TerrainPathFinder.NodeCoords ptg)
	{
		return TerrainInfo.GridToWorld(this.GridToTI(ptg), false);
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x000D6F2A File Offset: 0x000D512A
	private TerrainPathFinder.NodeCoords WorldToGrid(Vector3 ptw)
	{
		return this.TIToGrid(TerrainInfo.WorldToGrid(ptw));
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000D6F38 File Offset: 0x000D5138
	private TerrainInfo.Coords GridToTI(TerrainPathFinder.NodeCoords ptg)
	{
		return new TerrainInfo.Coords(ptg.x - this.ptTIOfs.x, ptg.y - this.ptTIOfs.y);
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000D6F63 File Offset: 0x000D5163
	private TerrainPathFinder.NodeCoords TIToGrid(TerrainInfo.Coords ptti)
	{
		return new TerrainPathFinder.NodeCoords(ptti.x + this.ptTIOfs.x, ptti.y + this.ptTIOfs.y);
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x000D6F90 File Offset: 0x000D5190
	private void CreateNodes()
	{
		this.size.x = (this.size.y = 0);
		this.nodes = null;
		Vector3 vector = (this.ptwDest - this.ptwStart) / TerrainInfo.cell_size;
		int num = (int)Mathf.Abs(vector.x);
		int num2 = (int)Mathf.Abs(vector.z);
		if (num2 > num)
		{
			num = num2;
		}
		else
		{
			num2 = num;
		}
		int num3 = (int)(0.5f * (float)num2) + 50;
		int num4 = (int)(0.5f * (float)num) + 50;
		this.size.x = 2 * num3 + num;
		this.size.y = 2 * num4 + num2;
		this.ptgStart.x = ((vector.x < 0f) ? (this.size.x - num3) : num3);
		this.ptgStart.y = ((vector.z < 0f) ? (this.size.y - num4) : num4);
		this.ptgEnd.x = this.ptgStart.x + (int)vector.x;
		this.ptgEnd.y = this.ptgStart.y + (int)vector.z;
		TerrainInfo.Coords coords = TerrainInfo.WorldToGrid(this.ptwStart);
		this.ptTIOfs.x = this.ptgStart.x - coords.x;
		this.ptTIOfs.y = this.ptgStart.y - coords.y;
		this.nodes = new TerrainPathFinder.Node[this.size.x, this.size.y];
		for (int i = 0; i < this.size.y; i++)
		{
			for (int j = 0; j < this.size.x; j++)
			{
				this.nodes[j, i] = new TerrainPathFinder.Node();
			}
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x000D717C File Offset: 0x000D537C
	private void HeapPush(TerrainPathFinder.NodeCoords ptg, float eval)
	{
		TerrainPathFinder.OpenNode openNode = new TerrainPathFinder.OpenNode(ptg, eval);
		int i = this.open.Count;
		this.open.Add(openNode);
		while (i > 0)
		{
			int num = TerrainPathFinder.HeapParent(i);
			if (eval > this.open[num].eval)
			{
				break;
			}
			this.open[i] = this.open[num];
			i = num;
		}
		this.open[i] = openNode;
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x000D71F4 File Offset: 0x000D53F4
	private void HeapPop()
	{
		int count = this.open.Count;
		if (count <= 0)
		{
			return;
		}
		TerrainPathFinder.OpenNode openNode = this.open[count - 1];
		int num = 0;
		for (int i = TerrainPathFinder.HeapLeftChild(num); i < this.open.Count; i = TerrainPathFinder.HeapLeftChild(num))
		{
			int num2 = TerrainPathFinder.HeapRightFromLeft(i);
			int num3 = (num2 < count && this.open[num2].eval < this.open[i].eval) ? num2 : i;
			if (this.open[num3].eval >= openNode.eval)
			{
				break;
			}
			this.open[num] = this.open[num3];
			num = num3;
		}
		this.open[num] = openNode;
		this.open.RemoveAt(count - 1);
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x000D72CE File Offset: 0x000D54CE
	private static int HeapParent(int i)
	{
		return (i - 1) / 2;
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000D72D5 File Offset: 0x000D54D5
	private static int HeapLeftChild(int i)
	{
		return i * 2 + 1;
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000D72DC File Offset: 0x000D54DC
	private static int HeapRightFromLeft(int i)
	{
		return i + 1;
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000D72E4 File Offset: 0x000D54E4
	private float Estimate(TerrainPathFinder.NodeCoords ptg)
	{
		int a = Mathf.Abs(ptg.x - this.ptgEnd.x);
		int b = Mathf.Abs(ptg.y - this.ptgEnd.y);
		int num = Mathf.Max(a, b);
		int num2 = Mathf.Min(a, b);
		int num3 = num2;
		return ((float)(num - num2) + (float)num3 * 1.4142f) * TerrainInfo.cell_size;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x000D7344 File Offset: 0x000D5544
	private bool Open(TerrainPathFinder.NodeCoords ptg, TerrainPathFinder.NodeCoords ptgPrev, bool riverJump)
	{
		if (ptg.x < 0 || ptg.x >= this.size.x || ptg.y < 0 || ptg.y >= this.size.y)
		{
			return false;
		}
		TerrainInfo.Coords coords = this.GridToTI(ptg);
		if (coords.x < 0 || coords.x >= TerrainInfo.width || coords.y < 0 || coords.y >= TerrainInfo.height)
		{
			return false;
		}
		Vector3 a = TerrainInfo.GridToWorld(coords, false);
		TerrainInfo.Coords ptg2 = this.GridToTI(ptgPrev);
		Vector3 b = TerrainInfo.GridToWorld(ptg2, false);
		TerrainPathFinder.Node node = this.GetNode(ptg);
		TerrainPathFinder.Node node2 = this.GetNode(ptgPrev);
		float magnitude = (a - b).magnitude;
		if (node.weight < 0f || this.settings.river)
		{
			int num = 2;
			if (this.settings.minHeight > 0f)
			{
				num |= 1;
			}
			TerrainInfo.Cell cell = TerrainInfo.Get(coords, num);
			if (cell.rivers > 0 && !this.settings.river && !riverJump)
			{
				return false;
			}
			if (cell.wall)
			{
				return false;
			}
			if (this.settings.minHeight > 0f && cell.height < this.settings.minHeight)
			{
				TerrainInfo.Cell cell2 = TerrainInfo.Get(ptg2, 1);
				if (cell.height < cell2.height)
				{
					return false;
				}
			}
			float num2 = (float)cell.slope;
			if (num2 >= this.settings.maxSlope)
			{
				return false;
			}
			node.weight = 1f + num2 * this.settings.slopeAvoidance / 90f;
			if (cell.rivers > 0)
			{
				return false;
			}
			if (riverJump)
			{
				node.weight *= this.settings.riverAvoidance;
			}
			if (cell.roads > 0)
			{
				node.weight /= 1f + this.settings.pathStickiness;
			}
			if (node.weight < 0.0001f)
			{
				node.weight = 0.0001f;
			}
		}
		float num3 = magnitude * node.weight;
		float num4 = node2.path_eval + num3;
		if (node.closed && num4 >= node.path_eval)
		{
			return false;
		}
		node.closed = true;
		node.prev = ptgPrev;
		node.path_eval = num4;
		node.open = true;
		if (this.settings.astar)
		{
			float num5 = this.Estimate(ptg);
			if (TerrainInfo.Get(coords, 0).roads > 0 && !this.settings.river)
			{
				num5 /= 1f + this.settings.pathStickiness;
			}
			node.eval = node.path_eval + num5;
			this.HeapPush(ptg, node.eval);
		}
		else
		{
			this.open.Add(new TerrainPathFinder.OpenNode(ptg, 0f));
		}
		return true;
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000D760B File Offset: 0x000D580B
	private bool OpenNeighbor(TerrainPathFinder.NodeCoords ptg, int dx, int dy, bool riverJump = false)
	{
		return this.Open(new TerrainPathFinder.NodeCoords(ptg.x + dx, ptg.y + dy), ptg, riverJump);
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000D762B File Offset: 0x000D582B
	public void Clean()
	{
		this.nodes = null;
		this.open = null;
		this.done = true;
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000D7644 File Offset: 0x000D5844
	public void Begin(TerrainPathFinder.Settings settings, Vector3 ptwStart, Vector3 ptwDest, float range = 0f)
	{
		this.Clean();
		this.path_len = 0f;
		this.path = null;
		this.path_points = null;
		this.total_steps = 0;
		TerrainInfo.Update();
		this.settings = settings;
		this.ptwStart = ptwStart;
		this.ptwDest = ptwDest;
		this.range = range;
		this.done = false;
		this.CreateNodes();
		this.open = new List<TerrainPathFinder.OpenNode>();
		this.Open(this.ptgStart, this.ptgStart, false);
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000D76C6 File Offset: 0x000D58C6
	private void Done(bool success)
	{
		this.done = true;
		this.succeeded = success;
		this.nodes = null;
		this.open = null;
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000D76E4 File Offset: 0x000D58E4
	private bool IsGoal(TerrainPathFinder.NodeCoords ptg)
	{
		if (ptg.x == this.ptgEnd.x && ptg.y == this.ptgEnd.y)
		{
			return true;
		}
		if (this.range <= 0f)
		{
			return false;
		}
		Vector3 vector = this.GridToWorld(ptg) - this.ptwDest;
		vector.y = 0f;
		return vector.sqrMagnitude <= this.range * this.range;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000D7760 File Offset: 0x000D5960
	public bool Process(int steps)
	{
		if (this.done)
		{
			return true;
		}
		if (this.open == null)
		{
			this.Done(false);
			return true;
		}
		if (steps <= 0)
		{
			steps = this.open.Count;
			if (steps <= 0)
			{
				this.Done(false);
				return true;
			}
		}
		for (int i = 0; i < steps; i++)
		{
			this.total_steps++;
			if (this.total_steps > 100000)
			{
				this.Done(false);
				return true;
			}
			if (this.open.Count <= 0)
			{
				this.Done(false);
				return true;
			}
			TerrainPathFinder.NodeCoords coords = this.open[0].coords;
			if (this.settings.astar)
			{
				this.HeapPop();
			}
			else
			{
				this.open.RemoveAt(0);
			}
			TerrainPathFinder.Node node = this.GetNode(coords);
			if (node.open)
			{
				node.open = false;
				if (this.IsGoal(coords))
				{
					this.GeneratePath(coords);
					this.GeneratePathPoints();
					this.Done(this.path_points != null);
					return true;
				}
				TerrainInfo.Cell cell = TerrainInfo.Get(this.GridToTI(coords), 0);
				if (!this.settings.greedy || this.ptgEnd.x < coords.x || this.ptgEnd.y < coords.y)
				{
					this.OpenNeighbor(coords, -1, -1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.y < coords.y)
				{
					this.OpenNeighbor(coords, 0, -1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.x > coords.x || this.ptgEnd.y < coords.y)
				{
					this.OpenNeighbor(coords, 1, -1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.x > coords.x)
				{
					this.OpenNeighbor(coords, 1, 0, false);
				}
				if (!this.settings.greedy || this.ptgEnd.x > coords.x || this.ptgEnd.y > coords.y)
				{
					this.OpenNeighbor(coords, 1, 1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.y > coords.y)
				{
					this.OpenNeighbor(coords, 0, 1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.x < coords.x || this.ptgEnd.y > coords.y)
				{
					this.OpenNeighbor(coords, -1, 1, false);
				}
				if (!this.settings.greedy || this.ptgEnd.x < coords.x)
				{
					this.OpenNeighbor(coords, -1, 0, false);
				}
				if (cell.river_offset != 0)
				{
					int num = cell.river_offset >> 7;
					int num2 = cell.river_offset >> 4 & 7;
					int num3 = cell.river_offset >> 3 & 1;
					int num4 = (int)(cell.river_offset & 7);
					int num5 = 3;
					Coord coord = new Coord(((num == 1) ? 1 : -1) * num2 * num5, ((num3 == 1) ? 1 : -1) * num4 * num5);
					this.OpenNeighbor(coords, coord.x, coord.y, true);
				}
			}
		}
		return false;
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x000D7A90 File Offset: 0x000D5C90
	private void GeneratePath(TerrainPathFinder.NodeCoords ptg)
	{
		this.path = new List<TerrainInfo.Coords>();
		for (;;)
		{
			TerrainPathFinder.Node node = this.GetNode(ptg);
			TerrainInfo.Coords item = this.GridToTI(ptg);
			this.path.Insert(0, item);
			if (ptg.x == this.ptgStart.x && ptg.y == this.ptgStart.y)
			{
				break;
			}
			ptg = node.prev;
		}
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x000D7AF8 File Offset: 0x000D5CF8
	public void GeneratePathPoints()
	{
		if (this.path == null || this.path.Count < 2)
		{
			return;
		}
		this.ptwStart.y = 0f;
		this.ptwDest.y = 0f;
		this.path_points = new List<Vector3>();
		this.path_len = 0f;
		this.path_points.Add(this.ptwStart);
		Vector3 a = this.ptwStart;
		int num = (this.range <= 0f) ? (this.path.Count - 1) : this.path.Count;
		for (int i = 1; i < num; i++)
		{
			Vector3 vector = TerrainInfo.GridToWorld(this.path[i], true);
			Vector3 vector2 = (a + vector) / 2f;
			a = vector;
			vector = vector2;
			vector.y = 0f;
			this.path_len += (vector - this.path_points[this.path_points.Count - 1]).magnitude;
			this.path_points.Add(vector);
		}
		if (this.range <= 0f)
		{
			this.path_len += (this.ptwDest - this.path_points[this.path_points.Count - 1]).magnitude;
			this.path_points.Add(this.ptwDest);
		}
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x000D7C68 File Offset: 0x000D5E68
	public static bool GetPathPoint(List<Vector3> path_points, float dist, out Vector3 pt, out Vector3 ptDest, float fDestLookAhead = 0.5f)
	{
		if (path_points.Count < 2)
		{
			pt = (ptDest = path_points[path_points.Count - 1]);
			return false;
		}
		Vector3 vector = path_points[0];
		pt = vector;
		ptDest = path_points[1];
		if (dist < 0f)
		{
			return false;
		}
		bool flag = false;
		for (int i = 1; i < path_points.Count; i++)
		{
			Vector3 vector2 = path_points[i];
			Vector3 a = vector2 - vector;
			float magnitude = a.magnitude;
			if (magnitude < dist)
			{
				dist -= magnitude;
				vector = vector2;
			}
			else
			{
				if (flag)
				{
					ptDest = vector + a * dist / magnitude;
					return true;
				}
				flag = true;
				pt = vector + a * dist / magnitude;
				ptDest = vector2;
				if (fDestLookAhead <= 0f)
				{
					return true;
				}
				dist += fDestLookAhead;
				if (dist <= magnitude)
				{
					ptDest = vector + a * dist / magnitude;
					return true;
				}
				dist -= magnitude;
				vector = vector2;
			}
		}
		ptDest = path_points[path_points.Count - 1];
		if (flag)
		{
			return true;
		}
		pt = ptDest;
		return false;
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000D7DB4 File Offset: 0x000D5FB4
	private Vector3 GetTerrainPoint(TerrainInfo.Coords ptti, float hofs)
	{
		return global::Common.SnapToTerrain(TerrainInfo.GridToWorld(ptti, false), hofs, null, -1f, false);
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x000D7DCC File Offset: 0x000D5FCC
	public void DrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(this.ptwDest, Vector3.one * TerrainInfo.cell_size);
		if (this.open == null || this.open.Count < 1)
		{
			return;
		}
		for (int i = 0; i < this.open.Count; i++)
		{
			TerrainPathFinder.NodeCoords coords = this.open[i].coords;
			this.GetNode(coords);
			if (i == 0)
			{
				Gizmos.color = Color.red;
				this.GeneratePath(coords);
				Vector3 from = this.GetTerrainPoint(this.path[0], 0.5f);
				for (int j = 1; j < this.path.Count; j++)
				{
					Vector3 terrainPoint = this.GetTerrainPoint(this.path[j], 0.5f);
					Gizmos.DrawLine(from, terrainPoint);
					from = terrainPoint;
				}
			}
			else
			{
				TerrainInfo.Cell cell = TerrainInfo.Get(this.GridToTI(coords), 0);
				if (cell.river_offset != 0)
				{
					Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
				}
				else if (cell.rivers > 0)
				{
					Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
				}
				else
				{
					Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
				}
			}
			Gizmos.DrawCube(global::Common.SnapToTerrain(this.GridToWorld(coords), 0.5f, null, -1f, false), Vector3.one * TerrainInfo.cell_size);
		}
	}

	// Token: 0x04000D94 RID: 3476
	public TerrainPathFinder.Settings settings;

	// Token: 0x04000D95 RID: 3477
	private Vector3 ptwStart;

	// Token: 0x04000D96 RID: 3478
	private Vector3 ptwDest;

	// Token: 0x04000D97 RID: 3479
	private float range;

	// Token: 0x04000D98 RID: 3480
	public List<TerrainInfo.Coords> path;

	// Token: 0x04000D99 RID: 3481
	public List<Vector3> path_points;

	// Token: 0x04000D9A RID: 3482
	public float path_len;

	// Token: 0x04000D9B RID: 3483
	private int total_steps;

	// Token: 0x04000D9C RID: 3484
	private bool done;

	// Token: 0x04000D9D RID: 3485
	private bool succeeded;

	// Token: 0x04000D9E RID: 3486
	private TerrainPathFinder.Node[,] nodes;

	// Token: 0x04000D9F RID: 3487
	private List<TerrainPathFinder.OpenNode> open;

	// Token: 0x04000DA0 RID: 3488
	private TerrainInfo.Coords ptTIOfs;

	// Token: 0x04000DA1 RID: 3489
	private TerrainPathFinder.NodeCoords ptgStart;

	// Token: 0x04000DA2 RID: 3490
	private TerrainPathFinder.NodeCoords ptgEnd;

	// Token: 0x04000DA3 RID: 3491
	private TerrainPathFinder.NodeCoords size;

	// Token: 0x020006BC RID: 1724
	[Serializable]
	public class Settings
	{
		// Token: 0x040036C6 RID: 14022
		public bool river;

		// Token: 0x040036C7 RID: 14023
		public float maxSlope = 60f;

		// Token: 0x040036C8 RID: 14024
		[NonSerialized]
		public float minHeight;

		// Token: 0x040036C9 RID: 14025
		public float slopeAvoidance = 10f;

		// Token: 0x040036CA RID: 14026
		public float riverAvoidance = 5f;

		// Token: 0x040036CB RID: 14027
		public float pathStickiness = 0.5f;

		// Token: 0x040036CC RID: 14028
		public bool astar = true;

		// Token: 0x040036CD RID: 14029
		public bool greedy;
	}

	// Token: 0x020006BD RID: 1725
	private struct NodeCoords
	{
		// Token: 0x06004883 RID: 18563 RVA: 0x00217F37 File Offset: 0x00216137
		public NodeCoords(int x = 0, int y = 0)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x040036CE RID: 14030
		public int x;

		// Token: 0x040036CF RID: 14031
		public int y;
	}

	// Token: 0x020006BE RID: 1726
	private class Node
	{
		// Token: 0x040036D0 RID: 14032
		public float weight = -1f;

		// Token: 0x040036D1 RID: 14033
		public bool open;

		// Token: 0x040036D2 RID: 14034
		public bool closed;

		// Token: 0x040036D3 RID: 14035
		public float path_eval;

		// Token: 0x040036D4 RID: 14036
		public float eval;

		// Token: 0x040036D5 RID: 14037
		public TerrainPathFinder.NodeCoords prev;
	}

	// Token: 0x020006BF RID: 1727
	private struct OpenNode
	{
		// Token: 0x06004885 RID: 18565 RVA: 0x00217F5A File Offset: 0x0021615A
		public OpenNode(TerrainPathFinder.NodeCoords coords, float eval)
		{
			this.coords = coords;
			this.eval = eval;
		}

		// Token: 0x040036D6 RID: 14038
		public TerrainPathFinder.NodeCoords coords;

		// Token: 0x040036D7 RID: 14039
		public float eval;
	}
}
