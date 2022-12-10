using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200039A RID: 922
	[Node.CreateNodeMenuAttribute("BSG Nodes/Heights/OutputHeights")]
	[Serializable]
	public class OutputHeightsNode : Node, IOutputNode
	{
		// Token: 0x06003519 RID: 13593 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x0600351A RID: 13594 RVA: 0x001A9C3C File Offset: 0x001A7E3C
		public unsafe override void Build()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.heights = base.GetInputValue<GetFloat2DCB>("heights", default(GetFloat2DCB));
			if (this.heights.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			int heightmapResolution = terrainData.heightmapResolution;
			float[,] array = new float[heightmapResolution, heightmapResolution];
			fixed (float* ptr = &array[0, 0])
			{
				float* pheights = ptr;
				OutputHeightsNode.BuildJob jobData = default(OutputHeightsNode.BuildJob);
				jobData.resolution = heightmapResolution;
				jobData.tile_size = new float2(terrainData.size.x, terrainData.size.z) / (float)(heightmapResolution - 1);
				jobData.pheights = pheights;
				jobData.cb = this.heights;
				if (this.SingleThreaded)
				{
					jobData.Run(heightmapResolution);
				}
				else
				{
					jobData.Schedule(heightmapResolution, 16, default(JobHandle)).Complete();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			terrainData.SetHeights(0, 0, array);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Heights initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x0600351B RID: 13595 RVA: 0x001A9D90 File Offset: 0x001A7F90
		public int2 GetResolution(NodePort port)
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				return terrainData.heightmapResolution;
			}
			return 0;
		}

		// Token: 0x0400244B RID: 9291
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB heights;

		// Token: 0x0400244C RID: 9292
		public bool SingleThreaded;

		// Token: 0x020008DB RID: 2267
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x06005216 RID: 21014 RVA: 0x0023FA38 File Offset: 0x0023DC38
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					float value = this.cb.GetValue(wx, wy);
					this.pheights[y * this.resolution + i] = value;
				}
			}

			// Token: 0x04004144 RID: 16708
			public int resolution;

			// Token: 0x04004145 RID: 16709
			public float2 tile_size;

			// Token: 0x04004146 RID: 16710
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pheights;

			// Token: 0x04004147 RID: 16711
			public GetFloat2DCB cb;
		}
	}
}
