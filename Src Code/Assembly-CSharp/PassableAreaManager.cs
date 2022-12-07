using System;
using System.Collections.Generic;
using Logic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class PassableAreaManager : MonoBehaviour
{
	// Token: 0x06001217 RID: 4631 RVA: 0x000BDD60 File Offset: 0x000BBF60
	public void Clear()
	{
		if (this.last_removed_areas != null)
		{
			this.last_removed_areas.Clear();
		}
		if (this.map.IsCreated)
		{
			this.map.Clear();
		}
		if (this.map_coords != null)
		{
			this.map_coords.Clear();
		}
		if (this.areas != null)
		{
			this.areas.Clear();
		}
		this.force_update = true;
		this.RecheckPF();
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x000BDDCB File Offset: 0x000BBFCB
	private void OnEnable()
	{
		this.onFinishPathfinding = new PassableAreaManager.OnFinishPathfinding(this.OnFinishPathfindingBase);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x000BDDE0 File Offset: 0x000BBFE0
	private unsafe void OnFinishPathfindingBase()
	{
		this.pf.data.BuildRuntimeHighPF(this.areas.Count, this.pf.game);
		if (Troops.squads != null)
		{
			Troops.CompleteAllJobs();
			for (int i = 0; i < Troops.squads.Length; i++)
			{
				global::Squad squad = Troops.squads[i];
				if (!(squad == null))
				{
					if (squad.logic.movement.PlanningToMoveThroughPAIDs(this.last_removed_areas, -1f))
					{
						squad.logic.Stop(true);
					}
					PPos pos = new PPos(squad.logic.position, 0);
					Troops.Troop troop = squad.data->FirstTroop;
					while (troop <= squad.data->LastTroop)
					{
						if (this.last_removed_areas.Contains(troop.pa_id))
						{
							troop.Throw();
							troop.pa_id = 0;
							troop.tgt_pa_id = 0;
							if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
							{
								troop.Kill();
							}
						}
						else if (!troop.HasFlags(Troops.Troop.Flags.Dead))
						{
							pos = troop.pos;
						}
						troop = ++troop;
					}
					if (this.last_removed_areas.Contains(squad.logic.position.paID))
					{
						squad.logic.position = pos;
					}
					squad.data->move_history = default(Troops.MoveHistory);
					if (squad.logic.position.paID != 0)
					{
						PathData.PassableArea pa = Troops.path_data.GetPA(squad.logic.position.paID - 1);
						if (pa.normal != default(Point3))
						{
							PPos[] array = new PPos[2];
							array[0] = squad.logic.position + pa.normal.xz * 10f;
							array[0].paID = 0;
							array[1] = squad.logic.position - pa.normal.xz * 10f;
							array[1].paID = 0;
							squad.logic.ValidateIfStuck(array);
						}
					}
				}
			}
		}
		this.pf.data.processing = false;
		Troops.SetPathData(this.pf.data.pointers);
		if (this.pf.game.kingdoms != null)
		{
			for (int j = 0; j < this.pf.game.kingdoms.Count; j++)
			{
				Logic.Kingdom kingdom = this.pf.game.kingdoms[j];
				for (int k = 0; k < kingdom.armies.Count; k++)
				{
					Logic.Army army = kingdom.armies[k];
					if (army.movement.IsMoving(true))
					{
						army.NotifyListeners("path_changed", null);
					}
				}
			}
		}
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x000BE0E4 File Offset: 0x000BC2E4
	public unsafe void RecheckPF()
	{
		Logic.PathFinding pathFinding = this.pf;
		if (((pathFinding != null) ? pathFinding.data : null) == null)
		{
			return;
		}
		if (this.pf.data.initted && this.pf.data.pointers.Initted != null && (this.force_update || !(*this.pf.data.pointers.Initted)))
		{
			this.ProcessAllAreas();
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x000BE158 File Offset: 0x000BC358
	public unsafe void Update()
	{
		if (this.pf != null && !this.pf.IsValid())
		{
			Game game = GameLogic.Get(true);
			this.pf = ((game != null) ? game.path_finding : null);
			return;
		}
		Logic.PathFinding pathFinding = this.pf;
		if (((pathFinding != null) ? pathFinding.data : null) == null)
		{
			return;
		}
		if (this.pf.data.processing)
		{
			if (!this.force_update)
			{
				Logic.PathFinding pathFinding2 = this.pf;
				bool flag;
				if (pathFinding2 == null)
				{
					flag = false;
				}
				else
				{
					PathData data = pathFinding2.data;
					if (data == null)
					{
						flag = false;
					}
					else
					{
						PathData.DataPointers pointers = data.pointers;
						flag = true;
					}
				}
				if (flag && this.pf.data.pointers.Initted != null && *this.pf.data.pointers.Initted)
				{
					this.onFinishPathfinding();
				}
			}
			return;
		}
		this.RecheckPF();
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x000BE228 File Offset: 0x000BC428
	public void RecalculateAreas()
	{
		Logic.PathFinding pathFinding = this.pf;
		if (((pathFinding != null) ? pathFinding.data : null) == null || this.pf.data.processing)
		{
			return;
		}
		this.force_update = true;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x000BE264 File Offset: 0x000BC464
	public PassableArea.Node ProcessNodeToLogic(List<PassableArea.Node> nodes, int node_index, ref PathData.PassableAreaNode lpn, ref PathData.Node ln, int i, ref PathData.PassableArea area)
	{
		int num = 1 + i / PathData.PassableArea.numNodes;
		PassableArea.Node node = nodes[node_index];
		lpn.type = node.type;
		if (node.type == PathData.PassableAreaNode.Type.Ground && area.type != PathData.PassableArea.Type.Teleport)
		{
			area.type = PathData.PassableArea.Type.ForcedGround;
		}
		lpn.pos = new PPos(nodes[node_index].pos, num);
		ln.estimate = 1;
		ln.clearance = byte.MaxValue;
		ln.weight = 1;
		ln.lsa = 2;
		ln.slope = 1;
		ln.pa_id = (short)num;
		PassableArea.Node node2 = new PassableArea.Node(node.pos, node.cornerPoint1, node.cornerPoint2, node.area_id, i, node.type);
		nodes[node_index] = node2;
		return node2;
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x000BE330 File Offset: 0x000BC530
	public void GatherAllAreas()
	{
		this.last_removed_areas.Clear();
		foreach (PassableArea area in UnityEngine.Object.FindObjectsOfType<PassableArea>())
		{
			this.AddArea(area);
		}
		Wall[] array2 = UnityEngine.Object.FindObjectsOfType<Wall>();
		bool flag = true;
		for (int j = 0; j < array2.Length; j++)
		{
			if (!array2[j].AlignPassableAreas(false))
			{
				flag = false;
				break;
			}
		}
		for (int k = 0; k < this.areas.Count; k++)
		{
			PassableArea passableArea = this.areas[k];
			if (!(passableArea == null) && !passableArea.gameObject.activeInHierarchy && passableArea.id > 0 && this.pf.data.pas.Length > k && this.pf.data.pas[k].enabled)
			{
				this.last_removed_areas.Add(passableArea.id);
			}
		}
		if (flag)
		{
			PassableArea.WeldAllCorners(this.areas);
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x000BE436 File Offset: 0x000BC636
	public void AddArea(PassableArea area)
	{
		if (area.id > 0)
		{
			return;
		}
		this.areas.Add(area);
		area.pam = this;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x000BE458 File Offset: 0x000BC658
	public unsafe void ProcessAllAreas()
	{
		if (!this.force_update)
		{
			Logic.PathFinding pathFinding = this.pf;
			if (((pathFinding != null) ? pathFinding.data : null) != null)
			{
				Logic.PathFinding pathFinding2 = this.pf;
				if (((pathFinding2 != null) ? pathFinding2.data.pas : null) != null)
				{
					Logic.PathFinding pathFinding3 = this.pf;
					if (pathFinding3 == null || pathFinding3.data.pas.Length != 0)
					{
						return;
					}
				}
			}
		}
		if (!this.force_update && this.pf.data.pointers.Initted != null && *this.pf.data.pointers.Initted)
		{
			return;
		}
		this.GatherAllAreas();
		this.pf.data.processing = true;
		this.force_update = false;
		int numNodes = PathData.PassableArea.numNodes;
		if (this.map.IsCreated)
		{
			this.map.Clear();
		}
		else
		{
			this.map = new NativeMultiHashMap<Vector3, PassableArea.Node>(32767 * numNodes, Allocator.Persistent);
		}
		this.map_coords.Clear();
		for (int i = 0; i < this.areas.Count; i++)
		{
			this.areas[i].ApplyCorners(this.pf.game, i + 1);
			this.areas[i].id = i + 1;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		PathData data = this.pf.data;
		bool flag = false;
		if (data.pas == null || data.pas.Length == 0)
		{
			flag = true;
			data.pas = new PathData.PassableArea[this.areas.Count];
			data.paNodes = new PathData.PassableAreaNode[this.areas.Count * numNodes];
			data.paNormalNodes = new PathData.Node[this.areas.Count * numNodes];
		}
		for (int j = 0; j < data.nodes.Length; j++)
		{
			data.nodes[j].pa_id = 0;
		}
		for (int k = 0; k < this.areas.Count; k++)
		{
			PassableArea passableArea = this.areas[k];
			bool activeInHierarchy = passableArea.gameObject.activeInHierarchy;
			PathData.PassableArea passableArea2 = data.pas[k];
			passableArea2.enabled = activeInHierarchy;
			passableArea2.normal = passableArea.normal;
			if (flag)
			{
				passableArea2.type = passableArea.area_type;
				passableArea2.battle_side = passableArea.battle_side;
				passableArea2.FriendsCanEnter = true;
			}
			bool connected_to_ground = false;
			for (int l = 0; l < PathData.PassableArea.numNodes; l++)
			{
				passableArea2.SetCorner(l, passableArea.cornerPoints[l]);
				if (this.ProcessNodeToLogic(passableArea.nodePoints, l, ref data.paNodes[k * numNodes + l], ref data.paNormalNodes[k * numNodes + l], k * numNodes + l, ref passableArea2).type == PathData.PassableAreaNode.Type.Ground)
				{
					connected_to_ground = true;
				}
			}
			passableArea2.connected_to_ground = connected_to_ground;
			data.pas[k] = passableArea2;
		}
		data.pointers.Dispose();
		NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(this.map_coords.ToArray(), Allocator.Persistent);
		data.pointers = new PathData.DataPointers(ref data);
		this.assign_job = new PassableAreaManager.AssignPathfinding
		{
			map = this.map,
			map_coords = nativeArray,
			xOffset = data.width,
			tileSize = data.settings.tile_size,
			pPFData = data.pointers
		};
		this.finish_job = new PassableAreaManager.FinishPathfinding
		{
			pPFData = data.pointers
		};
		if (Troops.debug_jobs)
		{
			this.assign_job.Run(this.map_coords.Count);
			this.finish_job.Run(1);
		}
		else
		{
			this.cur_job_handle = this.assign_job.Schedule(this.map_coords.Count, this.map_coords.Count / SystemInfo.processorCount, this.cur_job_handle);
			this.cur_job_handle = this.finish_job.Schedule(1, 1, this.cur_job_handle);
			this.cur_job_handle.Complete();
		}
		nativeArray.Dispose();
		this.map.Dispose();
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x000BE880 File Offset: 0x000BCA80
	public static void SetAreaPaids(GameObject go, Logic.PathFinding path_finding, List<int> paids = null, List<int> middle_paids = null)
	{
		PassableArea[] componentsInChildren = go.GetComponentsInChildren<PassableArea>(true);
		if (paids != null)
		{
			paids.Clear();
		}
		if (middle_paids != null)
		{
			middle_paids.Clear();
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			int id = componentsInChildren[i].id;
			if (paids != null)
			{
				paids.Add(id);
			}
			if (middle_paids != null && !path_finding.data.pointers.GetPA(id - 1).connected_to_ground)
			{
				middle_paids.Add(id);
			}
		}
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x000BE8EC File Offset: 0x000BCAEC
	~PassableAreaManager()
	{
		if (this.map.IsCreated)
		{
			this.map.Dispose();
		}
	}

	// Token: 0x04000C33 RID: 3123
	public bool force_update;

	// Token: 0x04000C34 RID: 3124
	public Logic.PathFinding pf;

	// Token: 0x04000C35 RID: 3125
	public List<PassableArea> areas = new List<PassableArea>();

	// Token: 0x04000C36 RID: 3126
	public List<Vector3> map_coords = new List<Vector3>();

	// Token: 0x04000C37 RID: 3127
	public NativeMultiHashMap<Vector3, PassableArea.Node> map;

	// Token: 0x04000C38 RID: 3128
	public List<int> last_removed_areas = new List<int>();

	// Token: 0x04000C39 RID: 3129
	private PassableAreaManager.AssignPathfinding assign_job;

	// Token: 0x04000C3A RID: 3130
	private PassableAreaManager.FinishPathfinding finish_job;

	// Token: 0x04000C3B RID: 3131
	private JobHandle cur_job_handle;

	// Token: 0x04000C3C RID: 3132
	public PassableAreaManager.OnFinishPathfinding onFinishPathfinding;

	// Token: 0x0200068B RID: 1675
	[BurstCompile]
	public struct AssignPathfinding : IJobParallelFor
	{
		// Token: 0x060047ED RID: 18413 RVA: 0x00215E90 File Offset: 0x00214090
		public void Execute(int index)
		{
			Vector3 vector = this.map_coords[index];
			PassableArea.Node cur_node;
			NativeMultiHashMapIterator<Vector3> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(vector, out cur_node, out nativeMultiHashMapIterator))
			{
				return;
			}
			this.CoordIterator(cur_node, vector);
			while (this.map.TryGetNextValue(out cur_node, ref nativeMultiHashMapIterator))
			{
				this.CoordIterator(cur_node, vector);
			}
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x00215EE0 File Offset: 0x002140E0
		private void CoordIterator(PassableArea.Node cur_node, Vector3 coord)
		{
			if (this.pPFData.GetPA(cur_node.area_id - 1).enabled)
			{
				PassableArea.Node node;
				NativeMultiHashMapIterator<Vector3> nativeMultiHashMapIterator;
				if (this.map.TryGetFirstValue(coord, out node, out nativeMultiHashMapIterator))
				{
					if (cur_node.id != node.id && Vector3.Distance(cur_node.pos, node.pos) < 0.1f && this.ConnectAdjacentNodes(cur_node, node))
					{
						return;
					}
					while (this.map.TryGetNextValue(out node, ref nativeMultiHashMapIterator))
					{
						if (cur_node.id != node.id && Vector3.Distance(cur_node.pos, node.pos) < 0.1f && this.ConnectAdjacentNodes(cur_node, node))
						{
							return;
						}
					}
				}
				if (cur_node.type == PathData.PassableAreaNode.Type.Ground)
				{
					int num = (int)(cur_node.pos.x / this.tileSize);
					int num2 = (int)(cur_node.pos.z / this.tileSize);
					PathData.PassableAreaNode panode = this.pPFData.GetPANode(cur_node.id);
					PathData.Node panormalNode = this.pPFData.GetPANormalNode(cur_node.id);
					PathData.Node node2 = this.pPFData.GetNode(num2 * this.xOffset + num);
					panode.link = num - this.xOffset;
					panormalNode.pa_id = 0;
					node2.pa_id = (short)cur_node.area_id;
					this.pPFData.SetPANode(cur_node.id, panode);
					this.pPFData.SetPANormalNode(cur_node.id, panormalNode);
					this.pPFData.SetNode(num2 * this.xOffset + num, node2);
					Point pt = cur_node.cornerPoint1;
					Point pt2 = cur_node.cornerPoint2;
					Coord coord2 = Coord.WorldToGrid(pt, this.tileSize);
					Point point = Coord.WorldToLocal(coord2, pt, this.tileSize);
					Point point2 = Coord.WorldToLocal(coord2, pt2, this.tileSize);
					Coord coord3;
					Coord coord4;
					while (Coord.RayStep(ref coord2, ref point, ref point2, 0.5f, out coord3, out coord4))
					{
						node2 = this.pPFData.GetNode(coord2.y * this.xOffset + coord2.x);
						node2.pa_id = (short)cur_node.area_id;
						this.pPFData.SetNode(coord2.y * this.xOffset + coord2.x, node2);
						if (coord3.valid)
						{
							node2 = this.pPFData.GetNode(coord3.y * this.xOffset + coord3.x);
							node2.pa_id = (short)cur_node.area_id;
							this.pPFData.SetNode(coord3.y * this.xOffset + coord3.x, node2);
						}
						if (coord4.valid)
						{
							node2 = this.pPFData.GetNode(coord4.y * this.xOffset + coord4.x);
							node2.pa_id = (short)cur_node.area_id;
							this.pPFData.SetNode(coord4.y * this.xOffset + coord4.x, node2);
						}
					}
					return;
				}
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						for (int k = -1; k <= 1; k++)
						{
							if (i != 0 || j != 0 || k != 0)
							{
								Vector3 key = new Vector3((float)((int)(coord.x + (float)i)), (float)((int)(coord.y + (float)j)), (float)((int)(coord.z + (float)k)));
								if (this.map.TryGetFirstValue(key, out node, out nativeMultiHashMapIterator) && Vector3.Distance(cur_node.pos, node.pos) < 0.1f && this.ConnectAdjacentNodes(cur_node, node))
								{
									return;
								}
							}
						}
					}
				}
			}
			PathData.PassableAreaNode panode2 = this.pPFData.GetPANode(cur_node.id);
			panode2.type = PathData.PassableAreaNode.Type.Unlinked;
			this.pPFData.SetPANode(cur_node.id, panode2);
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x002162B4 File Offset: 0x002144B4
		private bool ConnectAdjacentNodes(PassableArea.Node cur_node, PassableArea.Node next_node)
		{
			if (cur_node.type == PathData.PassableAreaNode.Type.Unlinked)
			{
				return false;
			}
			PathData.PassableAreaNode panode = this.pPFData.GetPANode(cur_node.id);
			PathData.Node panormalNode = this.pPFData.GetPANormalNode(cur_node.id);
			if (this.pPFData.GetPA(cur_node.area_id - 1).IsGround())
			{
				Point pt = cur_node.cornerPoint1;
				Point pt2 = cur_node.cornerPoint2;
				Coord coord = Coord.WorldToGrid(pt, this.tileSize);
				Point point = Coord.WorldToLocal(coord, pt, this.tileSize);
				Point point2 = Coord.WorldToLocal(coord, pt2, this.tileSize);
				Coord coord2;
				Coord coord3;
				while (Coord.RayStep(ref coord, ref point, ref point2, 0.5f, out coord2, out coord3))
				{
					PathData.Node node = this.pPFData.GetNode(coord.y * this.xOffset + coord.x);
					node.weight = 0;
					this.pPFData.SetNode(coord.y * this.xOffset + coord.x, node);
					if (coord2.valid)
					{
						node = this.pPFData.GetNode(coord2.y * this.xOffset + coord2.x);
						node.weight = 0;
						this.pPFData.SetNode(coord2.y * this.xOffset + coord2.x, node);
					}
					if (coord3.valid)
					{
						node = this.pPFData.GetNode(coord3.y * this.xOffset + coord3.x);
						node.weight = 0;
						this.pPFData.SetNode(coord3.y * this.xOffset + coord3.x, node);
					}
				}
			}
			PathData.PassableAreaNode panode2 = this.pPFData.GetPANode(next_node.id);
			PathData.Node panormalNode2 = this.pPFData.GetPANormalNode(next_node.id);
			panode.link = next_node.id;
			panormalNode.pa_id = (short)next_node.area_id;
			panode2.link = cur_node.id;
			panormalNode2.pa_id = (short)cur_node.area_id;
			this.pPFData.SetPANode(cur_node.id, panode);
			this.pPFData.SetPANormalNode(cur_node.id, panormalNode);
			this.pPFData.SetPANode(next_node.id, panode2);
			this.pPFData.SetPANormalNode(next_node.id, panormalNode2);
			return true;
		}

		// Token: 0x040035E4 RID: 13796
		[ReadOnly]
		public NativeMultiHashMap<Vector3, PassableArea.Node> map;

		// Token: 0x040035E5 RID: 13797
		[ReadOnly]
		public NativeArray<Vector3> map_coords;

		// Token: 0x040035E6 RID: 13798
		public PathData.DataPointers pPFData;

		// Token: 0x040035E7 RID: 13799
		public int xOffset;

		// Token: 0x040035E8 RID: 13800
		public float tileSize;
	}

	// Token: 0x0200068C RID: 1676
	[BurstCompile]
	public struct FinishPathfinding : IJobParallelFor
	{
		// Token: 0x060047F0 RID: 18416 RVA: 0x0021651A File Offset: 0x0021471A
		public unsafe void Execute(int index)
		{
			*this.pPFData.Initted = true;
		}

		// Token: 0x040035E9 RID: 13801
		public PathData.DataPointers pPFData;
	}

	// Token: 0x0200068D RID: 1677
	// (Invoke) Token: 0x060047F2 RID: 18418
	public delegate void OnFinishPathfinding();
}
