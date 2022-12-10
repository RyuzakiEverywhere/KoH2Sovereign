using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A0 RID: 928
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Trees/OutputTrees")]
	[Serializable]
	public class OutputTreesNode : Node, IOutputNode
	{
		// Token: 0x0600353A RID: 13626 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x0600353B RID: 13627 RVA: 0x001AB424 File Offset: 0x001A9624
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddTree(ref TreeInstance tree, void* data)
		{
			if (data == null)
			{
				return;
			}
			((OutputTreesNode.AddData*)data)->Add(tree);
		}

		// Token: 0x0600353C RID: 13628 RVA: 0x001AB438 File Offset: 0x001A9638
		public unsafe override void Build()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.trees = base.GetInputValue<GetTreesCB>("trees", default(GetTreesCB));
			if (this.trees.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			if (!OutputTreesNode.add_tree_func.IsCreated)
			{
				OutputTreesNode.add_tree_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(OutputTreesNode.AddTree));
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			GrowBuffer<TreeInstance> growBuffer = new GrowBuffer<TreeInstance>(Allocator.TempJob, 65536);
			OutputTreesNode.BuildJob buildJob = default(OutputTreesNode.BuildJob);
			buildJob.tile_size = new float2(32f, 32f);
			buildJob.resolution = (int)(terrainData.size.x / buildJob.tile_size.x);
			buildJob.cb = this.trees;
			buildJob.add_func = OutputTreesNode.add_tree_func;
			OutputTreesNode.AddData addData = default(OutputTreesNode.AddData);
			addData.buf_data = growBuffer.data;
			addData.inv_size_x = 1f / terrainData.size.x;
			addData.inv_size_z = 1f / terrainData.size.z;
			buildJob.add_func_data = (void*)(&addData);
			if (this.SingleThreaded)
			{
				buildJob.Run(buildJob.resolution);
			}
			else
			{
				buildJob.Schedule(buildJob.resolution, 1, default(JobHandle)).Complete();
			}
			TreeInstance[] array = growBuffer.ToArray();
			growBuffer.Dispose();
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			TreesBatching component = base.mm.tgt_terrain.GetComponent<TreesBatching>();
			if (component != null && component.isActiveAndEnabled && component.is_enabled)
			{
				component.do_rebuild = true;
			}
			terrainData.SetTreeInstances(array, true);
			long elapsedMilliseconds4 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Trees: {0}, initted for {1}ms, calculated for {2}ms, applied for {3}ms", new object[]
			{
				array.Length,
				elapsedMilliseconds,
				elapsedMilliseconds3,
				elapsedMilliseconds4
			}));
		}

		// Token: 0x0600353D RID: 13629 RVA: 0x001AB664 File Offset: 0x001A9864
		public int2 GetResolution(NodePort port)
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				float2 @float = new float2(32f, 32f);
				(int)(terrainData.size.x / @float.x);
			}
			return 0;
		}

		// Token: 0x0400246E RID: 9326
		private const int TILE_SIZE = 32;

		// Token: 0x0400246F RID: 9327
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB trees;

		// Token: 0x04002470 RID: 9328
		public bool SingleThreaded;

		// Token: 0x04002471 RID: 9329
		public static FunctionPointer<GetTreesCB.AddTreeFunc> add_tree_func;

		// Token: 0x020008E8 RID: 2280
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x06005224 RID: 21028 RVA: 0x00240148 File Offset: 0x0023E348
			public void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					this.cb.GetTrees(wx, wy, this.tile_size.x, this.tile_size.y, this.add_func, this.add_func_data);
				}
			}

			// Token: 0x04004180 RID: 16768
			public int resolution;

			// Token: 0x04004181 RID: 16769
			public float2 tile_size;

			// Token: 0x04004182 RID: 16770
			public GetTreesCB cb;

			// Token: 0x04004183 RID: 16771
			public FunctionPointer<GetTreesCB.AddTreeFunc> add_func;

			// Token: 0x04004184 RID: 16772
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* add_func_data;
		}

		// Token: 0x020008E9 RID: 2281
		public struct AddData
		{
			// Token: 0x06005225 RID: 21029 RVA: 0x002401B4 File Offset: 0x0023E3B4
			public unsafe void Add(TreeInstance tree)
			{
				tree.position.x = tree.position.x * this.inv_size_x;
				tree.position.z = tree.position.z * this.inv_size_z;
				if (tree.position.x > 1f || tree.position.x < 0f || tree.position.z > 1f || tree.position.z < 0f)
				{
					return;
				}
				GrowBuffer<TreeInstance> growBuffer = new GrowBuffer<TreeInstance>((void*)this.buf_data);
				growBuffer.Add(ref tree);
			}

			// Token: 0x04004185 RID: 16773
			[NativeDisableUnsafePtrRestriction]
			public unsafe GrowBufferData* buf_data;

			// Token: 0x04004186 RID: 16774
			public float inv_size_x;

			// Token: 0x04004187 RID: 16775
			public float inv_size_z;
		}
	}
}
